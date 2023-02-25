using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ZoneItemConfig
{
    public string id; 
    public string name;
    public string desc;
    public string icon;
    public Vector3 zoom_pos; 
    public float zoom_dis;
    public bool is_init;
    public int max_level;
    public string mod;
    public bool is_coin;
    public string property;
    public float step;
    public List<float> lvlup_cost;
    public List<int> room_levels;
}
public class MillstoneBonus
{
    public string item1;
    public int amount1;
    public string item2;
    public int amount2;
    public string item3;
    public int amount3;
}
public class ZoneConfig
{
    public string id; // 房间ID
    public string service;
    public string name;
    public string desc;
    public string small_icon;
    public string previewIcon;
    public Vector3 zoom_pos;
    public float zoom_dis;
    public Vector3 cameraPos; 
    public float cameraSize;
    public int init_level;
    public int max_level;
    public List<ZoneRequire> zone_require;
    public List<ZoneRequire> talent_require;
    public List<MillstoneBonus> bonus;
    public List<float> lvlup_duration;
    public List<float> lvlup_cost;
    public float service_effy;
    public float revenue;
    public float waiting_timeout;
    public List<int> guest_rate;
    public List<float> guest_effy;
    public string product1;
    public string product2;
    public string product3;
    public List<int> product_1_require;
    public List<int> product_2_require;
    public List<int> product_3_require;
    public bool auto_staff;
    public List<ZoneItemConfig> items;
}