using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;


public class PlayerModule : Singleton<PlayerModule>
{
    private PlayerArchive _playerArchive;
    private bool _inited = false;

    public int Money => _playerArchive.money;
    private float _moneyLimitShopRatio = 0f;
    public int _moneyLimit;
    public int MoneyLimit => _moneyLimit;

    public int Coin =>  _playerArchive.coin;
    public int Coupon => _playerArchive.coupon;
    public int Star => _playerArchive.star;

    public int LoginDay => _playerArchive.loginDay;

    public bool Bgm => _playerArchive.bgm;
    public bool Sfx => _playerArchive.sfx;

    public long OfflineTime => NtpModule.Instance.UtcNowSeconds - LastPlayTime;
    public long LastPlayTime => _playerArchive.lastPlayTime;

    private bool _review = false;
    public void Init()
    {
        if(_inited) return;

        _playerArchive = ArchiveModule.Instance.GetArchive<PlayerArchive>();

        _moneyLimit = _playerArchive.moneyLimit;
        Login();
        AudioModule.Instance.UpdateMute(!_playerArchive.bgm, !_playerArchive.sfx);
        _inited = true;

    }

    public void OnLoad()
    {
        TimerModule.Instance.CreateTimer(60, () => {
            _playerArchive.lastPlayTime = NtpModule.Instance.UtcNowSeconds;
            Save();
        }, loop: -1);


        TimerModule.Instance.CreateTimer(60, () => {
            var today = NtpModule.Instance.Today();
            if (_playerArchive.lastLoginTime < today) {
                Login();
            }

            // UnityEngine.Profiling.Profiler.BeginSample("cloud_save");
            // _playerArchive.lastPlayTime = NtpModule.Instance.UtcNowSeconds;
            // Save();
            // UnityEngine.Profiling.Profiler.EndSample();

        }, loop:-1);
        NotificationUtil.Register(GameUtil.DefaultNotificationChannel + "_id", GameUtil.DefaultNotificationChannel + "_name", GameUtil.DefaultNotificationChannel + "_desc");
        NotificationUtil.CancelAll();
        ScheduleNotification();
    }
    public void Login()
    {
        var last = _playerArchive.lastLoginTime;
        var now = NtpModule.Instance.UtcNowSeconds;
        var today = NtpModule.Instance.Today();
        if (last < today && now > today) {
            _playerArchive.loginDay++;
            _playerArchive.lastLoginTime = now;

            Save();
        }
    }

    public int TimeToGem(long time)
    {
        if(time <= 3600) {
            return Mathf.CeilToInt(time / 120f);
        } else {
            return 31 + (int)(time - 3600) / 300;
        }
    }
    public void Save()
    {
        ArchiveModule.Instance.SaveArchive(_playerArchive);
    }
    public bool IsMoneyFull()
    {
        return Money >= MoneyLimit;
    }
    public bool UseMoney(int num, bool force = false)
    {
        if(!force && num > _playerArchive.money) return false;
        int old = _playerArchive.money;
        _playerArchive.money -= num;
        _playerArchive.money = Mathf.Max(0, _playerArchive.money);
        int current = _playerArchive.money;
        FireUpdateItemEvent(ItemType.Money, old, current, false);
        Save();
        return true;
    }

    /// <summary>
    /// 增加现金
    /// </summary>
    /// <param name="num">金额</param>
    /// <param name="anim">现金条闪动动画</param>
    public void AddMoney(int num, bool anim = true)
    {
        int old = _playerArchive.money;
        _playerArchive.money += num;
        _playerArchive.money = Mathf.Min(MoneyLimit, _playerArchive.money);
        int current = _playerArchive.money;
        FireUpdateItemEvent(ItemType.Money, old, current, anim);
        Save();
    }

    /// <summary>
    /// 增加现金并播放小现金特效
    /// </summary>
    /// <param name="num">金额</param>
    /// <param name="anim">现金条闪动动画</param>
    /// <param name="position">小现金特效世界坐标</param>
    /// <param name="rotation">小现金特效旋转</param>
    public void AddMoneyWithSmallEffect(int num, bool anim, Vector3 position)
    {
        AddMoney(num, anim);
        FireItemEffectEvent(ItemEffectType.SmallMoney, position);
    }

