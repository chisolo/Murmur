using System;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class GMView : PopupUIBaseCtrl
{
    public static string PrefabPath = "Assets/Res/UI/Prefab/GM/GMView.prefab";
    [SerializeField] Button _limitBtn;
    [SerializeField] Button _addMoneyBtn;
    [SerializeField] Button _addCoinBtn;
    [SerializeField] Button _addGuestBtn;
    [SerializeField] Button _uiBtn;
    [SerializeField] Button _scaleBtn;
    [SerializeField] Button _clearBtn;
    public override void Init(PopupUIArgs arg)
    {
        _limitBtn.onClick.AddListener(OnLimitBtn);
        _addMoneyBtn.onClick.AddListener(OnAddMoneyBtn);
        _addCoinBtn.onClick.AddListener(OnAddCoinBtn);
        _addGuestBtn.onClick.AddListener(OnAddGuestBtn);
        _uiBtn.onClick.AddListener(OnUIBtn);
        _scaleBtn.onClick.AddListener(OnScaleBtn);
        _clearBtn.onClick.AddListener(OnClearBtn);
    }

    private void OnLimitBtn()
    {
        PlayerModule.Instance.UpdateMoneyLimit(100000000);
    }
    private void OnAddMoneyBtn()
    {
        PlayerModule.Instance.UpdateMoney(PlayerModule.Instance.MoneyLimit);
    }
    private void OnAddCoinBtn()
    {
        PlayerModule.Instance.AddCoin(1000000);
    }
    private void OnAddGuestBtn()
    {
        PuppetMgr.Instance.InstantSpawnGuest(10);
    }
    private void OnUIBtn()
    {
        RuntimeMgr.Instance.SetUIDebug();
    }
    private void OnScaleBtn()
    {
        RuntimeMgr.Instance.SetCameraDebug();
    }
    private void OnClearBtn()
    {
        ArchiveModule.Instance.ClearAll();
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit(0);
        #endif
    }
}
