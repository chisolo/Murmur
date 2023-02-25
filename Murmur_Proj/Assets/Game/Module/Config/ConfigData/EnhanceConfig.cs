using System.Collections.Generic;

public class EnhanceConfig
{
    public string id;
    public int max_level;
    public string enhance_effect;
    public float step;
    public List<int> lvlup_cost;
    public int ratio;
    public string name;
    public string desc;
    public string icon;
    public float MaxValue()
    {
        return max_level* step;
    }
}
