using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;
public class BuildingStatItem : MonoBehaviour
{
    [SerializeField] Image _statIcon;
    [SerializeField] Text _statValueTxt;

    private BuildingData _buildingData;
    private BuildingStat _buildingStat;
    private bool _isDark;


    public void Init(BuildingData buildingData, BuildingStat buildingStat, bool isDark = false)
    {
        _buildingData = buildingData;
        _buildingStat = buildingStat;
        _isDark = isDark;
    }

    public void Start()
    {
        _statIcon.ShowSprite(GameUtil.BuildingStatIcons[(int)_buildingStat]);
        if(_isDark)  _statValueTxt.color = new Color32(90, 74, 58, 255);
        else _statValueTxt.color = new Color32(219, 216, 193, 255);
        Refresh();
    }

    public void Refresh()
    {
        switch(_buildingStat) {
            case BuildingStat.Revenue:
            {
                _statValueTxt.text = FormatUtil.Revenue((int)_buildingData.GetProp(BuildingProperty.Revenue));
            }
            break;
            case BuildingStat.Salary:
            {
                _statValueTxt.text = FormatUtil.Salary((int)_buildingData.GetProp(BuildingProperty.Salary));
            }
            break;
            case BuildingStat.TotalSalary:
            {
                _statValueTxt.text = FormatUtil.Salary((int)_buildingData.GetChildProps(BuildingProperty.Salary));
            }
            break;
            case BuildingStat.BureauService:
            {
                _statValueTxt.text = _buildingData.GetPropEffy(BuildingProperty.ServiceDuration).ToString("0.00/h");
            }
            break;
            case BuildingStat.BureauCapacity:
            {
                _statValueTxt.text = _buildingData.GetProp(BuildingProperty.Capacity).ToString("0");
            }
            break;
            case BuildingStat.ReceptionService:
            {
                _statValueTxt.text = _buildingData.GetChildPropEffy(BuildingProperty.ServiceDuration).ToString("0.00/h");
            }
            break;
            case BuildingStat.ReceptionCell:
            {
                _statValueTxt.text = string.Format("{0:D}", _buildingData.GetBuildChildCount());
            }
            break;
            case BuildingStat.ReceptionCellService:
            {
                _statValueTxt.text = _buildingData.GetPropEffy(BuildingProperty.ServiceDuration).ToString("0.00/h");
            }
            break;
            case BuildingStat.ReceptionCapacity:
            {
                _statValueTxt.text = _buildingData.GetChildProps(BuildingProperty.Capacity).ToString("0");
            }
            break;
            case BuildingStat.DeliverCapacity:
            {
                _statValueTxt.text = _buildingData.GetChildProps(BuildingProperty.Capacity).ToString("0");
            }
            break;
            case BuildingStat.DeliverCell:
            {
                _statValueTxt.text = string.Format("{0:D}", _buildingData.GetBuildChildCount());
            }
            break;
            case BuildingStat.DeliverCellCapacity:
            {
                _statValueTxt.text = _buildingData.GetProp(BuildingProperty.Capacity).ToString("0");
            }
            break;
            case BuildingStat.DeliverCellSpeed:
            {
                _statValueTxt.text = _buildingData.GetProp(BuildingProperty.MoveSpeed).ToString("0.00");
            }
            break;
            case BuildingStat.StorageService:
            {
                _statValueTxt.text = _buildingData.GetChildPropEffy(BuildingProperty.ServiceDuration).ToString("0.00/h");
            }
            break;
            case BuildingStat.StorageCapacity:
            {
                _statValueTxt.text = _buildingData.GetChildProps(BuildingProperty.Capacity).ToString("0");
            }
            break;
            case BuildingStat.StorageCell:
            {
                _statValueTxt.text = string.Format("{0:D}", _buildingData.GetBuildChildCount());
            }
            break;
            case BuildingStat.StorageCellService:
            {
                _statValueTxt.text = _buildingData.GetPropEffy(BuildingProperty.ServiceDuration).ToString("0.00/h");
            }
            break;
        }
    }
}
