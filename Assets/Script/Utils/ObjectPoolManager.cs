using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPoolManager
{
	static private ObjectPoolManager instance_ = null;
	static private GameObject root_ = null;
	static private GameObject ui_root_ = null;

	static private int timer_id_ = -1;

	static public ObjectPoolManager Instance()
	{
		if(instance_ == null)
		{
			instance_ = new ObjectPoolManager();

			if(root_ == null)
			{
				root_ = new GameObject("ObjectPoolManager");

				GameObject.DontDestroyOnLoad(root_);

				ui_root_ = new GameObject("UIPoolManager");

				GameObject.DontDestroyOnLoad(ui_root_);

				timer_id_ = TimerManager.Instance().RepeatCallFunc(delegate(float dt) {
					
					instance_.RecycleNoUsedObjects();

				}, 10);
			}
		}

		return instance_;
	}


	private Dictionary<string, ObjectPool> pools_ = new Dictionary<string, ObjectPool>();

	public void Destory()
	{
		if(timer_id_ >= 0)
		{
			TimerManager.Instance().DestroyTimer(timer_id_);
			timer_id_ = -1;
		}
	}

	public GameObject GetRoot(bool is_ui)
	{
		return is_ui ? ui_root_ : root_;
	}
		
	public GameObject GetObject(string key, bool is_ui = false)
	{
		//DebugLogger.LogWarning("Get Object " + key);

		if(pools_.ContainsKey(key))
		{
			return pools_[key].GetObject(is_ui);
		}
		else
		{
			ObjectPool pool = new ObjectPool();
			pool.Initialize(key);

			pools_.Add(key, pool);

			return pool.GetObject(is_ui);
		}
	}

	public void ReturnObject(string key, GameObject obj, bool is_ui = false)
	{
		//DebugLogger.LogWarning("Return Object " + key + " name " + obj.name);

		if(pools_.ContainsKey(key))
		{
			pools_[key].ReturnObject(obj, is_ui);
		}
		else
		{
			Debug.LogError("回收了一个不存在的pool的对象 " + key);

			GameObject.Destroy(obj);
		}
	}

	public void RecycleNoUsedObjects()
	{
		List<string> remove_keys = new List<string>();

		int tick_count = System.Environment.TickCount;
		var enumerator = pools_.GetEnumerator();
		while(enumerator.MoveNext())
		{
			enumerator.Current.Value.RecycleNoUsedObjects(tick_count);

			if(enumerator.Current.Value.GetPoolCount() == 0)
			{
				enumerator.Current.Value.Destroy();

				remove_keys.Add(enumerator.Current.Key);
			}
		}

		var remove_enumerator = remove_keys.GetEnumerator();

		while(remove_enumerator.MoveNext())
		{
			pools_.Remove(remove_enumerator.Current);
		}

		remove_keys.Clear();
	}
}
