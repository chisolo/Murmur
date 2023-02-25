using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;

public class BuildingModule : Singleton<BuildingModule>, ITalentApply
{
    BuildingModule() {}
    private bool _inited = false;
    private Dictionary<string, BuildingConfig> _buildingConfigs;
    private BuildingArchive _buildingArchive;
    private Dictionary<string, BuildingData> _buildings;
    private Dictionary<string, List<BuildingData>> _services;
    private List<string> _buildingPropTypes = new List<string>();
    private List<float> _buildingRates = new List<float>();
    private List<string> _generateServices = new List<string>();
    private Dictionary<string, int> _products = new Dictionary<string, int>();
    private List<DeliverOrder> _orders = new List<DeliverOrder>();
    private WorkTeam _workTeam;
    public WorkTeam WorkTeam => _workTeam;
    public bool InServeTutorial {get; set;}
    public int SausageDeliverd {get; set;}
    public void Init()
    {
        InServeTutorial = false;
        if(_inited) return;
        _buildingConfigs = ConfigModule.Instance.Buildings();
        _buildingArchive = ArchiveModule.Instance.GetArchive<BuildingArchive>();
        _buildings = new Dictionary<string, BuildingData>();
        _services = new Dictionary<string, List<BuildingData>>();

        InitGenerate();
        InitPropertyDefine();

        InitBuildingDatas();

        TimerModule.Instance.CreateTimer(1f, OnUpdate, true, null, -1);
        _inited = true;
    }
    private void InitGenerate()
    {
        _generateServices.Add(ServiceType.HotdogBureau);
        _generateServices.Add(ServiceType.HambergBureau);
        _generateServices.Add(ServiceType.PizzaBureau);
        _buildingRates.Add(0);
        _buildingRates.Add(0);
        _buildingRates.Add(0);
    }
    private void InitPropertyDefine()
    {
        var buildingPropTypes = typeof(BuildingProperty).GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach(var propType in buildingPropTypes) {
            _buildingPropTypes.Add(propType.GetValue(null).ToString());
        }
        var ingredientTypes = typeof(IngredientType).GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach(var propType in ingredientTypes) {
            _products.Add(propType.GetValue(null).ToString(), 0);
        }
    }
    private void InitBuildingDatas()
    {
        var now = NtpModule.Instance.UtcNowSeconds;
        bool dirty = false;
        foreach(var buildingConfig in _buildingConfigs.Values) {
            var buildingId = buildingConfig.id;
            var service = buildingConfig.service;
            BuildingArchiveData archiveData = _buildingArchive.GetData(buildingId);
            if(archiveData == null) {
                var defaultLevel = buildingConfig.init_level;
                if(defaultLevel > 0 && buildingConfig.enhance_set.Count > 0) {
                    var enhanceSet = ConfigModule.Instance.GetEnhanceSet(buildingConfig.enhance_set[defaultLevel - 1]);
                    archiveData = new BuildingArchiveData(buildingId, buildingConfig.init_level, enhanceSet == null ? null : enhanceSet.enhances);
                } else {
                    archiveData = new BuildingArchiveData(buildingId, buildingConfig.init_level);
                }
                _buildingArchive.AddData(buildingId, archiveData);
                dirty = true;
            }

            BuildingData buildingData = new BuildingData();
            buildingData.Init(this, buildingConfig, archiveData);
            _buildings.Add(buildingConfig.id, buildingData);
            if(!_services.ContainsKey(service)) _services.Add(service, new List<BuildingData>());
            _services[service].Add(buildingData);
        }
        if(_buildingArchive.work_team.Capacity <= 0) {
            dirty = true;
            _buildingArchive.work_team.AddTeam(GameUtil.DefaultBuildTeam);
        }
        _workTeam = _buildingArchive.work_team;
        _workTeam.SetComplete(FinishLevelupBuilding);

        foreach(var team in _workTeam.teams) {
            if(!string.IsNullOrEmpty(team.owner) && team.endTime > 0) {
                if(team.endTime <= now) {
                    _buildings[team.owner].FinishLevelup(false);
                    team.Done();
                    dirty = true;
                } else {
                    _buildings[team.owner].Start(team);
                }
            }
        }
        if(dirty) Save();
    }
    public void PreLoad()
    {
        foreach(var building in _buildings.Values) {
            building.PreLoad();
        }
        CalculateGenerateRate();
        PlayerModule.Instance.UpdateMoneyLimit((int)GetServiceBuilding(ServiceType.TreasurySupply).GetProp(BuildingProperty.Capacity));
    }
    public void RefreshBuildingProps()
    {
        foreach(var building in _buildings.Values) {
            building.RefreshAllProp();
        }
    }
    #region 获取数据接口
    public List<string> GetBuildingPropTypes()
    {
        return _buildingPropTypes;
    }
    public BuildingData GetBuilding(string buildingId)
    {
        return _buildings.TryGetValue(buildingId, out var value) ? value : null;
    }
    public BuildingData GetServiceBuilding(string service)
    {
        return _services[service][0];
    }
    public Dictionary<string, BuildingData> GetBuildings()
    {
        return _buildings;
    }
    public string GetBuildingName(string buildingId)
    {
        if(_buildingConfigs.TryGetValue(buildingId, out var buildingConfig)) {
            return buildingConfig.name;
        }
        return string.Empty;
    }
    public int GetBuildingLevel(string buildingId)
    {
        if(_buildings.TryGetValue(buildingId, out var buildingData)) {
            return buildingData.GetLevel();
        }
        return 0;
    }
    public float GetTotalProp(string service, string buildingProperty)
    {
        float prop = 0;
        foreach(var building in _services[service]) {
            prop += building.GetProp(buildingProperty);
        }
        return prop;
    }
    public float GetTotalPropEffy(string service, string buildingProperty)
    {
        float propEffy = 0;
        foreach(var building in _services[service]) {
            propEffy += building.GetPropEffy(buildingProperty);
        }
        return propEffy;
    }
    public float GetTotalProp(string buildingProperty)
    {
        float prop = 0;
        foreach(var building in _buildings.Values) {
            prop += building.GetProp(buildingProperty);
        }
        return prop;
    }
    public string GetProfitBureau()
    {
        string profitBureau = ServiceType.HotdogBureau;
        float profitRevenue = 0f;
        foreach(var bureau in GameUtil.BureauList) {
            var revenue = GetTotalProp(bureau, BuildingProperty.Revenue);
            if(profitRevenue < revenue) {
                profitRevenue = revenue;
                profitBureau = bureau;
            }
        }
        return profitBureau;
    }
    public int GetStaffCount(string service)
    {
        var count = 0;
        foreach(var building in _services[service]) {
            if(building.HasStaff()) ++count;
        }
        return count;
    }
    public float GetBuildingsProp(string service, string prop)
    {
        float total = 0;
        if(_services.TryGetValue(service, out var buildingDatas)) {
            foreach(var data in buildingDatas) {
                total += data.GetProp(prop);
            }
        }
        return total;
    }
    public int GetBuiltBuildingCount(string service)
    {
        var count = 0;
        foreach(var building in _services[service]) {
            if(building.GetLevel() > 0) ++count;
        }
        return count;
    }
    public bool IsBuildingEnhanceMax(string buildingId)
    {
        if(_buildings.TryGetValue(buildingId, out var buildingData)) {
            return buildingData.IsEnhanceLevelRatioMax();
        }
        return false;
    }
    #endregion
    private void CalculateGenerateRate()
    {
        _buildingRates[0] = GetBuildingsProp(ServiceType.HotdogBureau, BuildingProperty.GuestRate);
        _buildingRates[1] = GetBuildingsProp(ServiceType.HambergBureau, BuildingProperty.GuestRate);
        _buildingRates[2] = GetBuildingsProp(ServiceType.PizzaBureau, BuildingProperty.GuestRate);
    }
    public string RandomService()
    {
        var idx = RandUtil.Pick(_buildingRates);
        if(idx < 0) return string.Empty;
        return _generateServices[idx];
    }
    #region 升级操作接口
    public void Save()
    {
        ArchiveModule.Instance.SaveArchive(_buildingArchive);
    }
    public void OnUpdate()
    {
        _workTeam.Process();
    }
    public void LevelupEnhance(string buildingId, string enhanceId)
    {
        if(_buildings.TryGetValue(buildingId, out var buildingData) && buildingData.LevelupEnhance(enhanceId)) {
            using (BuildingLevelupEventArgs args = BuildingLevelupEventArgs.Get()) {
                args.buildingId = buildingId;
                args.enhanceId = enhanceId;
                EventModule.Instance.FireEvent(EventDefine.BuildingEnhanceLevelup, args);
            }
            Save();
        }
    }
    public void LevelupBuilding(string buildingId)
    {
        if(_buildings.TryGetValue(buildingId, out var buildingData)) {
            if(PlayerModule.Instance.UseMoney((int)buildingData.GetProp(BuildingProperty.LvlUpCost))) {
                var team = _workTeam.GetFreeTeam();
                if(team == null) return;
                team.Begin(buildingId, (int)buildingData.GetProp(BuildingProperty.LvlUpDuration));
                buildingData.Start(team);
                using (BuildingLevelupEventArgs args = BuildingLevelupEventArgs.Get()) {
                    args.buildingId = buildingId;
                    EventModule.Instance.FireEvent(EventDefine.BuildingLevelup, args);
                }
                using (var arg  = EmptyEventArgs.Get()) {
                    EventModule.Instance.FireEvent(EventDefine.BuildingTeamUpdate, arg);
                }
                Save();
            }
        }
    }
    public void FinishLevelupBuilding(string buildingId)
    {
        if(_buildings.TryGetValue(buildingId, out var buildingData) && buildingData.FinishLevelup(true)) {
            CalculateGenerateRate();
            _workTeam.WorkDone(buildingId);
            using (BuildingLevelupEventArgs args = BuildingLevelupEventArgs.Get()) {
                args.buildingId = buildingId;
                EventModule.Instance.FireEvent(EventDefine.BuildingLevelupFinish, args);
            }
            if(buildingData.Config.type == BuidlingType.Supply) {
                PlayerModule.Instance.UpdateMoneyLimit((int)buildingData.GetProp(BuildingProperty.Capacity));
            }
            using (var arg  = EmptyEventArgs.Get()) {
                EventModule.Instance.FireEvent(EventDefine.BuildingTeamUpdate, arg);
            }
            Save();
        }
    }
    public bool IsBuildingReady(string buildingId)
    {
        if (string.IsNullOrEmpty(buildingId)) {
            return true;
        }
        if(_buildings.TryGetValue(buildingId, out var buildingData)) {
            return buildingData.GetLevel() >= 1;
        }
        return true;
    }
    public bool IsPreBuildingReady(BuildingRequire require)
    {
        if(_buildings.TryGetValue(require.id, out var buildingData)) {
            return buildingData.GetLevel() >= require.level;
        }
        return true;
    }
    public void AssignBuildingStaff(string buildingId, string staffId)
    {
        if(_buildings.TryGetValue(buildingId, out var buildingData) && buildingData.AssignStaff(staffId)) {
            CalculateGenerateRate();
            using (BuildingStaffEventArgs args = BuildingStaffEventArgs.Get()) {
                args.buildingId = buildingId;
                args.staffId = staffId;
                EventModule.Instance.FireEvent(EventDefine.AssignBuildingStaff, args);
            }
        }
        Save();
    }
    public string GetParentId(string buildingId)
    {
        if(_buildings.TryGetValue(buildingId, out var buildingData)) {
            return buildingData.Parent != null ? buildingData.Parent.Config.id : string.Empty;
        }

        return string.Empty;
    }
    public void ReduceTimeByAd(long time, string buildingId)
    {
        WorkTeamData team = _workTeam.GetTeamData(buildingId);
        if(team != null) {
            team.endTime -= time;
            team.ReduceAd();
            using (BuildingLevelupEventArgs args = BuildingLevelupEventArgs.Get()) {
                args.buildingId = buildingId;
                EventModule.Instance.FireEvent(EventDefine.ReduceBuildingTime, args);
            }
            Save();
        }
    }
    public BuildingData GetProcessBuildlingData()
    {
        long min = long.MaxValue;
        string buildingId = string.Empty;

        foreach (var team in _workTeam.teams) {
            if (team.endTime > 0 && team.endTime < min) {
                buildingId = team.owner;
                min = team.endTime;
            }
        }
        return GetBuilding(buildingId);
    }
    public int GetTotalProfit()
    {
        int profit = 0;
        foreach(var buildingData in _services[ServiceType.HotdogBureau]) {
            profit += buildingData.GetProfit();
        }
        foreach(var buildingData in _services[ServiceType.HambergBureau]) {
            profit += buildingData.GetProfit();
        }
        foreach(var buildingData in _services[ServiceType.PizzaBureau]) {
            profit += buildingData.GetProfit();
        }
        return profit;
    }
    #endregion

