using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System;


public class LoadingPanel : UIBase
{
    public static LoadingPanel Singleton;
    //private LoadingUI _loadingUI;

    //public Slider slider_progress1;
    //public Slider slider_progress2;
    //public Slider slider_progress3;
    //public GameObject sliderHandle1;
    //public GameObject sliderHandle2;
    //public Text text_progress;
    //public Text prompt_label;
    //public Text msg;
    float progress = 0;
    private LoadingEnum m_loadingEnum = LoadingEnum.None;

    //public Image bg;

    private string assetBundleName = "";


    protected override void Init()
    {
        //_loadingUI = new LoadingUI();
        //_loadingUI.LoadingPanel = gameObject.transform;
        //_loadingUI.Init();
    }

    protected override void Register()
    {

    }

    protected override void UnRegister()
    {

    }

    protected override void Destroy()
    {
        base.Destroy();
        //AssetBundleManager.UnloadSyncLoadedBundle(assetBundleName);
        AssetLoadManager.UnLoadAssetBundle(assetBundleName);
    }

    protected override void OnClose()
    {
        base.OnClose();
        AssetLoadManager.UnLoadAssetBundle(assetBundleName);
    }

    private void OnProgress(float progress)
    {
        //if (progress <= 0.5)
        //{
        //    _loadingUI.slider_progress1.value = progress * 2;
        //    _loadingUI.slider_progress2.value = 0;
        //    SetActive(_loadingUI.sliderHandle1, true);
        //    SetActive(_loadingUI.sliderHandle2, false);
        //}
        //else
        //{
        //    _loadingUI.slider_progress2.value = progress * 2 - 1;
        //    _loadingUI.slider_progress1.value = 1;
        //    SetActive(_loadingUI.sliderHandle1, false);
        //    SetActive(_loadingUI.sliderHandle2, true);
        //}

        //_loadingUI.slider_progress3.value = progress;
        //SetTextFormat(_loadingUI.text_progress, "{0}%", (int)(progress * 100));
    }

    private string PromptLoad()
    {
        System.Random range = new System.Random();
        int rangeCount = range.Next(1, 21);

        return "" + rangeCount;
    }

    protected override void OnStart()
    {
        m_loadingEnum = (LoadingEnum)Data.GetInt(0);

        //_loadingUI.msg.text = Data.GetString(1);

        //SetText(_loadingUI.prompt_label, PromptLoad());

        //SetRandomLoadingText();
    }

    protected override void OnLateUpdate()
    {
        switch (m_loadingEnum)
        {
            case LoadingEnum.Scene: DoSceneLoading(); break;
            case LoadingEnum.Config: DoConfigLoading(); break;
        }
    }

    private void DoSceneLoading()
    {
        progress = 50f;
        OnProgress(progress);
    }

    private void DoConfigLoading()
    {
        progress = ConfigManager.progress;
        OnProgress(progress);
    }

    private void SetRandomLoadingText()
    {
        
    }

    internal void ShowTip(string downloadNewVersion)
    {

    }

    internal void SetProgress(float progress)
    {

    }
}

public enum LoadingEnum
{
    None,
    Scene,
    Config,
}
