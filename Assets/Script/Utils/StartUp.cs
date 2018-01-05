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
		App.OnUpdate ();
	}

	void LateUpdate()
	{
		App.LateUpdate();
	}

	void OnApplicationFocus(bool focus)
	{
		Debug.Log("OnApplicationFocus " + focus.ToString());

		App.OnApplicationPause(!focus);
	}

	void OnApplicationPause(bool ispause) 
	{
		Debug.Log("OnApplicationPause " + ispause.ToString());

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
