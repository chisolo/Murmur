using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;

public static class GameUtil
{
    public const string ResPath = "Assets/Res";
    public const string ResConfigPath = ResPath + "/Config";
    public const string ResModConfigPath = ResConfigPath + "/mods.json";
    public const string ResSpotConfigPath = ResConfigPath + "/spots.json";
    public const string ResDataConfigPath = ResConfigPath + "/data.json";
    public const string ResLocalePath = ResPath + "/Locale/locale_{0}.json";
    public const string ResSpotCryptKey = "VfNOPies";
    public const string ResModCryptKey = "VfNOPies";
    public const string ResDataCryptKey = "xiHA3ITj";
    public const string ResLocalCryptKey = "";
    public const string ResBuildingIconPath = "Assets/Res/UI/Atlas/BuildingIconAtlas/{0}.png";
    public const string ResEnhanceIconPath = "Assets/Res/UI/Atlas/AttributeIconAtlas/{0}.png";
    public const string ResCommonIconPath = "Assets/Res/UI/Atlas/CommonAtlas/{0}.png";
    public const string ResWorkerIconPath = "Assets/Res/UI/Image/WokerIcons/{0}.png";
    public const string ResBgm = "Assets/Res/Sfx/bgm.mp3";
    public const string ResSfxOpen = "Assets/Res/Sfx/sfx_open.wav";
    public const string ResSfxClick = "Assets/Res/Sfx/sfx_click.wav";
    public const string ResSfxClose = "Assets/Res/Sfx/sfx_close.wav";
    public const string ResSfxBonus = "Assets/Res/Sfx/sfx_bonus.wav";
    public const int EntryAmount = 3;
    public const int ExitAmount = 5;
    public const float FloatThrehold = 0.001f;
    public const float DefaultTimeInterval = 30f;
    public const float DefaultGuestInterval = 60f;
    public const string DefaultNotificationChannel = "food_park_channel";
    public const string TriggerTutorialId = "tutorial_4";
    public static readonly Dictionary<string, string> ServiceFoodPrefab = new Dictionary<string, string>()
    {
        {"hotdog_bureau", "Assets/Res/Puppet/Mod/food_hotdog.fbx"},
        {"hamberg_bureau", "Assets/Res/Puppet/Mod/food_hamberg.fbx"},
        {"pizza_bureau", "Assets/Res/Puppet/Mod/food_pizza.fbx"}
    };
    public static readonly Dictionary<string, string> ItemIcons = new Dictionary<string, string>()
    {
        {ItemType.Money, "Assets/Res/UI/Atlas/CommonAtlas/icon_money_shadow.png"},
        {ItemType.Coin, "Assets/Res/UI/Atlas/CommonAtlas/icon_coin.png"},
        {ItemType.Coupon, "Assets/Res/UI/Atlas/CommonAtlas/icon_money_shadow.png"},
    };
    public static readonly Dictionary<string, List<BuildingStat>> BuildingServiceStats = new Dictionary<string, List<BuildingStat>>(){
        {ServiceType.HotdogBureau, new List<BuildingStat>(){BuildingStat.BureauService, BuildingStat.BureauCapacity, BuildingStat.Revenue}},
        {ServiceType.HambergBureau, new List<BuildingStat>(){BuildingStat.BureauService, BuildingStat.BureauCapacity, BuildingStat.Revenue}},
        {ServiceType.PizzaBureau, new List<BuildingStat>(){BuildingStat.BureauService, BuildingStat.BureauCapacity, BuildingStat.Revenue}},
        {ServiceType.ReceptionDivision, new List<BuildingStat>(){BuildingStat.ReceptionService, BuildingStat.ReceptionCell, BuildingStat.ReceptionCapacity, BuildingStat.TotalSalary}},
        {ServiceType.ReceptionCell, new List<BuildingStat>(){BuildingStat.ReceptionCellService, BuildingStat.Salary}},
        {ServiceType.DeliverDivision, new List<BuildingStat>(){BuildingStat.DeliverCapacity, BuildingStat.DeliverCell, BuildingStat.TotalSalary}},
        {ServiceType.DeliverCell, new List<BuildingStat>(){BuildingStat.DeliverCellCapacity, BuildingStat.DeliverCellSpeed, BuildingStat.Salary}},
        {ServiceType.SausageStorageDivision, new List<BuildingStat>(){BuildingStat.StorageService, BuildingStat.StorageCapacity, BuildingStat.StorageCell, BuildingStat.TotalSalary}},
        {ServiceType.CheeseStorageDivision, new List<BuildingStat>(){BuildingStat.StorageService, BuildingStat.StorageCapacity, BuildingStat.StorageCell, BuildingStat.TotalSalary}},
        {ServiceType.FlourStorageDivision, new List<BuildingStat>(){BuildingStat.StorageService, BuildingStat.StorageCapacity, BuildingStat.StorageCell, BuildingStat.TotalSalary}},
        {ServiceType.SausageStorageCell, new List<BuildingStat>(){BuildingStat.StorageCellService, BuildingStat.Salary}},
        {ServiceType.CheeseStorageCell, new List<BuildingStat>(){BuildingStat.StorageCellService, BuildingStat.Salary}},
        {ServiceType.FlourStorageCell, new List<BuildingStat>(){BuildingStat.StorageCellService, BuildingStat.Salary}},
    };
    public static readonly Dictionary<int, string> BuildingStatIcons = new Dictionary<int, string>()
    {
        {(int)BuildingStat.Revenue, string.Format(GameUtil.ResCommonIconPath, "icon_money")},
        {(int)BuildingStat.Salary, string.Format(GameUtil.ResCommonIconPath, "icon_money")},
        {(int)BuildingStat.TotalSalary, string.Format(GameUtil.ResCommonIconPath, "icon_money")},
        {(int)BuildingStat.BureauService, string.Format(GameUtil.ResBuildingIconPath, "icon_service_speed")},
        {(int)BuildingStat.BureauCapacity, string.Format(GameUtil.ResBuildingIconPath, "icon_lineup")},
        {(int)BuildingStat.ReceptionService, string.Format(GameUtil.ResBuildingIconPath, "icon_clients")},
        {(int)BuildingStat.ReceptionCell, string.Format(GameUtil.ResBuildingIconPath, "icon_receptionist")},
        {(int)BuildingStat.ReceptionCapacity, string.Format(GameUtil.ResBuildingIconPath, "icon_lineup")},
        {(int)BuildingStat.ReceptionCellService, string.Format(GameUtil.ResBuildingIconPath, "icon_clients")},
        {(int)BuildingStat.DeliverCapacity, string.Format(GameUtil.ResBuildingIconPath, "icon_carry_number")},
        {(int)BuildingStat.DeliverCell, string.Format(GameUtil.ResBuildingIconPath, "icon_lineup")},
        {(int)BuildingStat.DeliverCellCapacity, string.Format(GameUtil.ResBuildingIconPath, "icon_carry_number")},
        {(int)BuildingStat.DeliverCellSpeed, string.Format(GameUtil.ResBuildingIconPath, "icon_carrier_speed")},
        {(int)BuildingStat.StorageService, string.Format(GameUtil.ResBuildingIconPath, "icon_supply_speed")},
        {(int)BuildingStat.StorageCapacity, string.Format(GameUtil.ResBuildingIconPath, "icon_carry_number")},
        {(int)BuildingStat.StorageCell, string.Format(GameUtil.ResBuildingIconPath, "icon_lineup")},
        {(int)BuildingStat.StorageCellService, string.Format(GameUtil.ResBuildingIconPath, "icon_supply_speed")},
    };
    public static readonly Dictionary<string, List<BuildingTotalStat>> BuildingTotalServiceStats = new Dictionary<string, List<BuildingTotalStat>>(){
        {ServiceType.HotdogBureau, new List<BuildingTotalStat>(){BuildingTotalStat.BureauCapacity, BuildingTotalStat.Revenue, BuildingTotalStat.Salary, BuildingTotalStat.Staff}},
        {ServiceType.HambergBureau, new List<BuildingTotalStat>(){BuildingTotalStat.BureauCapacity, BuildingTotalStat.Revenue, BuildingTotalStat.Salary, BuildingTotalStat.Staff}},
        {ServiceType.PizzaBureau, new List<BuildingTotalStat>(){BuildingTotalStat.BureauCapacity, BuildingTotalStat.Revenue, BuildingTotalStat.Salary, BuildingTotalStat.Staff}},
        {ServiceType.SausageStorageCell, new List<BuildingTotalStat>(){BuildingTotalStat.StorageCapacity, BuildingTotalStat.StorageService, BuildingTotalStat.Salary, BuildingTotalStat.Staff}},
        {ServiceType.CheeseStorageCell, new List<BuildingTotalStat>(){BuildingTotalStat.StorageCapacity, BuildingTotalStat.StorageService, BuildingTotalStat.Salary, BuildingTotalStat.Staff}},
        {ServiceType.FlourStorageCell, new List<BuildingTotalStat>(){BuildingTotalStat.StorageCapacity, BuildingTotalStat.StorageService, BuildingTotalStat.Salary, BuildingTotalStat.Staff}},
        {ServiceType.ReceptionCell, new List<BuildingTotalStat>(){BuildingTotalStat.ReceptionCapacity, BuildingTotalStat.ReceptionService, BuildingTotalStat.Salary, BuildingTotalStat.Staff}},
        {ServiceType.DeliverCell, new List<BuildingTotalStat>(){BuildingTotalStat.DeliverCapacity, BuildingTotalStat.DeliverSpeed, BuildingTotalStat.Salary, BuildingTotalStat.Staff}},
    };
    public static readonly Dictionary<int, string> BuildingTotalStatIcons = new Dictionary<int, string>()
    {
        {(int)BuildingTotalStat.Revenue, string.Format(GameUtil.ResCommonIconPath, "icon_money")},
        {(int)BuildingTotalStat.Salary, string.Format(GameUtil.ResCommonIconPath, "icon_money")},
        {(int)BuildingTotalStat.Staff, string.Format(GameUtil.ResBuildingIconPath, "icon_receptionist")},
        {(int)BuildingTotalStat.BureauCapacity, string.Format(GameUtil.ResBuildingIconPath, "icon_lineup")},
        {(int)BuildingTotalStat.StorageCapacity, string.Format(GameUtil.ResBuildingIconPath, "icon_carry_number")},
        {(int)BuildingTotalStat.StorageService, string.Format(GameUtil.ResBuildingIconPath, "icon_supply_speed")},
        {(int)BuildingTotalStat.ReceptionCapacity, string.Format(GameUtil.ResBuildingIconPath, "icon_lineup")},
        {(int)BuildingTotalStat.ReceptionService, string.Format(GameUtil.ResBuildingIconPath, "icon_clients")},
        {(int)BuildingTotalStat.DeliverCapacity, string.Format(GameUtil.ResBuildingIconPath, "icon_carry_number")},
        {(int)BuildingTotalStat.DeliverSpeed, string.Format(GameUtil.ResBuildingIconPath, "icon_carrier_speed")},
    };
    public static readonly Dictionary<int, string> BuildingTotalStatTitles = new Dictionary<int, string>()
    {
        {(int)BuildingTotalStat.Revenue, "STAT_INCOME"},
        {(int)BuildingTotalStat.Salary, "STAT_COST"},
        {(int)BuildingTotalStat.Staff, "STAT_STAFF"},
        {(int)BuildingTotalStat.BureauCapacity, "STAT_QUEUE"},
        {(int)BuildingTotalStat.StorageCapacity, "STAT_CAPACITY"},
        {(int)BuildingTotalStat.StorageService, "STAT_SUPPLY"},
        {(int)BuildingTotalStat.ReceptionCapacity, "STAT_QUEUE"},
        {(int)BuildingTotalStat.ReceptionService, "STAT_CUSTOMER"},
        {(int)BuildingTotalStat.DeliverCapacity, "STAT_CAPACITY"},
        {(int)BuildingTotalStat.DeliverSpeed, "STAT_SPEED"},
    };
    public static int BureauToIndex(string service)
    {
        if(service == ServiceType.HotdogBureau) return 0;
        if(service == ServiceType.HambergBureau) return 1;
        if(service == ServiceType.PizzaBureau) return 2;
        return 0;
    }
    public static readonly string[] BureauList = new string[] {
        ServiceType.HotdogBureau,
        ServiceType.HambergBureau,
        ServiceType.PizzaBureau
    };
    public const string NullWorkerIcon = "Assets/Res/UI/Atlas/CommonAtlas/portrait_empty.png";
    public const string NullWorkerName = "NULL_WORKER";

