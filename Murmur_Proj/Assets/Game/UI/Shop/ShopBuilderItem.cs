using System;
using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuilderItem : MonoBehaviour
{
    [SerializeField] Text _titleText;
    [SerializeField] Button _buyBtn;
    [SerializeField] Transform _content;
    [SerializeField] List<ShopWorkTeamItem> _teams;
    [SerializeField] ShopBuyButton _priceButton;

    [SerializeField] GameObject _boughtRoot;
    [SerializeField] GameObject _buyBtnRoot;

    void Awake()
    {
        _buyBtn.onClick.AddListener(OnClickBuyBtn);
    }

    public void Init()
    {
        _titleText.text = string.Format("<color=#1FFC92>{0}</color>/{1} {2}"
            , BuildingModule.Instance.WorkTeam.Capacity
            , ConfigModule.Instance.Common().team_max
            , "TEAMS".Locale());

        for (var i = 0; i < _teams.Count; i++) {
            var id = GameUtil.BuildTeamIdList[i];
            var has = BuildingModule.Instance.WorkTeam.HasTeam(id);
            _teams[i].Init(id, has);
        }

        var isMAx = BuildingModule.Instance.WorkTeam.IsTeamMax();
        _buyBtnRoot.gameObject.SetActive(!isMAx);
        _boughtRoot.gameObject.SetActive(isMAx);

        var teamId = BuildingModule.Instance.FindNotOwnTeam();
        if (!string.IsNullOrEmpty(teamId)) {
            var productId = ShopModule.Instance.shopConfig.build_team_product_ids[teamId];
            _priceButton.Init(productId);
        }
    }

    public void OnClickBuyBtn()
    {
        AppLogger.Log("buy builder");
        Buy();
    }

    private void Buy()
    {
        var teamId = BuildingModule.Instance.FindNotOwnTeam();
        if (string.IsNullOrEmpty(teamId)) {
            Debug.LogError("no team");
            return;
        }
        var productId = ShopModule.Instance.shopConfig.build_team_product_ids[teamId];

        ShopModule.Instance.Purchase(productId, (ret) => {
            Init();
        });
    }
}