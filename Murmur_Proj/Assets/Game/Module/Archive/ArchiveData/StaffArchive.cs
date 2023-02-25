using System;
using System.Collections.Generic;
using System.Linq;
using Lemegeton;

public class StaffAttributeArchiveData
{
    public string attributeId;
    public float value;

    public StaffAttributeArchiveData() { }

    public StaffAttributeArchiveData(string attributeId, float value)
    {
        this.attributeId = attributeId;
        this.value = value;
    }
}

public class StaffArchiveData
{
    public string id;
    public string name;
    public string puppetId;
    public string model;
    public string icon;
    public string rarity;
    public int gender;
    public int salary;
    public string buildingId;

    public List<StaffAttributeArchiveData> attributes;

    public static StaffArchiveData CloneFromCandidate(CandidateArchiveData candidate)
    {
        var json = LitJson.JsonMapper.ToJson(candidate);
        var staff = LitJson.JsonMapper.ToObject<StaffArchiveData>(json);

        return staff;
    }

    public bool IsAssigned()
    {
        return !string.IsNullOrEmpty(buildingId);
    }
}

public class CandidateArchiveData : StaffArchiveData
{
    public int status;//0: 未雇佣，1: 已雇佣，2:拒绝雇佣
    public int offerSalary;

    public static CandidateArchiveData CloneFromStaff(StaffArchiveData staff)
    {
        var json = LitJson.JsonMapper.ToJson(staff);
        var candidate = LitJson.JsonMapper.ToObject<CandidateArchiveData>(json);

        return candidate;
    }
}

public class StaffArchive : IArchive
{
    public Dictionary<string, StaffArchiveData> staffs;

    public Dictionary<string, CandidateArchiveData> candidates;

    public long candidateRefreshTime;
    public int refreshCount;

    public long refreshCountResetTime;

    public void Default()
    {
        staffs = new Dictionary<string, StaffArchiveData>();
        candidates = new Dictionary<string, CandidateArchiveData>();

        candidateRefreshTime = 0;

        refreshCount = ConfigModule.Instance.Common().default_candidate_refresh_count;
        refreshCountResetTime = NtpModule.Instance.Tomorrow();
    }

    // public StaffArchiveData GetData(string id)
    // {
    //     return data.TryGetValue(id, out var value) ? value : null;
    // }

    public void ClearCandidate()
    {
        candidates.Clear();
    }

    public void AddCandidate(string id, CandidateArchiveData data)
    {
        candidates.Add(id, data);
    }

    public Dictionary<string, CandidateArchiveData> Candidates()
    {
        return candidates;
    }

    public CandidateArchiveData GetCandidate(string id)
    {
        return candidates.TryGetValue(id, out var value) ? value : null;
    }

    public List<CandidateArchiveData> CandidateList()
    {
        return candidates.Values.ToList();
    }

    public void AddStaff(StaffArchiveData data)
    {
        staffs.Add(data.id, data);
    }

    public void RemoveStaff(string id)
    {
        staffs.Remove(id);
    }

    public StaffArchiveData GetStaffData(string id)
    {
        return staffs.TryGetValue(id, out var value) ? value : null;
    }

    public void ResetRefreshCount()
    {
        refreshCount = ConfigModule.Instance.Common().default_candidate_refresh_count;
        refreshCountResetTime = NtpModule.Instance.Tomorrow();
    }
}