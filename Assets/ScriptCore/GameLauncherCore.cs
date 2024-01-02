using System;
using System.Collections;
using UnityEngine;

public class GameLauncherCore : MonoBehaviour
{
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

    public bool debugMode;
#if UNITY_EDITOR
    public static System.Reflection.Assembly dllAssembly;
    public static ILRuntime.Runtime.Enviorment.AppDomain ilrtApp;
#endif

    void Awake()
    {
        Debug.Log(Time.realtimeSinceStartup + ">GameLaucher.awake>" + CompatibleFlag);
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return;
        }
#endif

        Debuger.Enabled = true;
        Application.logMessageReceived += Debuger.OnSystemLog;

        initManager();

        if (debugMode)
            launchGame();
        else
            updateRes();

#if UNITY_ANDROID && !UNITY_EDITOR
        //if(ChannelManager.ChannelName == "GamiPlay")
        //    BuglyAgent.InitWithAppId("b898af8655");
        //else
        //    BuglyAgent.InitWithAppId("be56a8c87d"); //7.9换了新的id
        //BuglyAgent.EnableExceptionHandler();
        //BuglyAgent.ConfigAutoQuitApplication(false);
        //BuglyAgent.ConfigAutoReportLogLevel(LogSeverity.LogException);
#endif

        //加载配置表
        //new ConfigLoader001().LoadConfig();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //Debuger.Log("GameLaucher.awake.end");
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        //if (showProjectPath || !Application.isPlaying)
        //{
        //    var style = new GUIStyle();
        //    style.fontSize = 22;
        //    style.normal.textColor = Color.white;
        //    var branch = EditorPath.CurrentBranch + "/" + EditorPath.CurrentGuoShen;
        //    GUILayout.Label(Application.dataPath, style);
        //    GUILayout.Label(branch, style);
        //}
    }
#endif

    private void initManager()
    {
        gameObject.AddComponent<CoroutineManager>();
    }

    private void updateRes()
    {
        //StartupManager.Singleton.Start(() => launchGame());
    }

    private void launchGame()
    {
#if UNITY_EDITOR 
#if Main
        GameManager.Initialize(gameObject, debugMode);
#endif
        //StartCoroutine(launchEditor());
#elif ENABLE_IL2CPP
        StartCoroutine(launchIL2CPP());
#else
        StartCoroutine(launchMono());
#endif
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
        bool useILRT = UnityEditor.EditorPrefs.GetBool("ILRuntime_Editor_Enable", true);
        if (useILRT)
        {
            //ilruntime
            float time = Time.realtimeSinceStartup;
            var dll = System.IO.File.ReadAllBytes(PathUtil.EditorDllPath);
            var pdb = System.IO.File.ReadAllBytes(PathUtil.EditorPdbPath);
            if (EditorPath.HasGuoShen)
            {
                dll = System.IO.File.ReadAllBytes(EditorPath.GuoShenDebugDllPath);
                pdb = System.IO.File.ReadAllBytes(EditorPath.GuoShenPdbPath);
            }
            var appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            var fs = new System.IO.MemoryStream(dll);
            var ps = new System.IO.MemoryStream(pdb);
            appdomain.LoadAssembly(fs, ps, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            //appdomain.LoadAssembly(fs, ps, new Mono.Cecil.Pdb.PdbReaderProvider());

            ILRuntime.ILScriptBinder.Bind(appdomain);
            appdomain.DebugService.StartDebugService(56000);
            Debug.Log("init ILRunTime time > " + (Time.realtimeSinceStartup - time) + "s");
            appdomain.Invoke("GameManager", "Initialize", null, gameObject, debugMode);
            Debug.Log("editor init ILRunTime time > " + (Time.realtimeSinceStartup - time) + "s");
            ilrtApp = appdomain;
        }
        else
        {
            //dll
            var dll = System.IO.File.ReadAllBytes(PathUtil.EditorDllPath);
            if (EditorPath.HasGuoShen)
                dll = System.IO.File.ReadAllBytes(EditorPath.GuoShenDebugDllPath);

            float time = Time.realtimeSinceStartup;
            var assembly = System.Reflection.Assembly.Load(dll);
            Debug.Log("editor init dll time > " + (Time.realtimeSinceStartup - time) + "s");

            var type = assembly.GetType("GameManager");
            var met = type.GetMethod("Initialize", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            met.Invoke(null, new object[] { gameObject, debugMode });
            dllAssembly = assembly;
        }
        yield return null;
    }
#elif ENABLE_IL2CPP
    private IEnumerator launchIL2CPP()
    {
#if SO_FIX
        //so热更
        Debug.Log("launchIL2CPP>热更so GameManager.Initialize");
        GameManager.Initialize(gameObject, debugMode);
        yield return null;
#else
        Debug.Log("launchIL2CPP>" + Time.realtimeSinceStartup);
        var bytes = AbcManager.Singleton.GetDll();
        if(bytes == null)
            bytes = PathUtil.LoadBytes(PathUtil.DllScriptBundleName);
        float time = Time.realtimeSinceStartup;
        var appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
        var fs = new System.IO.MemoryStream(bytes);
        appdomain.LoadAssembly(fs);

        ILRuntime.ILScriptBinder.Bind(appdomain);
        Debug.Log("init ILRunTime time > " + (Time.realtimeSinceStartup - time) + "s");
        appdomain.Invoke("GameManager", "Initialize", null, gameObject, debugMode);
        yield return null;
#endif
    }
#else
    private IEnumerator launchMono()
    {
        Debug.Log("launchMono>" + Time.realtimeSinceStartup);
        var bytes = AbcManager.Singleton.GetDll();
        if(bytes == null)
            bytes = PathUtil.LoadBytes(PathUtil.DllScriptBundleName);
        float time = Time.realtimeSinceStartup;
        var assembly = System.Reflection.Assembly.Load(bytes);
        Debug.Log("init dll time > " + (Time.realtimeSinceStartup - time) + "s");
            
        var type = assembly.GetType("GameManager");
        var met = type.GetMethod("Initialize", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        met.Invoke(null, new object[] { gameObject, debugMode });
        yield return null;
    }
#endif
}