using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool 
{
	public class ObjectData
	{
		public bool is_in_use;
		public GameObject obj_instance;
		public int last_return_time_stamp;
	}

	const int kRecycleTime = 20000;

	private GameObject prefab_assets_ = null;

	private List<ObjectData> object_list_ = new List<ObjectData>();

	private string key_;

	public void Initialize(string key)
	{
		key_ = key;

		prefab_assets_ = Resources.Load<GameObject>(AssetsPathConfig.assets_path_config[key]);
	}

	public int GetPoolCount()
	{
		return object_list_.Count;	
	}

	public void Destroy()
	{
		// 这里要调用释放asset相关的接口，resources的方式没有对应的释放asset的接口
		//App.ResMgr.Unload(key_, typeof(GameObject));
	}

	public void RecycleNoUsedObjects(int tick_count)
	{
		for(int i = object_list_.Count - 1; i >= 0; --i)
		{
			if(!object_list_[i].is_in_use && (tick_count - object_list_[i].last_return_time_stamp) >= kRecycleTime)
			{
				GameObject.Destroy(object_list_[i].obj_instance);
				object_list_.RemoveAt(i);
			}
		}
	}

	public GameObject GetObject(bool is_ui)
	{
		var enumerator = object_list_.GetEnumerator();
		while(enumerator.MoveNext())
		{
			ObjectData data = enumerator.Current;
			if(!data.is_in_use)
			{
				data.is_in_use = true;
				data.obj_instance.name = key_;
				data.obj_instance.transform.position = Vector3.zero;
				data.obj_instance.SetActive(true);

				return data.obj_instance;
			}
		}

		//int raise_pool_count = object_list_.Count;

		//raise_pool_count = raise_pool_count == 0 ? 1 : raise_pool_count;

		int raise_pool_count = 1;

		for(int i = 0; i < raise_pool_count; ++i)
		{
			ObjectData data = new ObjectData();
			data.is_in_use = false;
			data.obj_instance = GameObject.Instantiate(prefab_assets_);

			data.obj_instance.transform.SetParent(ObjectPoolManager.Instance().GetRoot(is_ui).transform, false);

			data.obj_instance.transform.position = new Vector3(999.0f, 999.0f, 999.0f);

			data.last_return_time_stamp = System.Environment.TickCount;

			object_list_.Add(data);
		}

		return GetObject(is_ui);
	}

	public void ReturnObject(GameObject obj, bool is_ui)
	{
		var enumerator = object_list_.GetEnumerator();
		while(enumerator.MoveNext())
		{
			ObjectData data = enumerator.Current;
			if(data.obj_instance == obj)
			{
				data.is_in_use = false;
				data.obj_instance.SetActive(false);
				data.obj_instance.transform.SetParent(ObjectPoolManager.Instance().GetRoot(is_ui).transform);
				data.last_return_time_stamp = System.Environment.TickCount;
				return;
			}
		}
	}
}
