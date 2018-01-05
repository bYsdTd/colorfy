using UnityEngine;
using System.Collections;

public static class App {
	static bool inited = false;
	// Lua 的主循环
	static SLua.LuaFunction luaUpdateFunc = null;

	// 初始话方法
	public static void Init()
	{
		RegisterEventLiseners ();// 注册事件监听

		ShaderManager.Instance().Init();

		LobbyNetManager.Instance().Init();

		GUIManager.Instance ().Init ();

		LevelManager.Instance ().Init ();

		SoundManager.Instance ().Init ();

		LuaGameManager.Instance().InitAllManager();

		inited = true;
	}

	public static void OnUpdate(){
		if (inited == false) 
		{
			return;
		}

		LobbyNetManager.Instance().UpdateSocket ();

		if(luaUpdateFunc != null) 
		{
			luaUpdateFunc.call(Time.deltaTime); // 这个地方每秒60B的GC啊
		}
	}

	public static void LateUpdate(){ }

	public static void OnApplicationFocus(bool focus){ }

	public static void OnApplicationPause(bool ispause){ }

	public static void OnApplicationQuit(){ }

	public static void OnGUI(){	}

	public static void Quit(string reason)
	{
		Debug.Log (reason);
		Application.Quit ();
	}

	// 清理
	public static void DoCleanUp()
	{
		inited = false;

		LobbyNetManager.Instance().Dispose ();

		LuaGameManager.Instance().DestroyAllManager();

		GUIManager.Instance().Destroy();

		LevelManager.Instance ().Dispose ();

		RemoveEventLiseners ();
	}

	#region 事件响应相关
	// 添加事件监听
	static void RegisterEventLiseners()
	{
		EventManager.Instance ().RegisterEvent (EventConfig.LUA_MANAGER_INIT_FINISH, OnLuaManagerInitFinish);
	}
	// 移除事件监听
	static void RemoveEventLiseners()
	{
		EventManager.Instance ().UnRegisterEvent (EventConfig.LUA_MANAGER_INIT_FINISH, OnLuaManagerInitFinish);
	}

	public static void OnLuaManagerInitFinish(object param)
	{
		luaUpdateFunc = LuaGameManager.Instance().GetState().getFunction("mainUpdate");
	}
	#endregion

	#region 玩家的deviceid
	static string DeviceIdKey = "DeviceIdKey";
	public static string GetDeviceId(){
		if (PlayerPrefs.HasKey (DeviceIdKey)) {
			return PlayerPrefs.GetString (DeviceIdKey);
		}
		string id = SystemInfo.deviceUniqueIdentifier; // 此方法网友反应有坑

		PlayerPrefs.SetString (DeviceIdKey, id);

		return id;
	}
	#endregion
}