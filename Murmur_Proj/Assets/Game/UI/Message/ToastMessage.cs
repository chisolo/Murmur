
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Lemegeton;

public class ToastMessage : PopupUIBaseCtrl
{
    public class Args : PopupUIArgs
    {
        public string msg;
    }
    public static string PrefabPath = "Assets/Res/UI/Prefab/Popup/ToastMessage.prefab";

    [SerializeField] Text _msgText;

    public static void Show(string msg)
    {
        var args = new Args();
        args.msg = msg;
        UIMgr.Instance.OpenPopUpPanel(PrefabPath, args);
    }

    public override void Init(PopupUIArgs arg)
    {
        var param = arg as Args;
        _msgText.text = param.msg;

        StartCoroutine(TimeoutCoroutine());
    }

    IEnumerator TimeoutCoroutine()
    {
        yield return new WaitForSeconds(1);
        Hide();
    }
}
