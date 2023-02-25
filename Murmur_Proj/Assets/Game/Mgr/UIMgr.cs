using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;
using System.Threading.Tasks;

public class UIMgr : GameMgr<UIMgr>
{
    [SerializeField] Transform _mainPanel;
    [SerializeField] Transform _popupPanel;

    public int UICount => _popupCount;
    private bool _hasFirstLevel = false;
    private bool _opening = false;
    private int _popupCount = 0;
    void Awake()
    {
        _popupCount = 0;
    }

    public void OpenUI(string path, PopupUIArgs arg, bool hideTop, bool hideBottom, Action<PopupUIBaseCtrl> onOpen = null, Action onClose = null)
    {
        ++_popupCount;
        if(hideTop) EventModule.Instance.FireEvent(EventDefine.MainUIHideTop);
        if(hideBottom) EventModule.Instance.FireEvent(EventDefine.MainUIHideBottom);
        PrefabLoader.InstantiateAsync<PopupUIBaseCtrl>(path, _mainPanel, (popup) => {
            popup.OnClose = () => {
                --_popupCount;
                if(hideTop) EventModule.Instance.FireEvent(EventDefine.MainUIShowTop);
                if(hideBottom) EventModule.Instance.FireEvent(EventDefine.MainUIShowBottom);
                AudioModule.Instance.PlaySfx(GameUtil.ResSfxClose);
                onClose?.Invoke();
            };
            popup.onOpen = () => {
                onOpen?.Invoke(popup);
            };
            popup.Init(arg);
            popup.Show();
        });
    }

    public void OpenUIByClick(string path, PopupUIArgs arg, bool hideTop, bool hideBottom, Action<PopupUIBaseCtrl> onOpen = null, Action onClose = null)
    {
        if(_opening) return;
        _opening = true;
        ++_popupCount;
        if(hideTop) EventModule.Instance.FireEvent(EventDefine.MainUIHideTop);
        if(hideBottom) EventModule.Instance.FireEvent(EventDefine.MainUIHideBottom);
        AudioModule.Instance.PlaySfx(GameUtil.ResSfxOpen);
        PrefabLoader.InstantiateAsync<PopupUIBaseCtrl>(path, _mainPanel, (popup) => {
            popup.OnClose = () => {
                --_popupCount;
                if(hideTop) EventModule.Instance.FireEvent(EventDefine.MainUIShowTop);
                if(hideBottom) EventModule.Instance.FireEvent(EventDefine.MainUIShowBottom);
                AudioModule.Instance.PlaySfx(GameUtil.ResSfxClose);
                onClose?.Invoke();
            };
            popup.onOpen = () => {
                _opening = false;
                onOpen?.Invoke(popup);
            };
            popup.Init(arg);
            popup.Show();
        });
    }
    public void OpenConfirm(string title, string body, bool ok, bool cancel, string okText, string cancelText, Action onOk = null, Action onCancel = null, Action<PopupUIBaseCtrl> onOpen = null, Action onClose = null)
    {
        ++_popupCount;
        EventModule.Instance.FireEvent(EventDefine.MainUIHideTop);
        var arg = new ConfirmPopupCtrl.ConfirmPopupCtrlArgs() {
            title = title,
            body = body,
            ok = ok,
            cancel = cancel,
            okText = okText,
            cancelText = cancelText,
        };
        PrefabLoader.InstantiateAsync<ConfirmPopupCtrl>(ConfirmPopupCtrl.PrefabPath, _popupPanel, (popup) => {
            popup.OnOk = onOk;
            popup.OnCancel = onCancel;
            popup.OnClose = () => {
                --_popupCount;
                EventModule.Instance.FireEvent(EventDefine.MainUIShowTop);
                onClose?.Invoke();
            };
            popup.Init(arg);
            onOpen?.Invoke(popup);
            popup.Show();
        });
    }
    public void OpenPopUpPanel(string path, PopupUIArgs arg, Action<PopupUIBaseCtrl> onOpen = null, Action onClose = null)
    {
        ++_popupCount;
        PrefabLoader.InstantiateAsync<PopupUIBaseCtrl>(path, _popupPanel, (popup) => {
            popup.OnClose = () => {
                --_popupCount;
                onClose?.Invoke();
            };
            popup.Init(arg);
            onOpen?.Invoke(popup);
            popup.Show();
        });
    }
    public void OpenConfirmOk(string title, string body, string okText, Action onOk = null, Action<PopupUIBaseCtrl> onOpen = null)
    {
        OpenConfirm(title, body, true, false, okText, "", onOk, null, onOpen);
    }

    public void OpenConfirmOkCancel(string title, string body, string okText, string cancelText, Action onOk = null, Action onCancel = null, Action<PopupUIBaseCtrl> onOpen = null)
    {
        OpenConfirm(title, body, true, true, okText, cancelText, onOk, onCancel, onOpen);
    }

    public void OpenNotEnoughCoin()
    {
        OpenConfirmOkCancel(GameUtil.NotEnoughCoinConfirmTitleText.Locale(), GameUtil.NotEnoughCoinConfirmBodyText.Locale(), GameUtil.NotEnoughCoinConfirmOKText.Locale(), StaffText.Back.Locale(), () => {
            ShopUICtrl.Open(ShopUICtrl.Section.Coin);
        });
    }

    public void ShowLoading(float second)
    {
        var args = new LoadingView.Args();
        args.timeout = second;
        OpenPopUpPanel(LoadingView.PrefabPath, args, null, null);
    }

    public void HideLoading()
    {
        EventModule.Instance.FireEvent(EventDefine.HideLoading);
    }

    public void ShowMainParts()
    {
        MainUICtrl.ForceHide = false;
        EventModule.Instance.FireEvent("MainUIShowBottom");
        EventModule.Instance.FireEvent("MainUIShowTop");
    }

    public void HideMainParts()
    {
        MainUICtrl.ForceHide = true;
        EventModule.Instance.FireEvent("MainUIHideBottom");
        EventModule.Instance.FireEvent("MainUIHideTop");
    }
}
