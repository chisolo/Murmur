using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

[RequireComponent(typeof(Text))]
public class TextLocale : MonoBehaviour
{
    Text text;
    [SerializeField] string key;
    public string Key
    {
        get {
            return key;
        }

        set {
            key = value;
            UpdateText();
        }
    }

    public string LocaleText
    {
        get {
            if (string.IsNullOrEmpty(key)) return key;
            return key.Locale();
        }
    }

    private void Start()
    {
        this.text = GetComponent<Text>();
        this.UpdateText();
    }

    private void OnEnable()
    {
        UpdateText();
    }

    private void OnDisable()
    {

    }

    public void UpdateText()
    {
        if (text != null && !string.IsNullOrEmpty(this.key)) {
            text.text = LocaleText;
        }
    }
}

