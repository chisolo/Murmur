using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;

public class BureauCtrl : BuildingCtrl
{
    public struct BureauSpot
    {
        public SpotConfig bureauExit;
        public SpotConfig counter;
        public SpotConfig guestCounter;
        public SpotConfig kitchen;
        public List<SpotConfig> tables;
        public List<SpotConfig> enter_relays;
        public List<SpotConfig> exit_relays;
        public SpotConfig init_entry;
        public BureauSpot(string prefix, BuildingData buildingData)
        {
            var config = ConfigModule.Instance;
            bureauExit = config.GetSpot(prefix, SpotDefine.Exit, 1);
            counter = config.GetSpot(prefix, SpotDefine.Counter, 1);
            guestCounter = config.GetSpot(prefix, SpotDefine.GuestCounter, 1);
            kitchen = config.GetSpot(prefix, SpotDefine.Kitchen, 1);

            tables = new List<SpotConfig>();
            enter_relays = new List<SpotConfig>();
            exit_relays = new List<SpotConfig>();
            for(int i = 1; i <= 3; ++i) {
                tables.Add(config.GetSpot(prefix, SpotDefine.Table, i));
                var enter_relay = config.GetSpot(prefix, SpotDefine.EnterRelay, i);
                if(enter_relay != null) enter_relays.Add(enter_relay);
                var exit_relay = config.GetSpot(prefix, SpotDefine.ExitRelay, i);
                if(exit_relay != null) exit_relays.Add(exit_relay);
            }
            init_entry = enter_relays.Count > 0 ? enter_relays[enter_relays.Count - 1] : bureauExit;
        }
    }
    private LineupArea _lineup;
    private PuppetCtrl _staff;
    private PuppetCtrl _guest;
    private int _stage;
    private SpotConfig _staffSpot;
    private Dictionary<string, IngredientHubView> _igredientHubViews;
    private BureauSpot _spot;
    public override void InitFlow()
    {
        var prefix = _buildingData.Config.id;
        _spot = new BureauSpot(prefix, _buildingData);
        _lineup = new LineupArea((int)_buildingData.GetProp(BuildingProperty.Capacity), prefix, this);
        _staffSpot = _spot.counter;
        _igredientHubViews = new Dictionary<string, IngredientHubView>();
        _stage = 0;
        RefreshStaff();
    }
    public override async Task LoadExtraHub() 
    {
        IngredientHubView sausageView = await LoadModComponentTask<IngredientHubView>(ConfigModule.Instance.GetMod(GetSausageHubPath()), transform);
        sausageView?.SetCount(0, true);
        _igredientHubViews.Add(IngredientType.Sausage, sausageView);
        IngredientHubView cheeseView = await LoadModComponentTask<IngredientHubView>(ConfigModule.Instance.GetMod(GetCheeseHubPath()), transform);
        cheeseView?.SetCount(0, true);
        _igredientHubViews.Add(IngredientType.Cheese, cheeseView);
        IngredientHubView flourView = await LoadModComponentTask<IngredientHubView>(ConfigModule.Instance.GetMod(GetFlourHubPath()), transform);
        flourView?.SetCount(0, true);
        _igredientHubViews.Add(IngredientType.Flour, flourView);
    }
    public override void AddGuest(PuppetCtrl guest)
    {
        if(CanServe()) {
            ServeInGuest(guest);
        } else {
            guest.Timeout = _buildingData.GetProp(BuildingProperty.Timeout);
            _lineup.Push(guest);
            if(_spot.enter_relays.Count > 0) {
                guest.Move(_spot.enter_relays[0], OnLineEnterRelay0, 0.5f);
            } else {
                _lineup.InLine(guest);
            }
        }
    }
    public override bool CanAddGuest()
    {
        return _lineup.Avaliable() && _buildingData.GetLevel() > 0;
    }
    public override int LineupGuestCount()
    {
        return _lineup.Length();
    }
    public override void RefreshStaff()
    {
        if(_staff != null) {
            _staff.Clear();
            _staff = null;
        }
        var staffId = _buildingData.GetStaff();
        if(string.IsNullOrEmpty(staffId)) return;
        _staff = PuppetMgr.Instance.SpawnStaff(staffId, _buildingData.GetProp(BuildingProperty.MoveSpeed), _staffSpot, _buildingData.Config.service);
        if(_stage == 0 && _guest == null) {
            TryServe();
        } else if(_stage == 1) {
            ServeFlow(_staff);
        } else {
            --_stage;
            ServeFlow(_staff);
        }
    }
    public override void RefreshCapacity()
    {
        _lineup.UpdateCapacity((int)_buildingData.GetProp(BuildingProperty.Capacity));
    }
    public override void RefreshMoveSpeed()
    {
        _staff?.SetSpeed(_buildingData.GetProp(BuildingProperty.MoveSpeed));
    }
    public override void RefreshIngredient()
    {
        _igredientHubViews[IngredientType.Sausage]?.SetCount(_buildingData.Sausage);
        _igredientHubViews[IngredientType.Cheese]?.SetCount(_buildingData.Cheese);
        _igredientHubViews[IngredientType.Flour]?.SetCount(_buildingData.Flour);
        TryServe();
    }
    public override void SpawnInitGuest()
    {
        var count = (_lineup.Capacity() + 1) / 2;
        for(int i = 0; i < count; ++i) {
            var guest = PuppetMgr.Instance.SpawnGuest(ConfigModule.Instance.Common().default_puppet_speed, _buildingData.Config.service, _spot.init_entry);
            if(CanServe()) {
                ServeInGuest(guest);
            } else {
                guest.Timeout = _buildingData.GetProp(BuildingProperty.Timeout);
                _lineup.Push(guest);
                _lineup.InLine(guest);
            }
        }
    }
    private void Update()
    {
        var delta = Time.deltaTime;
        _lineup.Tick(delta);
    }
    private bool CanServe()
    {
        return _staff != null && _guest == null && _buildingData.IsIngredientOk();
    }
    private void TryServe()
    {
        if(CanServe()) {
            var next = _lineup.Pop();
            if(next != null) ServeGuest(next);
        }
    }
    private void ServeInGuest(PuppetCtrl guest)
    {
        _guest = guest;
        if(_spot.enter_relays.Count > 0) {
            guest.Move(_spot.enter_relays[0], OnServeEnterRelay0, 0.5f);
        } else {
            guest.Move(_spot.guestCounter, OnGuestServe);
        }
    }
    public void OnServeEnterRelay0(PuppetCtrl guest)
    {
        if(_spot.enter_relays.Count > 1) {
            guest.Move(_spot.enter_relays[1], OnServeEnterRelay1, 0.5f);
        } else {
            guest.Move(_spot.guestCounter, OnGuestServe);
        }
    }
    public void OnServeEnterRelay1(PuppetCtrl guest)
    {
        if(_spot.enter_relays.Count > 2) {
            guest.Move(_spot.enter_relays[2], OnServeEnterRelay2, 0.5f);
        } else {
            guest.Move(_spot.guestCounter, OnGuestServe);
        }
    }
    public void OnServeEnterRelay2(PuppetCtrl guest)
    {
        guest.Move(_spot.guestCounter, OnGuestServe);
    }
    public void OnLineEnterRelay0(PuppetCtrl guest)
    {
        if(_spot.enter_relays.Count > 1) {
            guest.Move(_spot.enter_relays[1], OnLineEnterRelay1, 0.5f);
        } else {
            _lineup.InLine(guest);
        }
    }
    public void OnLineEnterRelay1(PuppetCtrl guest)
    {
        if(_spot.enter_relays.Count > 2) {
            guest.Move(_spot.enter_relays[2], OnLineEnterRelay2, 0.5f);
        } else {
            _lineup.InLine(guest);
        }
    }
    public void OnLineEnterRelay2(PuppetCtrl guest)
    {
        _lineup.InLine(guest);
    }
    private void ServeGuest(PuppetCtrl guest)
    {
        _guest = guest;
        _guest.Move(_spot.guestCounter, OnGuestServe);
    }
    private void OnGuestServe(PuppetCtrl guest)
    {
        _stage = 1;
        ServeFlow(_staff);
    }
    private void ServeFlow(PuppetCtrl guest)
    {
        if(_staff == null) return;
        if(_stage == 1) {
            ++_stage;
            _staff.Do(ActionDefine.Talk, ConfigModule.Instance.Common().talk_duration, ServeFlow); 
            _guest.Do(ActionDefine.Talk, ConfigModule.Instance.Common().talk_duration);
        } else if(_stage == 2) {
            ++_stage;
            _staff.Move(_spot.kitchen, ServeFlow);
        } else if(_stage == 3) {
            ++_stage;
            _staff.Do(ActionDefine.Cook, ServiceDuration(), ServeFlow, true);
            _staff.PlayCookEffect(true);
        } else if(_stage == 4) {
            ++_stage;
            _staffSpot = _spot.kitchen;
            _staff.PlayCookEffect(false);
            _staff.TakeFood(true);
            _staff.Move(_spot.counter, ServeFlow);
        } else if(_stage == 5) {
            PlayerModule.Instance.AddMoneyWithSmallEffect(ServeRevenue(), true, _staff.GetEffectRoot());
            _stage = 0;
            _staffSpot = _spot.counter;
            _staff.TakeFood(false);

            if(NeedIngredient()) _buildingData.RequireIngredient();
            if(BuildingModule.Instance.InServeTutorial) EventModule.Instance.FireEvent(EventDefine.TutorialStepDone);
            QuestModule.Instance.OnServe(_buildingData.Config.service);

            GuestGoTable();
            _guest = null;
            TryServe();
        }
    }
    private void GuestGoTable()
    {
        if(_guest == null) return;
        _guest.TakeFood(true);
        _guest.Move(_spot.tables[Random.Range(0, 3)], GuestEat);
    }