    public const string AdsText = "ADS";
    public const string TalentTeamFullTitleText = "RESEACH_FULL_TITLE";
    public const string TalentTeamFullBodyText = "RESEACH_FULL_BODY";
    public const string TalentTeamFullShopText = "RESEACH_FULL_SHOP";
    public const string BuildingTeamFullTitleText = "BUILDING_FULL_TITLE";
    public const string BuildingTeamFullBodyText = "BUILDING_FULL_BODY";
    public const string BuildingTeamFullShopText = "BUILDING_FULL_SHOP";
    public const string DefaultBuildTeam = "build_default_team";
    public static readonly string[] BuildTeamIdList = new string[] {
        "build_default_team",
        "build_work_team_2",
        "build_work_team_3",
        "build_work_team_4",
    };

    public const string DefaultTalentTeam = "talent_default_team";
    public static readonly string[] TalentTeamIdList = new string[] {
        "talent_default_team",
        "talent_work_team_2",
        "talent_work_team_3",
        "talent_work_team_4",
    };

    public const int QuestCount = 4;

    public const string CookEffect = "Assets/Res/Fx/Prefab/cook.prefab";

    public const string PrivacyUrl = "https://www.jinshi-games.com/privacy/en.html";
    public const string TermsUrl = "https://www.jinshi-games.com/privacy/en_user.html";

    public const string TreasuryBuildingId = "treasury_supply_01";

    public const string NotEnoughCoinConfirmTitleText = "NOT_ENOUGH_COIN_CONFIRM_TITLE_TEXT";
    public const string NotEnoughCoinConfirmBodyText = "NOT_ENOUGH_COIN_CONFIRM_BODY_TEXT";
    public const string NotEnoughCoinConfirmOKText = "NOT_ENOUGH_COIN_CONFIRM_OK_TEXT";
}
