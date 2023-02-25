using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lemegeton;
using UnityEngine;

public class ShopModule : Singleton<ShopModule>
{

    private bool _inited = false;
    private ShopConfig _shopConfig;
    public ShopConfig shopConfig => _shopConfig;

    private ShopArchive _shopArchive;
    public ShopArchive ShopArchive => _shopArchive;

    private PurchaseArchice _purchaseArchice;

    private PurchaseManager _purchaseManager = new PurchaseManager();
    private Dictionary<string, string> _productPrices = new Dictionary<string, string>();

    public bool IsVip => _isVip;
    private bool _isVip = false;

    private ShopGiftConfig mainShowGift;
    public ShopGiftConfig PromotionGift => mainShowGift;

    public void Init()
    {
        if(_inited) return;

        _shopConfig = ConfigModule.Instance.ShopConfig();
        _shopArchive = ArchiveModule.Instance.GetArchive<ShopArchive>();
        _purchaseArchice = ArchiveModule.Instance.GetArchive<PurchaseArchice>();

        var list = GetValidGiftLimit();
        mainShowGift = GetShowGiftLimit(list);

        // TODO: restore
        SetVip();
        SetPlayerMoneyCapacityExtra();

        _inited = true;
    }

    public void OnLoad()
    {
        InitPurchase();
    }

    public List<ShopGiftConfig> GetValidGiftLimit()
    {
        List<ShopGiftConfig> validList = new List<ShopGiftConfig>();
        if (shopConfig == null) return validList;

        var gifts = shopConfig.shop_gift_limit;
        var loginDay = PlayerModule.Instance.LoginDay;

        var now = NtpModule.Instance.UtcNowSeconds;
        var today = NtpModule.Instance.Today();
        var save = false;
        foreach (var gift in gifts) {
            var time = GetGiftVaildTime(gift.id);
            if (gift.show_day.Contains(loginDay)) {
                // TODO: condition
                if (time > now) {
                    validList.Add(gift);
                }
                else if (time == 0 || time <= today) {
                    // set
                    var newTime = now + gift.time * 60;
                    _shopArchive.giftTimes[gift.id] = newTime;
                    save = true;
                    validList.Add(gift);
                }
            } else {
                if (time > now) {
                    validList.Add(gift);
                }
            }
        }

        if (save) Save();
        return validList;
    }

    public ShopGiftConfig GetShowGiftLimit(List<ShopGiftConfig> gifts)
    {
        var now = NtpModule.Instance.UtcNowSeconds;
        var min = long.MaxValue;
        ShopGiftConfig res = null;
        foreach (var gift in gifts) {
            var time = GetGiftVaildTime(gift.id);
            var own = AlreadyPurchased(gift.product_id);

            if (time > now && time < min && !own) {
                min = time;
                res = gift;
            }
        }

        return res;
    }

    public ShopGiftConfig GetVaildGiftConfig(string id)
    {
        var list = GetValidGiftLimit();
        return list.FirstOrDefault(x => x.id == id && !AlreadyPurchased(x.product_id));
    }

    public long GetGiftVaildTime(string id)
    {
        //long? time = null;

        _shopArchive.giftTimes.TryGetValue(id , out var time);
        return time;
    }

    public List<ShopGiftRewardConfig> GetProductRewards(string productId)
    {
        var product = GetProduct(productId);
        if (product == null) {
            Debug.LogError("no productId " + productId);
            return new List<ShopGiftRewardConfig>();
        }

        return product.rewards;
    }

    public string GetRewardName(string type) {
        var nameKey =  type switch
        {
            ShopRewardType.Income => ShopRewardName.Income,
            ShopRewardType.MoneyCapacity => ShopRewardName.Capacity,
            ShopRewardType.NoAds => ShopRewardName.NoAds,
            ShopRewardType.BuilderTeam => ShopRewardName.Builders,
            ShopRewardType.ResercherTeam => ShopRewardName.Researchers,
            _ => "",
        };

        if (string.IsNullOrEmpty(nameKey)) {
            return "";
        }

        return nameKey.Locale();
    }

    public void Save()
    {
        ArchiveModule.Instance.SaveArchive(_shopArchive);
    }

    public long BasicValidReaminTime()
    {
        var now = NtpModule.Instance.UtcNowSeconds;
        return _shopArchive.gachaValidTime - now;
    }

    public bool IsBasicGachaAdValid()
    {
        var now = NtpModule.Instance.UtcNowSeconds;
        return _shopArchive.gachaValidTime <= now;
    }

