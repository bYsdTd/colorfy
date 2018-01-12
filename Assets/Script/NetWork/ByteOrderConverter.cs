using UnityEngine;
using System;
using System.Collections;
using System.Net;

public class ByteOrderConverter
{
	public static short HostToNetworkOrder(short val)
	{
		return IPAddress.HostToNetworkOrder(val);
	}

	public static short NetworkToHostOrder(short val)
	{
		return IPAddress.NetworkToHostOrder(val);
	}

	public static int HostToNetworkOrder(int val)
	{
		return IPAddress.HostToNetworkOrder(val);
	}

	public static int NetworkToHostOrder(int val)
	{
		return IPAddress.NetworkToHostOrder(val);
	}

	public static long HostToNetworkOrder(long val)
	{
		return IPAddress.HostToNetworkOrder(val);
	}

	public static long NetworkToHostOrder(long val)
	{
		return IPAddress.NetworkToHostOrder(val);
	}

	public static UInt32 HostToNetworkOrder(UInt32 val)
	{
		byte[] buf = BitConverter.GetBytes(val);
		int intVal = BitConverter.ToInt32(buf, 0);
		int netVal = IPAddress.HostToNetworkOrder(intVal);
		buf = BitConverter.GetBytes(netVal);
		return BitConverter.ToUInt32(buf, 0);
	}

	public static UInt32 NetworkToHostOrder(UInt32 val)
	{
		byte[] buf = BitConverter.GetBytes(val);
		int intVal = BitConverter.ToInt32(buf, 0);
		int netVal = IPAddress.NetworkToHostOrder(intVal);
		buf = BitConverter.GetBytes(netVal);
		return BitConverter.ToUInt32(buf, 0);
	}
}
