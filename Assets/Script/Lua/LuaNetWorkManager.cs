using UnityEngine;
using System;
using SLua;

public static class LuaNetWorkManager{
	public static void SendProto(int requestType, byte[] baseVO, NetworkManager.OnSendOkCallBack onSendOK)
	{
		NetworkManager.Instance().LuaSendProtoImp (requestType, baseVO, onSendOK);
	}
}