    #region Ingredient
    public void AddProducts(string service, string product)
    {
        if(_products[product] >= (int)GetBuildingsProp(service, BuildingProperty.Capacity)) return;
        ++_products[product];
        using (StorageIngredientArgs args = StorageIngredientArgs.Get()) {
            args.product = product;
            EventModule.Instance.FireEvent(EventDefine.StorageIngredient, args);
        }
    }
    public bool IsProductsFull(string service, string product)
    {
        return _products[product] >= (int)GetBuildingsProp(service, BuildingProperty.Capacity);
    }
    public int TakeProduct(string product, int amount)
    {
        var ret = 0;
        var cur = _products[product];
        if(cur >= amount) {
            _products[product] -= amount;
            ret = amount;
        } else {
            _products[product] = 0;
            ret = cur;
        }
        using (StorageIngredientArgs args = StorageIngredientArgs.Get()) {
            args.product = product;
            EventModule.Instance.FireEvent(EventDefine.StorageIngredient, args);
        }
        return ret;
    }
    public bool HasProduct(string product)
    {
        return _products[product] > 0;
    }
    public int GetProductCount(string product)
    {
        return _products[product];
    }
    public void GenerateIngredientOrder(string buildingId, int sausage, int cheese, int flour)
    {
        if(sausage == 0 && cheese == 0 && flour == 0) return;
        _orders.Add(DeliverOrder.Get(buildingId, sausage, cheese, flour));
        using (UpdateIngredientArgs args = UpdateIngredientArgs.Get()) {
            args.buildingId = buildingId;
            EventModule.Instance.FireEvent(EventDefine.UpdateIngredient, args);
        }
    }
    public void DispatchOrder(int amount, ref List<DeliverOrder> orders)
    {
        foreach(var order in _orders) {
            var sausage = 0;
            var cheese = 0;
            var flour = 0;
            if(amount > 0 && HasProduct(IngredientType.Sausage)) {
                if(amount > order.sausage) {
                    sausage = order.sausage;
                    order.sausage -= sausage;
                    amount -= sausage;
                } else {
                    sausage = amount;
                    order.sausage -= amount;
                    amount = 0;
                }
            }
            if(amount > 0 && HasProduct(IngredientType.Flour)) {
                if(amount > order.flour) {
                    flour = order.flour;
                    order.flour -= flour;
                    amount -= flour;
                } else {
                    flour = amount;
                    order.flour -= amount;
                    amount = 0;
                }
            }
            if(amount > 0 && HasProduct(IngredientType.Cheese)) {
                if(amount > order.cheese) {
                    cheese = order.cheese;
                    order.cheese -= cheese;
                    amount -= cheese;
                } else {
                    cheese = amount;
                    order.cheese -= amount;
                    amount = 0;
                }
            }
            if(sausage > 0 || cheese > 0 || flour > 0) {
                orders.Add(DeliverOrder.Get(order.buildingId, sausage, cheese, flour));
            }
            if(amount <= 0) break;
        }

        for(int i = _orders.Count - 1; i >= 0; --i) {
            if(_orders[i].IsEmpty()) {
                _orders[i].Return();
                _orders.RemoveAt(i);
            }
        }
    }
    public void DeliverBuildingOrder(ref DeliverOrder deliveryOrder, ref Dictionary<string, int> ingredients)
    {
        if(_buildings.TryGetValue(deliveryOrder.buildingId, out var building)) {
            building.MetIngredient(ref deliveryOrder, ref ingredients);
            using (UpdateIngredientArgs args = UpdateIngredientArgs.Get()) {
                args.buildingId = deliveryOrder.buildingId;
                EventModule.Instance.FireEvent(EventDefine.UpdateIngredient, args);
            }
        }
    }
    public void ReturnOrder(List<DeliverOrder> orders)
    {
        foreach(var order in orders) {
            if(order.IsEmpty()) continue;
            int index = _orders.FindIndex(e => e.buildingId == order.buildingId);
            if(index > 0) {
                _orders[index].Add(order.sausage, order.cheese, order.flour);
            } else {
                _orders.Insert(0, DeliverOrder.Get(order.buildingId, order.sausage, order.cheese, order.flour));
            }
        }
    }
    #endregion

    public void ApplyNewTalent(string id, string type, float value)
    {
        foreach(var building in _buildings.Values) {
            building.RefreshAllProp();
        }
    }

    #region  team
    public string FindNotOwnTeam()
    {
        return GameUtil.BuildTeamIdList.FirstOrDefault(id => !_workTeam.teams.Any(team => team.id == id));
    }

    public void AddTeam()
    {
        if (_workTeam.IsTeamMax()) {
            return;
        }

        var teamId = FindNotOwnTeam();
        _workTeam.AddTeam(teamId);

        using (var arg  = EmptyEventArgs.Get()) {
            EventModule.Instance.FireEvent(EventDefine.BuildingTeamUpdate, arg);
        }
        Save();
    }

    public void AddTeam(string teamId)
    {
        _workTeam.AddTeam(teamId);
        using (var arg  = EmptyEventArgs.Get()) {
            EventModule.Instance.FireEvent(EventDefine.BuildingTeamUpdate, arg);
        }
        Save();
    }
    #endregion
}