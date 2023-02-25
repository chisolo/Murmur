using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;

public class ReceptionCellCtrl : BuildingCtrl
{    
    public struct ReceptionCellSpot
    {
        public SpotConfig counter;
        public SpotConfig guestCounter;
        public SpotConfig lobbyHotdog;
        public SpotConfig lobbyHamberg;
        public SpotConfig lobbyPizza;

        public ReceptionCellSpot(string prefix)
        {
            var config = ConfigModule.Instance;
            counter = config.GetSpot(prefix, SpotDefine.Counter, 1);
            guestCounter = config.GetSpot(prefix, SpotDefine.GuestCounter, 1);
            lobbyHotdog = config.GetSpot(SpotDefine.LobbyHotdogBureau, 1);
            lobbyHamberg = config.GetSpot(SpotDefine.LobbyHambergBureau, 1);
            lobbyPizza = config.GetSpot(SpotDefine.LobbyPizzaBureau, 1);
        }
    }
    private LineupArea _lineup;
    private PuppetCtrl _worker;
    private PuppetCtrl _guest;
    private int _stage;
    private ReceptionCellSpot _spot;

    public override void InitFlow()
    {
        var prefix = _buildingData.Config.id;
        _spot = new ReceptionCellSpot(prefix);
        _lineup = new LineupArea((int)_buildingData.GetProp(BuildingProperty.Capacity), prefix, this);
        _stage = 0;

        RefreshWorker();
    }
    public override void AddGuest(PuppetCtrl guest)
    {
        if(_worker != null && _guest == null) {
            ServeGuest(guest);
        } else {
            guest.Timeout = _buildingData.GetProp(BuildingProperty.Timeout);
            _lineup.Push(guest);
            _lineup.InLine(guest);
        }
    }
    public override bool CanAddGuest()
    {
        return _worker != null && _lineup.Avaliable();
    }
    public override int LineupGuestCount()
    {
        return _lineup.Length();
    }
    public override void RefreshWorker()
    {
        if(_worker != null) return;
        var workerId = _buildingData.GetStaff();
        if(string.IsNullOrEmpty(workerId)) return;
        _worker = PuppetMgr.Instance.SpawnWorker(workerId, _buildingData.GetProp(BuildingProperty.MoveSpeed), _spot.counter, _buildingData.Config.service);
        if(_stage == 0) {
            ServeNext();
        } else {
            --_stage;
            ServeFlow(_worker);
        }
    }
    public override void RefreshCapacity()
    {
        _lineup.UpdateCapacity((int)_buildingData.GetProp(BuildingProperty.Capacity));
    }
    public override void RemoveGuest(PuppetCtrl guest)
    {
        PuppetMgr.Instance.PuppetExit(guest);
    }
    private void Update()
    {
        var delta = Time.deltaTime;
        _lineup.Tick(delta);
    }
    private void ServeGuest(PuppetCtrl guest)
    {
        _guest = guest;
        _stage = 1;
        _guest.Move(_spot.guestCounter, ServeFlow);
    }
    private void ServeNext()
    {
        var next = _lineup.Pop();
        if(next != null) ServeGuest(next);
    }
    private void ServeFlow(PuppetCtrl puppet)
    {
        if(_worker == null) return;
        if(_stage == 1) {
            ++_stage;
            var buffDuration = BuffModule.Instance.GetBuff(BuffType.BuffServiceBoostType);
            var serviceDuration = buffDuration < 0.001f ? _buildingData.GetProp(BuildingProperty.ServiceDuration) : buffDuration;
            _worker.Do(ActionDefine.Talk, serviceDuration, ServeFlow, true);
            _guest.Do(ActionDefine.Talk, serviceDuration);
        } else if(_stage == 2) {
            int revenue = (int)_buildingData.GetProp(BuildingProperty.Revenue);
            if(revenue > 0 && HasExtraRevenue()) PlayerModule.Instance.AddMoneyWithSmallEffect(revenue, true, _worker.GetEffectRoot());
            _stage = 0;
            GuestMoveLobby();
            _guest = null;
            ServeNext();
        }
    }
    private void GuestMoveLobby()
    {
        if(_guest == null) return;
        SpotConfig targetSpot = null;
        if(_guest.Target == ServiceType.HotdogBureau) {
            targetSpot = _spot.lobbyHotdog;
            _guest.EmojiView.Show(EmojiType.Hotdog);
        } else if(_guest.Target == ServiceType.HambergBureau) {
            targetSpot = _spot.lobbyHamberg;
            _guest.EmojiView.Show(EmojiType.Hamberg);
        } else if(_guest.Target == ServiceType.PizzaBureau) {
            targetSpot = _spot.lobbyPizza;
            _guest.EmojiView.Show(EmojiType.Pizza);
        }
        if(targetSpot != null) {
            _guest.Move(targetSpot, (ret) => {
                BuildingMgr.Instance.AssignGuest(ret);
            }, 1f);
        }
    }
    private bool HasExtraRevenue()
    {
        float prob = 0;
        if(_guest.Target == ServiceType.HotdogBureau) prob = _buildingData.GetProp(BuildingProperty.ExtraRevenueHotdog);
        else if(_guest.Target == ServiceType.HambergBureau) prob = _buildingData.GetProp(BuildingProperty.ExtraRevenueHamberg);
        else if(_guest.Target == ServiceType.PizzaBureau) prob = _buildingData.GetProp(BuildingProperty.ExtraRevenuePizza);
        return RandUtil.Percent(prob);
    }
}
