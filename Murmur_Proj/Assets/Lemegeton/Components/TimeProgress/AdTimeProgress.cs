using System;
using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class AdTimeProgress : MonoBehaviour
{

    [SerializeField] TimeProgressBar _timeProgress;

    [SerializeField] Button _reduceBtn;
    [SerializeField] Text _reduceTimeTxt;
    [SerializeField] Text _adTxt;
    [SerializeField] Button _finishBtn;
    [SerializeField] Text _finishCostTxt;
    [SerializeField] Text _adBtnTxt;

    private long _endTime => _timeData.EndTime;
    private long _duration => _timeData.Duration;
    private bool _inited = false;
    private ITimeProgressBarData _timeData;

    public void Reset()
    {
        _inited = false;
    }

    public void Init(ITimeProgressBarData timeData)
    {
        if(_inited) return;
        _timeData = timeData;
        var now = NtpModule.Instance.UtcNowSeconds;
        var remain = _endTime - now;

        _reduceBtn.onClick.RemoveAllListeners();
        _reduceBtn.onClick.AddListener(OnClickAdBtn);

        _finishCostTxt.text = FormatUtil.Currency(PlayerModule.Instance.TimeToGem(remain));
        _finishBtn.gameObject.SetActive(true);
        _finishBtn.interactable = true;

        _finishBtn.onClick.RemoveAllListeners();
        _finishBtn.onClick.AddListener(OnClickFinish);

        _adTxt.text = string.Format("{0} {1}/{2}", GameUtil.AdsText.Locale(), _timeData.AdCount, ConfigModule.Instance.Common().ad_max);
        SetAdBtn();

        _timeProgress.Init(timeData, RefreshProgress, EndProgress);
        _inited = true;
    }

    public void UpdateStatus()
    {
        RefreshProgress();
        _adTxt.text = string.Format("{0} {1}/{2}", GameUtil.AdsText.Locale(), _timeData.AdCount, ConfigModule.Instance.Common().ad_max);
        SetAdBtn();
    }

    void SetAdBtn()
    {
        var now = NtpModule.Instance.UtcNowSeconds;
        var remain = _endTime - now;
        if (remain <= 0) {
            _reduceBtn.interactable = false;
            return;
        }
        if (_timeData.AdCount > 0) {
            // 有广告
            _reduceTimeTxt.gameObject.SetActiveIfNeed(true);
            if (_timeData.AdCooldownTime > now) {
                // 冷却中
                _adBtnTxt.text = "AVAILABLE_IN".Locale();
                _reduceBtn.interactable = false;
                _reduceTimeTxt.text = FormatUtil.FormatTimeAuto(_timeData.AdCooldownTime - now);
            } else {
                _adBtnTxt.text = "REDUCE".Locale();
                _reduceBtn.interactable = true;
                _reduceTimeTxt.text = FormatUtil.FormatTimeAuto(AdModule.Instance.GetReduceTime(remain));
            }
        } else {
            // 无次数
            _adBtnTxt.text = "NOT_AVAILABLE".Locale();
            _reduceBtn.interactable = false;
            _reduceTimeTxt.gameObject.SetActiveIfNeed(false);
        }
    }

    private void RefreshProgress()
    {
        var remain = _endTime - NtpModule.Instance.UtcNowSeconds;

        _finishCostTxt.text = FormatUtil.Currency(PlayerModule.Instance.TimeToGem(remain));
        SetAdBtn();
    }
    private void EndProgress()
    {
        _reduceBtn.interactable = false;
        _finishBtn.interactable = false;
    }

    private void OnClickAdBtn()
    {
        if (_timeData.AdCount <= 0) {
            return;
        }

        WatchAd();
    }

    private void WatchAd()
    {
        AdModule.Instance.ShowRewardAd(_timeData.GetAdUnitName(), () => {
            var remain = _endTime - NtpModule.Instance.UtcNowSeconds;
            var time = AdModule.Instance.GetReduceTime(remain);
            var adTime = AdModule.Instance.GetCooldown(remain);
            _timeData.ReduceTimeByAd(time, adTime);
            _adTxt.text = string.Format("{0} {1}/{2}", GameUtil.AdsText.Locale(), _timeData.AdCount, ConfigModule.Instance.Common().ad_max);
            SetAdBtn();
        });
    }

    private void OnClickFinish()
    {
        var remain = _endTime - NtpModule.Instance.UtcNowSeconds;
        var cost = PlayerModule.Instance.TimeToGem(remain);
        if (PlayerModule.Instance.UseCoin(cost)) {
            _timeData.Finish();
        } else {
            UIMgr.Instance.OpenNotEnoughCoin();
        }
    }

}
