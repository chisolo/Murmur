public static class AtlasDefine
{
    public const string CommonAtlasPath = "Assets/Res/UI/Atlas/CommonAtlas/";
    public const string StaffIconAtlasPath = "Assets/Res/UI/Atlas/StaffIconAtlas/";
    public const string AttributeIconAtlasPath = "Assets/Res/UI/Atlas/AttributeIconAtlas/";
    public const string TalentAtlasPath = "Assets/Res/UI/Atlas/TalentAtlas/";

    public static string GetAttributeIconPath(string name)
    {
        return string.Format("{0}{1}.png", AttributeIconAtlasPath, name);
    }

    public static string GetStaffIconPath(string name)
    {
        return string.Format("{0}{1}.png", StaffIconAtlasPath, name);
    }

    public static string GetCommonPath(string name)
    {
        return string.Format("{0}{1}.png", CommonAtlasPath, name);
    }

    public static string GetTalentIconPath(string name)
    {
        return string.Format("{0}{1}.png", TalentAtlasPath, name);
    }

    public static string GetAttributeIconBgPath(string rarity)
    {
        var name = "bg_icon_normal";
        if (rarity == StaffRarity.SR) {
            name = "bg_icon_sr";
        } else if (rarity == StaffRarity.RARE) {
            name = "bg_icon_rare";
        }

        return GetCommonPath(name);
    }

    public static string GetAttributeBgPath(string rarity)
    {
        var name = "bg_attr_normal";
        if (rarity == StaffRarity.SR) {
            name = "bg_attr_sr";
        } else if (rarity == StaffRarity.RARE) {
            name = "bg_attr_rare";
        }
        return GetCommonPath(name);
    }

    public static string GetStaffIconBgPath(string rarity)
    {
        var name = "bg_portrait_normal";
        if (rarity == StaffRarity.SR) {
            name = "bg_portrait_sr";
        } else if (rarity == StaffRarity.RARE) {
            name = "bg_portrait_rare";
        }
        return GetCommonPath(name);
    }

    public static string GetStaffRarityBgPath(string rarity)
    {
        var name = "bg_portrait_outline_normal";
        if (rarity == StaffRarity.SR) {
            name = "bg_portrait_outline_sr";
        } else if (rarity == StaffRarity.RARE) {
            name = "bg_portrait_outline_rare";
        }
        return GetCommonPath(name);
    }

    public static string GetStaffSmileIconPath(string rarity)
    {
        var name = "icon_smile_rare";
        if (rarity == StaffRarity.SR) {
            name = "icon_smile_sr";
        }
        return GetStaffIconPath(name);
    }

    public static string GetGiftPath(string name)
    {
        return string.Format("{0}{1}.png", "Assets/Res/UI/Atlas/ShopGiftAtlas/", name);
    }

    public static string GetGiftIconPath(string name)
    {
        return string.Format("{0}{1}.png", "Assets/Res/UI/Atlas/ShopIconAtlas/", name);
    }

    public static string GetMainPath(string name)
    {
        return string.Format("{0}{1}.png", "Assets/Res/UI/Atlas/MainAtlas/", name);
    }
}