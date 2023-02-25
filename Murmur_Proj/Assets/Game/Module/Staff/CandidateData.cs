using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lemegeton;

public class CandidateData
{
    private List<CandidateArchiveData> _candidates;

    private int _offerSalary;

    public int OfferSalary
    {
        get {
            return _offerSalary;
        }
        set {
            _offerSalary = value;
        }
    }

    public long CandidateRefreshTime;
    public int RefreshCount;

    public CandidateArchiveData ShowCandidate;

    public CandidateData(List<CandidateArchiveData> candidates, long candidateRefreshTime, int refreshCount)
    {
        _candidates = candidates;
        CandidateRefreshTime = candidateRefreshTime;
        RefreshCount = refreshCount;
    }

    public List<CandidateArchiveData> Candidates()
    {
        return _candidates;
    }

    public void UpdateCandidates(List<CandidateArchiveData> candidates)
    {
        _candidates.Clear();
        _candidates.AddRange(candidates);
    }
}