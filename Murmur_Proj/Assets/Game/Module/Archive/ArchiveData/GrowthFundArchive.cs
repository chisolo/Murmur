using System.Collections.Generic;


public class GrowthFundArchive : IArchive
{
    public Dictionary<string, bool> receiveList;

    public virtual void Default()
    {
        receiveList = new Dictionary<string, bool>();
    }
}