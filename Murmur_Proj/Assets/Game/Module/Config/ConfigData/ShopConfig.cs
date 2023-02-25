using System.Collections.Generic;
using UnityEngine;

public class ShopConfig
{
    public Dictionary<string, string> build_team_product_ids;
    public Dictionary<string, string> research_team_product_ids;
    public Dictionary<string, Product> product_list;
    public List<ShopGiftConfig> shop_gift_limit;
    public List<ShopGiftConfig> shop_gift_normal;
    public List<ShopGiftPrivilegeConfig> shop_gift_privilege;
    public List<ShopCoinConfig> shop_coin;
    public Dictionary<string, ShopGachaConfig> shop_gacha;
}

public class Product
{
    public string product_id;
    public int product_type;
    public List<ShopGiftRewardConfig> rewards;
}

public class ShopGiftRewardConfig
{
    public string type;
    public float value;
    public string style;
    public string teamId;
}

public class ShopGiftBaseConfig
{
    public string id;
    public string product_id;
    public int time;
    public string background;
    public string foreground;
    public string title;
}

public class ShopGiftConfig: ShopGiftBaseConfig
{
    public List<int> show_day;
    public int discount;
    public string main_ui_icon;
    public string popup_icon;
}

public class ShopGiftPrivilegeConfig: ShopGiftBaseConfig
{
    public string name;
    public string desc;
}

public class ShopCoinConfig: ShopGiftBaseConfig
{
    public int num;
    public bool popular;
}

public class ShopGachaConfig
{
    public int cost;
    public string cost_type;
    public List<string> group;
}
