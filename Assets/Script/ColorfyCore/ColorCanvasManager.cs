using UnityEngine;
using System.Collections;


public class ColorCanvasManager
{
    Color32[]     color_cache;
    int         width;
    int         height;

    UITexture   canvas_texture;

    GameObject  canvas_obj;

    GameObject  canvas_template;

    // 从模板加载原始的图片，或者是已经涂了色的图
    public void LoadColorTemplate(string path)
    {
        Texture2D source_image = Resources.Load<Texture2D>(path);
        color_cache = source_image.GetPixels32();
        width = source_image.width;
        height = source_image.height;

        canvas_template = Resources.Load<GameObject>("canvas_layout");
        canvas_obj = GameObject.Instantiate(canvas_template);
        canvas_obj.transform.SetParent(GUIManager.Instance().cache_root, false); 

        canvas_texture = canvas_obj.GetComponentInChildren<UITexture>();

        RefreshUI();

        // 释放图片资源

        ColorfyRegion(512, 300, new Color32(255, 0, 0, 255), new Color32(0, 0, 0, 255));

        RefreshUI();
    }

    // 刷新内存到UI的显示上
    public void RefreshUI()
    {
        Texture2D new_texture = new Texture2D(width, height);
        new_texture.SetPixels32(color_cache);
        new_texture.Apply();
        
        canvas_texture.mainTexture = new_texture;
    }

    // x, y 为像素的坐标，屏幕点击的坐标由用户输入相关的模块去处理转换成像素的坐标
	public void ColorfyRegion(int x, int y, Color32 front_color, Color32 back_color)
    {
        if(x < 0 || x >= width || y < 0 || y >= height)
        {
            return;
        }

        int color_index = x + y * width;

        Color32 current_color = color_cache[color_index];

        if((current_color.r != front_color.r ||
            current_color.g != front_color.g ||
            current_color.b != front_color.b ) && 
            current_color.r >125 && current_color.g > 125 && current_color.b > 125)
        {
            color_cache[color_index] = front_color;

            ColorfyRegion(x-1, y, front_color, back_color);
            ColorfyRegion(x+1, y, front_color, back_color);
            ColorfyRegion(x, y-1, front_color, back_color);
            ColorfyRegion(x, y+1, front_color, back_color);
        }
        else
        {
            // Debug.Log(current_color);
        }
    }

    // 点击画布的处理, 传入的是屏幕坐标
    public void OnClickScreenPositon(int x, int y)
    {

    }

    //
    public void OnDragStart()
    {

    }

    public void OnDragEnd()
    {

    }

    // 参数是屏幕坐标距离上次的相对变化
    public void OnDragging(Vector2 delta_position)
    {

    }

    // 手指缩放图片
    public void OnPinch(float delta_pinch77)
    {

    }

}