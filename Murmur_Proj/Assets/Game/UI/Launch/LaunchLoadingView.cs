using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class LaunchLoadingView : MonoBehaviour
{
    [SerializeField] Image _progressBar;
    [SerializeField] SceneCtrl _sceneCtrl;
    public float _progress = 0f;
    void Awake()
    {
        EventModule.Instance.Register(EventDefine.LoadingProgress, OnLoadingProgressEvent);
        _progress = 0;
        _progressBar.fillAmount = 0;
    }
    void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.LoadingProgress, OnLoadingProgressEvent);
    }
    private void OnLoadingProgressEvent(Component sender, EventArgs e)
    {
        LoadingProgressEventArgs args = e as LoadingProgressEventArgs;

        _progress += args.progress;
        if(_progress > 0.999f) {
            _progress = 1;
            _progressBar?.DOFillAmount(_progress, 0.2f);
            _sceneCtrl.OnLoaded();
        } else {
            _progressBar?.DOFillAmount(_progress, 0.2f);
        }
    }
}
