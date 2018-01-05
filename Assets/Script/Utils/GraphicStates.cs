using UnityEngine;
using System.Collections;

public class GraphicStates : MonoBehaviour {

	int nFrameCounter = 0;
	float timeElapsed = 0.0f;
	float currentFPS = 0.0f;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		++nFrameCounter;
		timeElapsed += Time.deltaTime;
		currentFPS = nFrameCounter / timeElapsed;

		// 
		if(timeElapsed > 1)
		{
			timeElapsed = 0;
			nFrameCounter = 0;
		}
	}

	void OnGUI() 
	{
		GUIStyle style = new GUIStyle();
		style.fontSize = 50;
		style.normal.textColor = Color.white;

		GUI.Label(new Rect(10,10, 100, 30), string.Format("FPS: {0:F1}", currentFPS), style);
	}

}
