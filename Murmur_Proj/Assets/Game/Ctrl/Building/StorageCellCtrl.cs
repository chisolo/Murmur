using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageCellCtrl : BuildingCtrl
{
    private SpotConfig _counter;
    private PuppetCtrl _worker;
    private bool _working;
    private float _timer;
    public override void InitFlow()
    {
        var prefix = _buildingData.Config.id;
        _counter = ConfigModule.Instance.GetSpot(prefix, SpotDefine.Counter, 1);
        _timer = 0;
        RefreshWorker();
    }
    public override void RefreshWorker()
    {
        if(_worker != null) return;
        var workerId = _buildingData.GetStaff();
        if(string.IsNullOrEmpty(workerId)) return;
        _worker = PuppetMgr.Instance.SpawnWorker(workerId, _buildingData.GetProp(BuildingProperty.MoveSpeed), _counter, _buildingData.Config.service);
    }
    public void Produce()
    {
        if(_worker == null || _working) return;
        if(BuildingModule.Instance.IsProductsFull(_buildingData.Config.service, _buildingData.Config.product)) return;
        _worker.Do(ActionDefine.Produce, _buildingData.GetProp(BuildingProperty.ServiceDuration), OnProduce, true);
        _working = true;
    }
    
    private void OnProduce(PuppetCtrl puppet)
    {
        BuildingModule.Instance.AddProducts(_buildingData.Config.service, _buildingData.Config.product);
        QuestModule.Instance.OnProduce(_buildingData.Config.product);
        _working = false;
    }

    private void Update()
    {
        if(_timer < 1) {
            _timer += Time.deltaTime;
            return;
        }
        _timer = 0;
        Produce();
    }
}
