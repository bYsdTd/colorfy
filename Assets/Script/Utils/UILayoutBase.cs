using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILayoutBase  
{
	public GUIManager.ui_config	config;

	public bool	is_show { set; get; }

	public GameObject	game_obj = null;

	virtual public void OnInit(object[] all_params)
	{
		
	}

	virtual public void OnDestroy()
	{
		
	}

	public GameObject GetChild(string name)
	{
		if(game_obj)
		{
			Transform transform = game_obj.transform.Find(name);
			if(transform)
			{
				return transform.gameObject;
			}
			else
			{
				return null;
			}
		}
		else
		{
			return null;
		}
	}
}
