
using System;
using System.Collections.Generic;
using UnityEngine;
using Max.ThirdParty;
using System.Collections;

public abstract class MaxSDKBase
{
    public static Dictionary<string, string> ParseDictionary(string s)
    {
        if (s == null || s.Length == 0)
        {
            return new Dictionary<string, string>();
        }
        var ret = new Dictionary<string, string>();
        try
        {
            var dict = Json.Deserialize(s) as Dictionary<string, object>;
            foreach (var item in dict)
            {
                ret.Add(item.Key, item.Value.ToString());
            }
        } catch(Exception ex)
        {
            Debug.Log("ParseDictionary exception - " + ex.Message);
        }
        return ret;
    }

    public static string ParseString(string s)
    {
        return s;
    }

    public static int ParseInt(string s)
    {
        return Int32.Parse(s);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="appv">游戏内部版本号</param>
    /// <param name="action">初始化回调</param>
    public void Init(string appv, Action<int, string, MaxInitData> action)
    {
        Init(new MaxInitOptions(appv), action);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="option">初始化参数</param>
    /// <param name="action">初始化回调</param>
    public abstract void Init(MaxInitOptions option, Action<int, string, MaxInitData> action);


    /// <summary>
    /// 获取ABTest 配置
    /// </summary>
    /// <param name="action">回调方法</param>
    public abstract void AbtestConfig(Action<int, string, Dictionary<string, string>> action);


    /// <summary>
    /// 上报日志
    /// </summary>
    /// <param name="_event">事件名</param>
    /// <param name="data">事件参数</param>
    public abstract void PushEvent(string _event, Dictionary<string, string> data);

    public void Login(Action<int, string, MaxLoginData> loginAction)
    {
        Login(loginAction, null);
    }

    /// <summary>
    /// 获取隐私协议
    /// </summary>
    /// <returns></returns>
    public abstract string GetPrivacyURL();

    /// <summary>
    /// 获取用户协议
    /// </summary>
    /// <returns></returns>
    public abstract string GetUserURL();

    /// <summary>
    /// 登陆
    /// </summary>
    /// <param name="loginAction">登陆回调</param>
    /// <param name="chargeAction">支付回调</param>
    public abstract void Login(Action<int, string, MaxLoginData> loginAction, Action<int, string, MaxChargeData> chargeAction);


    public void CustomLoginToekn(Action<int, string, MaxLoginData> loginAction)
    {
        CustomLoginToken(loginAction, null);
    }

    /// <summary>
    /// token登陆
    /// </summary>
    /// <param name="loginAction">登陆回调</param>
    /// <param name="chargeAction">支付回调</param>
    public abstract void CustomLoginToken(Action<int, string, MaxLoginData> loginAction, Action<int, string, MaxChargeData> chargeAction);


    public void CustomLogin(string type, Action<int, string, MaxLoginData> loginAction)
    {
        CustomLogin(type, loginAction, null);
    }

    /// <summary>
    /// 第三方登陆
    /// </summary>
    /// <param name="type">第三方登录类型：fb = facebook；gp = 谷歌playgames</param>
    /// <param name="loginAction">登陆回调</param>
    /// <param name="chargeAction">支付回调</param>
    public abstract void CustomLogin(string type, Action<int, string, MaxLoginData> loginAction, Action<int, string, MaxChargeData> chargeAction);


    /// <summary>
    /// 注销
    /// </summary>
    /// <param name="action">注销回调</param>
    public abstract void Logout(Action<int, string> action);

    /// <summary>
    /// 绑定第三方账号
    /// </summary>
    /// <param name="type">绑定类型</param>
    /// <param name="action">回调</param>
    public void Link(string type, Action<int, string, MaxLoginData> action)
    {
        Link(type, false, action);
    }

    /// <summary>
    /// 绑定第三方平台账号
    /// </summary>
    /// <param name="type">绑定类型：fb = facebook；gp = 谷歌playgames</param>
    /// <param name="shouldSwitch">是否切账号</param>
    /// <param name="action">回调</param>
    public abstract void Link(string type, bool shouldSwitch, Action<int, string, MaxLoginData> action);

    /// <summary>
    /// 获取已绑定第三方列表
    /// </summary>
    /// <returns></returns>
    public abstract List<string> getLinkedList();

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="name"></param>
    /// <param name="photo"></param>
    /// <param name="action"></param>
    public abstract void UpdateUser(string name, string photo, Action<int, string> action);

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <returns></returns>
    public abstract MaxGetUserData GetUser();

    /// <summary>
    /// 预加载激励广告
    /// </summary>
    public void RewardAdPreload()
    {
        RewardAdPreload("");
    }

    [Obsolete("not support unionId", false)]
    /// <summary>
    /// 预加载激励广告，废弃
    /// </summary>
    /// <param name="unionId">广告位id</param>
    public void RewardAdPreload(string unionId)
    {
        RewardAdPreload("", null);
    }

    /// <summary>
    /// 预加载激励广告
    /// </summary>
    /// <param name="action"></param>
    public void RewardAdPreload(Action<int, string, string> action)
    {
        RewardAdPreload("", action);
    }

    [Obsolete("not support unionId", false)]
    /// <summary>
    /// 预加载激励广告，废弃
    /// </summary>
    /// <param name="unionId">广告位id</param>
    /// <param name="action">回调方法</param>
    public abstract void RewardAdPreload(string unionId, Action<int, string, string> action);

    // <summary>
    /// 展示激励广告
    /// </summary>
    /// <param name="pname">广告场景名，如：双倍奖励弹框</param>
    /// <param name="action"></param>
    public void RewardAdShow(string pname, Action<int, string, string> action)
    {
        RewardAdShow("", pname, action);
    }

    [Obsolete("not support unionId", false)]
    /// <summary>
    /// 展示激励广告，废弃
    /// </summary>
    /// <param name="unionId">广告位id</param>
    /// <param name="pname">广告场景名，如：双倍奖励弹框</param>
    /// <param name="action"></param>
    public abstract void RewardAdShow(string unionId, string pname, Action<int, string, string> action);


    /// <summary>
    /// 预加载插屏广告
    /// </summary>
    public void FullVideoAdPreload()
    {
        FullVideoAdPreload("");
    }

    [Obsolete("not support unionId", false)]
    /// <summary>
    /// 预加载插屏广告
    /// </summary>
    /// <param name="unionId">广告位id</param>
    public abstract void FullVideoAdPreload(string unionId);

    /// <summary>
    /// 展示插屏广告
    /// </summary>
    /// <param name="pname">广告场景名</param>
    /// <param name="action">回调方法</param>
    public void FullVideoAdShow(string pname, Action<int, string, string> action)
    {
        FullVideoAdShow("", pname, action);
    }

    [Obsolete("not support unionId", false)]
    /// <summary>
    /// 展示插屏广告
    /// </summary>
    /// <param name="unionId">广告位id</param>
    /// <param name="pname">广告场景名</param>
    /// <param name="action">回调方法</param>
    public void FullVideoAdShow(string unionId, string pname, Action<int, string, string> action)
    {
        FullVideoAdShow("", pname, false, action);
    }


    [Obsolete("not support unionId/skipLoading", false)]
    /// <summary>
    /// 展示插屏广告
    /// </summary>
    /// <param name="unionId">广告位id</param>
    /// <param name="pname">广告场景名</param>
    /// <param name="skipLoading">跳过loading转圈，true=跳过，false=不跳过</param>
    /// <param name="action"></param>
    public abstract void FullVideoAdShow(string unionId, string pname, bool skipLoading, Action<int, string, string> action);

    /// <summary>
    /// 获取banner高度，单位：像素 
    /// </summary>
    /// <returns></returns>
    public abstract int BannerAdFitHeightPx();

    /// <summary>
    /// banner 初始化
    /// </summary>
    /// <param name="pname">广告场景</param>
    public void BannerAdInit(string pname)
    {
        BannerAdInit(pname, "", MaxBannerOptions.BannerPosition.BottomCenter, null);
    }

    /// <summary>
    /// banner 初始化
    /// </summary>
    /// <param name="pname">广告场景名</param>
    /// <param name="position">展示位置</param>
    /// <param name="color">banner背景颜色</param>
    public void BannerAdInit(string pname, MaxBannerOptions.BannerPosition position, Color? color)
    {
        BannerAdInit(pname, "", position, color);
    }

    [Obsolete("not support unionId", false)]
    /// <summary>
    /// banner初始化
    /// </summary>
    /// <param name="pname">广告场景</param>
    /// <param name="unionId">广告位id</param>
    public void BannerAdInit(string pname, string unionId)
    {
        BannerAdInit(pname, "", MaxBannerOptions.BannerPosition.BottomCenter, null);
    }

    [Obsolete("not support unionId", false)]
    /// <summary>
    /// banner初始化
    /// </summary>
    /// <param name="pname">广告场景名</param>
    /// <param name="unionId">广告位id</param>
    /// <param name="color">banner背景颜色</param>
    public void BannerAdInit(string pname, string unionId, Color color)
    {
        BannerAdInit(pname, "", MaxBannerOptions.BannerPosition.BottomCenter, color);
    }

    [Obsolete("not support unionId", false)]
    /// <summary>
    /// banner 初始化
    /// </summary>
    /// <param name="pname">广告场景名</param>
    /// <param name="unionId">广告位id</param>
    /// <param name="position">展示位置</param>
    /// <param name="color">banner背景颜色</param>
    public void BannerAdInit(string pname, string unionId, MaxBannerOptions.BannerPosition position, Color? color)
    {
        var options = new MaxBannerOptions(pname);
        options.Position = position;
        if (color != null)
        {
            options.SetColor(color.Value);
        }
        BannerAdInit(new List<MaxBannerOptions> { options });
    }

    /// <summary>
    /// banner 初始化
    /// </summary>
    /// <param name="options">banner初始化参数</param>
    public abstract void BannerAdInit(List<MaxBannerOptions> options);


    /// <summary>
    /// 展示banner
    /// </summary>
    /// <param name="pname">广告场景</param>
    public void BannerAdShow(string pname)
    {
        BannerAdShow(new List<string> { pname });
    }


    /// <summary>
    /// 展示banner
    /// </summary>
    /// <param name="pnames">广告场景列表</param>
    public abstract void BannerAdShow(List<string> pnames);

    /// <summary>
    /// 加载开屏广告
    /// </summary>
    public abstract void AppOpenAdLoad();

    /// <summary>
    /// 展示开屏广告
    /// </summary>
    /// <param name="action"></param>
    public void AppOpenAdStart(Action<int, string, string> action)
    {
        AppOpenAdStart("start", action);
    }

    /// <summary>
    /// 展示开屏广告
    /// </summary>
    /// <param name="pname">广告场景</param>
    /// <param name="action">回调</param>
    public abstract void AppOpenAdStart(string pname, Action<int, string, string> action);

    /// <summary>
    /// 停止展示开屏广告
    /// </summary>
    public abstract void AppOpenAdStop();

    /// <summary>
    /// 设置支付回调
    /// </summary>
    /// <param name="chargeAction">支付回调</param>
    public abstract void ChargeSetCallback(Action<int, string, MaxChargeData> chargeAction);

    /// <summary>
    /// 内购支付 （回调会在登陆的第二个参数中回调）
    /// </summary>
    /// <param name="productId">商品id</param>
    /// <param name="consumeExt">消耗型透传参数</param>
    public abstract void Charge(string productId, string consumeExt = null);

    /// <summary>
    /// 恢复购买（回调会在登陆的第二个参数中回调）
    /// </summary>
    public abstract void ChargeRecovery();


    /// <summary>
    /// 充值商品信息
    /// </summary>
    /// <param name="productIds">商品id列表</param>
    /// <param name="action">回调方法</param>
    public abstract void ChargeProducts(List<string> productIds, Action<int, string, List<MaxChargeProductData>> action);


    /// <summary>
    /// 内购完成
    /// </summary>
    /// <param name="orderSn">支付成功回调回返回参数</param>
    public abstract void ChargeFinish(string orderSn);


    /// <summary>
    /// 展示应用内评分
    /// </summary>
    /// <param name="action">回调方法</param>
    public abstract void RequestReview(Action<int, string> action);


    /// <summary>
    /// 邀请用户
    /// </summary>
    /// <param name="type">邀请类型，fb=facebook邀请，system=系统邀请</param>
    /// <param name="action">回调方法</param>
    public void SocialInvite(string type, Action<int, string> action)
    {
        SocialInvite(type, null, null, action);
    }

    /// <summary>
    /// 邀请用户
    /// </summary>
    /// <param name="type">邀请类型，fb=facebook邀请，system=系统邀请</param>
    /// <param name="name">邀请点位</param>
    /// <param name="action">回调方法</param>
    public void SocialInvite(string type, string name, Action<int, string> action)
    {
        SocialInvite(type, name, null, action);
    }

    /// <summary>
    /// 邀请用户
    /// </summary>
    /// <param name="type">邀请类型，fb=facebook邀请，system=系统邀请</param>
    /// <param name="name">邀请点位</param>
    /// <param name="ext">透传参数</param>
    /// <param name="action">回调方法</param>
    public abstract void SocialInvite(string type, string name, string ext, Action<int, string> action);

    /// <summary>
    /// 获取成功邀请数
    /// </summary>
    /// <param name="action">回调方法</param>
    public void SocialInvite(Action<int, string, int> action)
    {
        SocialInviteCount(null, action);
    }

    /// <summary>
    /// 获取成功邀请数
    /// </summary>
    /// <param name="name">邀请点位</param>
    /// <param name="action">回调方法</param>
    public abstract void SocialInviteCount(string name, Action<int, string, int> action);


    /// <summary>
    /// 邀请来源
    /// </summary>
    /// <param name="action">回调方法</param>
    public abstract void SocialFrom(Action<int, string, MaxSocialInviteFromData> action);

    /// <summary>
    /// 邀请奖励
    /// </summary>
    public abstract void SocialInviteReward();


    /// <summary>
    /// 更新排行榜的用户分数
    /// </summary>
    /// <param name="score"></param>
    public abstract void LbScore(int score);

    /// <summary>
    /// 排行榜查询初始化
    /// </summary>
    /// <param name="options"></param>
    public abstract void LbQueryInit(MaxLbQueryOptions options);

    /// <summary>
    /// 排行榜向下查询
    /// </summary>
    /// <param name="action"></param>
    public void LbGlobalQuery(Action<int, string, List<MaxLbQueryData>> action)
    {
        LbGlobalQuery(false, action);
    }

    /// <summary>
    /// 排行榜向下/向上查询
    /// </summary>
    /// <param name="up">false=向下；true=向上；</param>
    /// <param name="action"></param>
    public abstract void LbGlobalQuery(bool up, Action<int, string, List<MaxLbQueryData>> action);
}