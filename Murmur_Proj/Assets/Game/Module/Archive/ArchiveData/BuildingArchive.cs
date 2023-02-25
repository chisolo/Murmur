using System;
using System.Collections.Generic;

public class BuildingArchiveData
{
    public string id;
    public int level;
    public string staff;
    public Dictionary<string, int> enhance_levels;
    public BuildingArchiveData() {}
    public BuildingArchiveData(string buildingId, int buildingLevel, List<string> enhances) {
        id = buildingId;
        level = buildingLevel;
        staff = string.Empty;
        SetDefaultEnhance(enhances);
    }
    public BuildingArchiveData(string buildingId, int buildingLevel) {
        id = buildingId;
        level = buildingLevel;
        staff = string.Empty;
        enhance_levels = new Dictionary<string, int>();
    }
    public void SetDefaultEnhance(List<string> enhances)
    {
        enhance_levels = new Dictionary<string, int>();
        foreach(var enhance in enhances) {
            enhance_levels.Add(enhance, 0);
        }
    }
    public int GetEnhanceLevel(string enhanceId)
    {
        return enhance_levels.TryGetValue(enhanceId, out var value) ? value : 0;
    }

    public int LevelupEnhance(string enhanceId) {
        if(enhance_levels.ContainsKey(enhanceId)) {
            return ++enhance_levels[enhanceId];
        }
        return 0;
    }
}

public class BuildingArchive : IArchive
{
    public WorkTeam work_team;
    public Dictionary<string, BuildingArchiveData> data;

    public void Default()
    {
        data = new Dictionary<string, BuildingArchiveData>();
        work_team = new WorkTeam();
    }

    public BuildingArchiveData GetData(string id)
    {
        return data.TryGetValue(id, out var value) ? value : null;
    }
    public void AddData(string id, BuildingArchiveData data)
    {
        this.data.Add(id, data);
    }
}