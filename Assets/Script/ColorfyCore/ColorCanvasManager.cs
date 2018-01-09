using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class ColorCanvasManager : MonoBehaviour
{
    Color[]     color_cache;
    int         width;
    int         height;

    UITexture   canvas_texture;

    GameObject  canvas_obj;

    GameObject  canvas_template;

    bool        is_coloring;

    int         timer_id;
    
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {    
		Init();
		
		LoadColorTemplate("flower");
    }

    public void Init()
    {
        
        EventManager.Instance().RegisterEvent(EventConfig.EVENT_SCENE_CLICK_DOWN, OnClickScreenPositon);
    }

    public void Destroy()
    {
        EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_SCENE_CLICK_DOWN, OnClickScreenPositon);
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

        is_coloring = false;
        timer_id = -1;
    }

    // 刷新内存到UI的显示上
    public void RefreshUI()
    {
        Texture2D new_texture = new Texture2D(width, height);
        new_texture.SetPixels(color_cache);
        new_texture.Apply();
        
        canvas_texture.mainTexture = new_texture;
    }

    // struct ColorThreadParam
    // {
    //     public int x;
    //     public int y;
    //     public Color front_color;

    //     public ColorThreadParam(int x, int y, Color front_color)
    //     {
    //         this.x = x;
    //         this.y = y;
    //         this.front_color = front_color;
    //     }
    // }

    // void ColorfyThread(object param)
    // {
    //     ColorThreadParam colorfy_param = (ColorThreadParam)param;
    //     ColorfyRegion(colorfy_param.x, colorfy_param.y, colorfy_param.front_color);

    //     is_coloring = false;
    // }

    // x, y 为像素的坐标，屏幕点击的坐标由用户输入相关的模块去处理转换成像素的坐标
	IEnumerator ColorfyRegion(int x, int y, Color front_color)
    {
        Queue<Vector2i>  open_list = new Queue<Vector2i>();
        HashSet<Vector2i> close_list = new HashSet<Vector2i>();

        Vector2i[] neighbor = new Vector2i[4];

        if(x < 0 || x >= width || y < 0 || y >= height)
        {
            yield break;
        }

        open_list.Clear();
        close_list.Clear();
        
        open_list.Enqueue(new Vector2i(x, y));
        
        int time_stamp = System.Environment.TickCount;

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

            int elapsed = System.Environment.TickCount - time_stamp;

            if(elapsed > 1000)
            {
                RefreshUI();
                time_stamp = System.Environment.TickCount;
                yield return new WaitForSeconds(1.0f);
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

            //ColorfyRegion(x, y, Color.red);

            // if(!is_coloring)
            // {
            //     is_coloring = true;

            //     Thread color_thread = new Thread(ColorfyThread);
            //     color_thread.IsBackground = true;
            //     color_thread.Priority = System.Threading.ThreadPriority.Lowest;
            //     color_thread.Start(new ColorThreadParam(x, y, Color.red));
            //     timer_id = TimerManager.Instance().RepeatCallFunc(delegate (float dt){
                    
            //         RefreshUI();

            //     }, 1);

            // }

            StartCoroutine(ColorfyRegion(x, y, Color.red));
        }
    }

    // public void Update(float delta_time) 
    // {
    //     Debug.Log("M");
    //     if(!is_coloring && timer_id > 0)
    //     {
    //         TimerManager.Instance().DestroyTimer(timer_id);

    //         timer_id = -1;

    //         RefreshUI();
    //     }
    // }

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