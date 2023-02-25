using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class BuildingProgressView : MonoBehaviour
{
    [SerializeField] Image progressBar;
    [SerializeField] Text timeText;
    [SerializeField] Canvas canvas;
    [SerializeField] bool longFormat;
    private float _total;
    public void Show(float remain, float total, Camera camera)
    {
        canvas.worldCamera = camera;
        _total = total;
        timeText.text = longFormat ? FormatUtil.FormatTimeLong((long)remain) : FormatUtil.FormatTimeShort((long)remain);
        progressBar.fillAmount = 1 - remain / _total;
    }
    public void Hide()
    {
        Destroy(gameObject);
    }
    public void Refresh(float remain)
    {
        progressBar.fillAmount = 1 - (float)remain / _total;
        timeText.text = longFormat ? FormatUtil.FormatTimeLong((long)remain) : FormatUtil.FormatTimeShort((long)remain);
    }

}
