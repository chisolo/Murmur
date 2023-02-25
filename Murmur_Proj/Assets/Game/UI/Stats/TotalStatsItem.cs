using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class TotalStatsItem : MonoBehaviour
{
    [SerializeField] Transform _bigStatTrans;
    [SerializeField] Transform _statContentTrans;
    [SerializeField] GameObject _statItemPrefab;
    private string _service;
    public void Show(string service)
    {
        _service = service;
        if(GameUtil.BuildingTotalServiceStats.TryGetValue(_service, out var stats)) {
            var bigItem = GameObject.Instantiate(_statItemPrefab, _bigStatTrans).GetComponent<StatsItem>();
            bigItem.Init(_service, stats[0]);
            for(int i = 1; i < stats.Count; ++i) {
                var statItem = GameObject.Instantiate(_statItemPrefab, _statContentTrans).GetComponent<StatsItem>();
                statItem.Init(_service, stats[i]);
                statItem.transform.SetAsLastSibling();
            }
        }
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