    private void GuestEat(PuppetCtrl guest)
    {
        guest.TakeFood(false);
        guest.EatFood(true);
        guest.Do(ActionDefine.Eat, ConfigModule.Instance.Common().eat_duration, GuestLeaveBuilding);
    }

    private void GuestLeaveBuilding(PuppetCtrl guest)
    {
        guest.EatFood(false);
        guest.Move(_spot.bureauExit, RemoveGuest, 0.5f);
        guest.EmojiView.Show(EmojiType.Smile);
    }

    public override void RemoveGuest(PuppetCtrl guest)
    {
        if(_spot.exit_relays.Count > 0) {
            guest.Move(_spot.exit_relays[0], OnExitRelay0, 0.5f);
        } else {
            PuppetMgr.Instance.PuppetExit(guest);
        }
    }
    public void OnExitRelay0(PuppetCtrl guest)
    {
        if(_spot.exit_relays.Count > 1) {
            guest.Move(_spot.exit_relays[1], OnExitRelay1, 0.5f);
        } else {
            PuppetMgr.Instance.PuppetExit(guest);
        }
    }
    public void OnExitRelay1(PuppetCtrl guest)
    {
        if(_spot.exit_relays.Count > 2) {
            guest.Move(_spot.exit_relays[2], OnExitRelay2, 0.5f);
        } else {
            PuppetMgr.Instance.PuppetExit(guest);
        }
    }
    public void OnExitRelay2(PuppetCtrl guest)
    {
        PuppetMgr.Instance.PuppetExit(guest);
    }
    private string GetSausageHubPath()
    {
        return string.Format("{0}_{1}", _buildingData.Config.id, "hub_sausage");
    }
    private string GetCheeseHubPath()
    {
        return string.Format("{0}_{1}", _buildingData.Config.id, "hub_cheese");
    }
    private string GetFlourHubPath()
    {
        return string.Format("{0}_{1}", _buildingData.Config.id, "hub_flour");
    }
    private int ServeRevenue()
    {
        var tripleProb = _buildingData.GetProp(BuildingProperty.RevenueTriple);
        var doubleProb = _buildingData.GetProp(BuildingProperty.RevenueDouble);
        var ret = 1;

        if(RandUtil.Percent(tripleProb)) ret = 3;
        else if(RandUtil.Percent(doubleProb)) ret = 2;

        return ret * (int)_buildingData.GetProp(BuildingProperty.Revenue);
    }
    private float ServiceDuration()
    {
        var buffDuration = BuffModule.Instance.GetBuff(BuffType.BuffServiceBoostType);
        return buffDuration < 0.001f  ? RandUtil.Percent(_buildingData.GetProp(BuildingProperty.InstantService)) ? 0.5f : _buildingData.GetProp(BuildingProperty.ServiceDuration) : buffDuration;
    }
    private bool NeedIngredient()
    {
        return !RandUtil.Percent(_buildingData.GetProp(BuildingProperty.NoIngredient));
    }
}
