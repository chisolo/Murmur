using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Lemegeton;

public class PurchaseManager
{
    public struct PurchaseResult
    {
        public int ret;

        public PurchaseResult(int ret)
        {
            this.ret = ret;
        }

        public bool IsSuccess => ret == 1;
    }

    private string purchasingProductId = "";
    private bool isPurchasing = false;

    private TaskCompletionSource<PurchaseResult> purchaseTask = null;
    private Action<int> _onFinishAction;

    public void Setup()
    {
#if !UNITY_EDITOR
         MaxSDK.Instance.ChargeSetCallback(
            (int ret, string msg, MaxChargeData data) => {
                if (ret == 1) {
                    SaveOrderSendReward(data.ProductId, data.OrderSn);
                    MaxSDK.Instance.ChargeFinish(data.OrderSn);
                } else {
                    Debug.LogError("purchase failed :" + msg);
                }

                purchaseTask?.SetResult(new PurchaseResult(ret));
               _onFinishAction?.Invoke(ret);
               _onFinishAction = null;
            }
        );
#endif
    }

    private void PurchaseImpl(string productId)
    {
#if UNITY_EDITOR
        TaskModule.Instance.Delay(1000, () => {
            SaveOrderSendReward(productId, GuildUtil.NewUUID());
            purchaseTask?.SetResult(new PurchaseResult(1));
            _onFinishAction?.Invoke(1);
            _onFinishAction = null;
        });
#else
        MaxSDK.Instance.Charge(productId);
#endif
    }

    public void Purchase(string productId, Action<int> onFinish)
    {
        _onFinishAction = onFinish;
        purchaseTask = new TaskCompletionSource<PurchaseResult>();
        //isPurchasing = true;
        purchasingProductId = productId;
        //UIMgr.Instance.ShowLoading(-1f);

        PurchaseImpl(productId);
    }

    // public async Task<PurchaseResult> PurchaseAsync(string productId)
    // {
    //     if (isPurchasing) {
    //         Debug.LogWarning("last purchase not finish");
    //         return new PurchaseResult(0);
    //     }
    //     purchaseTask = new TaskCompletionSource<PurchaseResult>();
    //     isPurchasing = true;
    //     purchasingProductId = productId;
    //     //UIMgr.Instance.ShowLoading(-1f);

    //     PurchaseImpl(productId);
    //     await purchaseTask.Task;

    //     var res = purchaseTask.Task.Result;

    //     purchasingProductId = "";
    //     isPurchasing = false;
    //     purchaseTask = null;

    //     //UIMgr.Instance.HideLoading();

    //     return res;
    // }

    private void SaveOrderSendReward(string productId, string orderSn)
    {
        //ShopModule.Instance.SendReward(_config.rewards);
        if (ShopModule.Instance.AlreadyPurchased(productId, orderSn)) {
            return;
        }
        ShopModule.Instance.SavePurchaseOrder(productId, orderSn);

        var product = ShopModule.Instance.GetProduct(productId);
        if (product != null) {
            foreach (var reward in product.rewards) {
                SendReward(reward);
            }
        } else {
            Debug.LogError("no product " + productId);
        }
        //SendReward(productId);
    }

    // public void SendReward(string productId)
    // {

    // }

    public void SendReward(ShopGiftRewardConfig reward)
    {
        switch (reward.type) {
            case ShopRewardType.Income:
                BuffModule.Instance.UpdateShopIncomeBuff(ShopModule.Instance.getPlayerRightValue(ShopRewardType.Income));
            break;

            case ShopRewardType.MoneyCapacity:
                 ShopModule.Instance.SetPlayerMoneyCapacityExtra();
            break;

            case ShopRewardType.Coin:
                PlayerModule.Instance.AddCoinWithBigEffect((int)reward.value, true);
                break;

            case ShopRewardType.NoAds:
                ShopModule.Instance.SetVip();
            break;

            case ShopRewardType.BuilderTeam:
                var teamId = reward.teamId;
                if (string.IsNullOrEmpty(reward.teamId)) {
                    BuildingModule.Instance.AddTeam();
                } else {
                    BuildingModule.Instance.AddTeam(reward.teamId);
                }
            break;

            case ShopRewardType.ResercherTeam:
                if (string.IsNullOrEmpty(reward.teamId)) {
                    TalentModule.Instance.AddTeam();
                } else {
                    TalentModule.Instance.AddTeam(reward.teamId);
                }
                break;
            case ShopRewardType.GrowthFund:
                // TODO:
                AppLogger.Log("send GrowthFund ");
                break;
        }
    }

    public void Restore()
    {
#if UNITY_EDITOR
        Debug.Log("restore");
#else
        MaxSDK.Instance.ChargeRecovery();
#endif
    }


}