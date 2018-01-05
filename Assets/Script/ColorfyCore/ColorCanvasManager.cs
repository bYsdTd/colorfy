using UnityEngine;
using System.Collections;


public class ColorCanvasManager
{
    Color[]     color_cache;
    int         width;
    int         height;

    UITexture   canvas_texture;

    GameObject  canvas_obj;

    GameObject  canvas_template;

    // 从模板加载原始的图片，或者是已经涂了色的图
    public void LoadColorTemplate(string path)
    {
        Texture2D source_image = Resources.Load<Texture2D>(path);
        color_cache = source_image.GetPixels();
        width = source_image.width;
        height = source_image.height;

        canvas_template = Resources.Load<GameObject>("canvas_layout");
        canvas_obj = GameObject.Instantiate(canvas_template);
        canvas_obj.transform.SetParent(GUIManager.Instance().cache_root, false); 

        canvas_texture = canvas_obj.GetComponentInChildren<UITexture>();

        RefreshUI();

        // 释放图片资源
    }

    // 刷新内存到UI的显示上
    public void RefreshUI()
    {
        Texture2D new_texture = new Texture2D(width, height);
        new_texture.SetPixels(color_cache);
        new_texture.Apply();
        
        canvas_texture.mainTexture = new_texture;
    }

    // x, y 为像素的坐标，屏幕点击的坐标由用户输入相关的模块去处理转换成像素的坐标
	public void ColorfyRegion(int x, int y, Color front_color, Color back_color)
    {

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