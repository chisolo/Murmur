
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class LoadingView : PopupUIBaseCtrl
{
    public class Args : PopupUIArgs
    {
        public float timeout;
    }
    public static string PrefabPath = "Assets/Res/UI/Prefab/Loading/LoadingView.prefab";
    private float _timeout;
    public override void Init(PopupUIArgs arg)
    {
        _timeout = ((Args)arg).timeout;

        if (_timeout > 0) {
            StartCoroutine(TimeoutCoroutine());
        }
    }

    void Start()
    {
        EventModule.Instance.Register(EventDefine.HideLoading, OnHideLoadingEvent);
    }

    void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.HideLoading, OnHideLoadingEvent);
    }

    private void OnHideLoadingEvent(Component sender, System.EventArgs e)
    {
        Hide();
    }

    IEnumerator TimeoutCoroutine()
    {
        yield return new WaitForSeconds(_timeout);
        Hide();
    }
}
