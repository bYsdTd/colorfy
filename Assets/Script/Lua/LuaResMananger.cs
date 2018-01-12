using UnityEngine;
using System.Collections;

public delegate void OnAssetLoadedSuccess(GameObject obj);
public delegate void OnAssetLoadedFailed(string error);

public static class LuaResMananger 
{
	public static void LoadGameObject(string name, bool isUI, OnAssetLoadedSuccess successCallback, OnAssetLoadedFailed failedCallback)
	{
		GameObject go = ObjectPoolManager.Instance ().GetObject (name, isUI);

		if(go == null)
		{
			Debug.LogError("resource manager null");
			failedCallback.Invoke("Load res error " + name);
		}
		else
		{
			successCallback.Invoke(go);
		}
	}

	public static void UnloadGameObject(string name, GameObject go, bool isUI)
	{
		ObjectPoolManager.Instance ().ReturnObject (name, go, isUI);
	}
}