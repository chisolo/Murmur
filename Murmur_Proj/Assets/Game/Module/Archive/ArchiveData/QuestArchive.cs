using System.Collections.Generic;
using Lemegeton;

public class QuestArchiveData
{
    public string id;
    public int progress;
    public int status; // 0: finished, 1: claimed

    public QuestArchiveData() {}
    public QuestArchiveData(string i, int p, int s)
    {
        id = i;
        progress = p;
        status = s;
    }
}

public class QuestArchive : IArchive
{
    public List<QuestArchiveData> quests;
    public int next;
    public void Default()
    {
        quests = new List<QuestArchiveData>();
        next = 0;
    }
    public QuestArchiveData GetData(int index)
    {
        return quests[index];
    }
    public void SetData(QuestArchiveData archiveData, int index)
    {
        quests[index] = archiveData;
    }
}
