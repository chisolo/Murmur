using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneItemData
{
    public string ID => _itemConfig.id;
    public string Property => _itemConfig.property;
    public float Value => _value;
    public float NextValue => _nextValue;
    public ZoneItemConfig Config => _itemConfig;
    private ZoneData _zoneData;
    private ZoneItemConfig _itemConfig;
    private int _level;
    private float _value;
    private float _nextValue;
    private int _curMinLevel;
    private int _curMaxLevel;
    public ZoneItemData(ZoneData zoneData, ZoneItemConfig itemConfig, int level)
    {
        _zoneData = zoneData;
        _itemConfig = itemConfig;
        _level = level;
        _value = _level * _itemConfig.step;
        _nextValue = (_level >= _itemConfig.max_level ? _itemConfig.max_level : _level + 1) * _itemConfig.step;
        RefreshLevelRatio();
    }
    public bool IsMax()
    {
        return _level >= _itemConfig.max_level;
    }
    private void RefreshLevelRatio()
    {
        _curMinLevel = 0;
        _curMaxLevel = 0;
        var zoneLevel = _zoneData.Level;
        if(zoneLevel == 0) return;
        foreach(var unlockLevel in _itemConfig.room_levels) {
            if(unlockLevel < zoneLevel) {
                _curMaxLevel++;
                _curMinLevel++;
            }
            if(unlockLevel == zoneLevel) {
                _curMaxLevel++;
            }
        }
    }
    public int TotalLevelRato()
    {
        return _curMaxLevel - _curMinLevel;
    }
    public int CurLevelRatio()
    {
        return _level - _curMinLevel;
    }
    public bool IsLevelRatioMax()
    {
        return _level == _curMaxLevel;
    }
    public void Upgrade(int level)
    {
        _level = level;
    }
    public string GetMod()
    {
        return _itemConfig.mod;
    }
    public int LvlupCost()
    {
        if(_level >= _itemConfig.max_level) return 0;
        return (int)(_itemConfig.lvlup_cost[_level] * (1 - _zoneData.GetProp(ZoneProperty.ItemLvlupDiscount)));
    }
}
