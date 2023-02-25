using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class GrowthFundItem : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] Text _starText;
    [SerializeField] Text _rewardCoinText;

    [SerializeField] GameObject _activeBg;

    [SerializeField] GameObject _lockBg;
    [SerializeField] GameObject _lockIcon;

    [SerializeField] GameObject _checkIcon;
    [SerializeField] GameObject _receiveRoot;

    private GrowthReward _reward;
    private GrowthFundConfig config => ConfigModule.Instance.GrowthFund();
    void Awake()
    {
        _button.onClick.AddListener(OnClickButton);
    }

    public void Init(GrowthReward reward)
    {
        _reward = reward;

        var config = ConfigModule.Instance.GrowthFund();
        var own = ShopModule.Instance.AlreadyPurchased(config.product_id);
        var star = PlayerModule.Instance.Star;

        var received = GrowthFundModule.Instance.IsReceived(_reward.id);
        var canReceive = star >= reward.require_star && own;

        _activeBg.SetActive(canReceive);
        _receiveRoot.SetActive(own);
        _lockIcon.SetActive(!own);
        _lockBg.SetActive(!canReceive);

        _checkIcon.SetActive(received);
        _button.gameObject.SetActive(!received);

        _starText.text = reward.require_star.ToString();
        _rewardCoinText.text = reward.reward_coin.ToString();

        _button.interactable = star >= reward.require_star;
        if (star >= reward.require_star) {

        }


    }

    public void OnClickButton()
    {
        if (GrowthFundModule.Instance.Receive(_reward.id)) {
            var received = true;

            _checkIcon.SetActive(received);
            _button.gameObject.SetActive(!received);
        }
    }
}