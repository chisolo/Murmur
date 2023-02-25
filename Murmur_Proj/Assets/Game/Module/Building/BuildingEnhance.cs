using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingEnhance
{
    public string id;
    public EnhanceConfig cfg;
    public int level;
    public float value;
    private BuildingData _buildingData;
    
    public BuildingEnhance(string enhanceId, EnhanceConfig enhanceCfg, int enhanceLevel, BuildingData buildingData)
    {
        id = enhanceId;
        cfg = enhanceCfg;
        level = enhanceLevel;
        _buildingData = buildingData;
        value = level * cfg.step;
    }

    public bool IsMax()
    {
        return level >= cfg.max_level;
    }
    public float LevelRatio()
    {
        return (float)level / cfg.max_level;
    }
    public void SetLevel(int lvl)
    {
        if(lvl > cfg.max_level) return;
        level = lvl;
        value = level * cfg.step;
    }
    public int LvlupCost()
    {
        if(level >= cfg.max_level) return -1;
        return (int)(cfg.lvlup_cost[level] * (1 - _buildingData.GetProp(BuildingProperty.EnhanceLvlUpDiscount)));
    }

    public float NextValue()
    {
        if(level < cfg.max_level) return (level+1) * cfg.step;
        return level * cfg.step;
    }
}