    public bool IsShowFreeGacheReddot()
    {
        var config = ConfigModule.Instance.Common();

        return IsBasicGachaAdValid() && PlayerModule.Instance.Star >= config.free_gacha_reddot_unlock_star;
    }

    public void UpdateBasicGachaValidTime()
    {
        var now = NtpModule.Instance.UtcNowSeconds;
        _shopArchive.gachaValidTime = now + ConfigModule.Instance.Common().basic_gacha_refresh_time;
        Save();
    }

    public void UpdateadIncomeBoostEndTime(long time)
    {
        _shopArchive.adIncomeBoostEndTime = time;
        Save();
    }

    public void UpdateadServiceBoostEndTime(long time)
    {
        _shopArchive.serviceBoostEndTime = time;
        Save();
    }
    #region  purchase
    private void InitPurchase()
    {
        _purchaseManager.Setup();
        //LoadProducts();
    }

    public void LoadProducts()
    {
        if (_productPrices.Count != 0) {
            return;
        }
        //AppLogger.Log("ChargeProducts Start \n" + string.Join(", ", shopConfig.product_list.Keys));
#if UNITY_EDITOR
        TaskModule.Instance.Delay(2000, () => {
            _productPrices = shopConfig.product_list.Keys.ToDictionary(t => t, t => "$22");
            EventModule.Instance?.FireEvent(EventDefine.ProductListLoaded);
        });

#else
       // load product
       MaxSDK.Instance.ChargeProducts(
            shopConfig.product_list.Keys.ToList(),
            (int ret, string msg, List<MaxChargeProductData> datas) => {
                //AppLogger.Log("ChargeProducts Success \n" + string.Join(", ", datas.Select(x => x.ProductId)));
                _productPrices = datas.ToDictionary(t => t.ProductId, t => t.Money);
                //EventModule.Instance.FireEvent(EventDefine.ProductListLoaded);
            }
        );
#endif
    }

    // public async Task<PurchaseManager.PurchaseResult> Purchase(string productId)
    // {
    //     var res = await _purchaseManager.PurchaseAsync(productId);
    //     return res;
    // }

    public void Purchase(string productId, System.Action<int> onFinish)
    {
        _purchaseManager.Purchase(productId, onFinish);
    }

    public void Restore()
    {
        _purchaseManager.Restore();
    }

    public Product GetProduct(string productId)
    {
        shopConfig.product_list.TryGetValue(productId, out var val);
        return val;
    }

    public string GetPrice(string productId)
    {
        _productPrices.TryGetValue(productId, out var val);
        return val;
    }

    public bool AlreadyPurchased(string productId)
    {
        return _purchaseArchice.playerOwnedRights.ContainsKey(productId);
    }

    public bool AlreadyPurchased(string productId, string orderSn)
    {
        var product = GetProduct(productId);
        if (product.product_type == ProductType.NoneConsumable) {
            return _purchaseArchice.playerOwnedRights.ContainsKey(productId);
        } else {
            return _purchaseArchice.purchaseOrderData.ContainsKey(orderSn);
        }
    }

    public void SavePurchaseOrder(string productId, string orderSn)
    {
        _purchaseArchice.purchaseOrderData[orderSn] = new PurchaseOrderInfo()
        {
            productId = productId,
            orderSn = orderSn
        };

        var product = GetProduct(productId);
        if (product.product_type == ProductType.NoneConsumable) {
            _purchaseArchice.playerOwnedRights[productId] = new PlayerRight()
            {
                productId = productId,
            };
        }
        ArchiveModule.Instance.SaveArchive(_purchaseArchice);
    }

    public bool hasPlayerRight(string rewardType)
    {
        return _purchaseArchice.playerOwnedRights.Any(
            pair => {
                var product = GetProduct(pair.Key);
                if (product != null) {
                    return product.rewards.Any(r => r.type == rewardType);
                }
                return false;
            });
    }

    public float getPlayerRightValue(string rewardType)
    {
        return _purchaseArchice.playerOwnedRights.Sum(
            pair => {
                var product = GetProduct(pair.Key);
                if (product == null) return 0;

                return product.rewards
                .Where(r => r.type == rewardType)
                .Sum(r => r.value);
            });
    }

    public void SetVip()
    {
        _isVip = hasPlayerRight(ShopRewardType.NoAds);
    }

    public void SetPlayerMoneyCapacityExtra()
    {
        var ratio = getPlayerRightValue(ShopRewardType.MoneyCapacity);
        PlayerModule.Instance.UpdateMoneyLimitShopRatio(ratio);
    }
    #endregion
}