using UnityEngine;
using System.Collections.Generic;
using System;
	
public class PBMsg
{
	public short handle_id_;
	public byte[] bytes;
}

public class ReceiveMsg
{
	public SocketData data;
	public object resolvedObj;
}

public class NetworkManager
{
	static NetworkManager _instance;
	public static NetworkManager Instance(){
		if (_instance == null) {
			_instance = new NetworkManager ();
		}
		return _instance;
	}

	public const byte VERSION = 0;

	SocketConnectionEvent cacheConnectEvent = SocketConnectionEvent.None;

	private SocketConnection _conn; // socket连接

	public SocketConnectionState State
	{
		get{
			if (_conn == null){
				return SocketConnectionState.UnInit;
			}
			return _conn.State;
		}
	}

	private bool sessionAvailable = false;
	public bool SessionAvailable 
	{ 
		get { return sessionAvailable; } 
		set { sessionAvailable = value; }
	}

	// 初始化
	public void Init()
	{
	}

	// 销毁
	public void Dispose()
	{
		_instance = null;
		if (null != _conn)
		{
			_conn.Close();
		}
	}

	// 退出应用
	void OnApplicationQuit()
	{
		Dispose();
	}

	private Queue<ReceiveMsg> tobeHandledSocketData = new Queue<ReceiveMsg>();
	private short mMaxTimeout = 10;
	private short mOneFrameDealMsgCount = 1;

	private delegate void SocketEventHandler();

	private void OnConnectSuccessEvent(object sender, EventArgs e){
		cacheConnectEvent = SocketConnectionEvent.ConnectSuccess;
	}

	private void OnConnectFailedEvent(object sender, EventArgs e){
		cacheConnectEvent = SocketConnectionEvent.ConnectFailed;
	}

	private void OnConnectLostEvent(object sender, EventArgs e){
		cacheConnectEvent = SocketConnectionEvent.ConnectLost;
	}

	// 每帧检查处理消息
	public void UpdateSocket() 
	{
		// 向 Lua 发送缓存的socket链接状态
		if (cacheConnectEvent != SocketConnectionEvent.None) {
			Debug.Log (cacheConnectEvent.ToString ());

			LuaUtils.PostLuaEvent(cacheConnectEvent.ToString(), null);
			cacheConnectEvent = SocketConnectionEvent.None;
		}
		// 检查是否有消息来了
		if(null != _conn && _conn.State == SocketConnectionState.Connected)
		{
			List<SocketData> sockDatas = _conn.RecvSocketDataList ();

			if(sockDatas != null && sockDatas.Count > 0)
			{
				for(int i = 0; i < sockDatas.Count; ++i)
				{
					SocketData data = sockDatas [i];
					ReceiveMsg msgData = new ReceiveMsg ();
					msgData.data = data;

					tobeHandledSocketData.Enqueue(msgData);	
				}
			}
		}
		// 处理接收的消息
		if (tobeHandledSocketData.Count > 0) 
		{
			ReceiveMsg msgData = tobeHandledSocketData.Dequeue ();
			DealWithSocketData (msgData);
		} 
		else 
		{
		}
	}

	// 处理消息
	public void DealWithSocketData(ReceiveMsg msgData)
	{
		// lua 处理协议
		SLua.LuaFunction func = LuaGameManager.Instance().GetState().getFunction("OnProtobufCallBack");
		if(func != null)
		{
			func.call(msgData.data.handleId, new SLua.ByteArray(msgData.data.data));
		}
	}

	public delegate void OnSendOkCallBack();
	// Lua发送协议
	public void LuaSendProtoImp(int requestType, byte[] baseVO, OnSendOkCallBack onSendOK){
		if (_conn == null || SocketConnectionState.Connected != _conn.State){
			Debug.LogError("sock connect not working!");
			return;
		}

		PBMsg pb = new PBMsg();
		pb.handle_id_ = (short)requestType;
		pb.bytes = baseVO;
		lock(_conn.pb_send_) {
			_conn.pb_send_.Enqueue(pb);
		}
		onSendOK.Invoke();
	}

	public static SocketData ConvertToSocketData(PBMsg pb)
	{
		byte[] bytes = pb.bytes;

		SocketData sockData = new SocketData();
		sockData.handleId = pb.handle_id_;
		sockData.version = VERSION;
		if (bytes != null) {// 干掉了这个验证 ProtoBuf.Meta.RuntimeTypeModel.Default.CanSerialize (byteArray.data.GetType()))
			using(var ms = new System.IO.MemoryStream()) 
			{
				sockData.data = bytes;
			}
		} 
		else 
		{
			Debug.LogError("Can not serialize object to proto: " + pb.handle_id_.ToString());
		};
		return sockData;
	}

	// 链接服务器
	public void Connect(string ip, int port){
		_conn = new SocketConnection();

		_conn.ConnectionSuccess += new EventHandler(OnConnectSuccessEvent);
		_conn.ConnectionFailed += new EventHandler(OnConnectFailedEvent);
		_conn.ConnectionLost += new EventHandler(OnConnectLostEvent);

		_conn.Connect(ip, port);
	}
}