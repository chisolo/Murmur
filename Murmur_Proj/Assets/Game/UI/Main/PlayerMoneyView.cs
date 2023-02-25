using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoneyView : MonoBehaviour
{
    [SerializeField]
    private Text _moneyText;
    [SerializeField]
    private Image _progressBg;

    int lastMoney = -111;

    public void UpdateMoney(bool force = false)
    {
        if (lastMoney != PlayerModule.Instance.Money || force) {
            _moneyText.text = FormatUtil.Currency(PlayerModule.Instance.Money, false);
            _progressBg.fillAmount = (float)PlayerModule.Instance.Money / PlayerModule.Instance.MoneyLimit;

            lastMoney = PlayerModule.Instance.Money;
        }

    }

    public void OnClickPlus()
    {

    }
}