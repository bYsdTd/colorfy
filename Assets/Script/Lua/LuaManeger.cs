using UnityEngine;
using System.Collections;
using SLua;

public class LuaManager : MonoBehaviour {

	public LuaSvr l;
	private System.Action hdlOnFinish; // lua 的初始化是异步的，需要一个完成的回调

	public void Init (System.Action hdlOnFinish) 
	{
		Debug.Log("luaManager start");
		this.hdlOnFinish = hdlOnFinish;
		l = new LuaSvr();
		// 自定义加载方法
		LuaSvr.mainState.loaderDelegate = LoadFunc;
		
		l.init(null, complete);
	}

	public void OnDestroy()
	{
		LuaSvr.mainState = null;
		l = null;
	}

	void complete()
	{
		l.start("luaMain");
		hdlOnFinish.Invoke ();
		Debug.Log("compolete start main");
	}

	void tick(int p)
	{
		int progress = p;
		Debug.Log(progress);
	}

	byte[] LoadFunc(string fn){
		Debug.Log (fn);

		string fileName = "/" + fn.Replace (".", "/");

		return LuaUtils.ReadFileAsBytes(fileName);
	}
}