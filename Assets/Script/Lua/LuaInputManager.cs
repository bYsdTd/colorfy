using UnityEngine;
using System.Collections;
using SLua;

public class LuaInputManager : MonoBehaviour {
	public void Init ()
	{
	}

	public void OnDestroy ()
	{
	}

	void Update () 
	{
		#if UNITY_EDITOR
		if (!LuaGameManager.Instance().CheckAvailable())
			return;

		if(Input.GetMouseButtonDown(0))
		{
			LuaFunction func = LuaGameManager.Instance().GetState().getFunction("HandleTouchDown");

			if(func != null)
			{
				func.call(Input.mousePosition.x, Input.mousePosition.y);
			}
		}

		if(Input.GetMouseButton(0))
		{
			LuaFunction func = LuaGameManager.Instance().GetState().getFunction("HandleTouchMove");
			if(func != null)
			{
				func.call(Input.mousePosition.x, Input.mousePosition.y);	
			}
		}

		if(Input.GetMouseButtonUp(0))
		{
			LuaFunction func = LuaGameManager.Instance().GetState().getFunction("HandleTouchUp");
			if(func != null)
			{
				func.call(Input.mousePosition.x, Input.mousePosition.y);	
			}
		}

		#elif (UNITY_ANDROID || (UNITY_IOS || UNITY_IPHONE))
		if (Input.touchCount > 0) {

			Vector2 touchPosition = Input.GetTouch(0).position;

			if(Input.GetTouch(0).phase == TouchPhase.Began)
			{
				LuaFunction func = LuaGameManager.Instance().GetState().getFunction("HandleTouchDown");
				if(func != null)
				{
					func.call(touchPosition.x, touchPosition.y);	
				}
			}
			else if(Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				LuaFunction func = LuaGameManager.Instance().GetState().getFunction("HandleTouchMove");
				if(func != null)
				{
					func.call(touchPosition.x, touchPosition.y);	
				}
			}
			else if(Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				LuaFunction func = LuaGameManager.Instance().GetState().getFunction("HandleTouchUp");
				if(func != null)
				{
					func.call(touchPosition.x, touchPosition.y);	
				}
			}
		}
		#endif
	}
}
