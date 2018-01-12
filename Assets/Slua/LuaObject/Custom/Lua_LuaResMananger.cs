using System;
using SLua;
using System.Collections.Generic;
[UnityEngine.Scripting.Preserve]
public class Lua_LuaResMananger : LuaObject {
	[SLua.MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	[UnityEngine.Scripting.Preserve]
	static public int LoadGameObject_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			System.Boolean a2;
			checkType(l,2,out a2);
			OnAssetLoadedSuccess a3;
			checkDelegate(l,3,out a3);
			OnAssetLoadedFailed a4;
			checkDelegate(l,4,out a4);
			LuaResMananger.LoadGameObject(a1,a2,a3,a4);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[SLua.MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	[UnityEngine.Scripting.Preserve]
	static public int UnloadGameObject_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			UnityEngine.GameObject a2;
			checkType(l,2,out a2);
			System.Boolean a3;
			checkType(l,3,out a3);
			LuaResMananger.UnloadGameObject(a1,a2,a3);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[UnityEngine.Scripting.Preserve]
	static public void reg(IntPtr l) {
		getTypeTable(l,"LuaResMananger");
		addMember(l,LoadGameObject_s);
		addMember(l,UnloadGameObject_s);
		createTypeMetatable(l,null, typeof(LuaResMananger));
	}
}
