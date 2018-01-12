using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BytesWrapper
{
	// NonStateMsgMarker(1 byte) - Handler Id (2 byte) - Netpack Version(1 byte) - Data size(4 byte) - Data - CRC(4 byte) - NonStateMsgMarker
	public const byte PackageBreaker = 254;
	public const int WrapperLen = 13;
	public const int DataSizeOffset = 4;

	public static byte ReadByte(byte[] buf, ref int offset)
	{
		return buf[offset++];
	}

	public static int ReadInt(byte[] buf, ref int offset)
	{
		int val = ByteOrderConverter.NetworkToHostOrder(BitConverter.ToInt32(buf, offset));
		offset += 4;
		return val;
	}

	public static short ReadShort(byte[] buf, ref int offset)
	{
		short val = ByteOrderConverter.NetworkToHostOrder(BitConverter.ToInt16(buf, offset));
		offset += 2;
		return val;
	}

	public static UInt32 ReadUInt32(byte[] buf, ref int offset)
	{
		UInt32 val = ByteOrderConverter.NetworkToHostOrder(BitConverter.ToUInt32(buf, offset));
		offset += 4;
		return val;
	}

	public static void WriteByte(byte[] buf, byte val, ref int offset)
	{
		buf[offset++] = val;
	}

	public static void WriteShort(byte[] buf, short val, ref int offset)
	{
		byte[] bytes = BitConverter.GetBytes(ByteOrderConverter.HostToNetworkOrder(val));
		Array.Copy(bytes, 0, buf, offset, bytes.Length);
		offset += bytes.Length;
	}

	public static void WriteInt(byte[] buf, int val, ref int offset)
	{
		byte[] bytes = BitConverter.GetBytes(ByteOrderConverter.HostToNetworkOrder(val));
		Array.Copy(bytes, 0, buf, offset, bytes.Length);
		offset += bytes.Length;
	}

	public static void WriteUInt32(byte[] buf, UInt32 val, ref int offset)
	{
		byte[] bytes = BitConverter.GetBytes(ByteOrderConverter.HostToNetworkOrder(val));
		Array.Copy(bytes, 0, buf, offset, bytes.Length);
		offset += bytes.Length;
	}

	public static int Wrap(byte[] buf, int offset, SocketData sockData)
	{
		// marker
		WriteByte(buf, PackageBreaker, ref offset);
		// handle id
		WriteShort(buf, sockData.handleId, ref offset);
		// version
		WriteByte(buf, sockData.version, ref offset);
		// data size
		WriteInt(buf, sockData.data.Length, ref offset);
		// data
		Array.Copy(sockData.data, 0, buf, offset, sockData.data.Length);
		offset += sockData.data.Length;
		// crc
		WriteUInt32(buf, sockData.GetCRC(), ref offset);
		// marker
		WriteByte(buf, PackageBreaker, ref offset);
		return offset;
	}

	public static List<SocketData> UnwrapZiped(byte[] buf, ref int beginOffset, int bufLen){
		if (buf.Length == 0 || bufLen == 0)
			return null;

		int oriBeginOffset = beginOffset;
		while(beginOffset < bufLen - WrapperLen && buf[beginOffset] != PackageBreaker)
		{
			++beginOffset;
		}
		if (oriBeginOffset != beginOffset)
		{
			Debug.LogError("#Wrong Start Breaker. Offset => frome " + oriBeginOffset + " to " + beginOffset);
		}

		int absDataSizeOffset = beginOffset + DataSizeOffset;
		int dataSize = ReadInt(buf, ref absDataSizeOffset);
		int totalLen = beginOffset + dataSize + WrapperLen;
		if (totalLen > bufLen) // data is not complete, wait
			return null;

		if (buf[beginOffset + totalLen - 1] != PackageBreaker)
		{
			++beginOffset;
			Debug.LogError("#Wrong End Breaker. Offset => " + beginOffset);
			return null;
		}

		// begin unwrap
		SocketData sockData = new SocketData();
		int wrapOffset = beginOffset;
		// marker
		ReadByte(buf, ref wrapOffset);
		// handle id
		sockData.handleId = ReadShort(buf, ref wrapOffset);
		// version
		sockData.version = ReadByte(buf, ref wrapOffset);

		var list = new List<SocketData>();

		// data size
		ReadInt(buf, ref wrapOffset);
		// data
		sockData.data = new byte[dataSize];
		Array.Copy (buf, wrapOffset, sockData.data, 0, dataSize);
		wrapOffset += dataSize;
		// check crc
		UInt32 crc = ReadUInt32(buf, ref wrapOffset);
		if (!sockData.CheckCRC(crc))
		{
			++beginOffset;
			Debug.LogError("#Wrong CRC. Offset => " + beginOffset);
			return null;
		}	
		// marker
		ReadByte(buf, ref wrapOffset);
		beginOffset = wrapOffset;

		if (sockData.version != 2) 
		{
			list.Add (sockData);
			return list;
		}

		var unziped = GZIPWrapper.UnzipBytesWithoutHeader (sockData.data);

		var i = 0;
		var size = unziped.Length;
		while (i < size) 
		{
			var sData=Unwrap (unziped, ref i, size);
			if (sData != null) 
			{
				list.Add (sData);
			}
		}

		return list;
	}

	public static SocketData Unwrap(byte[] buf, ref int beginOffset, int bufLen){
		if (buf.Length == 0 || bufLen == 0)
			return null;

		int oriBeginOffset = beginOffset;
		while(beginOffset < bufLen - WrapperLen && buf[beginOffset] != PackageBreaker){
			++beginOffset;
		}
		if (oriBeginOffset != beginOffset){
			Debug.LogError("#Wrong Start Breaker. Offset => frome " + oriBeginOffset + " to " + beginOffset);
		}

		int absDataSizeOffset = beginOffset + DataSizeOffset;
		int dataSize = ReadInt(buf, ref absDataSizeOffset);
		int totalLen = beginOffset + dataSize + WrapperLen;
		if (totalLen > bufLen) // data is not complete, wait
			return null;

		if (buf[totalLen - 1] != PackageBreaker){
			++beginOffset;
			Debug.LogError("#Wrong End Breaker. Offset => " + beginOffset);
			return null;
		}

		// begin unwrap
		SocketData sockData = new SocketData();
		int wrapOffset = beginOffset;
		// marker
		ReadByte(buf, ref wrapOffset);
		// handle id
		sockData.handleId = ReadShort(buf, ref wrapOffset);
		// version
		sockData.version = ReadByte(buf, ref wrapOffset);
		// data size
		ReadInt(buf, ref wrapOffset);
		// data
		sockData.data = new byte[dataSize];
		Array.Copy (buf, wrapOffset, sockData.data, 0, dataSize);
		wrapOffset += dataSize;
		// check crc
		UInt32 crc = ReadUInt32(buf, ref wrapOffset);
		if (!sockData.CheckCRC(crc)){
			++beginOffset;
			Debug.LogError("#Wrong CRC. Offset => " + beginOffset);
			return null;
		}	
		// marker
		ReadByte(buf, ref wrapOffset);
		beginOffset = wrapOffset;
		return sockData;
	}
}