using System.Collections.Generic;
using Lemegeton;

public class TalentApplyMoudleSample : Singleton<TalentApplyMoudleSample>, ITalentApply
{
    public void ApplyNewTalent(string id, string type, float value)
    {
        AppLogger.Log("ApplyNewTalent sample");
    }
}