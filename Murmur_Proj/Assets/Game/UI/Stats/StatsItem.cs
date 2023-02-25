using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;
public class StatsItem : MonoBehaviour
{
    [SerializeField] Image _statIcon;
    [SerializeField] Text _statValueTxt;
    [SerializeField] Text _statNameTxt;

    private string _service;
    private BuildingTotalStat _stat;

    public void Init(string service, BuildingTotalStat stat)
    {
        _service = service;
        _stat = stat;
    }
    public void Start()
    {
        _statIcon.ShowSprite(GameUtil.BuildingTotalStatIcons[(int)_stat]);
        _statNameTxt.text = GameUtil.BuildingTotalStatTitles[(int)_stat].Locale();
        Refresh();
    }
    private void Refresh()
    {
        switch(_stat) {
            case BuildingTotalStat.Revenue:
            {
                _statValueTxt.text = FormatUtil.Revenue((int)BuildingModule.Instance.GetTotalProp(_service, BuildingProperty.Revenue), false);
            }
            break;
            case BuildingTotalStat.Salary:
            {
                _statValueTxt.text = FormatUtil.Salary((int)BuildingModule.Instance.GetTotalProp(_service, BuildingProperty.Salary), false);
            }
            break;
            case BuildingTotalStat.Staff:
            {
                _statValueTxt.text = BuildingModule.Instance.GetStaffCount(_service).ToString();
            }
            break;
            case BuildingTotalStat.BureauCapacity:
            {
                _statValueTxt.text = BuildingModule.Instance.GetTotalProp(_service, BuildingProperty.Capacity).ToString("0");
            }
            break;
            case BuildingTotalStat.StorageCapacity:
            {
                _statValueTxt.text = BuildingModule.Instance.GetTotalProp(_service, BuildingProperty.Capacity).ToString("0");
            }
            break;
            case BuildingTotalStat.StorageService:
            {
                _statValueTxt.text = BuildingModule.Instance.GetTotalPropEffy(_service, BuildingProperty.ServiceDuration).ToString("0.00/h");
            }
            break;
            case BuildingTotalStat.ReceptionCapacity:
            {
                _statValueTxt.text = BuildingModule.Instance.GetTotalProp(_service, BuildingProperty.Capacity).ToString("0");
            }
            break;
            case BuildingTotalStat.ReceptionService:
            {
                _statValueTxt.text = BuildingModule.Instance.GetTotalPropEffy(_service, BuildingProperty.ServiceDuration).ToString("0.00/h");
            }
            break;
            case BuildingTotalStat.DeliverCapacity:
            {
                _statValueTxt.text = BuildingModule.Instance.GetTotalProp(_service, BuildingProperty.Capacity).ToString("0");
            }
            break;
            case BuildingTotalStat.DeliverSpeed:
            {
                _statValueTxt.text = BuildingModule.Instance.GetTotalProp(_service, BuildingProperty.MoveSpeed).ToString("0.00");
            }
            break;
        }
    }
}
