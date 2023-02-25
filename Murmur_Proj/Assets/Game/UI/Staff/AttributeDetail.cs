using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class AttributeDetail : PopupUIBaseCtrl
{
    public class AttributeDetailArgs : PopupUIArgs
    {
        public string attributeId;
        public float value;
    }

    public static string PrefabPath = "Assets/Res/UI/Prefab/Popup/AttributeDetail.prefab";

    [SerializeField]
    private Image _icon;

    [SerializeField]
    private Text _nameText;

    [SerializeField]
    private Text _descriptionText;


    public override void Init(PopupUIArgs arg)
    {
        var param = (AttributeDetailArgs)arg;

        var attrCofing = ConfigModule.Instance.GetAttribute(param.attributeId);

        _icon.ShowSprite(AtlasDefine.GetAttributeIconPath(attrCofing.icon));
        _nameText.text = attrCofing.name.Locale();
        _descriptionText.text = string.Format(attrCofing.desc.Locale(), param.value);
    }

}