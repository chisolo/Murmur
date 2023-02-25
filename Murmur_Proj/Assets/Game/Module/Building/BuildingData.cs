using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;

public class BuildingData : ITimeProgressBarData
{
    public BuildingConfig Config => _config;
    public BuildingStatus Status => _status;
    public BuildingData Parent => _parent;
    public int Sausage => _sausage;
    public int Cheese => _cheese;
    public int Flour => _flour;
    public WorkTeamData Team => _team;
    public long EndTime => Team?.endTime ?? -1;
    public long Duration => (long)GetProp(BuildingProperty.LvlUpDuration);
    public int AdCount => Team?.adCount ?? 0;
    public long AdCooldownTime => Team?.adCooldownTime ?? 0;

    private BuildingModule _buildingModule;
    private BuildingConfig _config;
    private BuildingArchiveData _archive;
    private BuildingStatus _status;
    private List<BuildingData> _children;
    private BuildingData _parent;
    private EnhanceSetConfig _curEnhanceSet;
    private Dictionary<string, BuildingEnhance> _curEnhances;
    private Dictionary<string, float> _baseProps;
    private Dictionary<string, float> _props;
    private EnhanceProperty _buildingEnhanceProps;
    private EnhanceProperty _staffEnhanceProps;
    private WorkTeamData _team;
    private int _sausage;
    private int _cheese;
    private int _flour;
    public void Init(BuildingModule module, BuildingConfig config, BuildingArchiveData archive)
    {
        _buildingModule = module;
        _config = config;
        _archive = archive;
        _children = new List<BuildingData>();
        _baseProps = new Dictionary<string, float>();
        _props = new Dictionary<string, float>();
        _sausage = 0;
        _cheese = 0;
        _flour = 0;
        _team = null;

        var propTypes = _buildingModule.GetBuildingPropTypes();
        foreach(var propType in propTypes) {
            _baseProps.Add(propType, 0);
            _props.Add(propType, 0);
        }
        _buildingEnhanceProps = new EnhanceProperty(propTypes);
        _staffEnhanceProps = new EnhanceProperty(propTypes);

        if(_archive.level > 0 && Config.type == BuidlingType.Cell) _archive.staff = Config.id + "_worker";
    }

    public void PreLoad()
    {
        UpdateBuildingStatus();
        if(!string.IsNullOrEmpty(_config.parent)) {
            _parent = BuildingModule.Instance.GetBuilding(_config.parent);
        }
        foreach(var child in _config.children) {
            _children.Add(BuildingModule.Instance.GetBuilding(child));
        }
        RefreshBaseProp();
        _curEnhances = new Dictionary<string, BuildingEnhance>();
        RefreshCurEnhance();
        RefreshStaffEnhance();
        RefreshAllProp();
    }

    private void UpdateBuildingStatus()
    {
        if(_archive.level == 0) {
            if(_team != null) {
                _status = BuildingStatus.Progress;
            } else {
                _status = BuildingStatus.Unbuilt;
            }
        } else {
            if(_team != null) {
                _status = BuildingStatus.Progress;
            } else {
                _status = BuildingStatus.Normal;
            }
        }
    }