    /// <summary>
    /// 增加现金并播放大现金特效
    /// </summary>
    /// <param name="num">金额</param>
    /// <param name="anim">现金条闪动动画</param>
    public void AddMoneyWithBigEffect(int num, bool anim)
    {
        AddMoney(num, anim);
        FireItemEffectEvent(ItemEffectType.BigMoney, Vector3.zero);
        AudioModule.Instance.PlaySfx(GameUtil.ResSfxBonus);
    }

    /// <summary>
    /// 增加金币并播放大金币特效
    /// </summary>
    /// <param name="num">金额</param>
    /// <param name="anim">金币条闪动动画</param>
    public void AddCoinWithBigEffect(int num, bool anim)
    {
        AddCoin(num, anim);
        FireItemEffectEvent(ItemEffectType.BigCoin, Vector3.zero);
        AudioModule.Instance.PlaySfx(GameUtil.ResSfxBonus);
    }
    public void UpdateMoney(int num)
    {
        int old = _playerArchive.money;
        _playerArchive.money = num;
        int current = _playerArchive.money;
        FireUpdateItemEvent(ItemType.Money, old, current, false);
        Save();
    }

    public bool UseCoin(int num, bool force = false)
    {
        if(!force && num > Coin) return false;
        int old = _playerArchive.coin;
        _playerArchive.coin -= num;
        _playerArchive.coin = Mathf.Max(0, _playerArchive.coin);
        int current = _playerArchive.coin;
        FireUpdateItemEvent(ItemType.Coin, old, current, false);
        Save();
        return true;
    }

    public void AddCoin(int num, bool anim = true)
    {
        int old = _playerArchive.coin;
        _playerArchive.coin += num;
        int current = _playerArchive.coin;
        FireUpdateItemEvent(ItemType.Coin, old, current, anim);
        Save();
    }
    public void AddStar(int num, bool anim = true)
    {
        int old = _playerArchive.star;
        _playerArchive.star += num;
        int current = _playerArchive.star;
        FireUpdateItemEvent(ItemType.Star, old, current, anim);

        if(_playerArchive.lastReviewedStar != -1 && !_review && current - _playerArchive.lastReviewedStar >= ConfigModule.Instance.Common().star_trigger_rate) {
            _review = true;
            _playerArchive.lastReviewedStar = current;
            EventModule.Instance.FireEvent(EventDefine.RequestReview);
        }
        Save();
    }

    public void OnReview()
    {
        _review = false;
        _playerArchive.lastReviewedStar = -1;
        Save();
    }
    public void UpdateMoneyLimit(int moneyLimit)
    {
        if(_playerArchive.moneyLimit >= moneyLimit) return;
        _playerArchive.moneyLimit = moneyLimit;

        _moneyLimit = CalcMoneyLimit(moneyLimit, _moneyLimitShopRatio);

        Save();
        EventModule.Instance.FireEvent(EventDefine.UpdateMoneyLimit);
    }

    public void UpdateMoneyLimitShopRatio(float ratio)
    {
        if(_moneyLimitShopRatio >= ratio) return;
        _moneyLimitShopRatio = ratio;
        _moneyLimit = CalcMoneyLimit(_playerArchive.moneyLimit, ratio);

        EventModule.Instance.FireEvent(EventDefine.UpdateMoneyLimit);
    }

    private int CalcMoneyLimit(int moneyLimit, float ratio)
    {
        return (int)(moneyLimit * (1 + ratio));
    }

    private void FireUpdateItemEvent(string item, int old, int current, bool anim)
    {
        using (UpdateItemArgs args = UpdateItemArgs.Get()) {
            args.item = item;
            args.old = old;
            args.current = current;
            args.anim = anim;
            EventModule.Instance.FireEvent(EventDefine.UpdateItem, args);
        }
    }
    public bool UseCoupon(int num, bool force = false)
    {
        if(!force && num > Coupon) return false;
        int old = _playerArchive.coupon;
        _playerArchive.coupon -= num;
        _playerArchive.coupon = Mathf.Max(0, _playerArchive.coupon);
        int current = _playerArchive.coupon;
        FireUpdateItemEvent(ItemType.Coupon, old, current, false);
        Save();
        return true;
    }

