using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class TalentSectionTitle : MonoBehaviour
{
    [SerializeField]
    private Text _titleText;

    public void Init(string text)
    {
        _titleText.text = text.Locale();
    }
}