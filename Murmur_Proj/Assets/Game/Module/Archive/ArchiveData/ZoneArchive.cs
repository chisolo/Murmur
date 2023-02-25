using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneArchiveData
{
    public string id;
    public int level;
    public string staff;
    public Dictionary<string, int> item_levels;

    public ZoneArchiveData() {}
    public ZoneArchiveData(string zoneId, int zoneLevel, Dictionary<string, int> zone_item_levels)
    {
        id = zoneId;
        level = zoneLevel;
        staff = string.Empty;
        item_levels = zone_item_levels;
    }
    public int GetZoneItemLevel(string itemId)
    {
        return item_levels.TryGetValue(itemId, out var value) ? value : 0;
    }

    public int LevelupZoneItem(string itemId) {
        if(item_levels.ContainsKey(itemId)) {
            return ++item_levels[itemId];
        }
        return 0;
    }
}
public class ZoneArchive : IArchive
{
    public WorkTeam work_team;
    public Dictionary<string, ZoneArchiveData> data;

    public void Default()
    {
        data = new Dictionary<string, ZoneArchiveData>();
        work_team = new WorkTeam();
    }
    public ZoneArchiveData GetData(string id)
    {
        return data.TryGetValue(id, out var value) ? value : null;
    }
    public void AddData(string id, ZoneArchiveData data)
    {
        this.data.Add(id, data);
    }
}
