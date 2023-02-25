using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineupArea
{
    private List<SpotConfig> _lineSpots;
    private List<PuppetCtrl> _puppets;
    private SpotConfig _timeoutSpot;
    private int _tail;
    private string _prefix;
    private bool _dirty;
    private BuildingCtrl _buildingCtrl;
    public LineupArea(int capacity, string prefix, BuildingCtrl buildingCtrl)
    {
        _lineSpots = new List<SpotConfig>();
        _puppets = new List<PuppetCtrl>();
        _tail = 0;
        _prefix = prefix;
        _buildingCtrl = buildingCtrl;
        var config = ConfigModule.Instance;
        for(int i = 1; i <= capacity; ++i) {
            _lineSpots.Add(config.GetSpot(_prefix, SpotDefine.LineUp, i));
        }
        _timeoutSpot = config.GetSpot(_prefix, SpotDefine.Timeout, 1);
    }
    public int Length()
    {
        return _puppets.Count;
    }
    public int Capacity()
    {
        return _lineSpots.Count;
    }
    public void UpdateCapacity(int capacity)
    {
        var origin = _lineSpots.Count;
        if(capacity < origin) return;

        var config = ConfigModule.Instance;
        for(int i = origin + 1; i <= capacity; ++i) {
            _lineSpots.Add(config.GetSpot(_prefix, SpotDefine.LineUp, i));
        }
    }
    public bool Avaliable()
    {
        return _tail < _lineSpots.Count;
    }
    public void Push(PuppetCtrl puppet)
    {
        if(_tail >= _lineSpots.Count) return;
        _puppets.Add(puppet);
        puppet.WaitingIndex = _tail;
        _tail++;
    }
    public void InLine(PuppetCtrl puppet)
    {
        puppet.Move(_lineSpots[puppet.WaitingIndex], OnLineup);
    }
    public PuppetCtrl Pop()
    {
        if(_puppets.Count <= 0) return null;
        var puppet = _puppets[0];
        puppet.InLine = false;
        puppet.WaitingIndex = -1;
        _puppets.RemoveAt(0);
        _tail--;
        for(int i = 0; i < _tail; ++i) {
            _puppets[i].Move(_lineSpots[i], OnLineup);
        }
        return puppet;
    }
    public void Tick(float delta)
    {
        for(int i = _puppets.Count - 1; i >= 0; --i) {
            if(!_puppets[i].InLine) continue;
            if(_puppets[i].Timeout < 0) {
                _puppets[i].InLine = false;
                _puppets[i].WaitingIndex = -1;
                _puppets[i].EmojiView.Show(EmojiType.Timeout);
                _puppets[i].Move(_timeoutSpot, OnTimeoutSpot, 0.5f);
                _puppets.RemoveAt(i);
                _tail--;
                _dirty = true;
            } else {
               _puppets[i].Timeout -= delta;
            }
        }
        if(_dirty) {
            for(int j = 0; j < _tail; ++j) {
                _puppets[j].Move(_lineSpots[j], OnLineup);
            }
            _dirty = false;
        }
    }
    private void OnLineup(PuppetCtrl puppet)
    {
        puppet.InLine = true;
    }
    public void OnTimeoutSpot(PuppetCtrl puppet)
    {
        _buildingCtrl.RemoveGuest(puppet);
    }
}
