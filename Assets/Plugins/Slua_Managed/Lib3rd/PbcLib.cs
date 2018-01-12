using System.Collections.Generic;

namespace SLua
{
	//pbc lib
	public class PbcLib
	{
		public static void Reg(Dictionary<string, LuaCSFunction> reg_functions)
		{
			reg_functions.Add("protobuf.c", LuaDLL.luaopen_protobuf_c);
			reg_functions.Add("lpeg", LuaDLL.luaopen_lpeg);
		}
	}
}