using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceptionDivisionCtrl : BuildingCtrl
{
    private SpotConfig _entrySpot;
    public override void InitFlow()
    {
        var config = ConfigModule.Instance;
        _entrySpot = config.GetSpot(_buildingData.Config.id, SpotDefine.Entry, 1);
    }
    public override void AddGuest(PuppetCtrl guest)
    {
        guest.Move(_entrySpot, OnEnter);
    }
    private void OnEnter(PuppetCtrl guest)
    {
        var cell = BuildingMgr.Instance.GetValidBuilding(ServiceType.ReceptionCell);
        if(cell == null) {
            guest.EmojiView.Show(EmojiType.Angry);
            PuppetMgr.Instance.PuppetExit(guest);
        } else {
            cell.AddGuest(guest);
        }
    }
}
