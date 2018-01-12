using System;
using System.Collections.Generic;
namespace SLua {
	[LuaBinder(0)]
	public class BindUnity {
		public static Action<IntPtr>[] GetBindList() {
			Action<IntPtr>[] list= {
				Lua_UnityEngine_Application.reg,
				Lua_UnityEngine_Object.reg,
				Lua_UnityEngine_Component.reg,
				Lua_UnityEngine_Behaviour.reg,
				Lua_UnityEngine_GameObject.reg,
				Lua_UnityEngine_Renderer.reg,
				Lua_UnityEngine_Vector3.reg,
				Lua_UnityEngine_Quaternion.reg,
				Lua_UnityEngine_Mathf.reg,
				Lua_UnityEngine_MonoBehaviour.reg,
				Lua_UnityEngine_PlayerPrefs.reg,
				Lua_UnityEngine_Resources.reg,
				Lua_UnityEngine_Texture2D.reg,
				Lua_UnityEngine_Time.reg,
				Lua_UnityEngine_Transform.reg,
				Lua_UnityEngine_Color.reg,
				Lua_UnityEngine_Mesh.reg,
				Lua_UnityEngine_MeshFilter.reg,
				Lua_UnityEngine_Vector2.reg,
				Lua_UnityEngine_Vector4.reg,
				Lua_UnityEngine_Animator.reg,
				Lua_UnityEngine_BoxCollider.reg,
			};
			return list;
		}
	}
}
