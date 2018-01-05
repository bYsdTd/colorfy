using UnityEngine;
using System.Collections;


public class Timer
{
	public int	id { set; get; }
	public float interval { set; get; }
	public float delayTime { set; get; }

	// 触发次数和持续时间，两个是互斥的
	public int triggerTimes { set; get; }
	public float duration { set; get; }

	public bool markToDelete { set; get; }
	public bool started { set; get; }


	public TimerManager.OnBeginTimerDelegate 	OnBeginCallBack;
	public TimerManager.OnEndTimerDelegate		OnEndCallBack;
	public TimerManager.TimerTickDelegate		OnTick;

	// 剩余触发次数，如果是0的话，就是没有限制
	private int 	remainTimes;
	// 剩余延迟启动时间
	private float 	delayRemainTime;
	// 检查tick的时间戳
	private float	elapsedTime;
	// 
	private float	wholeTime;

	public void Init()
	{
		delayRemainTime = delayTime;
		wholeTime = 0;
	}

	// 返回是否计时器结束
	public bool Tick(float dt)
	{
		if(markToDelete)
		{
			TriggerEnd();
			return true;
		}

		// check delay start
		if(started == false)
		{
			if(delayTime > 0)
			{
				if(delayRemainTime > 0)
				{
					delayRemainTime -= dt;
				}
				else
				{
					return TriggerStart();
				}
			}
			else
			{
				return TriggerStart();
			}	
		}
		else
		{
			if(duration > 0)
			{
				wholeTime += dt;
				if(wholeTime > duration)
				{
					TriggerEnd();
					return true;
				}
			}

			elapsedTime += dt;

			if(elapsedTime >= interval)
			{
				// already started
				if(triggerTimes > 0)
				{
					// 有触发次数限制
					if(remainTimes > 0)
					{
						TriggerTick(elapsedTime);

						if(remainTimes <= 0)
						{
							TriggerEnd();

							return true;
						}
					}
				}
				else
				{
					// 无限次触发
					TriggerTick(elapsedTime);
				}

				elapsedTime = 0;
			}
		}

		return false;
	}

	private bool TriggerStart()
	{
		started = true;

		// 剩余触发次数，如果是-1的话，就是没有限制
		if(triggerTimes > 0)
		{
			remainTimes = triggerTimes;
		}
		else
		{
			remainTimes = 0;
		}

		elapsedTime = 0;

		delayRemainTime = 0;

		if(OnBeginCallBack != null)
		{
			OnBeginCallBack();	
		}

		// 启动的时候也要触发一次
		TriggerTick(0);

		// 有触发次数限制， 并且只有一次
		if(triggerTimes > 0 && remainTimes <= 0)
		{
			TriggerEnd();

			return true;
		}
		else
		{
			return false;
		}
	}

	public void TriggerEnd()
	{
		started = false;

		if(OnEndCallBack != null)
		{
			OnEndCallBack();	
		}

	}

	private void TriggerTick(float dt)
	{
		if(triggerTimes > 0 && remainTimes > 0)
		{
			--remainTimes;	
		}

		if(OnTick != null)
		{
			OnTick(dt);
		}
	}
}
