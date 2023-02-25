using System.Collections.Generic;


public class ShopArchive : IArchive
{
    public Dictionary<string, long> giftTimes;
    public long gachaValidTime;

    // TODO: move next prj
    public long adIncomeBoostEndTime;
    public long serviceBoostEndTime;

    public virtual void Default()
    {
        giftTimes = new Dictionary<string, long>();
        gachaValidTime = 0;
        adIncomeBoostEndTime = -1;
        serviceBoostEndTime = -1;
    }
}