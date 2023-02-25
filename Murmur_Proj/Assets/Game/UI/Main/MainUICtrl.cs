using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class MainUICtrl : MonoBehaviour
{
    public static bool ForceHide = false;
    [SerializeField] Transition _left;
    [SerializeField] Transition _right;
    [SerializeField] Transition _top;

    [SerializeField] Transition _topGradient;
    [SerializeField] Transition _bottomGradient;

    [SerializeField] PlayerMoneyView _moneyView;
    [SerializeField] Bound _moneyBound;
    [SerializeField] Text _gemText;
    [SerializeField] Bound _gemBound;
    [SerializeField] Text _starText;
    [SerializeField] Bound _starBound;
    [SerializeField] Text _couponText;
    [SerializeField] Bound _couponBound;
    [SerializeField] GameObject _questOn;

    [SerializeField] Text _builderTeamText;
    [SerializeField] Text _researcherTeamText;

    [SerializeField] GameObject _gachaReddot;

    private int _bottomHidingCount = 0;
    private int _topHidingCount = 0;
    private long _timerId = -1;

    private void Start()
    {
        EventModule.Instance.Register(EventDefine.MainUIHideBottom, OnHideBottomEvent);
        EventModule.Instance.Register(EventDefine.MainUIShowBottom, OnShowBottomEvent);

        EventModule.Instance.Register(EventDefine.MainUIHideTop, OnHideTopEvent);
        EventModule.Instance.Register(EventDefine.MainUIShowTop, OnShowTopEvent);

        EventModule.Instance.Register(EventDefine.UpdateQuest, OnUpdateQuestEvent);
        EventModule.Instance.Register(EventDefine.UpdateItem, OnUpdateItemEvent);
        EventModule.Instance.Register(EventDefine.UpdateMoneyLimit, OnUpdateMoneyLimitEvent);

        EventModule.Instance.Register(EventDefine.TalentTeamUpdate, OnUpdateTalentTeamEvent);
        EventModule.Instance.Register(EventDefine.BuildingTeamUpdate, OnUpdateBuildTeamEvent);
        EventModule.Instance.Register(EventDefine.MainUIShow, OnMainUIShowEvent);

        UpdateCoin();
        UpdateMoney();
        UpdateStar();
        UpdateCoupon();
        UpdateBuilderTeam();
        UpdateResearcherTeam();

        OnShow();

        _questOn.gameObject.SetActiveIfNeed(QuestModule.Instance.HasCompletedQuest());


        _gachaReddot.SetActive(ShopModule.Instance.IsShowFreeGacheReddot());
        _timerId = TimerModule.Instance.CreateTimer(1, () => {
            OnTimeUpdated();
        }, loop: -1);
    }

    private void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.MainUIHideBottom, OnHideBottomEvent);
        EventModule.Instance?.UnRegister(EventDefine.MainUIShowBottom, OnShowBottomEvent);

        EventModule.Instance?.UnRegister(EventDefine.MainUIHideTop, OnHideTopEvent);
        EventModule.Instance?.UnRegister(EventDefine.MainUIShowTop, OnShowTopEvent);

        EventModule.Instance?.UnRegister(EventDefine.UpdateQuest, OnUpdateQuestEvent);
        EventModule.Instance?.UnRegister(EventDefine.UpdateItem, OnUpdateItemEvent);
        EventModule.Instance?.UnRegister(EventDefine.UpdateMoneyLimit, OnUpdateMoneyLimitEvent);

        EventModule.Instance?.UnRegister(EventDefine.TalentTeamUpdate, OnUpdateTalentTeamEvent);
        EventModule.Instance?.UnRegister(EventDefine.BuildingTeamUpdate, OnUpdateBuildTeamEvent);

        EventModule.Instance?.UnRegister(EventDefine.MainUIShow, OnMainUIShowEvent);

        TimerModule.Instance?.CancelTimer(_timerId);

    }

    public void UpdateMoney(bool anim = false, bool force = false)
    {
        _moneyView.UpdateMoney(force);
        if (anim) _moneyBound.Play();
    }

    public void UpdateCoin(bool anim = false)
    {
        _gemText.text = FormatUtil.Currency(PlayerModule.Instance.Coin, false);
        if (anim) _gemBound.Play();
    }

    public void UpdateStar(bool anim = false)
    {
        _starText.text = FormatUtil.Currency(PlayerModule.Instance.Star, false);
        if (anim) _starBound.Play();
    }

    public void UpdateCoupon(bool anim = false)
    {
        _couponText.text = FormatUtil.Currency(PlayerModule.Instance.Coupon, false);
        if (anim) _couponBound.Play();
    }

    void UpdateBuilderTeam()
    {
        var team = BuildingModule.Instance.WorkTeam.GetFreeTeamCount();
        var cap = BuildingModule.Instance.WorkTeam.Capacity;
        _builderTeamText.text = $"{team}/{cap}";
    }

    void UpdateResearcherTeam()
    {
        var team = TalentModule.Instance.WorkTeam.GetFreeTeamCount();
        var cap = TalentModule.Instance.WorkTeam.Capacity;
        _researcherTeamText.text = $"{team}/{cap}";
    }

    public void OnClickStaff()
    {
        //AppLogger.Log("OnClickStaff");
        Resources.UnloadUnusedAssets();
        var arg = new StaffHireCtrl.StaffHireCtrlArgs();

        UIMgr.Instance.OpenUIByClick(StaffHireCtrl.PrefabPath, arg, false, true);
    }

    public void OnClickStats()
    {
        UIMgr.Instance.OpenUIByClick(StatsView.PrefabPath, null, false, true);
    }
    public void OnClickSettings()
    {
        UIMgr.Instance.OpenUIByClick(SettingView.PrefabPath, null, false, true);
    }

    public void OnClickAd()
    {
        AppLogger.Log("OnClickAd");
    }

    public void OnClickShopBtn()
    {
        OnClickShopBtn(0);
    }

    public void OnClickShopBtn(int type)
    {
        ShopUICtrl.Open((ShopUICtrl.Section)type);
    }

    public void OnClickQuestBtn()
    {
        UIMgr.Instance.OpenUIByClick(QuestView.PrefabPath, null, false, true);
    }

    public void OnClickMoneyBtn()
    {
        ShopUICtrl.Open(ShopUICtrl.Section.Privilege);
    }

    void OnShow()
    {
        WelcomePopup.Open(() => {
            PromotionPopup.Open();
        });
    }

    void OnTimeUpdated()
    {
        _gachaReddot.SetActiveIfNeed(ShopModule.Instance.IsShowFreeGacheReddot());
    }

    #region  消息处理
    private void OnMainUIShowEvent(Component sender, System.EventArgs e)
    {
        //OnShow();
    }

    private void OnHideBottomEvent(Component sender, System.EventArgs e)
    {
        _bottomHidingCount++;
        _left.Left();
        _right.Right();

        _left.FadeOut();
        _right.FadeOut();

        _bottomGradient.gameObject.SetActive(false);
    }

    private void OnShowBottomEvent(Component sender, System.EventArgs e)
    {
        if (ForceHide) return;
        _bottomHidingCount--;

        if(_bottomHidingCount == 0) {
            _left.Back();
            _right.Back();

            _left.FadeIn();
            _right.FadeIn();

            _bottomGradient.gameObject.SetActive(true);
        }
    }

    private void OnHideTopEvent(Component sender, System.EventArgs e)
    {
        _topHidingCount++;
        _top.Up();
        _top.FadeOut();

        _topGradient.Up();
        _topGradient.FadeOut();
    }

    private void OnShowTopEvent(Component sender, System.EventArgs e)
    {
        _topHidingCount--;
        //AppLogger.Log("_topHidingCount  " + _topHidingCount);

        if (ForceHide) return;
        if (_topHidingCount == 0) {
            _top.Back();
            _top.FadeIn();

            _topGradient.Back();
            _topGradient.FadeIn();
        }

    }
    private void OnUpdateQuestEvent(Component sender, System.EventArgs e)
    {
        bool hasCompleted = QuestModule.Instance.HasCompletedQuest();
        _questOn.gameObject.SetActiveIfNeed(hasCompleted);
    }
    private void OnUpdateItemEvent(Component sender, System.EventArgs e)
    {
        var args = e as UpdateItemArgs;
        if(args.item == ItemType.Money) {
            UpdateMoney(args.anim);
        } else if(args.item == ItemType.Coin) {
            UpdateCoin(args.anim);
        } else if (args.item == ItemType.Star) {
            UpdateStar(args.anim);
        } else if (args.item == ItemType.Coupon) {
            UpdateCoupon(args.anim);
        }
    }
    private void OnUpdateMoneyLimitEvent(Component sender, System.EventArgs e)
    {
        UpdateMoney(false, true);
    }

    private void OnUpdateTalentTeamEvent(Component sender, System.EventArgs e)
    {
        UpdateResearcherTeam();
    }

    private void OnUpdateBuildTeamEvent(Component sender, System.EventArgs e)
    {
        UpdateBuilderTeam();
    }
    #endregion
}
