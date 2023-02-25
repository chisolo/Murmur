using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Lemegeton;

public class ConfigModule : Singleton<ConfigModule>
{
    private class ConfigData
    {
        public CommonConfig common;
        public Dictionary<string, BuildingConfig> buildings;
        public Dictionary<string, EnhanceSetConfig> enhance_sets;
        public Dictionary<string, EnhanceConfig> enhances;
        public Dictionary<string, ZoneConfig> zones;
        public List<PuppetConfig> puppets;
        public List<QuestConfig> quests;
        public Dictionary<string, GachaGroupConfig> gacha_groups;
        public Dictionary<string, AttributeConfig> attributes;
        public StaffProfileConfig staff_profile;
        public List<TalentConfig> talents;
        public ShopConfig shop_config;
        public List<TutorialConfig> tutorials;
        public EmergencyConfig emergency;
        public List<NotificationConfig> notifications;
        public GrowthFundConfig growth_fund_config;
        public ServiceBoostConfig service_boost_config;
        public MoreGuestConfig more_guest_config;
    }
    protected ConfigModule() { }
    private bool _inited = false;
    private ConfigData _configData; // 策划数据
    private Dictionary<string, ModConfig> _mods; // 特殊数据，由地编生成
    private Dictionary<string, SpotConfig> _spots; // 特殊数据，由地编生成
    public async Task<bool> Init()
    {
        _mods = new Dictionary<string, ModConfig>();
        _spots = new Dictionary<string, SpotConfig>();
        return await LoadConfigAsync();
    }

    private async Task<bool> LoadConfigAsync()
    {
        var handleData = Addressables.LoadAssetAsync<TextAsset>(GameUtil.ResDataConfigPath);
        await handleData.Task;
        if (handleData.Status == AsyncOperationStatus.Succeeded) {
            var jsonStr = EncryptUtil.DESDecrypt(handleData.Result.text, GameUtil.ResDataCryptKey);
            _configData = LitJson.JsonMapper.ToObject<ConfigData>(jsonStr);
            Addressables.Release(handleData);
        } else {
            Debug.LogError("failed to load config data ");
            return false;
        }

        var handleMod = Addressables.LoadAssetAsync<TextAsset>(GameUtil.ResModConfigPath);
        await handleMod.Task;
        if (handleMod.Status == AsyncOperationStatus.Succeeded) {
            var jsonStr = EncryptUtil.DESDecrypt(handleMod.Result.text, GameUtil.ResModCryptKey);
            _mods = LitJson.JsonMapper.ToObject<Dictionary<string, ModConfig>>(jsonStr);
            Addressables.Release(handleMod);
        } else {
            Debug.LogError("failed to load mod data ");
            return false;
        }

        var handleSpot = Addressables.LoadAssetAsync<TextAsset>(GameUtil.ResSpotConfigPath);
        await handleSpot.Task;
        if (handleSpot.Status == AsyncOperationStatus.Succeeded) {
            var jsonStr = EncryptUtil.DESDecrypt(handleSpot.Result.text, GameUtil.ResSpotCryptKey);
            _spots = LitJson.JsonMapper.ToObject<Dictionary<string, SpotConfig>>(jsonStr);
            Addressables.Release(handleSpot);
        } else {
            Debug.LogError("failed to load spot data ");
            return false;
        }
        _inited = true;
        return true;
    }

    public CommonConfig Common() => _configData.common;
    public Dictionary<string, BuildingConfig> Buildings() => _configData.buildings;
    public Dictionary<string, EnhanceSetConfig> EnhanceSets() => _configData.enhance_sets;
    public Dictionary<string, EnhanceConfig> Enhances() => _configData.enhances;
    public Dictionary<string, ZoneConfig> Zones() => _configData.zones;
    public List<PuppetConfig> Puppets() => _configData.puppets;
    public List<QuestConfig> Quests() => _configData.quests;
    public List<TalentConfig> Talents() => _configData.talents;
    public ShopConfig ShopConfig() => _configData.shop_config;
    public List<TutorialConfig> Tutorials() => _configData.tutorials;
    public EmergencyConfig Emergency() => _configData.emergency;
    public List<NotificationConfig> Notifications() => _configData.notifications;
    public GrowthFundConfig GrowthFund() => _configData.growth_fund_config;
    public ServiceBoostConfig ServiceBoost() => _configData.service_boost_config;
    public MoreGuestConfig MoreGuest() => _configData.more_guest_config;

    public EnhanceSetConfig GetEnhanceSet(string id)
    {
        return _configData.enhance_sets.TryGetValue(id, out var value) ? value : null;
    }
    public EnhanceConfig GetEnhance(string id)
    {
        return _configData.enhances.TryGetValue(id, out var value) ? value : null;
    }
    public SpotConfig GetSpot(string id)
    {
       //return _spots.TryGetValue(id, out var value) ? value : SpotConfig.Default();
       return _spots.TryGetValue(id, out var value) ? value : null;
    }
    public SpotConfig GetSpot(string prefix, int index1, string func, int index2)
    {
        return GetSpot(String.Format("{0}_{1}_{2}_{3}", prefix, index1, func, index2));
    }
    public SpotConfig GetSpot(string prefix, string func, int index)
    {
        return GetSpot(String.Format("{0}_{1}_{2}", prefix, func, index));
    }
    public SpotConfig GetSpot(string func, int index)
    {
        return GetSpot(String.Format("{0}_{1}", func, index));
    }
    public ModConfig GetMod(string id)
    {
        return _mods.TryGetValue(id, out var value) ? value : null;
    }
    public Dictionary<string, AttributeConfig> Attributes()
    {
        return _configData.attributes;
    }
    public Dictionary<string, GachaGroupConfig> GachaGroups()
    {
        return _configData.gacha_groups;
    }
    public AttributeConfig GetAttribute(string id)
    {
        return _configData.attributes.TryGetValue(id, out var value) ? value : null;
    }
    public StaffProfileConfig GetStaffProfileConfig()
    {
        return _configData.staff_profile;
    }
}