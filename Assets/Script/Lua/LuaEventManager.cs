using UnityEngine;
using System;
using System.Collections;

// Lua 向C#发送的协议
using System.Collections.Generic;

public static class LuaEventManager 
{
	public static void PostCSEvent(string eventName, params object[] eventParam)
	{
		switch(eventName)
		{
		case "ConnectServer": 
			{
				string ip = (string)eventParam [0];;
				int post = System.Convert.ToInt32(eventParam [1]);

				NetworkManager.Instance ().Connect (ip, post);
			}
			break;
		case "JoinRoom":
			{
				string serverIp = (string)eventParam [0];
				int port = System.Convert.ToInt32(eventParam [1]);
				int roomId = System.Convert.ToInt32(eventParam [2]);
				string token = (string)eventParam [3];

				// // 这里逻辑层和渲染层同时初始化了
				// BL.BLUnitManager.Instance().InitBase();
				// BL.BLUnitManager.Instance().InitNeutrals();

				// 加载完成，加入房间1
//				NetManager.Instance().Connect(serverIp, port);
//				NetManager.Instance().JoinRoom(roomId);
			}
			break;
		default:
			break;
		}
	}
}
