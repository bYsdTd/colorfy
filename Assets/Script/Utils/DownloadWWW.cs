using UnityEngine;
using System.Collections;

public class DownloadWWW : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(Download());
	}

	IEnumerator Download()
	{
		WWW www = new WWW("http://cn.bing.com");
		yield return www;
		foreach (var h in www.responseHeaders)
			t += h.Key + "  " + h.Value + "\n";
		t += www.text;
	}
	string t;

}