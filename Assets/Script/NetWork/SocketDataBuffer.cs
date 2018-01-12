using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class SocketDataBuffer
{
	protected const int BUF_LEN = 1024 * 1024;
	protected object syncObj = new object();
	protected byte[] buffer = new byte[BUF_LEN];
	protected int dataLen = 0;

	public virtual void Clear()
	{
		lock (syncObj)
		{
			dataLen = 0;
		}
	}
}

public class SendBuffer : SocketDataBuffer
{
	protected const int BUF_LEN = 1024 * 1024;
	protected object syncObj = new object();
	protected byte[] buffer = new byte[BUF_LEN];
	protected int dataLen = 0;

	public virtual void Clear()
	{
		lock (syncObj)
		{
			dataLen = 0;
		}
	}
	//used for router
	public void Write(SocketData sockData)
	{
		lock (syncObj)
		{
			if (dataLen + sockData.data.Length + BytesWrapper.WrapperLen > BUF_LEN)
			{
				throw new Exception("send buffer is full");
			}
			int newDataLen = BytesWrapper.Wrap(buffer, dataLen, sockData);
			dataLen = newDataLen;
		}
	}

	//used for sock
	public byte[] Read()
	{
		lock (syncObj)
		{
			byte[] data = new byte[dataLen];
			Array.Copy(buffer, data, dataLen);
			Array.Clear(buffer, 0, dataLen);
			dataLen = 0;
			return data;
		}
	}
}

public class RecvBuffer : SocketDataBuffer
{
	public Queue<SocketData> deserialized_data_list_ = new Queue<SocketData>();

	//used for sock
	public void Write(byte[] data, int len)
	{
		lock (syncObj)
		{
			if (dataLen + len > BUF_LEN)
			{
				throw new Exception("recv buffer is full");
			}
			Array.Copy(data, 0, buffer, dataLen, len);
			dataLen += len;
		}
	}

	public void DeserializeBuffer()
	{
		int offset = 0;
		List<SocketData> sockData = BytesWrapper.UnwrapZiped(buffer, ref offset, dataLen);
		if (offset != 0){
			dataLen -= offset;
			Array.Copy(buffer, offset, buffer, 0, Math.Min(dataLen, BUF_LEN - offset));
		}

		if(sockData != null)
		{
			for(int i = 0; i < sockData.Count; ++i)
			{
				SocketData socket_data = sockData[i];

// c# 需要处理的socket需要进行解析因为当时是将这个另起了一个线程
//					common.ResponseId responseType = (common.ResponseId)socket_data.handleId;
//					if (SocketRouter._protoTypeMap.ContainsKey (responseType)) {
//						System.IO.MemoryStream fileHandler = new System.IO.MemoryStream(socket_data.data);
//						Type type = SocketRouter._protoTypeMap [responseType];
//						socket_data.deserialized_data = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(fileHandler, null, type);
//					}

				deserialized_data_list_.Enqueue(socket_data);
			}	
		}
	}

	//used for router
	public SocketData Read()
	{
		lock (syncObj)
		{
			int offset = 0;
			SocketData sockData = BytesWrapper.Unwrap(buffer, ref offset, dataLen);
			if (offset != 0){
				dataLen -= offset;
				Array.Copy(buffer, offset, buffer, 0, Math.Min(dataLen, BUF_LEN - offset));
			}
			return sockData;
		}
	}

	public List<SocketData> ReadList()
	{
		lock (syncObj)
		{
			List<SocketData> sockData = new List<SocketData>();

			while(deserialized_data_list_.Count > 0)
			{
				sockData.Add(deserialized_data_list_.Dequeue());	
			}

			return sockData;
		}
	}
}