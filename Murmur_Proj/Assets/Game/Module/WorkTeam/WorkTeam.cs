using System;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;


public class WorkTeam
{
    public List<WorkTeamData> teams;

    private Action<string> _onComplete;
    public WorkTeam()
    {
        teams = new List<WorkTeamData>();
    }
    public int Capacity => teams.Count;

    public void SetComplete(Action<string> onComplete)
    {
        _onComplete = onComplete;
    }
    public bool AddTeam(string teamId)
    {
        if(teams.Find(e => e.id == teamId) != null) return false;

        teams.Add(new WorkTeamData(teamId));
        return true;
    }
    public bool HasTeam(string teamId)
    {
        return teams.Find(e => e.id == teamId) != null;
    }
    public void Process()
    {
        var now = NtpModule.Instance.UtcNowSeconds;
        foreach(var team in teams) {
            if(!string.IsNullOrEmpty(team.owner) && team.endTime > 0 && team.endTime <= now) {
                _onComplete?.Invoke(team.owner);
            }
        }
    }
    public WorkTeamData GetFreeTeam()
    {
        return teams.Find(e => string.IsNullOrEmpty(e.owner));
    }

    public WorkTeamData GetTeamData(string owner)
    {
       return teams.Find(e => e.owner == owner);
    }
    public int WorkingTeamCount()
    {
        int count = 0;
        foreach(var team in teams) {
            if(!string.IsNullOrEmpty(team.owner)) {
                ++count;
            }
        }
        return count;
    }

    public int GetFreeTeamCount()
    {
        int count = 0;
        foreach(var team in teams) {
            if(string.IsNullOrEmpty(team.owner)) {
                ++count;
            }
        }
        return count;
    }

    public bool HasEmpty()
    {
        return teams.Find(e => string.IsNullOrEmpty(e.owner)) != null;
    }

    public bool IsTeamMax()
    {
        return teams.Count == ConfigModule.Instance.Common().team_max;
    }
    public void WorkDone(string owner)
    {
        var team = teams.Find(e => e.owner == owner);
        team?.Done();
    }
}
