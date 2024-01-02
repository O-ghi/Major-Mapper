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
        types.Add(typeof(ResManager));
        types.Add(typeof(ResPriority));
        types.Add(typeof(ABManager));
        types.Add(typeof(ABDownLoader));
        types.Add(typeof(ResDepManager));
        types.Add(typeof(ABLoader));
        types.Add(typeof(ObbManager));
        types.Add(typeof(PathUtil));
        types.Add(typeof(ShaderList));
        types.Add(typeof(WWWLoader));
        types.Add(typeof(Debuger));
        types.Add(typeof(CoroutineManager));
        types.Add(typeof(BibManager));
        types.Add(typeof(AbcManager));

        //启动
        //types.Add(typeof(LocalConfig));
        //types.Add(typeof(VersionConfig));
        //types.Add(typeof(VersionConfigChecker));
        //types.Add(typeof(SpecialMarker));
        //types.Add(typeof(ForceFileList));
        //types.Add(typeof(BackFileList));
        //types.Add(typeof(ServerList));
        //types.Add(typeof(Notice));
        
        types.Add(typeof(TimeUtils));
        types.Add(typeof(MessageHandle));
        types.Add(typeof(GoLoader));
        //types.Add(typeof(PrefabSceneHolder));
        //types.Add(typeof(PrefabSceneItemHolder));

        //基础类型暂不wrap，可能引起逻辑异常
        //types.Add(typeof(int));
        //types.Add(typeof(float));
        //types.Add(typeof(long));
        //types.Add(typeof(object));
        //types.Add(typeof(string));
        //types.Add(typeof(Array));
        
        //Unity
        types.Add(typeof(Vector2));
        types.Add(typeof(Vector3));
        types.Add(typeof(Quaternion));
        types.Add(typeof(GameObject));
        types.Add(typeof(UnityEngine.Object));
        types.Add(typeof(Transform));
        types.Add(typeof(Time));
        types.Add(typeof(Debug));
        types.Add(typeof(Resources));
        types.Add(typeof(Application));
        types.Add(typeof(Camera));
        types.Add(typeof(Input));
        types.Add(typeof(Screen));
        types.Add(typeof(ScreenCapture));
        //types.Add(typeof(QualitySettings));
        types.Add(typeof(Ray));
        types.Add(typeof(RaycastHit));
        types.Add(typeof(Physics));

        //system
        types.Add(typeof(DateTime));
        types.Add(typeof(System.IO.Path));
        types.Add(typeof(System.IO.File));
        types.Add(typeof(System.IO.Directory));

      

        //Json
        types.Add(typeof(SimpleJSON.JSON));
        types.Add(typeof(SimpleJSON.JSONNode));
        types.Add(typeof(SimpleJSON.JSONArray));
        types.Add(typeof(SimpleJSON.JSONClass));
        types.Add(typeof(SimpleJSON.JSONData));

        // ULiteWebView
        //types.Add(typeof(Jing.ULiteWY.ULiteWY));

        //logic
        //types.Add(typeof(CameraCutscene));

       


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
        ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain(ILRuntime.Runtime.ILRuntimeJITFlags.JITImmediately);
        using (System.IO.FileStream fs = new System.IO.FileStream(Application.dataPath + "/../GameDll/GameLogic.dll", System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            domain.LoadAssembly(fs);
            //Crossbind Adapter is needed to generate the correct binding code
            InitILRuntime(domain);

            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, "Assets/PluginAssets/ILRuntime/Generated");
            //GenerateCLRBinding();

        }


        foreach (var file in System.IO.Directory.GetFiles(Application.dataPath + "/../ILRuntimeBindScript", "*.*"))
            System.IO.File.Copy(file, Application.dataPath + "/PluginAssets/ILRuntime/Generated/" + System.IO.Path.GetFileName(file), true);

        AssetDatabase.Refresh();

        /*if (PlayerBuilderWindow.UseXILHotFix)
        {
            System.IO.File.Copy(Application.dataPath + "/../GameDll/PGameLogic.dll", Application.dataPath + "/Plugins/PGameLogic.dll", true);
            AssetDatabase.Refresh();
        }*/
    }

    [MenuItem("ILRuntime/IOS Generate CLR Binding Code by Analysis")]
    public static void IOSGenerateCLRBindingByAnalysis()
    {
        //用新的分析热更dll调用引用来生成绑定代码
        ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain(ILRuntime.Runtime.ILRuntimeJITFlags.JITImmediately);
        using (System.IO.FileStream fs = new System.IO.FileStream(Application.dataPath + "/../GameDll_IOS/GameLogic.dll", System.IO.FileMode.Open, System.IO.FileAccess.Read))
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
    }

    static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain domain)
    {
        ILRuntime.ILScriptBinder.Bind(domain);
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
}
#endif
