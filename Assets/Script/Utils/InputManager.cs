using UnityEngine;
using System.Collections;

public class InputManager  
{
	static private InputManager instance = null;

	static public InputManager Instance()
	{
		if(instance == null)
		{
			instance = new InputManager();
			Input.multiTouchEnabled = true;
		}

		return instance;
	}

	private Vector2 last_touch_down_position = Vector2.zero;
	private Vector2 last_touch_move_position = Vector2.zero;

	private int		pre_touch_up_time_stamp = System.Environment.TickCount;
	private Vector2 pre_touch_up_position = Vector2.zero;

	public void Tick(float delta_time)
	{
		#if UNITY_EDITOR
		if(Input.GetMouseButton(0))
		{
			Vector2 mouse_pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

			if(Input.GetMouseButtonDown(0))
			{
				last_touch_down_position = mouse_pos;
				last_touch_move_position = last_touch_down_position;
				HandleTouchBegan(last_touch_down_position);
			}
			else
			{
				Vector2 delta_position = mouse_pos - last_touch_move_position;
				last_touch_move_position = mouse_pos;

				HandleTouchMove(mouse_pos, delta_position);
			}

		}
		else if(Input.GetMouseButtonUp(0))
		{
			Vector2 mouse_pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			HandleTouchEnded(mouse_pos);
		}

		if(Input.mouseScrollDelta.y != 0)
		{
			// 模拟缩放的操作
			HandleScrollChanged(Input.mouseScrollDelta.y);
		}
		#else
		if(Input.touchCount == 1)
		{
			Touch input_touch = Input.GetTouch(0);

			if(input_touch.phase == TouchPhase.Began)
			{
				HandleTouchBegan(input_touch.position);
			}

			if(input_touch.phase == TouchPhase.Moved)
			{
				HandleTouchMove(input_touch.position, input_touch.deltaPosition);
			}

			if(input_touch.phase == TouchPhase.Ended)
			{
				HandleTouchEnded(input_touch.position);
			}	
		}else if (Input.touchCount == 2) 
		{
			Touch touch_0 = Input.GetTouch(0);
			Touch touch_1 = Input.GetTouch(1);

			Vector2 touch_0_pre_pos = touch_0.position - touch_0.deltaPosition;
			Vector2 touch_1_pre_pos = touch_1.position - touch_1.deltaPosition;

			float pre_touch_delta_mag = (touch_0_pre_pos - touch_1_pre_pos).magnitude;
			float touch_delta_mag = (touch_0.position - touch_1.position).magnitude;

			float delta_mag_diff = pre_touch_delta_mag - touch_delta_mag;
			
			HandleScrollChanged(delta_mag_diff * -0.02f);
		}
		#endif
	}

	private bool IsInputHandledByUI(Vector2 touch_position)
	{
		return UICamera.Raycast(new Vector3(touch_position.x, touch_position.y, 0));
	}

	private void HandleTouchBegan(Vector2 touch_position)
	{
		if(IsInputHandledByUI(touch_position))
		{
			return;
		}

		EventManager.Instance().PostEvent(EventConfig.EVENT_SCENE_CLICK_DOWN, new object[]{touch_position});
	}
	// 单指拖动
	private void HandleTouchMove(Vector2 touch_position, Vector2 delta_position)
	{
		EventManager.Instance().PostEvent(EventConfig.EVENT_SCENE_CLICK_MOVE, new object[]{delta_position, touch_position});
	}

	private void HandleTouchEnded(Vector2 touch_position)
	{
		if(IsInputHandledByUI(touch_position))
		{
			return;
		}

		int double_click_time = (System.Environment.TickCount - pre_touch_up_time_stamp);

		if(double_click_time < 500 && (touch_position - pre_touch_up_position).sqrMagnitude < 500)
		{
			EventManager.Instance().PostEvent(EventConfig.EVENT_SCENE_DOUBLE_CLICK, new object[]{touch_position});
			pre_touch_up_time_stamp = 0;
		}
		else
		{
			EventManager.Instance().PostEvent(EventConfig.EVENT_SCENE_CLICK_UP, new object[]{touch_position});
			pre_touch_up_time_stamp = System.Environment.TickCount;
			pre_touch_up_position = touch_position;
		}
	}

	private void HandleScrollChanged(float delta)
	{
		EventManager.Instance().PostEvent(EventConfig.EVENT_SCENE_SCROLL_CHANGED, new object[]{ delta });
	}
}
