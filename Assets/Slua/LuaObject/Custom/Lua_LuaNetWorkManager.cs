using System;
using SLua;
using System.Collections.Generic;
[UnityEngine.Scripting.Preserve]
public class Lua_LuaNetWorkManager : LuaObject {
	[SLua.MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	[UnityEngine.Scripting.Preserve]
	static public int SendProto_s(IntPtr l) {
		try {
			System.Int32 a1;
			checkType(l,1,out a1);
			System.Byte[] a2;
			checkBinaryString(l,2,out a2);
			NetworkManager.OnSendOkCallBack a3;
			checkDelegate(l,3,out a3);
			LuaNetWorkManager.SendProto(a1,a2,a3);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[UnityEngine.Scripting.Preserve]
	static public void reg(IntPtr l) {
		getTypeTable(l,"LuaNetWorkManager");
		addMember(l,SendProto_s);
		createTypeMetatable(l,null, typeof(LuaNetWorkManager));
	}
}
