using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lemegeton;
using UnityEngine;

public class GrowthFundModule : Singleton<GrowthFundModule>
{

    private bool _inited = false;
    private GrowthFundConfig _config;

    private GrowthFundArchive _archive;

    private PurchaseArchice _purchaseArchice;

    private PurchaseManager _purchaseManager = new PurchaseManager();
    private Dictionary<string, string> _productPrices = new Dictionary<string, string>();


    public void Init()
    {
        if(_inited) return;

        _config = ConfigModule.Instance.GrowthFund();
        _archive = ArchiveModule.Instance.GetArchive<GrowthFundArchive>();

        _inited = true;
    }

    public void OnLoad()
    {

    }

    public bool Receive(string id)
    {
        var reward = _config.rewards.FirstOrDefault(x => x.id == id);
        var star = PlayerModule.Instance.Star;
        if (reward == null || reward.require_star > star) {
            return false;
        }


        PlayerModule.Instance.AddCoinWithBigEffect(reward.reward_coin, true);

        _archive.receiveList[id] = true;
        ArchiveModule.Instance.SaveArchive(_archive);

        return true;
    }

    public bool IsReceived(string id)
    {
        var res = false;
        _archive.receiveList.TryGetValue(id, out res);
        return res;
    }

    public int AllRewardCoin()
    {
        return _config.rewards.Sum(x => x.reward_coin);
    }

    public int GetNotReceivedCount()
    {
        var star = PlayerModule.Instance.Star;
        int sum = 0;
        foreach (var reward in _config.rewards) {
            if (star >= reward.require_star && !IsReceived(reward.id))
            {
                sum++;
            }
        }

        return sum;
    }
}