using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class GameLauncherCore : MonoBehaviour
{
    public GameObject gameUpdate;
    //兼容版本号
    //2---2019年3月6日添加GameSDK添加剪切板功能
    //3---2019年3月22日添加蕉玩ui统计接口JWInterface.OnWindowOpen/ABDownLoader.maxCoroutineNum改为可修改
    //4---2019年4月11日ABDownLoader.maxCoroutineNum改为可修改
    //5---2019年4月19日添加接口PathUtil.ClearConfigAndForceAndBack
    //6---2019年5月05日常用Code Bind ILRunTime
    //7---2019年5月06日添加开关ABDownLoader.WriteFileInBackThread
    //8---2019年5月10日添加ILRunTime性能优化处理 FGUI自定义组件只需注册一个，网络消息监听方式添加，配置表可按需加载
    //9---2019年5月22日添加specialMarker判断过审
    //10---2019年6月4日章鱼ios serverList通过渠道id拉取
    //11---2019年6月26日Unity升级到2017.4.23/添加函数UIObjectFactory.AddPackge(byte[],string,func)
    //12---2019年7月18日添加GameSDK.GetStringValue接口
    //13---2019年8月3日baseMessage消息长度异常处理
    //14---2019年8月21日热更目录结构修改
    //15---2019年9月25日 SceneManager.LoadScene小游戏接口
    //16---2019年9月30日 添加abcManager
    //17---2019年11月16日 使用接口render.materials = mats;    
    public static int CompatibleFlag = 17;
    public bool isNoUpdate;
    public bool debugMode;
    public bool debugModeILruntime;

    //#if UNITY_EDITOR
    public bool showProjectPath;
    public static System.Reflection.Assembly dllAssembly;
    public static ILRuntime.Runtime.Enviorment.AppDomain ilrtApp;
    //#endif

    void Awake()
    {
        Debug.Log(Time.realtimeSinceStartup + ">GameLaucher.awake>" + CompatibleFlag);

#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;
#endif
        Debuger.Enabled = true;
        Application.logMessageReceived += Debuger.OnSystemLog;

        //ResDepManager.Singleton.LoadDeps();


        initManager();
        //Tam thoi pass hotupdate
        if (debugMode)
            launchGame();
        else
        {
            StartCoroutine(updateRes());
        }

        //加载配置表
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Debuger.Log("GameLaucher.awake.end");
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (showProjectPath || !Application.isPlaying)
        {
            var style = new GUIStyle();
            style.fontSize = 22;
            style.normal.textColor = Color.white;
            GUILayout.Label(Application.dataPath, style);
        }
    }
#endif

    private void initManager()
    {
        gameObject.AddComponent<CoroutineManager>();
    }

    private IEnumerator updateRes()
    {
        //Duong remove tat asset cu lan dau dang nhap
        if (PlayerPrefs.GetInt("FirstJson", -999) == -999)
        {
            PathUtil.ClearConfigAndForceAndBack();
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("FirstJson", 1);
            PlayerPrefs.Save();
            //UnityEditor.EditorApplication.isPaused = true;
            yield return new WaitForSeconds(0.2f);
        }

        StartupManager.Singleton.Start(() => launchGame());
    }

    private void launchGame()
    {
        gameUpdate.SetActive(true);

        PathUtil.codeOffset = 123;
        PathUtil.codeKey = "GameDataManager.Bean";
#if UNITY_EDITOR
        StartCoroutine(launchEditor());
#elif ENABLE_IL2CPP
        StartCoroutine(launchIL2CPP());
#else
        StartCoroutine(launchMono());
#endif

    }

    private bool checkExistDllBundle()
    {
        //check exist in force
        var path = PathUtil.GetForceABPath(PathUtil.DllScriptBundleName);
        if (System.IO.File.Exists(path))
            return true;
        //check exist in back
        path = PathUtil.GetBackABPath(PathUtil.DllScriptBundleName);
        if (System.IO.File.Exists(path))
            return true;

        return false;
    }


#if UNITY_EDITOR
    private void OnDestroy()
    {
        bool useILRT = UnityEditor.EditorPrefs.GetBool("ILRuntime_Editor_Enable", true);
        if (useILRT && ilrtApp != null)
            ilrtApp.DebugService.StopDebugService();
    }

    private IEnumerator launchEditor()
    {
#if !Main
        bool useILRT = UnityEditor.EditorPrefs.GetBool("ILRuntime_Editor_Enable", true);
        if (useILRT)
        {
            //ilruntime
            float time = Time.realtimeSinceStartup;
            var dll = System.IO.File.ReadAllBytes(PathUtil.EditorDllPath);
            var pdb = System.IO.File.ReadAllBytes(PathUtil.EditorPdbPath);
            var appdomain = new ILRuntime.Runtime.Enviorment.AppDomain(ILRuntime.Runtime.ILRuntimeJITFlags.JITOnDemand);
#if DEBUG && !NO_PROFILER && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS)
            appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            var fs = new System.IO.MemoryStream(dll);
            var ps = new System.IO.MemoryStream(pdb);
            appdomain.LoadAssembly(fs, ps, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            //appdomain.LoadAssembly(fs, ps, new PdbReaderProvider());
            ILRuntime.ILScriptBinder.Bind(appdomain);
            appdomain.DebugService.StartDebugService(56000);
            Debug.Log("init ILRunTime time > " + (Time.realtimeSinceStartup - time) + "s");
            //Debuger.Log("GameManagerHotupdate is " + (appdomain.GetType("GameManagerHotupdate")!=null) );
            ilrtApp = appdomain;
            if (debugModeILruntime)
            {
                UnityEditor.EditorApplication.isPaused = true;
            }
            appdomain.Invoke("GameManager", "Initialize", null, gameUpdate, debugMode);
            Debug.Log("editor init ILRunTime time > " + (Time.realtimeSinceStartup - time) + "s");
        }
        else
        {
            //dll
            var dll = System.IO.File.ReadAllBytes(PathUtil.EditorDllPath);
            float time = Time.realtimeSinceStartup;
            var assembly = System.Reflection.Assembly.Load(dll);
            Debug.Log("editor init dll time > " + (Time.realtimeSinceStartup - time) + "s");

            var type = assembly.GetType("GameManager");
            var met = type.GetMethod("Initialize", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            met.Invoke(null, new object[] { gameUpdate, debugMode });
            dllAssembly = assembly;
        }
#endif
        yield return null;
    }
#elif ENABLE_IL2CPP
       private IEnumerator launchIL2CPP()
    {

        if (isNoUpdate || VersionConfig.IsInAuditing 
            || checkExistDllBundle() == false)
        {
#if !Main
            Debug.Log("launchIL2CPP>" + Time.realtimeSinceStartup);
            var bytes = AbcManager.Singleton.GetDll();
            if (bytes == null)
                bytes = PathUtil.LoadBytesUnZip(PathUtil.DllScriptBundleName);
            float time = Time.realtimeSinceStartup;

            var appdomain = new ILRuntime.Runtime.Enviorment.AppDomain(ILRuntime.Runtime.ILRuntimeJITFlags.JITOnDemand);
            var fs = new System.IO.MemoryStream(bytes);
            appdomain.LoadAssembly(fs);

            ILRuntime.ILScriptBinder.Bind(appdomain);
            ilrtApp = appdomain;
            Debug.Log("init ILRunTime time > " + (Time.realtimeSinceStartup - time) + "s");
            appdomain.Invoke("GameManager", "Initialize", null, gameUpdate, debugMode);
#endif
        yield return null;
        }
        else
        {
#if !Main
            ABLoader.Singleton.LoadAssetBundle(PathUtil.DllScriptBundleName, (s, ab) =>
            {
                if (ab == null)
                {
                    Debug.LogError("代码ab加载失败");
                    return;
                }

                var ta = ab.LoadAsset(s) as TextAsset;
                if (ta == null || ta.bytes == null)
                {
                    Debug.LogError("代码ab中没有东西");
                    return;
                }

                Debug.Log("launchIL2CPP>" + Time.realtimeSinceStartup);
                var bytes = ta.bytes;
                PathUtil.Decode(bytes);
                float time = Time.realtimeSinceStartup;
                var appdomain = new ILRuntime.Runtime.Enviorment.AppDomain(ILRuntime.Runtime.ILRuntimeJITFlags.JITOnDemand);
                var fs = new System.IO.MemoryStream(bytes);
                appdomain.LoadAssembly(fs);

                ILRuntime.ILScriptBinder.Bind(appdomain);
                ilrtApp = appdomain;
                Debug.Log("init ILRunTime time > " + (Time.realtimeSinceStartup - time) + "s");
                appdomain.Invoke("GameManager", "Initialize", null, gameUpdate, debugMode);
                ab.Unload(true);
            });
#endif
            yield return null;
        }
       
    }
#else
    private IEnumerator launchMono()
    {

        if (isNoUpdate /*|| VersionConfig.IsInAuditing */
            || checkExistDllBundle() == false)
        {
#if !Main
            Debug.Log("launchMono>" + Time.realtimeSinceStartup);
            var bytes = AbcManager.Singleton.GetDll();
            if (bytes == null)
                bytes = PathUtil.LoadBytesUnZip(PathUtil.DllScriptBundleName);
            float time = Time.realtimeSinceStartup;
            var assembly = System.Reflection.Assembly.Load(bytes);
            Debug.Log("init dll time > " + (Time.realtimeSinceStartup - time) + "s");

            var type = assembly.GetType("GameManager");
            var met = type.GetMethod("Initialize", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            met.Invoke(null, new object[] { gameUpdate, debugMode });
#endif
            yield return null;
        }
        else
        {
#if !Main
            //dll更新
            ABLoader.Singleton.LoadAssetBundle(PathUtil.DllScriptBundleName, (s, ab) => {
                if (ab == null)
                {
                    Debug.LogError("代码ab加载失败");
                    return;
                }

                var ta = ab.LoadAsset(s) as TextAsset;
                if (ta == null || ta.bytes == null)
                {
                    Debug.LogError("代码ab中没有东西");
                    return;
                }

                Debug.Log("launchMono>" + Time.realtimeSinceStartup);
                var bytes = ta.bytes;
                PathUtil.Decode(bytes);
                float time = Time.realtimeSinceStartup;
                var assembly = System.Reflection.Assembly.Load(bytes);
                Debug.Log("init dll time > " + (Time.realtimeSinceStartup - time) + "s");

                var type = assembly.GetType("GameManager");
                var met = type.GetMethod("Initialize", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                met.Invoke(null, new object[] { gameUpdate, debugMode });
                ab.Unload(true);
            });
#endif
            yield return null;
        }
       
    }
#endif
}