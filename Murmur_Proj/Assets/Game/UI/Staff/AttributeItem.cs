using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class AttributeItem : MonoBehaviour
{
    [SerializeField]
    private Image _icon;
    [Header("属性图标底图")]
    [SerializeField]
    private Image _iconBg;
    [Header("属性底图")]
    [SerializeField]
    private Image _bg;

    [SerializeField]
    private Text _value;

    [SerializeField]
    private Text _nameText;

    [SerializeField]
    private Text _descriptionText;

    private StaffAttributeArchiveData _attribute;

    public void Init(StaffAttributeArchiveData attribute)
    {
        _attribute = attribute;
        var attrCofing = ConfigModule.Instance.GetAttribute(attribute.attributeId);

        _value.text = string.Format(attrCofing.format, attribute.value);
        _icon.ShowSprite(AtlasDefine.GetAttributeIconPath(attrCofing.icon));
        if (_iconBg != null) {
            _iconBg.ShowSprite(AtlasDefine.GetAttributeIconBgPath(attrCofing.rarity));
        }

        if (_bg != null) {
            _bg.ShowSprite(AtlasDefine.GetAttributeBgPath(attrCofing.rarity));
        }

        if (_nameText != null) {
            _nameText.text = attrCofing.name.Locale();
        }

        if (_descriptionText != null) {
            _descriptionText.text = attrCofing.desc.Locale();
        }
    }

    public void OnClickSelf()
    {
        var arg = new AttributeDetail.AttributeDetailArgs();
        arg.attributeId = _attribute.attributeId;
        arg.value = _attribute.value;
        UIMgr.Instance.OpenUIByClick(AttributeDetail.PrefabPath, arg, true, false);
    }
}