    public void AddCoupon(int num, bool anim = true)
    {
        int old = _playerArchive.coupon;
        _playerArchive.coupon += num;
        int current = _playerArchive.coupon;
        FireUpdateItemEvent(ItemType.Coupon, old, current, anim);
        Save();
    }
    private void FireItemEffectEvent(ItemEffectType effect, Vector3 pos)
    {
        using (ItemEffectEventArgs args = ItemEffectEventArgs.Get()) {
            args.effect = effect;
            args.pos = pos;
            EventModule.Instance.FireEvent(EventDefine.ItemEffect, args);
        }
    }

    public void SetBgm()
    {
        _playerArchive.bgm = !_playerArchive.bgm;
        Save();
        AudioModule.Instance.UpdateMute(!_playerArchive.bgm, !_playerArchive.sfx);
    }
    public void SetSfx()
    {
        _playerArchive.sfx = !_playerArchive.sfx;
        Save();
        AudioModule.Instance.UpdateMute(!_playerArchive.bgm, !_playerArchive.sfx);
    }

    void OnApplicationPause(bool pause)
    {
        //AppLogger.Log("Player OnApplicationPause " + pause);
        if (pause) {
            _playerArchive.lastPlayTime = NtpModule.Instance.UtcNowSeconds;
            Save();
        } else {
            NotificationUtil.CancelDelivered();
            WelcomePopup.Open();
        }
    }
    private void ScheduleNotification()
    {
        var notifications = ConfigModule.Instance.Notifications();
        if(notifications == null) return;
        foreach(var notification in notifications) {
            var count = notification.contents.Count;
            if(count <= 0) continue;
            var index = RandUtil.Range(count);
            NotificationUtil.Schedule(notification.contents[index].title.Locale(), notification.contents[index].text.Locale(), notification.hour, notification.minute, 1, GameUtil.DefaultNotificationChannel + "_id", true);
        }
    }

#if UNITY_EDITOR
    // TODO: debug mode in mobile
    [UnityEditor.MenuItem("Dev/Money/钱清零")]
    public static void SetPlayMoney_Menu()
    {
        PlayerModule.Instance.UpdateMoney(0);
        PlayerModule.Instance.Save();
    }

    [UnityEditor.MenuItem("Dev/Money/钱加满")]
    public static void SetPlayMoneyMax_Menu()
    {
        PlayerModule.Instance.UpdateMoney(PlayerModule.Instance.MoneyLimit);
        PlayerModule.Instance.Save();
    }

    [UnityEditor.MenuItem("Dev/Money/钱加1W")]
    public static void SetPlayMoney1w_Menu()
    {
        PlayerModule.Instance.AddMoney(10000, false);
        PlayerModule.Instance.Save();
    }

    [UnityEditor.MenuItem("Dev/Money/钱加100")]
    public static void SetPlayMoney100_Menu()
    {
        PlayerModule.Instance.AddMoney(100, false);
        PlayerModule.Instance.Save();
    }

    [UnityEditor.MenuItem("Dev/Money/上限设成一亿")]
    public static void SetPlayMoneyLimit1oku_Menu()
    {
        Instance.UpdateMoneyLimit(100000000);
        //SetPlayMoney100_Menu();
    }

    [UnityEditor.MenuItem("Dev/Coin/金币清零")]
    public static void SetGem0_Menu()
    {
        PlayerModule.Instance.UseCoin(PlayerModule.Instance.Coin);
    }

    [UnityEditor.MenuItem("Dev/Coin/金币加1W")]
    public static void SetGemAdd1w_Menu()
    {
        PlayerModule.Instance.AddCoin(10000);
    }

    [UnityEditor.MenuItem("Dev/Star/清零")]
    public static void SetStar0_Menu()
    {
        PlayerModule.Instance._playerArchive.star = 0;
        PlayerModule.Instance.Save();
    }

    [UnityEditor.MenuItem("Dev/Star/加1")]
    public static void SetStarAdd1_Menu()
    {
        PlayerModule.Instance.AddStar(1);
    }

    [UnityEditor.MenuItem("Dev/Star/加10")]
    public static void SetStarAdd10_Menu()
    {
        PlayerModule.Instance.AddStar(10);
    }

    public static void SavePlayTime()
    {
        if (PlayerModule.Instance == null) return;
        if (!PlayerModule.Instance._inited) return;

        //Debug.Log("save time " + NtpModule.Instance.UtcNowSeconds);
        PlayerModule.Instance._playerArchive.lastPlayTime = NtpModule.Instance.UtcNowSeconds;
        PlayerModule.Instance.Save();
    }
#endif
}
