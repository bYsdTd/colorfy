using UnityEngine;
using System.Collections;

public static class App 
{
	static bool inited = false;

	// 初始话方法
	public static void Init()
	{
		ShaderManager.Instance().Init();

		GUIManager.Instance ().Init ();

		SoundManager.Instance ().Init ();

		LuaGameManager.Instance().InitAllManager();
		
		inited = true;
	}

	public static void LateUpdate(){ }

	public static void Update()
	{
		InputManager.Instance().Tick(Time.deltaTime);
	}

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

		GUIManager.Instance().Destroy();
	}
}