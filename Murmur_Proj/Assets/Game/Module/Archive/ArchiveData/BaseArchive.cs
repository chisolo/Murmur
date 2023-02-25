using System.Collections.Generic;

public class BaseArchive<T> : IArchive where T : class
{
    public Dictionary<string, T> archiveDatas;

    public virtual void Default()
    {
        archiveDatas = new Dictionary<string, T>();
    }

    public T GetData(string id)
    {
        return archiveDatas.TryGetValue(id, out var value) ? value : null;
    }

    public void AddData(string id, T archiveData)
    {
        archiveDatas[id] = archiveData;
    }
}