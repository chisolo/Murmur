using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Lemegeton;

public class PuppetModule : Singleton<PuppetModule>
{
    PuppetModule() {}

    private List<PuppetConfig> _puppets;
    private List<PuppetConfig> _guestPuppets = new List<PuppetConfig>();
    private Dictionary<string, PuppetConfig> _staffPuppets = new Dictionary<string, PuppetConfig>();
    private Dictionary<string, PuppetConfig> _workerPuppets = new Dictionary<string, PuppetConfig>();

    private int _guestCount;
    private bool _inited = false;
    public void Init()
    {
        if(_inited) return;
        _puppets = ConfigModule.Instance.Puppets();
        
        foreach(var puppet in _puppets) {
            if(puppet.type == PuppetType.Guest) {
                _guestPuppets.Add(puppet);
            } else if(puppet.type == PuppetType.Staff) {
                _staffPuppets.Add(puppet.id, puppet);
            } else if(puppet.type == PuppetType.Worker) {
                _workerPuppets.Add(puppet.id, puppet);
            }
        }

        _guestCount = _guestPuppets.Count;
        _inited = true;
    }

    public List<PuppetConfig> Puppets()
    {
        return _puppets;
    }
    public PuppetConfig RandomGuest()
    {
        int index = Random.Range(0, _guestCount);
        return _guestPuppets[index];
    }
    public PuppetConfig GetStaffPuppet(string id)
    {
        return _staffPuppets.TryGetValue(id, out var value) ? value : null;
    }
    public PuppetConfig GetWorkerPuppet(string id)
    {
        return _workerPuppets.TryGetValue(id, out var value) ? value : null;
    }
}
