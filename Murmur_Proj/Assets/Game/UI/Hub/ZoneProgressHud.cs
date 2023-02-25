using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class ZoneProgressHud : MonoBehaviour
{
    [SerializeField] Image progressBar;
    [SerializeField] Text timeText;
    [SerializeField] Canvas canvas;
    [SerializeField] bool longFormat;

    private ZoneData _zoneData;

    public void Init(ZoneData zoneData)
    {
        _zoneData = zoneData;
        canvas.worldCamera = RuntimeMgr.Instance.GetWorldCamera();
    }
    void Update()
    {
        if(_zoneData != null) {
            var remain = _zoneData.GetWorkTeamRemainTime();
            var remainMilli = _zoneData.GetWorkTeamRemainMilliTime();
            var total = _zoneData.GetWorkTeamDuration();
            progressBar.fillAmount = 1 - (float)remainMilli / total;
            timeText.text = longFormat ? FormatUtil.FormatTimeLong(remain) : FormatUtil.FormatTimeShort(remain);
        }
    }
}
