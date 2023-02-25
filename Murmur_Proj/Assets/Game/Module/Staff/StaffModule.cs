using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;

public class StaffModule : Singleton<StaffModule>
{
    protected StaffModule() { }
    private bool _inited = false;

    private Dictionary<string, AttributeConfig> _attributeConfigs;
    private CommonConfig _commonConfig;
    private StaffArchive _staffArchive;
    private CandidateData _candidateData;
    private StaffData _staffData;

    public void Init()
    {
        if(_inited) return;


        _attributeConfigs = ConfigModule.Instance.Attributes();
        _commonConfig = ConfigModule.Instance.Common();
        _staffArchive = ArchiveModule.Instance.GetArchive<StaffArchive>();
        _candidateData = new CandidateData(_staffArchive.CandidateList(), _staffArchive.candidateRefreshTime, _staffArchive.refreshCount);
        _staffData = new StaffData(_staffArchive.staffs);

        _inited = true;
    }

#region  staff
    public StaffData GetStaffData()
    {
        return _staffData;
    }
    public StaffArchiveData GetArchiveData(string id)
    {
        return _staffArchive.GetStaffData(id);
    }

    public int GetStaffCount()
    {
        return _staffArchive.staffs.Count;
    }

    public int GetStaffMaxCount()
    {
        return _commonConfig.default_staff_max_counnt;
    }

    public void FireStaff(string id)
    {
        var staff = GetArchiveData(id);
        if (staff == null) {
            AppLogger.LogError("no staff !!");
            return;
        }
        // TODO: add token
        _staffArchive.RemoveStaff(id);

        _staffData.UpdateStaffs(_staffArchive.staffs);

        ArchiveModule.Instance.SaveArchive(_staffArchive);
    }

    public void RemoveBuilding(string id)
    {
        var staff = GetArchiveData(id);
        if (staff == null) {
            AppLogger.LogError("no staff !!");
            return;
        }

        if (staff.IsAssigned()) {
            BuildingModule.Instance.AssignBuildingStaff(staff.buildingId, string.Empty);
        }
        staff.buildingId = "";
        _staffData.UpdateStaffs(_staffArchive.staffs);
        ArchiveModule.Instance.SaveArchive(_staffArchive);
    }

    public void AssignBuilding(string staffId, string buildingId)
    {
        var staff = GetArchiveData(staffId);
        if (staff == null) {
            AppLogger.LogError("no staff !!");
            return;
        }

        var buildingData = BuildingModule.Instance.GetBuilding(buildingId);
        var buildingStaff = buildingData.GetStaff();
        if (!string.IsNullOrEmpty(buildingStaff)) {
            AppLogger.Log("remove old");
            var oldStaff = GetArchiveData(buildingStaff);
            oldStaff.buildingId = "";
        }

        if (staff.IsAssigned()) {
            AppLogger.Log("remove");
            BuildingModule.Instance.AssignBuildingStaff(staff.buildingId, string.Empty);
        }

        // must set buildingId before call AssignBuildingStaff
        staff.buildingId = buildingId;
        BuildingModule.Instance.AssignBuildingStaff(buildingId, staffId);
        _staffData.UpdateStaffs(_staffArchive.staffs);
        ArchiveModule.Instance.SaveArchive(_staffArchive);
    }

    public void SaveStaff()
    {
        _staffData.UpdateStaffs(_staffArchive.staffs);
        ArchiveModule.Instance.SaveArchive(_staffArchive);
    }
#endregion

#region  candidate
    public CandidateData GetCandidateData()
    {
        return _candidateData;
    }

    /// <summary>
    /// 候选人刷新时间是否已过期
    /// </summary>
    /// <returns></returns>
    public bool IsCandidateExpire()
    {
        return NtpModule.Instance.UtcNowSeconds > _staffArchive.candidateRefreshTime;
    }

    /// <summary>
    /// 候选人刷新时间
    /// </summary>
    /// <returns></returns>
    public int GetCandidateRefreshSeconds()
    {
        var val = 1- TalentModule.Instance.GetEnhanceValue(TalentEnhanecType.CANDIDATE_REFRESH_TIME);
        var sec = Mathf.FloorToInt(_commonConfig.default_candidate_refresh_seconds * val);
        return sec;
    }

    /// <summary>
    /// 刷新次数的重置时间是否已过期
    /// </summary>
    /// <returns></returns>
    public bool IsRefreshCountResetTimeExpire()
    {
        return NtpModule.Instance.UtcNowSeconds  > _staffArchive.refreshCountResetTime;
    }

    /// <summary>
    /// 重置刷新次数
    /// </summary>
    public void ResetRefreshCount()
    {
        _staffArchive.ResetRefreshCount();

        _candidateData.RefreshCount = _staffArchive.refreshCount;

        ArchiveModule.Instance.SaveArchive(_staffArchive);
    }

    public float GetCandidateCount()
    {
        var val = TalentModule.Instance.GetEnhanceValue(TalentEnhanecType.CANDIDATE_NUMBER);
        return _commonConfig.default_candidate_count + val;
    }

    public bool HasRefreshCount()
    {
        return _staffArchive.refreshCount > 0;
    }

