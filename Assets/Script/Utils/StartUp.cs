using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUp : MonoBehaviour 
{
	// 游戏开始的地方
	void Awake () 
	{
		App.Init ();
	}

	// 游戏主循环
	void Update()
	{
		App.Update();
	}

	void LateUpdate()
	{
		App.LateUpdate();
	}

	void OnApplicationFocus(bool focus)
	{
		App.OnApplicationPause(!focus);
	}

	void OnApplicationPause(bool ispause) 
	{
		App.OnApplicationPause(ispause);
	}

	void OnApplicationQuit()
	{
		App.OnApplicationQuit();
	}

	void OnGUI()
	{
		App.OnGUI();
	}
}
