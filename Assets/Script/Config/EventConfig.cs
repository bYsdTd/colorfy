using UnityEngine;
using System.Collections;

public class EventConfig  
{
	// 场景输入事件
	public static string EVENT_SCENE_CLICK_DOWN = "EVENT_SCENE_CLICK_DOWN";
	public static string EVENT_SCENE_CLICK_UP = "EVENT_SCENE_CLICK_UP";
	public static string EVENT_SCENE_DOUBLE_CLICK = "EVENT_SCENE_DOUBLE_CLICK";
	public static string EVENT_SCENE_CLICK_MOVE = "EVENT_SCENE_CLICK_MOVE";
	public static string EVENT_SCENE_SCROLL_CHANGED = "EVENT_SCENE_SCROLL_CHANGED";
	public static string EVENT_SCENE_SELECT_UNIT = "EVENT_SCENE_SELECT_UNIT";
	public static string EVENT_SCENE_UNIT_SELECT_CHANGE = "EVENT_SCENE_UNIT_SELECT_CHANGE";
	public static string EVENT_SCENE_FOCUES_POSTION = "EVENT_SCENE_FOCUES_POSTION"; // 聚焦一个位置

	// 战斗逻辑层到表现层的消息
	public static string EVENT_L2R_START_MOVE = "EVENT_L2R_START_MOVE";
	public static string EVENT_L2R_END_MOVE = "EVENT_L2R_END_MOVE";

	public static string EVENT_L2R_PLAY_ATTACK = "EVENT_L2R_PLAY_ATTACK";
	public static string EVENT_L2R_PLAY_HIT = "EVENT_L2R_PLAY_HIT";
	public static string EVENT_L2R_PLAY_DEAD = "EVENT_L2R_PLAY_DEAD";
	public static string EVENT_L2R_PLAY_REINIT = "EVENT_L2R_PLAY_REINIT"; // 不播放死亡的重新初始化

	public static string EVENT_L2R_TEAM_ID_CHANGE = "EVENT_L2R_TEAM_ID_CHANGE";

	public static string EVENT_L2R_BULLET_START = "EVENT_L2R_BULLET_START";
	public static string EVENT_L2R_BULLET_END = "EVENT_L2R_BULLET_END";

	// UI 事件
	public static string EVENT_SCREEN_SIZE_CHANGED = "EVENT_SCREEN_SIZE_CHANGED";
	public static string EVENT_UI_OPEN = "EVENT_UI_OPEN";
	public static string EVENT_UI_CLOSE = "EVENT_UI_CLOSE";

	// 逻辑层数据事件
	public static string EVENT_LOGIC_GOLD_CHANGED = "EVENT_LOGIC_GOLD_CHANGED";

	// Lua 相关
	public static string LUA_MANAGER_INIT_FINISH = "LUA_MANAGER_INIT_FINISH";

}
