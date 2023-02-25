using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingArea
{
    private class WaitingSpot
    {
        public SpotConfig spot;
        public PuppetCtrl puppet;
        public WaitingSpot(SpotConfig spot)
        {
            this.spot = spot;
            puppet = null;
        }
    }
    private List<WaitingSpot> _waitSpots;
    private Queue<PuppetCtrl> _puppets;
    private bool _sit;
    private string _prefix;
    public WaitingArea(BuildingCtrl buildingCtrl, string prefix, int capacity, bool sit)
    {
        _prefix = prefix;
        _waitSpots = new List<WaitingSpot>();
        _puppets = new Queue<PuppetCtrl>();

        var config = ConfigModule.Instance;
        for(int i = 1; i <= capacity; ++i) {
            _waitSpots.Add(new WaitingSpot(config.GetSpot(_prefix, SpotDefine.SitWaiting, i)));
        }
    }
    public int Length()
    {
        return _puppets.Count;
    }
    public int Capacity()
    {
        return _waitSpots.Count;
    }
    public void UpdateCapacity(int capacity)
    {
        var origin = _waitSpots.Count;
        if(capacity < origin) return;

        var config = ConfigModule.Instance;
        for(int i = origin + 1; i <= capacity; ++i) {
            _waitSpots.Add(new WaitingSpot(config.GetSpot(_prefix, SpotDefine.SitWaiting, i)));
        }
    }
    public bool Avaliable()
    {
        return _puppets.Count < _waitSpots.Count;
    }
    public void Push(PuppetCtrl puppet)
    {
        for(int i = 0; i < _waitSpots.Count; ++i) {
            var spot = _waitSpots[i];
            if(spot.puppet == null) {
                spot.puppet = puppet;
                puppet.WaitingIndex = i;
                _puppets.Enqueue(puppet);
                puppet.Move(spot.spot);
            }
        }
    }
    public PuppetCtrl Pop()
    {
        if(_puppets.TryDequeue(out var puppet)) {
            _waitSpots[puppet.WaitingIndex].puppet = null;
            return puppet;
        }
        return null;
    }

    public void Tick()
    {
        // TODO Check Timeout
    }
}
