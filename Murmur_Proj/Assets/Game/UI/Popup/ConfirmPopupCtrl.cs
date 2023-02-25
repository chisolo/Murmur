using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;
using System;
using UnityEngine.UI;

public class ConfirmPopupCtrl : PopupUIBaseCtrl
{
    public class ConfirmPopupCtrlArgs : PopupUIArgs
    {
        public string title;
        public string body;

        public bool ok;
        public bool cancel;

        public string okText;
        public string cancelText;
    }

    public static string PrefabPath = "Assets/Res/UI/Prefab/Popup/ConfirmPopup.prefab";

    [SerializeField]
    private Text _titleText;

    [SerializeField]
    private Text _bodyText;

    [SerializeField]
    private GameObject _okBtn;

    [SerializeField]
    private GameObject _cancelBtn;

    [SerializeField]
    private Text _okText;

    [SerializeField]
    private Text _cancelText;

    public Action OnOk = null;
    public Action OnCancel = null;

    private bool _isOk = false;

    public override void Init(PopupUIArgs arg)
    {
        var param = (ConfirmPopupCtrlArgs)arg;

        _titleText.text = param.title;
        _bodyText.text = param.body;

        _okText.text = param.okText;
        _cancelText.text = param.cancelText;

        _okBtn.SetActive(param.ok);
        _cancelBtn.SetActive(param.cancel);
    }

    public void OnClickOK()
    {
        _isOk = true;
        Hide();
    }

    protected override void OnBeforeHide()
    {
        base.OnBeforeHide();
        if (_isOk) {
            OnOk?.Invoke();
        } else {
            OnCancel?.Invoke();
        }

        OnOk = null;
        OnCancel = null;

    }
}