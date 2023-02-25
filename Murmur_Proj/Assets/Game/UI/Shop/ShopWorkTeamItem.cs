using System;
using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class ShopWorkTeamItem : MonoBehaviour
{
    [SerializeField] GameObject _own;
    [SerializeField] GameObject _notOwn;
    [SerializeField] Text _teamName;
    [SerializeField] Text _teamName2;


    public void Init(string name, bool own)
    {
        _own.SetActive(own);
        _notOwn.SetActive(!own);

        _teamName.text = name.Locale();
        _teamName2.text = name.Locale();
    }

}