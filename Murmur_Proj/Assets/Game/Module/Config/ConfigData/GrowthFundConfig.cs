using System.Collections.Generic;
using UnityEngine;

public class GrowthFundConfig
{
    public string product_id;
    public int unlock_star_num;

    public List<GrowthReward> rewards;
}

public class GrowthReward
{
    public string id;
    public int require_star;
    public int reward_coin;
}