    #region 数据相关
    public int GetLevel()
    {
        return _archive.level;
    }
    public string GetStaff()
    {
        return _archive.staff;
    }
    public bool HasStaff()
    {
        return !string.IsNullOrEmpty(_archive.staff);
    }
    public bool IsMaxLevel()
    {
        return _archive.level == _config.max_level;
    }
    public BuildingRequire GetPreBuilding()
    {
        if(_config.pre_building.Count <= 0 || IsMaxLevel()) return null;
        var buildingRequire = _config.pre_building[_archive.level];
        if(string.IsNullOrEmpty(buildingRequire.id)) return null;
        return _buildingModule.IsPreBuildingReady(buildingRequire) ? null : buildingRequire;
    }
    public BuildingRequire GetPreTalent()
    {
        if(_config.pre_talent.Count <= 0 || IsMaxLevel()) return null;
        var talentRequire = _config.pre_talent[_archive.level];
        if(string.IsNullOrEmpty(talentRequire.id)) return null;
        return TalentModule.Instance.IsComplete(talentRequire.id) ? null : talentRequire;

    }
    public long GetWorkTeamRemainTime()
    {
        return _team != null ? _team.GetRemainTime() : 0;
    }
    public long GetWorkTeamDuration()
    {
        return _team != null ? _team.duration : 0;
    }
    public int GetBuildChildCount()
    {
        int count = 0;
        foreach(var child in _children) {
            if(child.GetLevel() > 0) ++count;
        }
        return count;
    }
    public BuildingData GetChildData(int index) {
        if(index < 0 || index >= _children.Count) return null;
        return _children[index];
    }
    #endregion
    #region  房间操作相关
    public bool PreBuildingFit()
    {
        var level = _archive.level;
        if(level >= _config.max_level) return false;
        if(!IsEnhanceLevelRatioMax() && level > 0) return false;
        if(_config.pre_building.Count <= 0) return true;
        return _buildingModule.IsPreBuildingReady(_config.pre_building[level]);
    }
    public bool CanLevelup()
    {
        var level = _archive.level;
        if(level >= _config.max_level) return false;
        if(!IsEnhanceLevelRatioMax() && level > 0) return false;
        if(_config.pre_building.Count > 0 && !_buildingModule.IsPreBuildingReady(_config.pre_building[level])) return  false;
        if(_config.pre_talent.Count > 0 && !TalentModule.Instance.IsTalentReady(_config.pre_talent[level].id)) return  false;
        return true;
    }
    public bool FinishLevelup(bool refresh)
    {
        _archive.level++;
        if(Config.type == BuidlingType.Cell) {
            _archive.staff = Config.id + "_worker";
        }
        if(_config.enhance_set.Count > 0) {
            EnhanceSetConfig enhanceSet = ConfigModule.Instance.GetEnhanceSet(_config.enhance_set[_archive.level - 1]);
            _archive.SetDefaultEnhance(enhanceSet.enhances);
        }
        _team = null;
        TraceModule.Instance.TraceBuild(_config.id, _archive.level);
        if(!refresh) return false;
        UpdateBuildingStatus();
        RefreshBaseProp();
        RefreshCurEnhance();
        RefreshAllProp();
        return true;
    }
    public bool LevelupEnhance(string enhanceId)
    {
        if(_curEnhances.TryGetValue(enhanceId, out var enhance)) {
            if(enhance.IsMax()) return false;
            if(PlayerModule.Instance.UseMoney(enhance.LvlupCost())) {
                var oldValue = enhance.value;
                var oldRatio = EnhanceLevelRatio();

                enhance.SetLevel(_archive.LevelupEnhance(enhanceId));

                var newValue = enhance.value;
                var newRatio = EnhanceLevelRatio();
                if(oldRatio < 0.3 && newRatio >= 0.3) {
                    AddMillstoneBonus(_curEnhanceSet.bonus[0].item, _curEnhanceSet.bonus[0].amount);
                }
                if(oldRatio < 0.6 && newRatio >= 0.6) {
                    AddMillstoneBonus(_curEnhanceSet.bonus[1].item, _curEnhanceSet.bonus[1].amount);
                }
                if(oldRatio < 1 && newRatio >= 1) {
                    AddMillstoneBonus(_curEnhanceSet.bonus[2].item, _curEnhanceSet.bonus[2].amount);
                }

                SetEnhanceEffect(enhance.cfg.enhance_effect, oldValue, newValue, enhance.cfg.ratio, true);
                return true;
            }

        }
        return false;
    }
    public bool AssignStaff(string staffId)
    {
        if(staffId == _archive.staff) return false;
        _archive.staff = staffId;
        RefreshBaseProp();
        RefreshStaffEnhance();
        RefreshAllProp();
        return true;
    }
    private void AddMillstoneBonus(string itemType, int number)
    {
        if(itemType == ItemType.Money) {
            PlayerModule.Instance.AddMoneyWithBigEffect(number, true);
        } else if(itemType == ItemType.Coin) {
            PlayerModule.Instance.AddCoinWithBigEffect(number, true);
        }
        PlayerModule.Instance.AddStar(1, true);
    }
    #endregion
    #region  Enhance相关
    private float CalSalary()
    {
        var serviceType = Config.service;
        switch(serviceType) {
            case ServiceType.ReceptionCell:
                return ConfigModule.Instance.Common().reception_worker_salary;
            case ServiceType.HotdogBureau:
            case ServiceType.HambergBureau:
            case ServiceType.PizzaBureau:
            {
                if(String.IsNullOrEmpty(_archive.staff)) return 0;
                return StaffModule.Instance.GetSalary(_archive.staff);
            }
            case ServiceType.SausageStorageCell:
                return ConfigModule.Instance.Common().sausage_storage_worker_salary;
            case ServiceType.CheeseStorageCell:
                return ConfigModule.Instance.Common().cheese_storage_worker_salary;
            case ServiceType.FlourStorageCell:
                return ConfigModule.Instance.Common().flour_storage_worker_salary;
            case ServiceType.DeliverCell:
                return ConfigModule.Instance.Common().deliver_worker_salary;
        }
        return 0;
    }
    private void RefreshBaseProp()
    {
        var level = _archive.level;
        _baseProps[BuildingProperty.LvlUpDuration] = level < _config.max_level ? _config.lvlup_duration[level] : 0;
        _baseProps[BuildingProperty.LvlUpCost] = level < _config.max_level ? _config.lvlup_cost[level] : 0;
        _baseProps[BuildingProperty.ServiceDuration] = level > 0 ? _config.service_duration[level-1] : 0;
        _baseProps[BuildingProperty.Capacity] = level > 0 ? _config.capacity[level-1] : 0;
        _baseProps[BuildingProperty.Revenue] = level > 0 ? _config.revenue[level-1] : 0;
        _baseProps[BuildingProperty.Salary] = level > 0 ? CalSalary() : 0;
        _baseProps[BuildingProperty.EnhanceLvlUpDiscount] = 0;
        _baseProps[BuildingProperty.MoveSpeed] = level > 0 ? ConfigModule.Instance.Common().default_puppet_speed : 0;
        _baseProps[BuildingProperty.InstantService] = 0;
        _baseProps[BuildingProperty.Sausage] = level > 0 ? (_config.sausage_require.Count > 0 ? _config.sausage_require[level-1] : 0) : 0;
        _baseProps[BuildingProperty.Cheese] = level > 0 ? (_config.cheese_require.Count > 0 ? _config.cheese_require[level-1] : 0) : 0;
        _baseProps[BuildingProperty.Flour] = level > 0 ? (_config.flour_require.Count > 0 ? _config.flour_require[level-1] : 0) : 0;
        _baseProps[BuildingProperty.Timeout] = _config.timeout;
        _baseProps[BuildingProperty.ExtraRevenueHotdog] = 0;
        _baseProps[BuildingProperty.ExtraRevenueHamberg] = 0;
        _baseProps[BuildingProperty.ExtraRevenuePizza] = 0;
        _baseProps[BuildingProperty.RevenueDouble] = 0;
        _baseProps[BuildingProperty.RevenueTriple] = 0;
        _baseProps[BuildingProperty.NoIngredient] = 0;
        _baseProps[BuildingProperty.GuestRate] = level > 0 ? (_config.guest_rate.Count > 0 ? _config.guest_rate[level-1] : 0) : 0;
        _baseProps[BuildingProperty.GuestInterval] = level > 0 ? (_config.guest_interval.Count > 0 ? _config.guest_interval[level-1] : 0) : 0;
    }
    public float GetNextBaseProp(string prop)
    {
        var level = _archive.level;
        if(level >= _config.max_level) return _baseProps[prop];
        switch(prop) {
            case BuildingProperty.Capacity:
            {
                return _config.capacity[level];
            }
        }
        return 0f;
    }
    private void RefreshCurEnhance()
    {
        var level = _archive.level;
        if(_config.enhance_set == null || _config.enhance_set.Count == 0 || level <= 0) return;
        _curEnhances.Clear();
        _buildingEnhanceProps.Clear();

        for(int i = 0; i < level - 1; ++i) {
            var enhanceSet = ConfigModule.Instance.GetEnhanceSet(_config.enhance_set[i]);
            for(int j = 0; j < enhanceSet.enhances.Count; ++j) {
                var enhanceCfg = ConfigModule.Instance.GetEnhance(enhanceSet.enhances[j]);
                SetEnhanceEffect(enhanceCfg.enhance_effect, 0, enhanceCfg.MaxValue(), enhanceCfg.ratio);
            }
        }

        _curEnhanceSet = ConfigModule.Instance.GetEnhanceSet(_config.enhance_set[level - 1]);
        foreach(var enhanceId in _curEnhanceSet.enhances) {
            var enhanceCfg = ConfigModule.Instance.GetEnhance(enhanceId);
            var enhance = new BuildingEnhance(enhanceCfg.id, enhanceCfg, _archive.GetEnhanceLevel(enhanceCfg.id), this);
            if(enhance.level > 0) SetEnhanceEffect(enhanceCfg.enhance_effect, 0, enhance.value, enhanceCfg.ratio);
            _curEnhances.Add(enhance.id, enhance);
        }
    }
    private void RefreshStaffEnhance()
    {
        _staffEnhanceProps.Clear();
        var staffData = StaffModule.Instance.GetArchiveData(_archive.staff);
        if(staffData != null) {
            foreach(var staffAttr in staffData.attributes) {
                var attr = ConfigModule.Instance.GetAttribute(staffAttr.attributeId);
                if(attr.global < 1 && (attr.service == _config.service || string.IsNullOrEmpty(attr.service))) {
                    _staffEnhanceProps.SetProp(attr.enhance_effect, staffAttr.value, attr.ratio > 0);
                }
            }
        }
    }
    private void SetEnhanceEffect(string prop, float oldValue, float newValue, int ratio, bool refresh = false)
    {
        _buildingEnhanceProps.SetProp(prop, newValue-oldValue, ratio > 0);
        if(refresh) RefreshProp(prop);
    }
    public void RefreshProp(string prop)
    {
        if(!_props.ContainsKey(prop)) return;
        _props[prop] = CalculateProp(prop, _baseProps[prop]);
    }
    private float CalculateProp(string prop, float baseValue)
    {
        var endValue = baseValue;
        var func = prop[0];
        var talentRatio = string.Format("{0}_{1}_{2}", prop, _config.service, "ratio");
        var talentValue = string.Format("{0}_{1}_{2}", prop, _config.service, "value");

        var adBuffRatio = string.Format("{0}_{1}", prop, "ad_ratio");
        var shopBuffRatio = string.Format("{0}_{1}", prop, "shop_ratio");
        if(func == 'a') {
            endValue = endValue * (1 + _buildingEnhanceProps.RatioEnhance(prop) + _staffEnhanceProps.RatioEnhance(prop) + TalentModule.Instance.GetEnhanceValue(talentRatio) + BuffModule.Instance.GetBuff(adBuffRatio) + BuffModule.Instance.GetBuff(shopBuffRatio))
                        + _buildingEnhanceProps.ValueEnhance(prop) + _staffEnhanceProps.ValueEnhance(prop) + TalentModule.Instance.GetEnhanceValue(talentValue);
        } else if(func == 's') {
            endValue = (endValue - _buildingEnhanceProps.ValueEnhance(prop) - _staffEnhanceProps.ValueEnhance(prop) - TalentModule.Instance.GetEnhanceValue(talentValue))
                        * (1 - _buildingEnhanceProps.RatioEnhance(prop) - _staffEnhanceProps.RatioEnhance(prop) - TalentModule.Instance.GetEnhanceValue(talentRatio));
        } else if(func == 'd') {
            endValue = (endValue - _buildingEnhanceProps.ValueEnhance(prop) - _staffEnhanceProps.ValueEnhance(prop) - TalentModule.Instance.GetEnhanceValue(talentValue))
                        / (1 + _buildingEnhanceProps.RatioEnhance(prop) + _staffEnhanceProps.RatioEnhance(prop) + TalentModule.Instance.GetEnhanceValue(talentRatio));
        }
        return endValue < 0 ? 0 : endValue;
    }
    public void RefreshAllProp()
    {
        var propTypes = _buildingModule.GetBuildingPropTypes();
        foreach(var propType in propTypes) {
            RefreshProp(propType);
        }
    }
    public float EnhanceLevelRatio()
    {
        int totalLevel = 0;
        int curLevel = 0;
        foreach(var enhancePair in _curEnhances) {
            var enhance = enhancePair.Value;
            totalLevel += enhance.cfg.max_level;
            curLevel += enhance.level;
        }
        return totalLevel == 0 ? 0 : (float)curLevel / totalLevel;
    }
    public bool IsEnhanceLevelRatioMax()
    {
        foreach(var enhance in _curEnhances.Values) {
            if(enhance.level < enhance.cfg.max_level) return false;
        }
        return true;
    }
    public Dictionary<string, BuildingEnhance> GetCurEnhances()
    {
        return _curEnhances;
    }
    public BuildingEnhance GetEnhance(string enhanceId)
    {
        return _curEnhances.TryGetValue(enhanceId, out var value) ? value : null;
    }
    public List<Bonus> GetEnhanceSetBonus()
    {
        if(_curEnhanceSet != null) {
            return _curEnhanceSet.bonus;
        }
        return new List<Bonus>();
    }
    public float GetProp(string prop)
    {
        return _props[prop];
    }
    public float GetPropEffy(string prop)
    {
        var value = _props[prop];
        return value > 0 ? GameUtil.DefaultTimeInterval / value : 0;
    }
    public float GetNextProp(string prop)
    {
        if(IsMaxLevel()) return _props[prop];
        return CalculateProp(prop, GetNextBaseProp(prop));
    }
    public float GetChildProps(string prop)
    {
        float total = 0;
        foreach(var child in _children) {
            total += child.GetProp(prop);
        }
        return total;
    }
    public float GetChildPropEffy(string prop)
    {
        float total = 0;
        foreach(var child in _children) {
            var value = child.GetProp(prop);
            total += value > 0 ? GameUtil.DefaultTimeInterval / value : 0;
        }
        return total;
    }
    public int GetProfit()
    {
        if(_archive.level <= 0) return 0;
        return (int)(GetProp(BuildingProperty.Revenue) * GetPropEffy(BuildingProperty.ServiceDuration));
    }
    #endregion
    #region Ingredient
    public void RequireIngredient()
    {
        _sausage = (int)GetProp(BuildingProperty.Sausage);
        _cheese = (int)GetProp(BuildingProperty.Cheese);;
        _flour = (int)GetProp(BuildingProperty.Flour);
        _buildingModule.GenerateIngredientOrder(_config.id, _sausage, _cheese, _flour);
    }
    public void MetIngredient(ref DeliverOrder deliveryOrder, ref Dictionary<string, int> ingredients)
    {
        if(deliveryOrder.sausage > 0 && ingredients[IngredientType.Sausage] > 0) {
            var count = ingredients[IngredientType.Sausage];
            if(deliveryOrder.sausage >= count) {
                _sausage -= count;
                deliveryOrder.sausage -= count;
                ingredients[IngredientType.Sausage] = 0;
                QuestModule.Instance.OnDeliver(IngredientType.Sausage, count);
                BuildingModule.Instance.SausageDeliverd += count;
            } else {
                ingredients[IngredientType.Sausage] -= deliveryOrder.sausage;
                QuestModule.Instance.OnDeliver(IngredientType.Sausage, _sausage);
                BuildingModule.Instance.SausageDeliverd += _sausage;
                _sausage -= deliveryOrder.sausage;
                deliveryOrder.sausage = 0;
            }
        }
        if(deliveryOrder.cheese > 0 && ingredients[IngredientType.Cheese] > 0) {
            var count = ingredients[IngredientType.Cheese];
            if(deliveryOrder.cheese >= count) {
                _cheese -= count;
                deliveryOrder.cheese -= count;
                ingredients[IngredientType.Cheese] = 0;
                QuestModule.Instance.OnDeliver(IngredientType.Cheese, count);
            } else {
                ingredients[IngredientType.Cheese] -= deliveryOrder.cheese;
                QuestModule.Instance.OnDeliver(IngredientType.Cheese, _cheese);
                _cheese -= deliveryOrder.cheese;
                deliveryOrder.cheese = 0;
            }
        }
        if(deliveryOrder.flour > 0 && ingredients[IngredientType.Flour] > 0) {
            var count = ingredients[IngredientType.Flour];
            if(deliveryOrder.flour >= count) {
                _flour -= count;
                deliveryOrder.flour -= count;
                ingredients[IngredientType.Flour] = 0;
                QuestModule.Instance.OnDeliver(IngredientType.Flour, count);
            } else {
                ingredients[IngredientType.Flour] -= deliveryOrder.flour;
                QuestModule.Instance.OnDeliver(IngredientType.Flour, _flour);
                _flour -= deliveryOrder.flour;
                deliveryOrder.flour = 0;
            }
        }
    }
    public bool IsIngredientOk()
    {
        return _sausage == 0 && _cheese == 0 && _flour == 0;
    }
    #endregion
    #region ITimeProgressBarData
    public void Start(WorkTeamData team)
    {
        _team = team;
        UpdateBuildingStatus();
    }
    public void Finish()
    {
        _buildingModule.FinishLevelupBuilding(_config.id);
    }
    public void ReduceTimeByAd(long time, long adCooldown)
    {
        if (_team != null) _team.adCooldownTime = NtpModule.Instance.UtcNowSeconds + adCooldown;
        _buildingModule.ReduceTimeByAd(time, _config.id);
    }

