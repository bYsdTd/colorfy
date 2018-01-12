using UnityEngine;
using System.Collections;
using SLua;

public class LuaGameManager 
{
	static LuaGameManager _instance;
	static public LuaGameManager Instance()
	{
		if(_instance == null)
		{
			_instance = new LuaGameManager();
		}

		return _instance;
	}
		
	public LuaInputManager luaInputManager = null;
	public LuaManager luaManager = null;

	GameObject luaGameManagerObj = null;

	public void InitAllManager()
	{
		luaGameManagerObj = new GameObject("LuaGameManager");
		GameObject.DontDestroyOnLoad(luaGameManagerObj);

		// input
		GameObject inputObj = new GameObject("inputManager");
		luaInputManager = inputObj.AddComponent<LuaInputManager>();
		luaInputManager.Init();
		GameObject.DontDestroyOnLoad(inputObj);
		inputObj.transform.SetParent(luaGameManagerObj.transform);

		// lua
		GameObject luaObj = new GameObject("luaManager");
		luaManager = luaObj.AddComponent<LuaManager>();
		luaManager.Init(OnFinish);
		GameObject.DontDestroyOnLoad(luaObj);
		luaObj.transform.SetParent(luaGameManagerObj.transform);
	}

	public void DestroyAllManager()
	{
		if(luaInputManager != null)
		{
			luaInputManager.OnDestroy();
			luaInputManager = null;
		}

		if(luaManager != null)
		{
			luaManager.OnDestroy();
			luaManager = null;
		}

		if(luaGameManagerObj != null)
		{
			GameObject.Destroy(luaGameManagerObj);
			luaGameManagerObj = null;
		}
	}

	public bool CheckAvailable()
	{
		return luaManager != null && luaManager.l != null & LuaSvr.mainState != null;
	}

	public LuaState GetState()
	{
		return LuaSvr.mainState;
	}

	private void OnFinish()
	{
		EventManager.Instance ().PostEvent (EventConfig.LUA_MANAGER_INIT_FINISH, null);
	}
}