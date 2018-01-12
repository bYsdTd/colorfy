using UnityEngine;
using System;
using SLua;
using System.IO;

public static class LuaUtils{
	// 读取文件
	public static string ReadFileAsString(string filePath)
	{
		string ret = FileManager.Instance().ReadAllText(filePath);
		return ret;
	}

	// 加载lua脚本使用的
	public static byte[] ReadFileAsBytes(string fileName)
	{
		string path = GetFilePath(fileName);
		byte[] bytes = FileManager.Instance ().ReadAllBytes (path);
		return bytes;
	}

	// 二进制数据 >> lua的二进制数组  加载pb文件使用的
	public static ByteArray ReadFileAsByteArray(string fileName)
	{
		string path = GetFilePath(fileName);
		byte[] bytes = FileManager.Instance ().ReadAllBytes (path);
		return new ByteArray(bytes);
	}

	#region 临时加载内容，之后是文件系统代替
	const string PathLua = "Lua";
	const string PathPb = "Lua/network/autogen/pb/";

	private static string GetFilePath(string FileName)
	{
		string path = string.Empty;
		if (FileName.Contains (".pb")) { // pb文件
			path = PathPb + FileName;
		} else {
			path = PathLua + FileName + ".txt";
		}
		return path;
	}
	#endregion

	// CS 向Lua发送事件
	public static void PostLuaEvent(string eventName, params object[] args)
	{
		if (!LuaGameManager.Instance().CheckAvailable())
			return;
		
		SLua.LuaFunction func = LuaGameManager.Instance().GetState().getFunction("OnHandleEventFromCS");
		if(func != null)
		{
			func.call(eventName, args);
		}
	}

	//case
	public static void SwitchToLevel(string sceneName)
	{
		LevelManager.Instance ().SwitchToLevel (sceneName, delegate(float obj) {
			LuaUtils.PostLuaEvent("UpdateBattleLoading", obj);
		});
	}
}