    public void RefreshCandidates(bool byAd)
    {
        _staffArchive.candidateRefreshTime = NtpModule.Instance.UtcNowSeconds + GetCandidateRefreshSeconds();
        _staffArchive.ClearCandidate();
        //AppLogger.Log("refresh " + _staffArchive.candidateRefreshTime);

        var candidateCount = GetCandidateCount();
        for (int i = 0; i < candidateCount; i++) {
            StaffArchiveData staff = GachaModule.Instance.DrawStaff(_commonConfig.candidate_gacha_groups);
            CandidateArchiveData candidate = CandidateArchiveData.CloneFromStaff(staff);
            candidate.status = 0;
            candidate.offerSalary = (int)(candidate.salary * RandUtil.Range(0.9f, 1.1f));
            _staffArchive.AddCandidate(staff.id, candidate);
        }
        if (byAd) {
            _staffArchive.refreshCount--;
        }

        _candidateData.UpdateCandidates(_staffArchive.CandidateList());
        _candidateData.CandidateRefreshTime = _staffArchive.candidateRefreshTime;
        _candidateData.RefreshCount = _staffArchive.refreshCount;

        ArchiveModule.Instance.SaveArchive(_staffArchive);
    }

    public string DrawStaff(List<string> group)
    {
        StaffArchiveData staff = GachaModule.Instance.DrawStaff(group);
        _staffArchive.AddStaff(staff);

        _staffData.UpdateStaffs(_staffArchive.staffs);

        ArchiveModule.Instance.SaveArchive(_staffArchive);

        return staff.id;
    }

    public bool ForceMakeOffer = false;

    public bool MakeOffer(string id, int offerSalary, int salary)
    {
        var p = GetProbability(offerSalary, salary);
        //AppLogger.Log("probability " + p);

        var result = false;
        if (RandUtil.Percent(p) || ForceMakeOffer) {
            // offer acccept
            StaffModule.Instance.HireCandidate(id);
            result = true;
        } else {
            // offer reject
            StaffModule.Instance.RejectCandidate(id);
        }

        return result;
    }

    public void HireCandidate(string id)
    {
        var candidata = _staffArchive.GetCandidate(id);
        candidata.status = (int)CandidateOfferStatus.OfferAccepted;

        var staff = StaffArchiveData.CloneFromCandidate(candidata);
        staff.salary = _candidateData.OfferSalary;

        _staffArchive.AddStaff(staff);

        _staffData.UpdateStaffs(_staffArchive.staffs);

        ArchiveModule.Instance.SaveArchive(_staffArchive);
    }

    public void RejectCandidate(string id)
    {
        _staffArchive.GetCandidate(id).status = (int)CandidateOfferStatus.OfferRejected;

        ArchiveModule.Instance.SaveArchive(_staffArchive);
    }

    private float GetProbability(int offerSalary, int salary)
    {
        if (offerSalary < salary * HireProbability.ProbabilityZero) {
            // almost zero
            return (float)offerSalary / salary * _commonConfig.hire_probability_zero_ratio;
        } else if (offerSalary >= salary * HireProbability.ProbabilityZero && offerSalary < salary * HireProbability.ProbabilityLow) {
            // low
            return  (float)offerSalary / salary * _commonConfig.hire_probability_low_ratio;
        } else if (offerSalary >= salary * HireProbability.ProbabilityLow && offerSalary < salary * HireProbability.ProbabilityMedium) {
            // medium
            return (float)offerSalary / salary *  _commonConfig.hire_probability_medium_ratio;
        } else if (offerSalary >= salary * HireProbability.ProbabilityMedium && offerSalary < salary * HireProbability.ProbabilityHigh) {
            // high
            return (float)offerSalary / salary * _commonConfig.hire_probability_high_ratio;
        }

        // very high
        return (float)offerSalary / salary * _commonConfig.hire_probability_very_high_ratio;;
    }

    /// <summary>
    /// 厨师工资
    /// </summary>
    public int GetSalary(string staffId)
    {
        var staff = GetArchiveData(staffId);
        if(staff == null) return 0;
        return staff.salary;
    }

    /// <summary>
    /// 未分配厨师工资
    /// </summary>
    public int GetAllSalaryNotAssigned()
    {
        var sum = 0;
        foreach (var staff in _staffData.GetNoOfficeStaffList())
        {
            sum += staff.salary;
        }
        return sum;
    }


#endregion


#if UNITY_EDITOR

    [UnityEditor.MenuItem("Dev/ResetRefreshCount")]
    public static void ResetRefreshCount_Menu()
    {
        StaffModule.Instance.ResetRefreshCount();
    }

    [UnityEditor.MenuItem("Dev/SetRefreshTimeTo10s")]
    public static void SetRefreshTimeTo10_Menu()
    {
        var _staffArchive = StaffModule.Instance._staffArchive;
        var _candidateData = StaffModule.Instance._candidateData;
        _staffArchive.candidateRefreshTime = NtpModule.Instance.UtcNowSeconds + 10;
        _candidateData.CandidateRefreshTime = _staffArchive.candidateRefreshTime;
        ArchiveModule.Instance.SaveArchive(_staffArchive);
    }
#endif

}
