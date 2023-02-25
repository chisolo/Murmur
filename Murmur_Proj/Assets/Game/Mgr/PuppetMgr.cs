using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Lemegeton;

public class PuppetMgr : GameMgr<PuppetMgr>
{
    private bool _loaded;
    private int _genId;
    private List<SpotConfig> _entrySpots = new List<SpotConfig>();
    private List<SpotConfig> _exitSpots = new List<SpotConfig>();
    private PuppetModule _puppet;
    private BuildingModule _building;
    public async Task Load()
    {
        _loaded = false;
        _genId = 0;
        _puppet = PuppetModule.Instance;
        _building = BuildingModule.Instance;

        for(int i = 1; i <= GameUtil.EntryAmount; ++i)
        {
            _entrySpots.Add(ConfigModule.Instance.GetSpot(SpotDefine.Entry, i));
        }
        for(int i = 1; i <= GameUtil.ExitAmount; ++i)
        {
            _exitSpots.Add(ConfigModule.Instance.GetSpot(SpotDefine.Exit, i));
        }
        var puppets = PuppetModule.Instance.Puppets();
        var total = puppets.Count + GameUtil.ServiceFoodPrefab.Count;
        var count = 0;

        foreach(var puppet in puppets) {
            AddressablePoolModule.Instance.Prepare(puppet.prefab, () => {
                ++count;
            });
        }

        foreach(var foodPrefab in GameUtil.ServiceFoodPrefab.Values) {
            AddressablePoolModule.Instance.Prepare(foodPrefab, () => {
                ++count;
            });
        }

        while(count < total) {
            await Task.Yield();
        }
        _loaded = true;

        StartCoroutine(SpawnGuestCoroutine());
    }
    // 生成顾客
    public void SpawnReceptionGuest(float speed, string service)
    {
        var spotConfig = RandomEntrySpot();
        BuildingMgr.Instance.OnGuestSpawn(SpawnGuest(speed, service, spotConfig));
    }

    public PuppetCtrl SpawnGuest(float speed, string service, SpotConfig entry)
    {
        if(string.IsNullOrEmpty(service)) return null;
        var guestCfg = _puppet.RandomGuest();
        var guestCtrl = AddressablePoolModule.Instance.Get(guestCfg.prefab).GetComponent<PuppetCtrl>();

        guestCtrl.Init(++_genId, guestCfg, speed, service);
        var tmp = new Vector3(entry.position.x, 0.15f, entry.position.z);
        guestCtrl.NavAgent.Warp(tmp);
        guestCtrl.transform.rotation = entry.rotation;
        guestCtrl.transform.SetParent(transform);
        guestCtrl.Active = true;
        return guestCtrl;
    }
    // 生成工人
    public PuppetCtrl SpawnWorker(string workId, float speed, SpotConfig spawnSpot, string servie)
    {
        PuppetConfig workerCfg = _puppet.GetWorkerPuppet(workId);
        if(workerCfg == null) return null;
        var workCtrl = AddressablePoolModule.Instance.Get(workerCfg.prefab).GetComponent<PuppetCtrl>();
        workCtrl.Init(++_genId, workerCfg, speed, servie);
        workCtrl.NavAgent.Warp(spawnSpot.position);
        workCtrl.transform.rotation = spawnSpot.rotation;
        workCtrl.transform.SetParent(transform);
        workCtrl.Active = true;
        return workCtrl;
    }
    // 生成员工
    public PuppetCtrl SpawnStaff(string staffId, float speed, SpotConfig spawnSpot, string servie)
    {
        StaffArchiveData staffData = StaffModule.Instance.GetArchiveData(staffId);
        if(staffData == null) {
            AppLogger.LogError("Staff not found ! staffId = " + staffId);
        }
        PuppetConfig staffCfg = _puppet.GetStaffPuppet(staffData.puppetId);
        if(staffCfg == null) return null;
        var staffCtrl = AddressablePoolModule.Instance.Get(staffCfg.prefab).GetComponent<PuppetCtrl>();
        staffCtrl.Init(++_genId, staffCfg, speed, servie);
        staffCtrl.NavAgent.Warp(spawnSpot.position);
        staffCtrl.transform.rotation = spawnSpot.rotation;
        staffCtrl.transform.SetParent(transform);
        staffCtrl.Active = true;
        return staffCtrl;
    }
    public void PuppetExit(PuppetCtrl puppet)
    {
        puppet.MoveClear(RandomExitSpot());
    }
    private IEnumerator SpawnGuestCoroutine()
    {
        while(true) {
            yield return new WaitForSeconds(GenerateInterval());
            var service = _building.RandomService();
            SpawnReceptionGuest(ConfigModule.Instance.Common().default_puppet_speed, service);
        }
    }
    private SpotConfig RandomEntrySpot()
    {
        int index = Random.Range(0, GameUtil.EntryAmount);
        return _entrySpots[index];
    }
        private SpotConfig RandomExitSpot()
    {
        int index = Random.Range(0, GameUtil.ExitAmount);
        return _exitSpots[index];
    }

    private float GenerateInterval()
    {
        var interval = _building.GetTotalProp(ServiceType.HotdogBureau, BuildingProperty.GuestInterval) + _building.GetTotalProp(ServiceType.HambergBureau, BuildingProperty.GuestInterval) 
                        + _building.GetTotalProp(ServiceType.PizzaBureau, BuildingProperty.GuestInterval);
        return GameUtil.DefaultGuestInterval / (1 + interval) / (1 + TalentModule.Instance.GetEnhanceValue(TalentEnhanecType.GUEST_INTERVAL));
    }

    public void InstantSpawnGuest(int num)
    {
        StartCoroutine(InstantSpawnGuestCoroutine(num));
    }
    private IEnumerator InstantSpawnGuestCoroutine(int num)
    {
        var count = num;
        while(count > 0) {
            yield return new WaitForSeconds(0.5f);
            var service = _building.RandomService();
            SpawnReceptionGuest(ConfigModule.Instance.Common().default_puppet_speed, service);
            count--;
        }
    }
}
