using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimerManager 
{
	static private TimerManager instance = null;

	static public TimerManager Instance()
	{
		if(instance == null)
		{
			instance = new TimerManager();
		}

		return instance;
	}

	static int timerIndex = 0;

	public delegate void TimerTickDelegate(float dt);
	public delegate void OnBeginTimerDelegate();
	public delegate void OnEndTimerDelegate();

	List<Timer> timerList = new List<Timer>();

	public void Init()
	{
		
	}

	public void Destroy()
	{
		for(int i = 0; i < timerList.Count; ++i)
		{
			timerList[i].TriggerEnd();
		}

		timerList.Clear();
	}

	public int DelayCallFunc(TimerTickDelegate tickFunc, float delayTime)
	{
		return StartTimer(tickFunc, 0.0f, delayTime, 1, 0, null, null);
	}

	// triggerTimes 和 duration 不能同时大于0
	// 如果同时，那么哪个先到，哪个起作用
	public int RepeatCallFunc(TimerTickDelegate tickFunc, float interval, float duration = 0, OnBeginTimerDelegate beginFunc = null, OnEndTimerDelegate endFunc = null)
	{
		return StartTimer(tickFunc, interval, 0.0f, 0, duration, beginFunc, endFunc);
	}

	private int StartTimer(TimerTickDelegate tickFunc, float interval, float delayTime, int triggerTimes, float duration, OnBeginTimerDelegate beginFunc, OnEndTimerDelegate endFunc)
	{
		++timerIndex;

		Timer timer = new Timer();

		timer.id = timerIndex;
		timer.interval = interval;
		timer.triggerTimes = triggerTimes;
		timer.delayTime = delayTime;
		// duration 0 表示一直触发
		timer.duration = duration;

		timer.OnBeginCallBack = beginFunc;
		timer.OnEndCallBack = endFunc;
		timer.OnTick = tickFunc;

		timer.Init();

		timerList.Add(timer);

		return timerIndex;
	}

	public void DestroyTimer(int id)
	{
		for(int i = 0; i < timerList.Count; ++i)
		{
			Timer timer = timerList[i];
			if(id == timer.id)
			{
				timer.markToDelete = true;
				break;
			}
		}
	}

	public void Tick(float dt)
	{
		for(int i = timerList.Count-1;i >= 0 ; i--)
		{
			Timer timer = timerList[i];

			if(timer.Tick(dt))
			{
				// delete
				timerList.RemoveAt(i);
			}
		}

	}
}
