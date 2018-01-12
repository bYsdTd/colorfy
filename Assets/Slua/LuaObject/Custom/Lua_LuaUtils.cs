using System;
using SLua;
using System.Collections.Generic;
[UnityEngine.Scripting.Preserve]
public class Lua_LuaUtils : LuaObject {
	[SLua.MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	[UnityEngine.Scripting.Preserve]
	static public int ReadFileAsString_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=LuaUtils.ReadFileAsString(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[SLua.MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	[UnityEngine.Scripting.Preserve]
	static public int ReadFileAsBytes_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=LuaUtils.ReadFileAsBytes(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[SLua.MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	[UnityEngine.Scripting.Preserve]
	static public int ReadFileAsByteArray_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=LuaUtils.ReadFileAsByteArray(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[SLua.MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	[UnityEngine.Scripting.Preserve]
	static public int PostLuaEvent_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			System.Object[] a2;
			checkParams(l,2,out a2);
			LuaUtils.PostLuaEvent(a1,a2);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[SLua.MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	[UnityEngine.Scripting.Preserve]
	static public int SwitchToLevel_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			LuaUtils.SwitchToLevel(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[UnityEngine.Scripting.Preserve]
	static public void reg(IntPtr l) {
		getTypeTable(l,"LuaUtils");
		addMember(l,ReadFileAsString_s);
		addMember(l,ReadFileAsBytes_s);
		addMember(l,ReadFileAsByteArray_s);
		addMember(l,PostLuaEvent_s);
		addMember(l,SwitchToLevel_s);
		createTypeMetatable(l,null, typeof(LuaUtils));
	}
}
