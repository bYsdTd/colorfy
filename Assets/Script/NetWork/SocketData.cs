using UnityEngine;
using System;
using System.Collections;
using DamienG.Security.Cryptography;

public class SocketData
{
	public static bool NeedCheckCRC = false;

	public byte[] data;
	public short handleId;
	public byte version;
	public UInt32 crc;

	public object deserialized_data;

	public UInt32 GetCRC()
	{
		crc = Crc32.Compute(data);
		return crc;
	}

	public bool CheckCRC(UInt32 uncheckedCrc)
	{
		if (!NeedCheckCRC)
			return true;
		GetCRC();
		return (Crc32.Compute(data) == uncheckedCrc);
	}
}
