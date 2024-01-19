using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System;

public class SceneManager : ManagerTemplate<SceneManager>
{
    private static string lastSceneBundleName;
    private TaskCoreCfgLoader taskCoreCfgLoader;
    protected override void InitManager()
    {
        ManagerLogic.Singleton.AddManagerUpdate(this.GetType(), Update);

        taskCoreCfgLoader = ConfigManager.Instance.Get<TaskCoreCfgLoader>();
    }

    private List<Type> sceneTypes = new List<Type>()
    {
        typeof(CampingScene)
    };

    

    protected override void Update()
    {
        base.Update();
    }

    public static void ChangeScene(int taskId)
    {
        if (SceneDataManager.currentScene != null && SceneDataManager.currentScene._taskCoreCfg.ID == taskId)
        {
            return;
        }
        AssetLoadManager.OnDispose();

        //Bật event thoát scene
        

        Instance.StartCoroutine(Instance.ReadyChangeScene(taskId));

    }

    private IEnumerator ReadyChangeScene(int taskId)
    {
        //GameEventManager.RaiseEvent(GameEventTypes.ExitScene);

        UnityEngine.SceneManagement.SceneManager.LoadScene("empty");
        if (!string.IsNullOrEmpty(lastSceneBundleName))
        AssetLoadManager.UnLoadAssetBundle(lastSceneBundleName);

        yield return ChangeSceneInternal(taskId);
    }
    private static float m_progress;

    private IEnumerator ChangeSceneInternal(int taskId)
    {

        //GameEventManager.EnableEventFiring = false; // dùng để ngăn không cho gọi event khi đang chuyển scene

        var config = taskCoreCfgLoader.GetCfg(taskId);

        string sceneName = "";

        if (config == null)
        {
            Debug.Log("Không có scene này");
        }

        sceneName = config.Scene;
        Debug.Log("_________________change Scene|" + sceneName + "_________SceneID| " + taskId);
#if UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
        //SceneGoodsCfgLoader sceneGoodsCfgLoader = ConfigManager.Get<SceneGoodsCfgLoader>();
        // bool isExsitSceneGoods = sceneGoodsCfgLoader.isExistsSceneGoodS(sceneName);
        // if (isExsitSceneGoods)
        // {
        // sceneName = sceneName + "_goods";
        // }
#endif
        string sceneBundleName = string.Format("scenes/{0}", sceneName.ToLower());
        lastSceneBundleName = sceneBundleName;

        var m_operation = AssetLoadManager.LoadLevelAsync(sceneBundleName, sceneName, false);

        m_progress = 0;

        // 10s
        //Duong them kiem tra config da load chua
        while (!ConfigManager.isAfterLoadDone)
        {
            m_progress += (Time.deltaTime / 40);
            if (m_progress > 0.3f)
                m_progress = 0.3f;
            yield return null;
        }
        //
        //step 1 7s
        while (!m_operation.IsDone())
        {
            m_progress += (Time.deltaTime / 10);
            if (m_progress > 0.7f)
                m_progress = 0.7f;
            yield return null;
        }

        //GameEventManager.EnableEventFiring = true;

        // 等待主角模型加载完毕
        //yield return StartCoroutine(WaitForMainPlayerLoading());
        //Duong merge code coroutine

        while (true)
        {
            
            var scene = System.Activator.CreateInstance(typeof(CampingScene), config.Scene, config) as SceneBase;
            if (scene == null)
            {
                Debug.Log("Chưa có Scene " + "Name");
            };
            using (QuickVarList varlist = QuickVarList.Get())
            {
                varlist.AddObject(config);
                
                SceneDataManager.Instance.SetScene(scene, varlist);
            }
            
            yield return null;
            // step 2 1.5s
            m_progress += (Time.deltaTime / 10);
            m_progress = Mathf.Min(0.85f, m_progress);

            var playerEntity = EntityManager.GetMainPlayerEntity();
            //Debuger.Log("playerEntity is: " + (playerEntity != null));
            //Debuger.Log("Ready is: " + playerEntity.Ready  + " ||| SkillActionLoaded: "+ playerEntity.SkillActionLoaded);
            if (playerEntity == null)
            {
                continue;

            } else
            {
                break;
            }

        }

        m_progress = 0.9f;

        //yield return StartCoroutine(PreloadEffects());


        m_progress = 1;

        

        yield return null;
        



        using (QuickVarList varlist = QuickVarList.Get())
        {
            varlist.AddInt(taskId);
            //GameEventManager.RaiseEvent(GameEventTypes.EnterScene, varlist);
        }

        
        Resources.UnloadUnusedAssets();
    }
}
