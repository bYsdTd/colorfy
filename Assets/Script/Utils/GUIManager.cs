using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager  
{
	public class ui_config
	{
		public static Dictionary<string, ui_config> _dict_ = new Dictionary<string, ui_config>();

		public string layout_name;
		public string resource_name;
		public string class_name;
	}

	static private GUIManager instance = null;

	static public GUIManager Instance()
	{
		if(instance == null)
		{
			instance = new GUIManager();
		}

		return instance;
	}

	private Transform _cache_root;

	public Transform cache_root
	{
		get
		{
			return _cache_root;
		}
	}
	bool is_init = false;
	private GameObject 	ngui_root_;
	private UIRoot		ui_root_componet_;

	private Dictionary<string, UILayoutBase> all_layouts = new Dictionary<string, UILayoutBase>();

	public void Init()
	{
		// 暂时加个标记
		if (is_init)
			return;

		is_init = true;

		GameObject asset = Resources.Load<GameObject> (AssetsPathConfig.assets_path_config["UIRoot"]);

		ngui_root_ = GameObject.Instantiate (asset);
		ngui_root_.name = "UIRoot";
		GameObject.DontDestroyOnLoad(ngui_root_);

		GameObject temp = NGUITools.AddChild(ngui_root_);
		temp.name = "UIRootNode";
		_cache_root = temp.transform;

		ui_root_componet_ = ngui_root_.GetComponent<UIRoot>();

		cache_root.localPosition = new Vector3(-ui_root_componet_.manualWidth * 0.5f, -ui_root_componet_.activeHeight * 0.5f);

		EventManager.Instance().RegisterEvent(EventConfig.EVENT_UI_OPEN, OnOpenWindow);
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_UI_CLOSE, OnCloseWindow);
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_SCREEN_SIZE_CHANGED, OnScreenSizeChanged);
	}

	public void Destroy()
	{
		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_UI_OPEN, OnOpenWindow);
		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_UI_CLOSE, OnCloseWindow);
		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_SCREEN_SIZE_CHANGED, OnScreenSizeChanged);
	}

	public Vector3 ScreenPosToUIPos(Vector3 screen_pos)
	{
		Vector3 ui_position = Vector3.zero;

		ui_position.x = ui_root_componet_.manualWidth * screen_pos.x / Camera.main.pixelWidth;
		ui_position.y = ui_root_componet_.activeHeight * screen_pos.y / Camera.main.pixelHeight;
	
		return ui_position;
	}

	public void OnScreenSizeChanged(object[] all_params)
	{
		cache_root.localPosition = new Vector3(-ui_root_componet_.manualWidth * 0.5f, -ui_root_componet_.activeHeight * 0.5f);
	}

	public void OnOpenWindow(object[] all_params)
	{
		string ui_name = all_params[0] as string;

		if(all_layouts.ContainsKey(ui_name))
		{
			Debug.LogError("打开界面，已经存在界面：  " + ui_name);

			return;
		}

		ui_config config = ui_config._dict_[ui_name];

		System.Type ui_type = System.Type.GetType(config.class_name);

		UILayoutBase layout = System.Activator.CreateInstance(ui_type) as UILayoutBase;

		layout.config = config;
		layout.is_show = true;
		layout.game_obj = ObjectPoolManager.Instance().GetObject(config.resource_name, true);
		layout.game_obj.transform.SetParent(cache_root, false);
		layout.OnInit(all_params);

		all_layouts.Add(ui_name, layout);
	}

	public void OnCloseWindow(object[] all_params)
	{
		string ui_name = all_params[0] as string;

		if(!all_layouts.ContainsKey(ui_name))
		{
			Debug.LogError("关闭界面, 界面不存在：  " + ui_name);

			return;
		}

		UILayoutBase layout = all_layouts[ui_name];
		layout.OnDestroy();

		ObjectPoolManager.Instance().ReturnObject(layout.config.resource_name, layout.game_obj, true);

		all_layouts.Remove(ui_name);
	}

	public void CloseAllView()
	{
		foreach(var kv in all_layouts)
		{
			UILayoutBase layout = kv.Value;
			layout.OnDestroy();

			ObjectPoolManager.Instance().ReturnObject(layout.config.resource_name, layout.game_obj, true);
		}
		all_layouts.Clear ();
	}
}
