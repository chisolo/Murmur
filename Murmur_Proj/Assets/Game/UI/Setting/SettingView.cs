using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class SettingView : PopupUIBaseCtrl
{
    public static string PrefabPath = "Assets/Res/UI/Prefab/Setting/SettingView.prefab";
    [SerializeField] Button _musicBtn;
    [SerializeField] Button _soundBtn;
    [SerializeField] ToggleUI _musicToggle;
    [SerializeField] ToggleUI _soundToggle;
    [SerializeField] Button _privacyBtn;
    [SerializeField] Button _termsBtn;
    [SerializeField] Button _restoreBtn;
    [SerializeField] Text _verTxt;
    public override void Init(PopupUIArgs arg)
    {
        _musicBtn.onClick.AddListener(OnMusicBtn);
        _soundBtn.onClick.AddListener(OnSoundBtn);
        _privacyBtn.onClick.AddListener(OnPrivacyBtn);
        _termsBtn.onClick.AddListener(OnTermsBtn);
        _restoreBtn.onClick.AddListener(OnRestoreBtn);
        _musicToggle.Toggle(PlayerModule.Instance.Bgm ? 0 : 1);
        _soundToggle.Toggle(PlayerModule.Instance.Sfx ? 0 : 1);
        _verTxt.text = Application.version;
    }
    private void OnMusicBtn()
    {
        PlayerModule.Instance.SetBgm();
        _musicToggle.Toggle(PlayerModule.Instance.Bgm ? 0 : 1);
    }
    private void OnSoundBtn()
    {
        PlayerModule.Instance.SetSfx();
        _soundToggle.Toggle(PlayerModule.Instance.Sfx ? 0 : 1);
    }
    private void OnPrivacyBtn()
    {
        Application.OpenURL(GameUtil.PrivacyUrl);
    }
    private void OnTermsBtn()
    {
        Application.OpenURL(GameUtil.TermsUrl);
    }
    private void OnRestoreBtn()
    {
        ShopModule.Instance.Restore();
    }
}
