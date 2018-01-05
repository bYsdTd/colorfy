using UnityEngine;
using System.Collections;

public class ShaderManager
{
	static private ShaderManager instance = null;

	static public ShaderManager Instance()
	{
		if(instance == null)
		{
			instance = new ShaderManager();
		}

		return instance;
	}

	bool inited = false;

	public Shader transparent_colored_shader_;
	public Shader grey_color_shader_;
	public Shader text_shader_;

	public void Init()
	{
		if (inited)
			return;
			
		transparent_colored_shader_ = Resources.Load<Shader>("Shader/Unlit - Transparent Colored");
		grey_color_shader_ = Resources.Load<Shader>("Shader/GreyColor");
		text_shader_ = Resources.Load<Shader>("Shader/Unlit - Text");

		inited = true;
	}

	public void Destroy()
	{

	}
}
