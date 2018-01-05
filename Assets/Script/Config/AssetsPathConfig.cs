using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetsPathConfig 
{
	public static Dictionary<string, string> assets_path_config = new Dictionary<string, string>()
	{
		{"UIRoot", "UI/UIRoot"},
		{"ToonSoldier", "HeroUnit/Soldier/ToonSoldier"},
		{"BigTank", "HeroUnit/Tank/BigTank"},
		{"BigTank2", "HeroUnit/Tank/BigTank2"},
		{"rocket_tank", "HeroUnit/RocketTank/rocket_tank"},
		{"rocket_tank2", "HeroUnit/RocketTank/rocket_tank2"},
		{"Stealth_Bomber", "HeroUnit/B2/Prefabs/Stealth_Bomber"},
		{"war_plane_1", "HeroUnit/Aircraft/war_plane_1"},
		{"base1", "BuildingUnit/base1"},
		{"base2", "BuildingUnit/base2"},
		{"UnitSelectCircle", "Effect/UnitSelectCircle"},
		{"fire_effect", "Effect/fire_effect"},
		{"hit_effect1", "Effect/hit_effect1"},
		{"hit_effect2", "Effect/hit_effect2"},
		{"hit_effect3", "Effect/hit_effect3"},
		{"Missil_01", "Effect/Bullet/Missil_01"},
		{"move_effect", "Effect/move_effect"},
		{"mat_line", "Grid/mat_line"},
		{"BloodHud", "UI/blood_hud"},
		{"OccupyHud", "UI/occupy_hud"},
		{"ResourceHud", "UI/resource_hud"},
		{"hero_operate", "UI/hero_operate_layout"},
		{"battle_result", "UI/battle_result_layout"},
		{"join_room", "UI/join_room_layout"},
		{"LobbyLayout", "UI/Lobby/LobbyLayout"},
		{"BattleLoadingLayout", "UI/Lobby/BattleLoadingLayout"},
		{"FindBattleLayout", "UI/Lobby/FindBattleLayout"},
		{"LoginLayout", "UI/Lobby/LoginLayout"},

		// 音效
		{"gun_fire_1", "Sound/gun_fire_1"},
		{"tank_fire_1", "Sound/tank_fire_1"},
		{"aircraft_fire_1", "Sound/aircraft_fire_1"},
		{"cannon_fire_1", "Sound/cannon_fire_1"},
	};
}
