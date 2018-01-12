using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Net;

public enum SocketConnectionState{
	UnInit,
	Connecting,
	Connected,
	Closing,
	Closed,
	Interrupt
}

public enum SocketConnectionEvent{
	None,
	ConnectSuccess,
	ConnectFailed,
	ConnectLost
}

public class SocketConnection
{
	private volatile SocketConnectionState _state = SocketConnectionState.UnInit;
	private SocketError _error;

	private byte[] _recvBytes = new byte[256];
	private SendBuffer _sendBuf = new SendBuffer();
	private RecvBuffer _recvBuf = new RecvBuffer();

	private Thread _worker;
	private string ip;
	private int port;

	public Queue<PBMsg> pb_send_ = new Queue<PBMsg>();

	public bool mainThreadCloseSocket { set; get; }

	public event EventHandler ConnectionSuccess;
	public event EventHandler ConnectionFailed;
	public event EventHandler ConnectionLost;

	protected virtual void OnConnectionSuccess(System.EventArgs e)
	{
		EventHandler handler = ConnectionSuccess;
		if(handler != null)
		{
			handler(this, e);
		}
	}

	protected virtual void OnConnectionFailed(System.EventArgs e)
	{
		EventHandler handler = ConnectionFailed;
		if(handler != null)
		{
			handler(this, e);
		}
	}

	protected virtual void OnConnectionLost(System.EventArgs e)
	{
		EventHandler handler = ConnectionLost;
		if(handler != null)
		{
			handler(this, e);
		}
	}

	public SocketConnectionState State
	{
		get
		{
			return _state;
		}
		private set
		{
			_state = value;
		}
	}

	public SocketError SocketError{
		get{
			return _error;
		}
	}

	public void Connect(string ip, int port){

		this.ip = ip;
		this.port = port;
		this.mainThreadCloseSocket = false;

		_worker = new Thread(this.Worker);
		_worker.Start();
	}

	public void SendSocketData(SocketData sockData){
		_sendBuf.Write(sockData);
	}

	public SocketData RecvSocketData(){
		return _recvBuf.Read();
	}

	// 主线程调用，socket线程读
	public void Close()
	{
		mainThreadCloseSocket = true;	
	}

	public List<SocketData> RecvSocketDataList(){
		return _recvBuf.ReadList();
	}

	void UpdateSendBuf() {
		lock(pb_send_) {
			while(pb_send_.Count > 0) {
				PBMsg pb = pb_send_.Dequeue();
				SocketData sockData = NetworkManager.ConvertToSocketData(pb);
				_sendBuf.Write(sockData);
			}
		}
	}

	#region Worker
	public void Worker(){
		IPAddress addr = null;
		bool isIPV6 = false;

		if(IPAddress.TryParse(ip, out addr))
		{
//			isIPV6 = AppEnv.IsInIPV6;
		}
		else
		{
			// 域名，需要dns解析出ip
			IPHostEntry ipHost = Dns.GetHostEntry(ip);
			if(ipHost.AddressList.Length > 0)
			{
				addr = ipHost.AddressList[0];
				var nameType = Uri.CheckHostName(addr.ToString());
				isIPV6 = nameType == UriHostNameType.IPv6;
			}
			else
			{
				State = SocketConnectionState.Closed;
				Debug.LogError("# dns parse error");
				return;
			}
		}

		IPEndPoint ipe = new IPEndPoint(addr, port);

		Socket socket = new Socket(isIPV6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
		socket.NoDelay = true;
		socket.SendBufferSize = 1024 * 32;
		socket.SendTimeout = 10 * 1000; //10s
		socket.Blocking = true;

		State = SocketConnectionState.Connecting;

		int startTick = Environment.TickCount;

		do
		{
			try
			{
				// 重试30s
				int nowTick = Environment.TickCount;
				if(nowTick - startTick >= 30000)
				{
					break;
				}

				socket.Connect(ipe);

				if (socket.Connected)
				{
					State = SocketConnectionState.Connected;
					Debug.Log("# Socket Connected Success");

					OnConnectionSuccess(new EventArgs());

					break;
				}
			}
			catch (SocketException ex)
			{
				// 什么都不做，因为要反复的重新connect
			}
			catch (Exception ex)
			{
			}
		}
		while(true);

		if(State != SocketConnectionState.Connected)
		{
			State = SocketConnectionState.Closed;
			Debug.Log("# Socket connect failed");

			OnConnectionFailed(new EventArgs());
			return;
		}

		//Read & Write
		while(true)
		{
			if(mainThreadCloseSocket)
			{
				// 主线程关闭, 调用socketmgr的close接口会触发
				break;
			}

			try
			{
				//Read
				while (socket.Poll(10, SelectMode.SelectRead))
				{
					int recvSize = 0;
					SocketError sock_err = SocketError.Success;
					recvSize = socket.Receive(_recvBytes, 0, _recvBytes.Length, SocketFlags.None, out sock_err);

					if (sock_err == SocketError.Success && recvSize > 0)
					{
						_recvBuf.Write(_recvBytes, recvSize);
					} 
					else if(sock_err == SocketError.Success && recvSize == 0)
					{
						// 服务器断开连接，或者客户端拔网线，切网络之类的
						throw new Exception("server disconnect or client timeout!");
					} 
					else if (sock_err == SocketError.WouldBlock || 
						sock_err == SocketError.IOPending || 
						sock_err == SocketError.NoBufferSpaceAvailable) 
					{
						Debug.LogError("socket woold block " + sock_err.ToString());

						continue;
					}
					else if(sock_err == SocketError.TimedOut)
					{
						Debug.LogError("socket receive timeout");
						continue;
					}
					else 
					{
						throw new Exception("Socket Receive " + recvSize + " bytes, SocketError " + sock_err);
					}
				}

				_recvBuf.DeserializeBuffer();

				//Write
				UpdateSendBuf();
				if (socket.Poll(10, SelectMode.SelectWrite))
				{
					byte[] msg = _sendBuf.Read();

					if (null != msg && msg.Length != 0)
					{
						int msgLen = msg.Length;
						int sent = 0;

						while(sent < msgLen)
						{
							SocketError sock_err = SocketError.Success;
							int size = socket.Send(msg, sent, msgLen - sent, SocketFlags.None, out sock_err);

							if (sock_err == SocketError.Success && size > 0)
							{
								sent += size;
							} 
							else if (sock_err == SocketError.WouldBlock || 
								sock_err == SocketError.IOPending || 
								sock_err == SocketError.NoBufferSpaceAvailable) 
							{
								continue;
							} 
							else 
							{
								throw new Exception("Socket Send " + size + " bytes, SocketError " + sock_err);
							}
						}
					}
				}
			}
			catch (SocketException ex)
			{
				Debug.LogError(ex);
				break;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
				break;
			}

			Thread.Sleep(10);
		}

		//Close
		try
		{
			if (socket.Connected)
			{
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}

		// 最后无论怎样都设置socket 关闭的状态
		State = SocketConnectionState.Closed;

		OnConnectionLost(new EventArgs());
		Debug.Log("# Socket Thread Close");
	}
	#endregion
}