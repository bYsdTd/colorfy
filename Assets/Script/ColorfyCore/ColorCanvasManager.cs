using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorCanvasManager
{
    Color[]     color_cache;
    int         width;
    int         height;

    UITexture   canvas_texture;

    GameObject  canvas_obj;

    GameObject  canvas_template;

    public void Init()
    {
        EventManager.Instance().RegisterEvent(EventConfig.EVENT_SCENE_CLICK_DOWN, OnClickScreenPositon);
    }

    public void Destroy()
    {

    }

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

        // todo 释放图片资源
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
	public void ColorfyRegion(int x, int y, Color front_color)
    {
        // if(x < 0 || x >= width || y < 0 || y >= height)
        // {
        //     return;
        // }

        // int color_index = x + y * width;

        // Color current_color = color_cache[color_index];

        // if(current_color == Color.white)
        // {
        //     color_cache[color_index] = front_color;

        //     ColorfyRegion(x-1, y, front_color);
        //     ColorfyRegion(x, y-1, front_color);
        //     ColorfyRegion(x+1, y, front_color);
        //     ColorfyRegion(x, y+1, front_color);
        // }
        // else
        // {
        //     // Debug.Log(current_color);

        //     return;
        // }

        if(x < 0 || x >= width || y < 0 || y >= height)
        {
            return;
        }

        Queue<Vector2i>  open_list = new Queue<Vector2i>();
        HashSet<Vector2i> close_list = new HashSet<Vector2i>();
        
        open_list.Enqueue(new Vector2i(x, y));

        while(open_list.Count > 0)
        {
            // 处理当前节点
            Vector2i current_node = open_list.Dequeue();

            if(close_list.Contains(current_node))
            {
                continue;
            }

            close_list.Add(current_node);

            int color_index = current_node.x + current_node.y * width;

            Color current_color = color_cache[color_index];
            color_cache[color_index] = front_color;

            // 周围的加入队列
            Vector2i[] neighbor = new Vector2i[4];
            
            neighbor[0] = new Vector2i(current_node.x-1, current_node.y);
            neighbor[1] = new Vector2i(current_node.x, current_node.y+1);
            neighbor[2] = new Vector2i(current_node.x+1, current_node.y);
            neighbor[3] = new Vector2i(current_node.x, current_node.y-1);

            for(int i = 0; i < neighbor.Length; ++i)
            {
                Vector2i node = neighbor[i];
                if(!close_list.Contains(node) && !open_list.Contains(node) && !IsBorder(node))
                {
                    open_list.Enqueue(node);
                }
            }
        }

        RefreshUI();
    }

    // 判断填充颜色的边界函数
    private bool IsBorder(Vector2i node)
    {
        int x = node.x;
        int y = node.y;

        if(x < 0 || x >= width || y < 0 || y >= height)
        {
            return true;
        }

        int color_index = x + y * width;

        Color current_color = color_cache[color_index];

        float H;
        float V;
        float S;

        Color.RGBToHSV(current_color, out H, out S, out V);

        return S==0 && V < 0.7f;
    }

    // 点击画布的处理, 传入的是屏幕坐标
    public void OnClickScreenPositon(params System.Object[] all_params)
    {
        Vector2 screen_pos = (Vector2)all_params[0];

        Vector3 ui_position = GUIManager.Instance().ScreenPosToUIPos(new Vector3(screen_pos.x, screen_pos.y, 0));

        Vector2 current_left_bottom = new Vector2(-canvas_texture.width/2, -canvas_texture.height/2);

        Vector2 current_top_right = new Vector2(canvas_texture.width/2, canvas_texture.height/2);

        Vector2 click_pos = new Vector2(ui_position.x, ui_position.y);

        if(click_pos.x >= current_left_bottom.x
            && click_pos.x <= current_top_right.x
            && click_pos.y >= current_left_bottom.y
            && click_pos.y <= current_top_right.y)
        {
            int x = (int)((click_pos.x - current_left_bottom.x) * width / canvas_texture.width);
            int y = (int)((click_pos.y - current_left_bottom.y) * height / canvas_texture.height);

            ColorfyRegion(x, y, Color.red);
        }
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