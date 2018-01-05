using UnityEngine;
using System.Collections;

public static class GameObjectExtension
{
	public static T GetOrAddComponent<T>(this GameObject go) where T : Component{
		T com = go.GetComponent<T>();
		if (com == null){
			com = go.AddComponent<T>();
		}
		return com;
	}

	public static void SetLayerRec(this GameObject go, int layer){
		go.layer = layer;
		for (int i=0; i<go.transform.childCount; ++i){
			go.transform.GetChild(i).gameObject.SetLayerRec(layer);
		}
	}
}