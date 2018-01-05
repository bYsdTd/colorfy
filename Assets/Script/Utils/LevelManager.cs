using UnityEngine;
using System;

// 场景的定义,lua中也会有一样的
using UnityEngine.SceneManagement;


public enum SceneId
{
	scn_none = 0,
	scn_loading = 1,
	scn_lobby = 2,
	scn_battle = 3,
}

public class LevelManager 
{
	static LevelManager _instance;
	public static LevelManager Instance()
	{
		if (_instance == null)
		{
			_instance = new LevelManager ();
		}
		return _instance;
	}

	int load_timer_id = -1;

	public void Init()
	{
	}

	public void Dispose()
	{
	}

	private SceneId _cur_scene_id = SceneId.scn_none;

	void Start()
	{
		_cur_scene_id = Parse(Application.loadedLevelName);
	}

	public static SceneId Parse(string levelName)
	{
		return (SceneId)System.Enum.Parse(typeof(SceneId), levelName);
	}
		
	public void SwitchToLevel(SceneId sceneId, Action<float> loadCallback = null)
	{
		if (sceneId == _cur_scene_id){
			Debug.LogError ("已经在该场景 " + sceneId.ToString());
		}
		_cur_scene_id = sceneId;

		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneId.ToString());

		load_timer_id = TimerManager.Instance ().RepeatCallFunc (delegate(float dt) {
			if (asyncLoad.isDone)
			{
				TimerManager.Instance().DestroyTimer(load_timer_id);
				load_timer_id = -1;
				if (loadCallback != null)
					loadCallback.Invoke(100f);
			}
			else
			{	
				if (loadCallback != null)
					loadCallback.Invoke(asyncLoad.progress);
			}
		}, 0.01f);
	}

	public void SwitchToLevel(string sceneName, Action<float> loadCallback = null)
	{
		SwitchToLevel((SceneId)Enum.Parse(typeof(SceneId), sceneName), loadCallback);
	}
}