using System;
using System.Collections.Generic;
namespace SLua {
	[LuaBinder(3)]
	public class BindCustom {
		public static Action<IntPtr>[] GetBindList() {
			Action<IntPtr>[] list= {
				Lua_LuaResMananger.reg,
				Lua_LuaUtils.reg,
				Lua_LuaEventManager.reg,
				Lua_LuaNetWorkManager.reg,
				Lua_UIEventListener.reg,
				Lua_UIPopupList.reg,
				Lua_UIInput.reg,
				Lua_UIProgressBar.reg,
				Lua_UILabel.reg,
			};
			return list;
		}
	}
}
