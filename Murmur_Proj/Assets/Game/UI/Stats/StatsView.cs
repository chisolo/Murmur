using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class StatsView : PopupUIBaseCtrl
{
    public static string PrefabPath = "Assets/Res/UI/Prefab/Stats/StatsView.prefab";

    [SerializeField] Text _totalIncomeTxt;
    [SerializeField] Text _totalSalaryTxt;
    [SerializeField] Text _unassignedSalaryTxt;
    [SerializeField] Text _profitBureau;
    [SerializeField] GameObject _dinerTitle;
    [SerializeField] TotalStatsItem _hotdogStats;
    [SerializeField] TotalStatsItem _hamburgerStats;
    [SerializeField] TotalStatsItem _pizzaStats;
    [SerializeField] GameObject _storageTitle;
    [SerializeField] TotalStatsItem _sausageStats;
    [SerializeField] TotalStatsItem _flourStats;
    [SerializeField] TotalStatsItem _cheeseStats;
    [SerializeField] GameObject _receptionTitle;
    [SerializeField] TotalStatsItem _receptionStats;
    [SerializeField] GameObject _deliverTitle;
    [SerializeField] TotalStatsItem _deliverStats;
    public override void Init(PopupUIArgs arg)
    {

    }
    void Start()
    {
        Refresh();
    }
    private void Refresh()
    {
        var unassignedSalary = StaffModule.Instance.GetAllSalaryNotAssigned();
        _totalIncomeTxt.text = FormatUtil.Revenue((int)BuildingModule.Instance.GetTotalProp(BuildingProperty.Revenue), false);
        _totalSalaryTxt.text = FormatUtil.Salary((int)BuildingModule.Instance.GetTotalProp(BuildingProperty.Salary) + unassignedSalary, false);
        _unassignedSalaryTxt.text = FormatUtil.Salary(unassignedSalary, false);
        _profitBureau.text = BuildingModule.Instance.GetProfitBureau().Locale();

        bool unlockHotdog = TalentModule.Instance.IsComplete("TALENT_DATA_1");
        bool unlockHamburger = TalentModule.Instance.IsComplete("TALENT_DATA_2");
        bool unlockPizza = TalentModule.Instance.IsComplete("TALENT_DATA_3");

        if(unlockHotdog) _hotdogStats.Show(ServiceType.HotdogBureau);
        else _hotdogStats.Hide();

        if(unlockHamburger) _hamburgerStats.Show(ServiceType.HambergBureau);
        else _hamburgerStats.Hide();

        if(unlockPizza) _pizzaStats.Show(ServiceType.PizzaBureau);
        else _pizzaStats.Hide();

        _dinerTitle.SetActive(unlockHotdog || unlockPizza || unlockHamburger);
         
        bool unlockSausage = BuildingModule.Instance.GetBuiltBuildingCount(ServiceType.SausageStorageCell) > 0;
        bool unlockFlour = BuildingModule.Instance.GetBuiltBuildingCount(ServiceType.FlourStorageCell) > 0;
        bool unlockCheese = BuildingModule.Instance.GetBuiltBuildingCount(ServiceType.CheeseStorageCell) > 0;

        if(unlockSausage) _sausageStats.Show(ServiceType.SausageStorageCell);
        else _sausageStats.Hide();

        if(unlockFlour) _flourStats.Show(ServiceType.FlourStorageCell);
        else _flourStats.Hide();

        if(unlockCheese) _cheeseStats.Show(ServiceType.CheeseStorageCell);
        else _cheeseStats.Hide();

        _storageTitle.SetActive(unlockSausage || unlockFlour || unlockCheese);

        bool unlockReception = TalentModule.Instance.IsComplete("TALENT_DATA_4");
        if(unlockReception) _receptionStats.Show(ServiceType.ReceptionCell);
        else _receptionStats.Hide();

        _receptionTitle.SetActive(unlockReception);

        bool unlockDeliver = BuildingModule.Instance.GetBuiltBuildingCount(ServiceType.DeliverCell) > 0;
        if(unlockDeliver) _deliverStats.Show(ServiceType.DeliverCell);
        else _deliverStats.Hide();
        
        _deliverTitle.SetActive(unlockDeliver);
    }
}
