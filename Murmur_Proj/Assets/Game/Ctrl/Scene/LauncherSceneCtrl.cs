
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Lemegeton;

public class LauncherSceneCtrl : SceneCtrl
{
    [SerializeField] GameObject _eventSystem;
    void Awake()
    {
        Application.targetFrameRate = 60;
        EventModule.Instance.Init();
        AudioModule.Instance.Init();
        TraceModule.Instance.Init();
        SetupDevConsole();
    }

    void Start()
    {
        Load();
    }
    public async void Load()
    {
        using (LoadingProgressEventArgs args = LoadingProgressEventArgs.Get()) {
            args.progress = 0.1f;
            EventModule.Instance.FireEvent(EventDefine.LoadingProgress, args);
        }
        RandUtil.InitSeed();
        TimerModule.Instance.Init();
        await LocaleModule.Instance.Init();
        var ret = await ConfigModule.Instance.Init();
        if(ret) {
            ArchiveModule.Instance.Init();
            PlayerModule.Instance.Init();
            TalentModule.Instance.Init();
            BuildingModule.Instance.Init();
            PuppetModule.Instance.Init();
            StaffModule.Instance.Init();
            GachaModule.Instance.Init();
            QuestModule.Instance.Init();
            ShopModule.Instance.Init();
            EmergencyModule.Instance.Init();
            BuffModule.Instance.Init();
            AdModule.Instance.Init();
            TutorialModule.Instance.Init();
            GrowthFundModule.Instance.Init();
            OnInit();
        } else {
            // TODO 弹出重启提示框
            Debug.Log("Config Init failed!");
        }
    }
    public void OnInit()
    {
        BuildingModule.Instance.PreLoad();

        Destroy(_eventSystem);
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }
    public override void OnLoaded()
    {
        StartCoroutine(OnLoadCoroutine());
    }
    private IEnumerator OnLoadCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.UnloadSceneAsync(0);
        Resources.UnloadUnusedAssets();

        PlayerModule.Instance.OnLoad();
        ShopModule.Instance.OnLoad();
        TutorialModule.Instance.OnLoad();
        EmergencyModule.Instance.OnLoad();
        AdModule.Instance.OnLoad();

        EventModule.Instance.FireEvent(EventDefine.MainUIShow);
    }
    private void SetupDevConsole()
    {
#if IS_DEV
        var prefab = App.Dev.DevAssetsLoader.devAssets.IngameDebugConsolePrefab;
        if (prefab != null) {
            Instantiate(prefab);
        }
#endif
    }

    public void ShowDebugText()
    {
        Debug.Log("Debug!");
    }
}
