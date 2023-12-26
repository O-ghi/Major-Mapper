using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System.IO;
using System;

public class GameLauncher : MonoBehaviour
{
    public static bool launcherFinish = false;
    private Dictionary<string, string> m_logConfig;

    private float SyncInterval;
    private static bool isCreateHandle;
    void Awake()
    {
        SyncInterval = 0.2f;
    }
    void Start()
    {
#if UNITY_EDITOR
        Caching.ClearCache();
#endif
        //
        if (!isCreateHandle)
        {
            isCreateHandle = true;
            this.createHandleEvent();

        }

        TextAsset configAsset = Resources.Load<TextAsset>("GameUpdaterConfig");
        m_logConfig = JsonMapper.ToObject<Dictionary<string, string>>(configAsset.text);

        InitGame();


        ConfigManager.CreateInstance();
        //CoroutineManager.Singleton.startCoroutine(doDelayConfig());
        QuickVarList varlist = QuickVarList.Get();
        varlist.AddInt((int)LoadingEnum.Config);
        varlist.AddString(m_logConfig["10008"]);

        //PanelManager.OpenPanel("LoadingPanel", UILAYER.HIGH, varlist);


    }
    //private IEnumerator doDelayConfig()
    //{
    //    yield return null;
    //    yield return null;
    //    ConfigManager.Instance.DelayInit();

    //}
    //Duong add gameoject handle event from core game
    private void createHandleEvent()
    {
        GameObject gameObject = new GameObject();
        gameObject.name = "EventDispatcherHandle";
        //gameObject.AddComponent<EventDispatcherHandleFunction>();
        //gameObject.AddComponent<EventDispatcherHandleObject>();
        DontDestroyOnLoad(gameObject);

        //

        GameObject gameObject2 = new GameObject();
        gameObject2.name = "CoroutineManager";
        gameObject2.AddComponent<CoroutineManager>();
        DontDestroyOnLoad(gameObject2);


    }

    private void InitGame()
    {

        GameObject assetManager = new GameObject();
        assetManager.AddComponent<AssetLoadManager>();

        PanelManager.CreateInstance();
        PanelManager.Instance.DelayInit();

        //load URPAssetsSetting
        GameObject.Instantiate(AssetLoadManager.LoadAsset<GameObject>("urpassetssetting", "urpassetssetting"));
        //load shaderlist
        GameObject.Instantiate(AssetLoadManager.LoadAsset<GameObject>("shader_list", "shaderlist"));

    }

    #region 五个客户端的限制
    private bool ClientMaxCount()
    {
        //int count = 0;
        //var info = System.Diagnostics.Process.GetProcesses();
        //for (int i = 0; i < info.Length; i++)
        //    if ("Nhat Mong Giang Ho" == info[i].ProcessName)
        //        ++count;

        //if (count > 5)
        //    return true;
        return false;
    }

    private bool m_Exit = false;
    IEnumerator DealExit()
    {
        while (!m_Exit)
            yield return null;
        Application.Quit();
    }
    #endregion

    private void DoNext()
    {
        InputManager.CreateInstance();
        
        //Caching.maximumAvailableDiskSpace = 100 * 1000 * 1000;
#if UNITY_EDITOR
        Application.targetFrameRate = 100000;
#else
        Application.targetFrameRate = 30;
#endif
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //Debuger.Log("Pass Gamelauncher");
        //Shader.WarmupAllShaders();
#if DEBUG_SKILL_SHPERE
		DebugShowSkillSphere.CreateInstance();
#endif

#if UNITY_EDITOR
        //GameObject commandGo = new GameObject("Command");
        //GameObject.DontDestroyOnLoad(commandGo);

        //JumpCommandGUI commandGUI = commandGo.AddComponent<JumpCommandGUI>();
        //commandGUI.verticalPos = 0;
#endif
    }

    //void Start()
    //   {

    //   }

    void Update()
    {
        if (ConfigManager.isDone && launcherFinish == false)
        {
            launcherFinish = true;

            DoNext();
        }
        else
        {
            if (launcherFinish )
            {

            }
        }
    }

    void OnDestroy()
    {
    }
}
