using System;
using System.Collections;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.UI;

public class TimeProgressBar : MonoBehaviour
{

    [SerializeField]
    private Text _timeText;
    [SerializeField]
    private Image _progressBar;

    private long _endTime => _timeData.EndTime;
    private long _duration => _timeData.Duration;

    private ITimeProgressBarData _timeData;

    private long _lastTime = -1;

    private Action _onTimeUpdated;
    private Action _onTimeEnd;

    private bool _inited = false;
    private bool _isEnd = false;

    public void Init(ITimeProgressBarData timeData, Action onTimeUpdated, Action onTimeEnd)
    {
        _timeData = timeData;

        _onTimeUpdated = onTimeUpdated;
        _onTimeEnd = onTimeEnd;

        var remain = _endTime - NtpModule.Instance.UtcNowSeconds;
        UpdateTime(remain);
        _isEnd = false;
        _inited = true;
    }

    public void UpdateTime(long remain)
    {
        _progressBar.fillAmount = 1 - (float)remain / _duration;
        _timeText.text = FormatUtil.FormatTimeLong(remain);
    }

    public void OnUpdate()
    {
        if (_isEnd || !_inited) return;
        var remain = _endTime - NtpModule.Instance.UtcNowSeconds;
        if (_lastTime != remain) {
            UpdateTime(remain);
            _onTimeUpdated?.Invoke();
            _lastTime = remain;
        }

        if (_duration <= 5000) {
            // update every frame
            var remain2 = _endTime - NtpModule.Instance.UtcNowMillSeconds * 0.001d;
            _progressBar.fillAmount = (float)(1 - remain2 / _duration);
        }

        if (remain <= 0) {
            _isEnd = true;
            _onTimeEnd?.Invoke();
        }

    }

    void Update()
    {
        OnUpdate();
    }
}
