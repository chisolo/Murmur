using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Lemegeton;

public class StaffHireView : MonoBehaviour
{

    /// <summary>
    /// 每次点击调整的幅度
    /// </summary>
    private const float AdjustRange = 0.05f;

    [SerializeField]
    private AssetReference _staffItemReference;
    [SerializeField]
    private Text _offerSalary;
    [SerializeField]
    private Transform _staffRoot;
    [SerializeField]
    private CenterOnChild _scrollView;
    [SerializeField]
    private GameObject _probabilityTextRoot;
    [SerializeField]
    private Text _probabilityText;
    [SerializeField]
    private Text _refreshTimeText;
    [SerializeField]
    private Text _refreshCountText;
    [SerializeField]
    private Button _refreshBtn;
    [SerializeField]
    private Button _officeBtn;
    [SerializeField]
    private Text _officeBtnText;

    [SerializeField]
    private List<GameObject> _rejectObjs;
    [SerializeField]
    private List<GameObject> _acceptObjs;
    [SerializeField]
    private List<GameObject> _offerObjs;

    private StaffItem _staffItemPrefab;


    private CandidateData _candidateData;
    private CandidateArchiveData _showCandidate => _candidateData.ShowCandidate;

    private long _lastTime;

    private void Start()
    {
        var handle = _staffItemReference.LoadAssetAsync<GameObject>();
        handle.Completed += (x) => {
            _staffItemPrefab = x.Result.GetComponent<StaffItem>();
        };

        _refreshTimeText.text = FormatUtil.FormatTimeLong(_candidateData.CandidateRefreshTime - NtpModule.Instance.UtcNowSeconds);
        _lastTime = NtpModule.Instance.UtcNowSeconds - _candidateData.CandidateRefreshTime;
    }

    private void OnDestroy()
    {
        _staffItemReference.ReleaseAsset();
        _candidateData = null;
    }

    private void Update()
    {
        if (_candidateData == null) {
            return;
        }

        var lastTime = NtpModule.Instance.UtcNowSeconds - _candidateData.CandidateRefreshTime;
        if (lastTime != _lastTime) {
            _refreshTimeText.text = FormatUtil.FormatTimeLong(_candidateData.CandidateRefreshTime - NtpModule.Instance.UtcNowSeconds);
            _lastTime = lastTime;
        }
    }

    public void Init(CandidateData candidateData)
    {
        _candidateData = candidateData;

        SetRefreshButton();


        StartCoroutine(InitCo());
    }

    private IEnumerator InitCo()
    {
        yield return new WaitUntil(() => _staffItemPrefab != null);
        _scrollView.OnFinished.AddListener(OnCenter);

        CreateList();
    }

    public void RefreshView()
    {
        foreach (Transform child in _staffRoot) {
            Destroy(child.gameObject);
        }

        SetRefreshButton();

        CreateList();
    }

    private void SetRefreshButton()
    {
        _refreshCountText.text = string.Format("{0}/{1}", _candidateData.RefreshCount, ConfigModule.Instance.Common().default_candidate_refresh_count);
        if (_candidateData.RefreshCount <= 0) {
            _refreshBtn.interactable = false;
        }
    }

    private void CreateList()
    {
        var candidates = _candidateData.Candidates();
        foreach (var candidata in candidates) {
            var obj = Instantiate(_staffItemPrefab, _staffRoot);
            obj.Init(candidata);
        }
        _scrollView.RefreshView();

        _candidateData.ShowCandidate = candidates[0];
        _candidateData.OfferSalary = _showCandidate.offerSalary;
        ShowCandidateInfo();
    }

    private void OnCenter(int id)
    {
        //AppLogger.Log("Oncner " + id);
        var candidates = _candidateData.Candidates();
        _candidateData.ShowCandidate = candidates[id];
        _candidateData.OfferSalary = _showCandidate.offerSalary;


        ShowCandidateInfo();
    }

    public void OfferUp()
    {
        _candidateData.OfferSalary += Mathf.CeilToInt(_showCandidate.salary * 0.05f);
        ShowCandidateInfo();
    }

    public void OfferDown()
    {
        _candidateData.OfferSalary -= Mathf.CeilToInt(_showCandidate.salary * 0.05f);
        _candidateData.OfferSalary = Mathf.Max(0, _candidateData.OfferSalary);
        ShowCandidateInfo();
    }

    private void ShowCandidateInfo()
    {
        _offerSalary.text = string.Format("{0}/h", _candidateData.OfferSalary.ToString());
        _probabilityText.text = GetProbabilityText(_candidateData.OfferSalary, _showCandidate.salary);

        switch((CandidateOfferStatus)_showCandidate.status) {
            case CandidateOfferStatus.None:
                _officeBtnText.text = StaffText.MakeOffer.Locale();
                _probabilityTextRoot.SetActiveIfNeed(true);
                _offerObjs.ForEach(x => x.SetActiveIfNeed(true));
                _rejectObjs.ForEach(x => x.SetActiveIfNeed(false));
                _acceptObjs.ForEach(x => x.SetActiveIfNeed(false));
                break;
            case CandidateOfferStatus.OfferAccepted:
                _officeBtnText.text = StaffText.OfferAccepted.Locale();
                _probabilityTextRoot.SetActiveIfNeed(false);
                _offerObjs.ForEach(x => x.SetActiveIfNeed(false));
                _rejectObjs.ForEach(x => x.SetActiveIfNeed(false));
                _acceptObjs.ForEach(x => x.SetActiveIfNeed(true));
                break;
            case CandidateOfferStatus.OfferRejected:
                _officeBtnText.text = StaffText.OfferRejected.Locale();
                _offerObjs.ForEach(x => x.SetActiveIfNeed(false));
                _rejectObjs.ForEach(x => x.SetActiveIfNeed(true));
                _acceptObjs.ForEach(x => x.SetActiveIfNeed(false));
                break;
        }
    }

    public string MakeOffer()
    {
        if ((CandidateOfferStatus)_showCandidate.status != CandidateOfferStatus.None) {
            AppLogger.LogError("status error!!");
            return string.Empty;
        }

        var result = StaffModule.Instance.MakeOffer(_showCandidate.id, _candidateData.OfferSalary, _showCandidate.salary);
        ShowCandidateInfo();

        if (result) {
            return _showCandidate.id;
        }
        return string.Empty;
    }

    private string GetProbabilityText(int offerSalary, int salary)
    {
        if (offerSalary < salary * HireProbability.ProbabilityZero) {
            return StaffText.AlmostZero.Locale();
        } else if (offerSalary >= salary * HireProbability.ProbabilityZero && offerSalary < salary * HireProbability.ProbabilityLow) {
            return StaffText.Low.Locale();
        } else if (offerSalary >= salary * HireProbability.ProbabilityLow && offerSalary < salary * HireProbability.ProbabilityMedium) {
            return StaffText.Medium.Locale();
        } else if (offerSalary >= salary * HireProbability.ProbabilityMedium && offerSalary < salary * HireProbability.ProbabilityHigh) {
            return StaffText.High.Locale();
        }

        return StaffText.VeryHigh.Locale();
    }
}