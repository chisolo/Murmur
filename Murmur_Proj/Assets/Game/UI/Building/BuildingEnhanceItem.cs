using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;
using DG.Tweening;

public class BuildingEnhanceItem : MonoBehaviour
{
    [SerializeField] Image _enhanceIcon;
    [SerializeField] Text _enhanceNameTxt;
    [SerializeField] Text _enhanceDescTxt;
    [SerializeField] Text _enhanceLevelTxt;
    [SerializeField] Image _enhanceProgress;
    [SerializeField] ButtonEx _enhanceBtn;
    [SerializeField] Text _enhanceCostTxt;
    [SerializeField] GameObject _enhanceMax;

    private BuildingData _buildingData;
    private BuildingEnhance _buildingEnhance;
    private bool _longPress = false;
    private IEnumerator _coroutine;
    public void Init(BuildingData buildingData, BuildingEnhance buildingEnhance)
    {
        _buildingData = buildingData;
        _buildingEnhance = buildingEnhance;
        _enhanceIcon.ShowSprite(string.Format(GameUtil.ResEnhanceIconPath, _buildingEnhance.cfg.icon));
        _enhanceNameTxt.text = _buildingEnhance.cfg.name.Locale();
        _enhanceBtn.onClick.AddListener(OnEnhanceBtn);
        _enhanceBtn.OnLongPress.AddListener(OnEnhanceBtnLongBegin);
        _coroutine = EnhanceCoroutine();
        Refresh(false);
    }

    public void Refresh(bool anim = true)
    {
        _enhanceDescTxt.text = _buildingEnhance.cfg.desc.Locale(_buildingEnhance.NextValue());
        if(_buildingEnhance.IsMax()) {
            _enhanceLevelTxt.text = "Max";
            _enhanceBtn.gameObject.SetActive(false);
            _enhanceMax.SetActive(true);
            if(!anim) _enhanceProgress.fillAmount = 1;
            else _enhanceProgress.DOFillAmount(1, 0.1f);
            StopCoroutine(_coroutine);
        } else {
            _enhanceLevelTxt.text = _buildingEnhance.level.ToString();
            _enhanceBtn.gameObject.SetActive(true);
            var cost = _buildingEnhance.LvlupCost();
            bool enough = PlayerModule.Instance.Money >= cost;
            _enhanceCostTxt.text = FormatUtil.Currency(cost);
            _enhanceMax.SetActive(false);
            if(!anim) _enhanceProgress.fillAmount = _buildingEnhance.LevelRatio();
            else _enhanceProgress.DOFillAmount(_buildingEnhance.LevelRatio(), 0.1f);
            _enhanceBtn.interactable = enough;
            if(!enough) StopCoroutine(_coroutine);
        }
    }
    public void RefreshMoney()
    {
        if(!_buildingEnhance.IsMax()) {
            var cost = _buildingEnhance.LvlupCost();
            bool enough = PlayerModule.Instance.Money >= cost;
            _enhanceBtn.interactable = enough;
        }
    }
    private void OnEnhanceBtn()
    {
        if(_longPress) {
            _longPress = false;
            StopCoroutine(_coroutine);
        } else {
            AudioModule.Instance.PlaySfx(GameUtil.ResSfxClick);
            BuildingModule.Instance.LevelupEnhance(_buildingData.Config.id, _buildingEnhance.id);
        }
    }
    private void OnEnhanceBtnLongBegin()
    {
        _longPress = true;
        
        StartCoroutine(_coroutine);
    }
    private IEnumerator EnhanceCoroutine()
    {
        while(true) {
            AudioModule.Instance.PlaySfx(GameUtil.ResSfxClick);
            BuildingModule.Instance.LevelupEnhance(_buildingData.Config.id, _buildingEnhance.id);
            yield return new WaitForSeconds(0.15f);
        }
}
}
