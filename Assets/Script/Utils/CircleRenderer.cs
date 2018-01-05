using UnityEngine;
using System.Collections;

public class CircleRenderer : MonoBehaviour 
{
	public LineRenderer line_renderer;

	// Use this for initialization
	public void Init (Material line_mat) 
	{
		line_renderer = gameObject.GetOrAddComponent<LineRenderer>();
		line_renderer.material = line_mat;
		line_renderer.startWidth = 0.05f;
		line_renderer.endWidth = 0.05f;

		line_renderer.enabled = false;

	}

	public void SetColor(Color color)
	{
		line_renderer.startColor = color;
		line_renderer.endColor = color;
	}

	public void SetCircle(Vector3 center, float radius, int segement_count = 64)
	{
		line_renderer.enabled = true;

		line_renderer.positionCount = segement_count + 1;//设置线的段数

		float x;
		float y;

		for (int i = 0; i < segement_count+1; ++i) 
		{
			x = Mathf.Sin ((360f * i / segement_count) * Mathf.Deg2Rad) * radius;//横坐标
			y = Mathf.Cos ((360f * i / segement_count) * Mathf.Deg2Rad) * radius;//纵坐标

			line_renderer.SetPosition (i, center + new Vector3 (x, 0, y));
		}
	}

}
