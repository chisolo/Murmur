using System.Collections.Generic;
using UnityEngine;

public class BuildingConfig
{
    public string id;
    public string type;
    public string service;
    public int init_level;
    public int max_level;
    public List<BuildingRequire> pre_building;
    public List<BuildingRequire> pre_talent;
    public List<float> lvlup_duration;
    public List<float> lvlup_cost;
    public List<float> service_duration;
    public List<float> service_capacity;
    public List<float> capacity;
    public List<float> revenue;
    public List<string> enhance_set;
    public List<int> sausage_require;
    public List<int> cheese_require;
    public List<int> flour_require;
    public string product;
    public float timeout;
    public string name;
    public string desc;
    public string small_icon;
    public string big_icon;
    public string preview_icon;
    public string click_mod;
    public List<string> mods;
    public string waiting_mod;
    public string parent;
    public List<string> children;
    public Vector3 zoom_pos;
    public float zoom_dis;
    public List<int> guest_rate = new List<int>();
    public List<float> guest_interval = new List<float>();
}
