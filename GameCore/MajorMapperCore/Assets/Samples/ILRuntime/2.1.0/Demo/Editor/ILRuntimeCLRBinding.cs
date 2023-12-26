#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding
{
    static void GenerateCLRBinding()
    {
        List<Type> types = new List<Type>();

        //资源加载
        //types.Add(typeof(ResManager));
        //types.Add(typeof(ResPriority));
        //types.Add(typeof(ABManager));
        //types.Add(typeof(ABDownLoader));
        //types.Add(typeof(ResDepManager));
        //types.Add(typeof(ABLoader));
        //types.Add(typeof(ObbManager));
        //types.Add(typeof(PathUtil));
        //types.Add(typeof(ShaderList));
        //types.Add(typeof(WWWLoader));
        //types.Add(typeof(GameSDK));
        //types.Add(typeof(Debuger));
        //types.Add(typeof(AndroidBrigde));
        //types.Add(typeof(CoroutineManager));
        //types.Add(typeof(BibManager));
        //types.Add(typeof(AbcManager));

        ////启动
        //types.Add(typeof(LocalConfig));
        //types.Add(typeof(VersionConfig));
        //types.Add(typeof(VersionConfigChecker));
        //types.Add(typeof(SpecialMarker));
        //types.Add(typeof(ForceFileList));
        //types.Add(typeof(BackFileList));
        //types.Add(typeof(ServerList));
        //types.Add(typeof(Notice));

        //types.Add(typeof(TimeUtils));
        //types.Add(typeof(MessageHandle));
        //types.Add(typeof(GoLoader));
        ////types.Add(typeof(PrefabSceneHolder));
        ////types.Add(typeof(PrefabSceneItemHolder));

        ////基础类型暂不wrap，可能引起逻辑异常
        ////types.Add(typeof(int));
        ////types.Add(typeof(float));
        ////types.Add(typeof(long));
        ////types.Add(typeof(object));
        ////types.Add(typeof(string));
        ////types.Add(typeof(Array));

        ////Unity
        //types.Add(typeof(Vector2));
        //types.Add(typeof(Vector3));
        //types.Add(typeof(Quaternion));
        //types.Add(typeof(GameObject));
        //types.Add(typeof(UnityEngine.Object));
        //types.Add(typeof(Transform));
        //types.Add(typeof(Time));
        //types.Add(typeof(Debug));
        //types.Add(typeof(Resources));
        //types.Add(typeof(Application));
        //types.Add(typeof(Camera));
        //types.Add(typeof(Input));
        //types.Add(typeof(Screen));
        //types.Add(typeof(ScreenCapture));
        ////types.Add(typeof(QualitySettings));
        //types.Add(typeof(Ray));
        //types.Add(typeof(RaycastHit));
        //types.Add(typeof(Physics));

        ////system
        //types.Add(typeof(DateTime));
        //types.Add(typeof(System.IO.Path));
        //types.Add(typeof(System.IO.File));
        //types.Add(typeof(System.IO.Directory));

        ////DoTween
        //types.Add(typeof(DG.Tweening.Ease));
        //types.Add(typeof(DG.Tweening.Tweener));
        //types.Add(typeof(DG.Tweening.DOVirtual));
        //types.Add(typeof(DG.Tweening.TweenExtensions));
        //types.Add(typeof(DG.Tweening.ShortcutExtensions));

        ////Json
        //types.Add(typeof(SimpleJSON.JSON));
        //types.Add(typeof(SimpleJSON.JSONNode));
        //types.Add(typeof(SimpleJSON.JSONArray));
        //types.Add(typeof(SimpleJSON.JSONClass));
        //types.Add(typeof(SimpleJSON.JSONData));

        ////FGUI
        //types.Add(typeof(FairyGUI.Timers));
        //types.Add(typeof(FairyGUI.GObject));
        //types.Add(typeof(FairyGUI.GButton));
        //types.Add(typeof(FairyGUI.GComponent));
        //types.Add(typeof(FairyGUI.GList));
        //types.Add(typeof(FairyGUI.GImage));
        //types.Add(typeof(FairyGUI.GGroup));
        //types.Add(typeof(FairyGUI.GLoader));
        //types.Add(typeof(FairyGUI.GoWrapper));
        //types.Add(typeof(FairyGUI.GRichTextField));
        //types.Add(typeof(FairyGUI.GTextField));
        //types.Add(typeof(FairyGUI.GTextInput));
        //types.Add(typeof(FairyGUI.Stage));
        //types.Add(typeof(FairyGUI.GRoot));
        //types.Add(typeof(FairyGUI.GProgressBar));
        //types.Add(typeof(FairyGUI.GSlider));
        //types.Add(typeof(FairyGUI.GScrollBar));
        //types.Add(typeof(FairyGUI.GTweener));
        //types.Add(typeof(FairyGUI.GGraph));
        //types.Add(typeof(FairyGUI.GMovieClip));
        //types.Add(typeof(FairyGUI.GLabel));
        //types.Add(typeof(FairyGUI.UIPackage));
        //types.Add(typeof(FairyGUI.UIObjectFactory));
        //types.Add(typeof(FairyGUI.UIConfig));

        //// ULiteWebView
        //types.Add(typeof(Jing.ULiteWY.ULiteWY));

        ////logic
        //types.Add(typeof(CameraCutscene));

        ////sdk
        //types.Add(typeof(Sdk.Gosu.OrderInfo));
        //types.Add(typeof(SDKGosuIOSBehaviour));

#if UNITY_IPHONE || UNITY_IOS
        types.Add(typeof(GosuIOSSDKCalller));
#endif

        ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(types, "Assets/PluginAssets/ILRuntime/Generated");
        AssetDatabase.Refresh();
    }

    [MenuItem("ILRuntime/Generate CLR Binding Code by Analysis")]
    public static void GenerateCLRBindingByAnalysis()
    {
        //用新的分析热更dll调用引用来生成绑定代码
        ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
        using (System.IO.FileStream fs = new System.IO.FileStream(Application.dataPath + "/../GameDll/PGameLogic.dll", System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            domain.LoadAssembly(fs);
        }
        //Crossbind Adapter is needed to generate the correct binding code
        InitILRuntime(domain);
        ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, "Assets/PluginAssets/ILRuntime/Generated");
        GenerateCLRBinding();

        foreach (var file in System.IO.Directory.GetFiles(Application.dataPath + "/../ILRuntimeBindScript", "*.*"))
            System.IO.File.Copy(file, Application.dataPath + "/PluginAssets/ILRuntime/Generated/" + System.IO.Path.GetFileName(file), true);

        AssetDatabase.Refresh();

        /*if (PlayerBuilderWindow.UseXILHotFix)
        {
            System.IO.File.Copy(Application.dataPath + "/../GameDll/PGameLogic.dll", Application.dataPath + "/Plugins/PGameLogic.dll", true);
            AssetDatabase.Refresh();
        }*/
    }
    static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain domain)
    {
        Bind(domain);
        //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
        //domain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        //domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        //domain.RegisterCrossBindingAdaptor(new InheritanceAdapter());
    }

    #region 是否启用ILRunTime
    private static bool useILRT = false;
    [MenuItem("ILRuntime/启用ILRunTime", false, 1)]
    public static void useILRuneTime()
    {
        useILRT = EditorPrefs.GetBool("ILRuntime_Editor_Enable", true);
        useILRT = !useILRT;
        EditorPrefs.SetBool("ILRuntime_Editor_Enable", useILRT);
        Menu.SetChecked("ILRuntime/启用ILRunTime", useILRT);
    }

    [MenuItem("ILRuntime/启用ILRunTime", true, 1)]
    public static bool useILRuneTimeOption()
    {
        useILRT = EditorPrefs.GetBool("ILRuntime_Editor_Enable", true);
        Menu.SetChecked("ILRuntime/启用ILRunTime", useILRT);
        return true;
    }
    #endregion

    public static Mono.Cecil.ArrayDimension[] arr;
    public static void Bind(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
    {
        //appdomain.RegisterCrossBindingAdaptor(new ILCoroutineAdapter());
        //appdomain.RegisterCrossBindingAdaptor(new ILMonoBehaviourAdapter());
        //appdomain.RegisterCrossBindingAdaptor(new ILPlayableAssetAdapter());
        //appdomain.RegisterCrossBindingAdaptor(new ILPlayableBehaviourAdapter());
        //appdomain.RegisterCrossBindingAdaptor(new ILBasePlayableBehaviourAdapter());

        //appdomain.RegisterCrossBindingAdaptor(new ILIMsgAdapter());
        //appdomain.RegisterCrossBindingAdaptor(new ILBaseMessageAdapter());
        //appdomain.RegisterCrossBindingAdaptor(new ILBaseBehaviourAdapter());
        //appdomain.RegisterCrossBindingAdaptor(new ILClassCacheAdapter());

        //appdomain.RegisterCrossBindingAdaptor(new ILGComponentAdapter());
        //appdomain.RegisterCrossBindingAdaptor(new ILGLoaderAdapter());
        //appdomain.RegisterCrossBindingAdaptor(new ILGScrollBarAdapter());
        //appdomain.RegisterCrossBindingAdaptor(new ILGButtonAdapter());
        //appdomain.RegisterCrossBindingAdaptor(new ILGProgressBarAdapter());
        ////appdomain.RegisterCrossBindingAdaptor(new ILFGUIEvtDispatcherAdapter());
        //appdomain.RegisterCrossBindingAdaptor(new ILGComboxAdapter());
        //appdomain.RegisterCrossBindingAdaptor(new ILGLabelAdapter());

#if USE_HOT
            appdomain.RegisterCrossBindingAdaptor(new ILServiceAdapter());
            appdomain.RegisterCrossBindingAdaptor(new ILBaseWindowAdapter());
#endif
        //ILGSliderAdatper

        arr = Empty<Mono.Cecil.ArrayDimension>.Array;
        if (Application.isPlaying)
        {
#if !ONLY_PGAME
            //ILGetAddComponent.Bind(appdomain);
            //float time = Time.realtimeSinceStartup;
            //ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);
            //Debug.Log("Binding Wrap time > " + (Time.realtimeSinceStartup - time) + "s");
#endif

#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS)
            appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
        }

        //委托

    }

}
static class Empty<T>
{

    public static readonly T[] Array = new T[0];
}
#endif
