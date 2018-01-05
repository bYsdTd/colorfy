using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager 
{
	static private EventManager instance = null;

	static public EventManager Instance()
	{
		if(instance == null)
		{
			instance = new EventManager();
		}

		return instance;
	}

	public delegate void EventHandler(params System.Object[] all_params);

	Dictionary<string, List<EventHandler>> event_handlers = new Dictionary<string, List<EventHandler>>();

	public void RegisterEvent(string event_name, EventHandler event_handler)
	{
		if(!event_handlers.ContainsKey(event_name))
		{
			event_handlers.Add(event_name, new List<EventHandler>());
		}

		List<EventHandler> handlers = event_handlers[event_name];

		handlers.Add(event_handler);
	}

	public void UnRegisterEvent(string event_name, EventHandler event_hander)
	{
		if(event_handlers.ContainsKey(event_name))
		{
			List<EventHandler> handlers = event_handlers[event_name];

			for(int i = 0; i < handlers.Count; ++i)
			{
				if(handlers[i] == event_hander)
				{
					handlers.RemoveAt(i);
					break;
				}
			}
		}
	}

	public void PostEvent(string event_name, params System.Object[] all_params)
	{
		if(event_handlers.ContainsKey(event_name))
		{
			List<EventHandler> handlers = event_handlers[event_name];

			for(int i = 0; i < handlers.Count; ++i)
			{
				handlers[i].Invoke(all_params);
			}
		}
	}
}
