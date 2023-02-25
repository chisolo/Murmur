using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;
using System;
using System.Linq;

public class GachaModule : Singleton<GachaModule>
{
    protected GachaModule() { }
    private bool _inited = false;

    private Dictionary<string, GachaGroupConfig> _gachaGroupConfigs;
    private CommonConfig _commonConfig;
    private GachaArchive _gachaArchive;
    private StaffProfileConfig _staffProfileConfig;

    public void Init()
    {
        if(_inited) return;

        _gachaGroupConfigs = ConfigModule.Instance.GachaGroups();
        _commonConfig = ConfigModule.Instance.Common();
        _gachaArchive = ArchiveModule.Instance.GetArchive<GachaArchive>();
        _staffProfileConfig = ConfigModule.Instance.GetStaffProfileConfig();

        _inited = true;
    }



    public StaffArchiveData DrawStaff(List<string> groupIds) {
        var weightAll = groupIds.Sum(x => _gachaGroupConfigs[x].weight);
        var groups = _gachaGroupConfigs.Values.Where(x => groupIds.Contains(x.id)).OrderBy(x => x.weight);
        var random = RandUtil.Range(weightAll);
        //AppLogger.Log("staff random " + random);
        foreach (var group in groups) {
            random -= group.weight;
            if (random < 0) {
                //AppLogger.Log("hit group " + group.id);

                var attributeList = DrawAttributeDataList(group);


                List<string> nameList = null;
                List<string> profileList = null;
                if (RandUtil.Range(2) == 0) {
                    // male
                    nameList    = _staffProfileConfig.name_list_staff_male;
                    profileList = _staffProfileConfig.profile_list_staff_male;
                } else {
                    // female
                    nameList    = _staffProfileConfig.name_list_staff_female;
                    profileList = _staffProfileConfig.profile_list_staff_female;
                }

                var staff = new StaffArchiveData();

                staff.id = GuildUtil.NewUUID();
                staff.name = nameList[RandUtil.Range(nameList.Count)];
                staff.puppetId = profileList[RandUtil.Range(profileList.Count)];
                staff.attributes = attributeList;
                staff.rarity = GetMaxRarity(attributeList);
                staff.salary = CalcSalary(attributeList);

                return staff;

            }
        }

        throw new Exception("No Attribute Find");
    }

    public List<StaffAttributeArchiveData> DrawAttributeDataList(GachaGroupConfig group)
    {
        List<StaffAttributeArchiveData> attributes = new List<StaffAttributeArchiveData>();
        for (var i = 0; i < group.rarity.Count; i++) {
            for (var j = 0; j < group.amount[i]; j++) {
                var data = DrawAttributeData(group.rarity[i], attributes);
                if (data != null) {
                    attributes.Add(data);
                }
            }
        }

        return attributes;
    }

    public StaffAttributeArchiveData DrawAttributeData(string rarity, List<StaffAttributeArchiveData> lastResults)
    {
        var allAttributes = ConfigModule.Instance.Attributes().Values
            .Where(x => x.rarity == rarity
                && BuildingModule.Instance.IsBuildingReady(x.building_required)
                && !lastResults.Any(y => y.attributeId == x.id))
            .OrderBy(x => x.weight);

        var weightAll = allAttributes.Sum(x => x.weight);
        var random = RandUtil.Range(weightAll);
        //AppLogger.Log("random " + random);

        foreach (var attr in allAttributes) {
            random -= attr.weight;
            if (random < 0) {
                //AppLogger.Log("hit attr " + attr.id);

                var val = MakeAttributeValue(attr);
                return new StaffAttributeArchiveData(attr.id, val);
            }
        }

        //throw new Exception("No Attribute Find");
        Debug.LogError($"No Attribute Find: {rarity}");
        return null;
    }

    public float MakeAttributeValue(AttributeConfig attributeConfig)
    {
        // 金库等级
        int vaultLevel = BuildingModule.Instance.GetServiceBuilding(ServiceType.TreasurySupply).GetLevel();

        // 计算公式:random(0, max) * mul + base_value+vault_ratio*vault_level
        return RandUtil.Range(0, attributeConfig.max + 1) * attributeConfig.multiplier + attributeConfig.base_value + vaultLevel * attributeConfig.vault_ratio;
    }

    private string GetMaxRarity(List<StaffAttributeArchiveData> attributes)
    {
        // 最高品质
        if (attributes.Any(x => ConfigModule.Instance.GetAttribute(x.attributeId).rarity == StaffRarity.SR)) {
            return StaffRarity.SR;
        }

        if (attributes.Any(x => ConfigModule.Instance.GetAttribute(x.attributeId).rarity == StaffRarity.RARE)) {
            return StaffRarity.RARE;
        }

        return StaffRarity.NORMAL;
    }

    private int CalcSalary(List<StaffAttributeArchiveData> attributes)
    {
        int salary = 0;
        foreach (var attr in attributes) {
            var attrConfig = ConfigModule.Instance.GetAttribute(attr.attributeId);
            if (attrConfig.rarity == StaffRarity.SR) {
                salary += _commonConfig.sr_base_salary;
            } else if (attrConfig.rarity == StaffRarity.RARE) {
                salary += _commonConfig.rare_base_salary;
            }
            salary += _commonConfig.normal_base_salary;
        }

        // 金库等级
        int vaultLevel = BuildingModule.Instance.GetServiceBuilding(ServiceType.TreasurySupply).GetLevel();
        salary *= vaultLevel;

        return salary;
    }

}
