using System;
using System.Collections.Generic;

public class GachaArchiveData
{
    public string id;
    public DateTime start_time;
    public DateTime end_time;
    public string staff;
}

public class GachaArchive : IArchive
{
    public Dictionary<string, GachaArchiveData> data;



    public void Default()
    {
        data = new Dictionary<string, GachaArchiveData>();
    }

    public GachaArchiveData GetData(string id)
    {
        return data.TryGetValue(id, out var value) ? value : null;
    }
}