    public string GetAdUnitName()
    {
        return AdUnitName.ad_unit_reward_building;
    }
    #endregion
    #region Debug Code
    public void DumpProperty()
    {
        string log = "[Building Property] BuildingId = " + _config.id;
        log += "\n***Ingredients: \n";
        log += "sausage = " + _sausage + ", cheese = " + _cheese + ", flour = " + _flour + "\n";
        log += "-------------------------------------------------";
        log += "\n***Enhances: \n";
        foreach(var enhance in _curEnhances.Values) {
            log += enhance.id + " : " + enhance.value + "\n";
        }
        log += "-------------------------------------------------";
        log += "\n***Property: \n";
        var propTypes = _buildingModule.GetBuildingPropTypes();
        foreach(var propType in propTypes) {
            log += propType;
            log += ": base = " + _baseProps[propType];
            log += ": building_enhance_ratio = " + _buildingEnhanceProps.RatioEnhance(propType);
            log += ": building_enhance_value = " + _buildingEnhanceProps.ValueEnhance(propType);
            log += ": staff_enhance_ratio = " + _staffEnhanceProps.RatioEnhance(propType);
            log += ": staff_enhance_value = " + _staffEnhanceProps.ValueEnhance(propType);
            log += "; all = " + _props[propType];
            log += "\n";
        }
        AppLogger.Log(log);
    }
    #endregion
}
