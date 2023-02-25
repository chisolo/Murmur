using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lemegeton;

public class StaffData
{

    public bool isDirty = false;
    private List<StaffArchiveData> _noOfficeStaffList;
    private List<StaffArchiveData> _assignedOfficeStaffList;

    public StaffData(Dictionary<string, StaffArchiveData> staffs)
    {
        _noOfficeStaffList = staffs.Values.Where(x => !x.IsAssigned()).ToList();
        _assignedOfficeStaffList = staffs.Values.Where(x => x.IsAssigned()).ToList();
    }

    public void UpdateStaffs(Dictionary<string, StaffArchiveData> staffs)
    {
        _noOfficeStaffList = staffs.Values.Where(x => !x.IsAssigned()).ToList();
        _assignedOfficeStaffList = staffs.Values.Where(x => x.IsAssigned()).ToList();

        isDirty = true;
    }

    public List<StaffArchiveData> GetNoOfficeStaffList()
    {
        return _noOfficeStaffList;
    }

    public List<StaffArchiveData> GetAssignedOfficeStaffList()
    {
        return _assignedOfficeStaffList;
    }
}