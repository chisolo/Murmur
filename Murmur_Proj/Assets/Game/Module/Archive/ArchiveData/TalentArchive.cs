
using System.Collections.Generic;

public class TalentArchiveData
{
    public string id;
    public bool completed;

    public TalentArchiveData()
    {

    }

    public TalentArchiveData(string id)
    {
        this.id = id;
        this.completed = false;
    }
}



public class TalentArchive : IArchive
{
    public WorkTeam work_team;

    public Dictionary<string, TalentArchiveData> talents;

    public virtual void Default()
    {
        talents = new Dictionary<string, TalentArchiveData>();
        work_team = new WorkTeam();
    }

    public TalentArchiveData GetData(string id)
    {
        return talents.TryGetValue(id, out var value) ? value : null;
    }

    public void AddData(string id, TalentArchiveData archiveData)
    {
        talents[id] = archiveData;
    }

}