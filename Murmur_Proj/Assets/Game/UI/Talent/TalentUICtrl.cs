using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lemegeton;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public class TalentUICtrl : PopupUIBaseCtrl
{
    public static string PrefabPath = "Assets/Res/UI/Prefab/Talent/TalentUI.prefab";
    public class Args : PopupUIArgs
    {
        public string talentId;

        public Args() {}
        public Args(string talentId)
        {
            this.talentId = talentId;
        }
    }

    [SerializeField]
    private TalentListView[] _listViewList;
    [SerializeField]
    private Tab[] _tabList;

    private List<string> tabNameList = new List<string>();

    public override void Init(PopupUIArgs arg)
    {
        var param = arg as Args;

        var talentDatas = TalentModule.Instance.GetTalentDatas();

        var lastTab = talentDatas[0].Cfg.tab;
        var i = 0;
        var lastSection = talentDatas[0].Cfg.section;
        tabNameList.Add(lastTab);

        var talentShowList = new List<TalentData>();

        foreach (var talentData in talentDatas) {
            if (talentData.Cfg.tab != lastTab) {
                _listViewList[i].Init(talentShowList);
                talentShowList.Clear();
                i++;

                lastTab = talentData.Cfg.tab;
                tabNameList.Add(lastTab);
            }

            talentShowList.Add(talentData);
        }

        _listViewList[i].Init(talentShowList);

        if (!string.IsNullOrEmpty(param.talentId)) {
            ShowTalent(param.talentId);
        }
    }

    private void Start()
    {
        EventModule.Instance.Register(EventDefine.TalentOpen, OnTalentOpenEvent);
    }

    private void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.TalentOpen, OnTalentOpenEvent);
    }

    private void OnTalentOpenEvent(Component sender, System.EventArgs e)
    {
        var arg = e as TalentOpenEventArgs;
        ShowTab(arg.talentId);
    }

    private int ShowTab(string talentId, bool move = true)
    {
        var talentData = TalentModule.Instance.GetTalentData(talentId);

        var index = tabNameList.IndexOf(talentData.Cfg.tab);
        for (var i = 0; i < _tabList.Length; i++) {
            if (i == index) {
                _tabList[i].Toggle.isOn = true;
                if (move) _listViewList[i].GotoItem(talentId);
                return i;
            }
        }
        return -1;
    }

    private void ShowTalent(string talentId)
    {
        var tab = ShowTab(talentId, false);
        GotoItem(tab, talentId);

        // var action = DOTween.Sequence();
        // action.SetDelay(0.3f);
        // action.OnComplete(() => {
        //     if (tab != -1) {
        //         _listViewList[tab].GotoItem(talentId);
        //     }
        //     var arg = new TalentItemDetail.UIArgs();
        //     arg.talentId = talentId;
        //     UIMgr.Instance.Open(TalentItemDetail.PrefabPath, arg);
        // });
    }

    private async void GotoItem(int tab, string talentId)
    {
        await Task.Delay(300);
        if (tab != -1) {
            _listViewList[tab].GotoItem(talentId);
        }
        var arg = new TalentItemDetail.UIArgs();
        arg.talentId = talentId;
        UIMgr.Instance.OpenUI(TalentItemDetail.PrefabPath, arg, false, false);
    }

}