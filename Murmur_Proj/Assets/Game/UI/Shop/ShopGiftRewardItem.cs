using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class ShopGiftRewardItem : MonoBehaviour
{
    [SerializeField] Text _rewardText;
    [SerializeField] Text _amountText;
    [SerializeField] string _format;


    public void Init(float value, string name)
    {
        if (_rewardText != null) {
            _rewardText.text = name;
        }

        if (_amountText != null) {
            _amountText.text = string.Format(_format, value);
        }
    }
}