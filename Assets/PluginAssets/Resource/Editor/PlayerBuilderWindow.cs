//using System.IO;
//using UnityEditor;
//using UnityEngine;
//using System.Collections.Generic;
//using System.Text;
//using System.Linq;

//public class PlayerBuilderWindow : EditorWindow
//{
//    public enum BuildPlayerMode
//    {
//        UpdateAB = 0,
//        NoUpdateAB = 1,
//    }

//    public const string BuildKey_Develop = "PlayerBuilder_develop";
//    public const string BuildKey_Profiler = "PlayerBuilder_profiler";
//    public const string BuildKey_Back = "PlayerBuilder_back";
//    public const string BuildKey_Obb = "PlayerBuilder_obb";
//    public const string BuildKey_uiAB = "PlayerBuilder_uiAB";
//    public const string BuildKey_Add = "PlayerBuilder_Add";
//    public const string BuildKey_Cpp = "PlayerBuilder_il2cpp";


//    [MenuItem("Tools/Player/PlayerWindow #&W", false, 0)]
//    public static void builderWays()
//    {
//        var window = GetWindow<PlayerBuilderWindow>();
//        window.Show();
//    }

//    private bool autoProfiler = false;
//    private bool developmentBuild = false;

//    private List<string> channelList;
//    private List<string> channelValueList;
//    private string curChannelValue;

//    private int curChannelIndex = 0;
//    private int curLanIndex = 0;

//    private bool androidBuildAAB = true;
//    private bool androidUseSoFix = false;
//    private bool androidIL2CPP = true;
//    private bool androidBuild = true;
//    private bool makeObb = false;
//    private bool genCode = false;
//    private int codeCount = 500;

//    //localConfigBoo 
//    private int appVer = 1;
//    private int forceVer = 1;
//    private string configTag = "android";
//    private bool fullPkg = false;

//    private string obbBasePath;
//    private string lastForcePath;

//    private List<string> languageList = new List<string>();
//    private List<string> removeLanList = new List<string>();

//    //过审
//    private int guoshenIdx;
//    private List<string> guoshenList = new List<string>();


//    private string versionFormatCheck(string version)
//    {
//        if (string.IsNullOrEmpty(version))
//            return "1.0.0";
//        var arr = version.Split('.');
//        if (arr.Length != 3)
//            return "1.0.0";
//        foreach (var val in arr)
//        {
//            int iv = 0;
//            if (!int.TryParse(val, out iv))
//                return "1.0.0";
//            if (iv < 0)
//                return "1.0.0";
//        }
//        return version;
//    }

//    private void OnEnable()
//    {
//        androidBuild = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
//        developmentBuild = EditorPrefs.GetBool(BuildKey_Develop, false);
//        autoProfiler = EditorPrefs.GetBool(BuildKey_Profiler, false);
//        makeObb = EditorPrefs.GetBool(BuildKey_Obb, false) && androidBuild;
//        androidIL2CPP = EditorPrefs.GetBool(BuildKey_Cpp, false);


//        TextAsset ta = new TextAsset();
//        var json = new SimpleJSON.JSONClass();

//        //load language
//        ta = Resources.Load<TextAsset>("language");
//        json = SimpleJSON.JSONClass.Parse(ta.text) as SimpleJSON.JSONClass;
//        var arr = System.Enum.GetNames(typeof(SystemLanguage));
//        for (int i = 0; i < arr.Length; ++i)
//        {
//            if (json["default"].Value == arr[i].ToLower())
//            {
//                curLanIndex = i;
//                break;
//            }
//        }

//        //localConfig
//        ta = Resources.Load<TextAsset>("LocalConfig");
//        json = SimpleJSON.JSONClass.Parse(ta.text) as SimpleJSON.JSONClass;
//        json = json["formal"] as SimpleJSON.JSONClass;
//        appVer = json["appVer"].AsInt;
//        forceVer = json["forceResVer"].AsInt;
//        fullPkg = json["fullPackage"].AsBool;
//        configTag = json["configTag"].Value;

//        updatelanguageList();

//        if (!isInitGihotConfig)
//        {
//            this.initGihotConfig();
//        }

//    }

//    private void updatelanguageList()
//    {
//        //多语言
//        var rootArr = new string[] { PathUtil.GetForceABPath(), PathUtil.GetBackABPath(),
//            Application.dataPath + "/Resources/UITextures", Application.dataPath + "/Resources/UITextures_IOS"};

//        foreach (var dicPath in rootArr)
//        {
//            if (!Directory.Exists(dicPath))
//                continue;
//            var dicArr = Directory.GetDirectories(dicPath);
//            foreach (var dic in dicArr)
//            {
//                var name = Path.GetFileName(dic);
//                if (!languageList.Contains(name))
//                    languageList.Add(name);
//            }
//        }
//    }
//    #region GuildParams
//    //development
//    private bool tgDevelopmentBuild;

//    //Android build
//    private bool tgBuildAndroid;
//#if UNITY_IPHONE
//    private PlatformData targetPlatform = PlatformData.ios_uat;
//#else 
//    private PlatformData targetPlatform = PlatformData.android_dev;
//#endif
//    private string[] platformList;
//    private GihotConfig currentGihotConfig;
//    private bool isInitGihotConfig;

//    //build mode
//    private BuildPlayerMode buildPlayerMode = BuildPlayerMode.UpdateAB;
//    private string[] buildModeList;

//    #endregion

//    private void initGihotConfig()
//    {
//        isInitGihotConfig = true;
//        //get GihotConfig
//        this.currentGihotConfig = GihotConfigList.GetGihot(this.targetPlatform);
//        //create platform list
//        this.platformList = System.Enum.GetNames(typeof(PlatformData));
//        //build mode
//        this.buildModeList = System.Enum.GetNames(typeof(BuildPlayerMode));
//        GameLauncherCore gameLauncher = (Object.FindObjectsOfType(typeof(GameLauncherCore))[0] as GameLauncherCore);
//        if(gameLauncher != null)
//        {
//            this.buildPlayerMode = gameLauncher.isNoUpdate ? BuildPlayerMode.NoUpdateAB : BuildPlayerMode.UpdateAB;
//            if (gameLauncher.isNoUpdate)
//            {
//                this.forceVer = 10000;
//            }
//        }
//        //load channel list
//        //
//        channelList = System.Enum.GetNames(typeof(Channel)).ToList();
//        channelValueList = System.Enum.GetNames(typeof(Channel)).ToList();
//        //Setting IOS
//#if UNITY_IOS
//        tgBuildAndroid = false;
//        buildDll = false;
//        codeCount = 0;
//#endif
//    }

//    private void OnGUI()
//    {
//        if (!isInitGihotConfig)
//        {
//            this.initGihotConfig();
//        }
//        //development setting
//        {
//            GUILayout.Space(20);
//            GuiLabelBold("Development Setting");
//            GUILayout.Space(5);
//            {
//                EditorGUI.indentLevel++;
//                tgDevelopmentBuild = EditorGUILayout.Toggle("1.Development build", tgDevelopmentBuild);
//                EditorGUI.BeginDisabledGroup(!tgDevelopmentBuild);
//                {
//                    if (tgDevelopmentBuild)
//                        autoProfiler = EditorGUILayout.Toggle("2.Profiler build", autoProfiler);
//                    else
//                        autoProfiler = EditorGUILayout.Toggle("2.Profiler build", false);
//                }
//                EditorGUI.EndDisabledGroup();
//                EditorGUI.indentLevel--;
//            }

//        }
//        GuiLine();

//        //Version config
//        {
//            GuiLabelBold("Player Setting");
//            GUILayout.Space(5);

//            {
//                EditorGUI.indentLevel++;
//                var buildInt = EditorGUILayout.Popup("Select BuildMode", (int)this.buildPlayerMode, buildModeList.ToArray(), GUILayout.MaxWidth(300));

//                if(buildInt != (int)this.buildPlayerMode)
//                {
//                    this.buildPlayerMode = (BuildPlayerMode)buildInt;
//                    GameLauncherCore gameLauncher = (Object.FindObjectsOfType(typeof(GameLauncherCore))[0] as GameLauncherCore);
//                    if(gameLauncher == null)
//                    {
//                        Debug.Log("gameLauncher nullllllll !!!!!!!!!");
//                    }
//                    else
//                    {
//                        switch (this.buildPlayerMode)
//                        {
//                            case BuildPlayerMode.UpdateAB:
//                                gameLauncher.isNoUpdate = false;
//                                this.forceVer = 0;
//                                break;
//                            case BuildPlayerMode.NoUpdateAB:
//                                gameLauncher.isNoUpdate = true;
//                                this.forceVer = 10000;
//                                break;
//                        }
//                        EditorGHUtils.ForceSaveCurrentScene();
//                    }
                
//                }

//                EditorGUI.indentLevel--;

//            }
//            GUILayout.Space(5);
//            EditorGUI.indentLevel++;

//            GuiLabelBold("1.Version Setting");
//            {
//                EditorGUI.indentLevel++;
//                //app version
//                {
//                    GUILayout.BeginHorizontal();
//                    appVer = EditorGUILayout.IntField("1.AppVersion", appVer, GUILayout.Width(300));
//                    if (appVer <= 0)
//                        appVer = 1;
//                    GUILayout.EndHorizontal();
//                }

//                {
//                    GUILayout.BeginHorizontal();
//                    forceVer = EditorGUILayout.IntField("2.ForceResVersion", forceVer, GUILayout.MaxWidth(300));
//                    if (forceVer < 0)
//                        forceVer = 0;
//                    GUILayout.EndHorizontal();
//                }

//                {
//                    GUILayout.BeginHorizontal();
//                    fullPkg = EditorGUILayout.Toggle("3.FullPackage", fullPkg);
//                    GUILayout.EndHorizontal();
//                }
//                {
//                    var platformId = EditorGUILayout.Popup("4.Target Platform", (int)this.targetPlatform, platformList.ToArray(), GUILayout.MaxWidth(300));
//                    if (platformId != (int)this.targetPlatform)
//                    {
//                        this.targetPlatform = (PlatformData)platformId;
//                        this.currentGihotConfig = GihotConfigList.GetGihot(this.targetPlatform);
//                    }

//                    //Review data
//                    {
//                        EditorGUI.indentLevel++;
//                        GuiLabelBold("Preview Version Data");
//                        configTag = EditorGUILayout.TextField("1.ConfigTag", this.currentGihotConfig.platformData.ToString(), GUILayout.MaxWidth(300));
//                        curChannelValue = EditorGUILayout.TextField("2.Channel",this.currentGihotConfig.ChannelName, GUILayout.MaxWidth(300));
//                        EditorGUILayout.TextField("3.Channel Url", this.currentGihotConfig.versionPrefixUrl, GUILayout.MaxWidth(400));
//                        EditorGUI.indentLevel--;

//                    }
//                }
//                {
//                    GUILayout.BeginHorizontal();
//                    GUILayout.EndHorizontal();
//                }

//                {
//                    GUILayout.BeginHorizontal();
                   
//                    GUILayout.EndHorizontal();
//                }

//                EditorGUI.indentLevel--;
//            }


//            //Build Player
//            {
//                GuiLabelBold("2.Build Platform");
//                EditorGUI.indentLevel++;
//                GuiNormalToggle("Build Android", ref tgBuildAndroid);
//                //for android
//                EditorGUI.BeginDisabledGroup(!tgBuildAndroid);
//                {
//                    EditorGUI.indentLevel++;
//                    GuiNormalToggle("IL2CPP(for GooglePlay)", ref androidIL2CPP);
//                    EditorGUI.BeginDisabledGroup(!androidIL2CPP);
//                    {
//                        EditorGUI.indentLevel++;
//                        GuiNormalToggle("updateSo*(For Experienced)", ref androidUseSoFix);
//                        EditorGUI.indentLevel--;
//                    }
//                    EditorGUI.EndDisabledGroup();

//                    GuiNormalToggle("Build Appbundle(*.aab)", ref androidBuildAAB);
//                    if (androidBuildAAB)
//                    {
//                        EditorUserBuildSettings.buildAppBundle = true;
//#if UNITY_ANDROID
//                        PlayerSettings.Android.useAPKExpansionFiles = true;
//#endif
//                    }
//                    else
//                    {
//                        EditorUserBuildSettings.buildAppBundle = false;
//#if UNITY_ANDROID
//                        PlayerSettings.Android.useAPKExpansionFiles = false;
//#endif
//                    }
//                    EditorGUI.indentLevel--;
//                }
//                EditorGUI.EndDisabledGroup();

//                //for ios
//                GuiNormalToggle("Build IOS", ref tgBuildAndroid, false);
//                EditorGUI.indentLevel--;
//            }

//            {
//                GUILayout.Space(10);
//                GuiLabelBold("3.Resources Setting");
//                EditorGUI.indentLevel++;
//                genCode = !androidBuild;
//                {
//                    EditorGUI.BeginDisabledGroup(!genCode);

//                    GuiNormalToggle("Gen code (Only IOS)", ref genCode);
//                    EditorGUI.BeginDisabledGroup(!genCode);
//                    {
//                        codeCount = EditorGUILayout.IntField("gen code", codeCount, GUILayout.Width(300));

//                    }
//                    EditorGUI.EndDisabledGroup();
//                }

//                EditorGUI.EndDisabledGroup();


//                //Todo: Tạm thời ko xử dụng  mode review
//                //if (guoshenList.Count > 1)
//                //{
//                //    GUILayout.Space(10);
//                //    guoshenIdx = EditorGUILayout.Popup("audit res", guoshenIdx, guoshenList.ToArray());
//                //    if (androidBuild)
//                //        guoshenIdx = 0;
//                //}
//                EditorGUI.indentLevel--;
//            }
//            {
//                GUILayout.Space(10);
//                GuiLabelBold("4.Language Setting");
//                EditorGUI.indentLevel++;
//                var newLanIdx = EditorGUILayout.Popup("1.Default language", curLanIndex, System.Enum.GetNames(typeof(SystemLanguage)), GUILayout.Width(300));
//                if (newLanIdx != curLanIndex)
//                {
//                    curLanIndex = newLanIdx;
//                    removeLanList.Clear();
//                    updatelanguageList();
//                }
//                if (languageList.Count > 0)
//                {
//                    EditorGUILayout.LabelField("2.Contain languages:");
//                    EditorGUILayout.LabelField("Chú ý: Hiện tại chỉ sử dụng chinese(Việt Nam), đã tự remove các ngôn ngữ phụ", EditorStyles.miniBoldLabel);
//                    EditorGUI.indentLevel++;
//                    var selLan = System.Enum.GetNames(typeof(SystemLanguage))[curLanIndex].ToLower();
//                    for (int i = languageList.Count - 1; i >= 0; --i)
//                    {
//                        if (selLan == languageList[i])
//                            continue;
//                        removeLanList.Add(languageList[i]);
//                        //GUILayout.BeginHorizontal();
//                        //EditorGUILayout.LabelField(languageList[i], GUILayout.MaxWidth(145));
//                        //if (GUILayout.Button("-", GUILayout.MaxWidth(30)))
//                        //{
//                        //    removeLanList.Add(languageList[i]);
//                        //    languageList.RemoveAt(i);
//                        //}
//                        //GUILayout.EndHorizontal();
//                    }
//                    EditorGUI.indentLevel--;
//                }

//                EditorGUI.indentLevel--;
//            }

//            {
//                //GUILayout.Space(10);
//                //GuiLabelBold("5.Back Resources Setting");
//                //EditorGUILayout.LabelField("Chú ý: Hiện tại không xử dụng back resources", EditorStyles.miniBoldLabel);

//                //EditorGUI.indentLevel++;
//                //if (androidBuild && !backInApp)
//                //{
//                //    GuiNormalToggle("1.back resource to obb*(For Experienced)", ref makeObb);
//                //    if (makeObb)
//                //        backInApp = false;
//                //}
//                //if (!makeObb || !androidBuild)
//                //    GuiNormalToggle("2.include back resource*(For Experienced)", ref backInApp);

//                //GUILayout.Space(10);
//                //GUILayout.Label(EditorPath.CurrentBranch + "/" + EditorPath.CurrentGuoShen);
//                //EditorGUI.indentLevel--;
//            }
                
                
//        }

//        EditorGUI.indentLevel--;

//        GuiLine();
//        EditorGUILayout.LabelField("Chú ý: Mọi thay đổi \"Version Setting\" sẽ được apply file \"LocalConfig\"sau khi build Player", EditorStyles.miniBoldLabel);
//        if (GUILayout.Button("Build Player", GUILayout.Width(200)))
//        {
//            //EditorPrefs.SetBool(BuildKey_Develop, developmentBuild);
//            //EditorPrefs.SetBool(BuildKey_Profiler, autoProfiler);

//            EditorPrefs.SetBool(BuildKey_Develop, false);
//            EditorPrefs.SetBool(BuildKey_Profiler, false);
//            EditorPrefs.SetBool(BuildKey_Obb, makeObb);
//            EditorPrefs.SetBool(BuildKey_Cpp, androidIL2CPP);

//            //localConfig
//            TextAsset ta = Resources.Load<TextAsset>("LocalConfig");
//            var json = SimpleJSON.JSONClass.Parse(ta.text) as SimpleJSON.JSONClass;
//            foreach (var item in json.GetContainer)
//            {
//                item.Value["appVer"].AsInt = appVer;
//                item.Value["forceResVer"].AsInt = forceVer;
//                item.Value["fullPackage"].AsBool = fullPkg;
//                item.Value["configTag"] = configTag;
//                //write new url
//                item.Value["channelUrl"][this.curChannelValue][0] = this.currentGihotConfig.versionPrefixUrl;
//            }
//            var infoNode = new SimpleJSON.JSONClass();
//            json["formal"]["buildInfo"] = infoNode;
//            infoNode["device"] = SystemInfo.deviceName;
//            infoNode["path"] = Application.dataPath;
//            infoNode["time"] = System.DateTime.Now.ToString();
//            infoNode["branch"] = EditorPath.CurrentBranch + "/" + EditorPath.CurrentGuoShen;
//            infoNode["unity"] = Application.unityVersion + "_" + PlayerSettings.advancedLicense;
//            infoNode["buildID"] = System.DateTime.Now.Ticks.ToString();


//#if Main
//            File.WriteAllText(Application.dataPath + "/Hotupdate Assets/Resources/LocalConfig.json", json.ToString(""));
//#else
//            File.WriteAllText(Application.dataPath + "/Resources/LocalConfig.json", json.ToString(""));
//#endif
//            //选择渠道
//            ta = Resources.Load<TextAsset>("channel");
//            json = SimpleJSON.JSONClass.Parse(ta.text) as SimpleJSON.JSONClass;
//            json["current"].Value = this.curChannelValue;
//            json[this.curChannelValue].Value = this.curChannelValue;
//#if Main
//            File.WriteAllText(Application.dataPath + "/Hotupdate Assets/Resources/channel.json", json.ToString(""));
//#else
//            File.WriteAllText(Application.dataPath + "/Resources/channel.json", json.ToString(""));
//#endif

//            var curChannel = json["current"].Value;

//            //设置内置语言
//            ta = Resources.Load<TextAsset>("language");
//            json = SimpleJSON.JSONClass.Parse(ta.text) as SimpleJSON.JSONClass;
//            json["default"].Value = System.Enum.GetNames(typeof(SystemLanguage))[curLanIndex].ToLower();
//#if Main
//            File.WriteAllText(Application.dataPath + "/Hotupdate Assets/Resources/language.json", json.ToString(""));
//#else
//            File.WriteAllText(Application.dataPath + "/Resources/language.json", json.ToString(""));
//#endif
//            var language = json["default"].Value;
//            //obb基础版本目录
//            if (makeObb && androidBuild)
//            {
//                var dic = new DirectoryInfo(Application.dataPath + "/../ABout/Android/");
//                obbBasePath = EditorUtility.OpenFilePanel("选择back基础文件", dic.FullName, "json");
//                Debug.Log("back基础文件路径>" + obbBasePath);
//            }

//#if UNITY_ANDROID
//            if (androidBuild)
//            {
//                //googleplay 64位支持
//                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, androidIL2CPP ? ScriptingImplementation.IL2CPP : ScriptingImplementation.Mono2x);
//                if (androidIL2CPP)
//                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
//                else
//                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
//                EditorUserBuildSettings.exportAsGoogleAndroidProject = androidUseSoFix;
//            }
//#endif


//            var timer = System.Diagnostics.Stopwatch.StartNew();
//            buildPlayer(curChannel, language, androidBuild ? BuildTarget.Android : BuildTarget.iOS);
//            timer.Stop();
//            Debug.Log("Lần này tốn thời gian đóng gói>" + timer.Elapsed.ToString());
//            this.Close();
//        }

//        if (androidBuild)
//        {
//            GUILayout.Space(20);
//            if (GUILayout.Button("BuildForSo*(For Experienced)", GUILayout.Width(200)))
//            {
//                var timer = System.Diagnostics.Stopwatch.StartNew();
//                soBuild(channelList[curChannelIndex], BuildTarget.Android);
//                timer.Stop();
//                Debug.Log("Điều này khiến việc đóng gói tốn nhiều thời gian>" + timer.Elapsed.ToString());
//            }
//        }
//    }

//    private void buildPlayer(string curSdk, string language, BuildTarget target)
//    {
//        if (EditorUserBuildSettings.activeBuildTarget != target)
//        {
//            if (EditorUtility.DisplayDialog("Prompt", "Nền tảng hiện tại và mục tiêu không khớp, bạn có muốn chuyển đổi nền tảng không?", "Chuyển nền tảng", "Hủy đóng gói"))
//            {
//                if (target == BuildTarget.Android)
//                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, target);
//                else
//                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, target);
//            }
//            else
//            {
//                ShowNotification(new GUIContent("Bao bì bị hủy"));
//                return;
//            }
//        }
//        AssetDatabase.Refresh();
//        //PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");

//        //Đường đầu ra
//        string outputPath = Application.dataPath + "/../build_out/" + target.ToString().ToLower() + "/";
//        DirectoryInfo di = new DirectoryInfo(outputPath);
//        if (!di.Exists)
//            di.Create();

//        //channel
//        string timeStr = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm");
//#if UNITY_IPHONE
//        string fullPath = di.FullName + curSdk + "_" + PlayerSettings.productName;
//#elif UNITY_ANDROID
//        string fullPath = di.FullName + curSdk  + "_" + PlayerSettings.productName + "_" + timeStr;
//#endif
//        if (target == BuildTarget.Android && !androidUseSoFix)
//        {
//            string serveType = this.currentGihotConfig.lastName;
//            if (androidBuildAAB)
//            {
//                fullPath = di.FullName + serveType+ "_" + PlayerSettings.productName + "_" + timeStr + ".aab";

//            }
//            else
//            {
//                fullPath = di.FullName + serveType + "_" + PlayerSettings.productName + "_" + timeStr + ".apk";

//            }
//        }

//        //dll
//        //PathUtil.codeOffset = PathUtil.scriptOffset;
//        //PathUtil.codeKey = PathUtil.scriptKey;
//        //if (!File.Exists(PathUtil.EditorDllPath))
//        //{

//        //    Debug.LogError("dll does not exist, please check for compilation error");
//        //    ShowNotification(new GUIContent("dll does not exist, packaging has been cancelled"));
//        //    return;
//        //}
//        //EditorUtility.DisplayProgressBar("Package", "Package Dll", 0.1f);
//        //PlayerBuilder.BuildAsBundle(PathUtil.EditorDllPath, PlayerBuilder.PlayerBuildTempPath, PathUtil.DllScriptBundleName);
//        //AssetDatabase.Refresh();

//        //Generate useless code
//        if (genCode && codeCount > 0)
//        {
//            var path = Application.dataPath + "/../channel/GenCode/";
//            var dllBat = new System.Diagnostics.Process();
//            dllBat.StartInfo.WorkingDirectory = path;
//            //dllBat.StartInfo.FileName = path + "gen_code.bat";
//            dllBat.StartInfo.FileName = path + "GenCode.bat";
//            dllBat.StartInfo.Arguments = "close " + codeCount;
//            dllBat.Start();
//            dllBat.WaitForExit();
//            AssetDatabase.Refresh();
//            Debuger.Log("Generate useless code to complete");
//        }

//        //bean
//        //EditorUtility.DisplayProgressBar("Package", "Package Configuration Table", 0.2f);
//        //PlayerBuilder.BuildDirToAB(Application.dataPath + "/Bin", PlayerBuilder.PlayerBuildTempPath, "*.bytes", PathUtil.ConfigBundleName, true, false);
//        //AssetDatabase.Refresh();

//        //多语言
//        //EditorUtility.DisplayProgressBar("Package", "Multilingual Arrangement", 0.3f);
//        //PlayerBuilder.MakeDeflanguageConf(target, removeLanList, PlayerBuilder.PlayerBuildTempPath, PathUtil.LanguageConfName);
//        //AssetDatabase.Refresh();

//        //依赖
//        //EditorUtility.DisplayProgressBar("Package", "Dependency Arrangement", 0.45f);
//        //PlayerBuilder.MergeABDependence(new string[] { PathUtil.GetForceABPath() }, PlayerBuilder.PlayerBuildTempPath, PathUtil.BuildinDepConfName);
//        //AssetDatabase.Refresh();

//        //检查现打的ab
//        //int checkNum = 0;
//        //var arr = Directory.GetFiles(PlayerBuilder.PlayerBuildTempPath, "*.*");
//        //foreach (var f in arr)
//        //{
//        //    //dll,配置表,依赖必须有
//        //    if (f.EndsWith(PathUtil.DllScriptBundleName))
//        //    {
//        //        Debug.Log("Check: đã gen dll assetbundle");
//        //        checkNum++;

//        //    }
//        //    if (f.EndsWith(PathUtil.ConfigBundleName))
//        //    {
//        //        Debug.Log("Check: đã gen bean assetbundle");
//        //        checkNum++;

//        //    }
//        //    if (f.EndsWith(PathUtil.BuildinDepConfName))
//        //    {
//        //        Debug.Log("Check: đã gen Dependency assetbundle");
//        //        checkNum++;

//        //    }
//        //}
//        //if (checkNum < 3)
//        //{
//        //    EditorUtility.ClearProgressBar();
//        //    Debug.LogError("dll, configuration table, dependency, package ab failed, check the compilation error, or restart Unity and try again");
//        //    ShowNotification(new GUIContent("dll, configuration table, dependency, package ab failed"));
//        //    return;
//        //}

//        //Copy ab
//        EditorUtility.DisplayProgressBar("Package", "Generate bib", 0.5f);
//        var bibRet = PlayerBuilder.MakeForceAsBib(language, removeLanList);
//        AssetDatabase.Refresh();

//        AssetDatabase.Refresh();
//        if (!bibRet)
//        {
//            EditorUtility.DisplayDialog("Error", "Bib generation failed! Packaging interrupted", "ok");
//            return;
//        }

//        //if (makeObb && target == BuildTarget.Android)
//        //{
//        //    //obb, first tidy up the sdk, after sdk tidy up, the package name will affect the obb name
//        //    EditorUtility.DisplayProgressBar("Package", "make obb", 0.8f);
//        //    PlayerBuilder.MakeBackAsObb(obbBasePath, language, removeLanList, outputPath);
//        //}

//        //EditorUtility.DisplayProgressBar("Package", "make abc", 0.9f);
//        //Debug.Log("EditorPath.HasGuoShen " + EditorPath.HasGuoShen);
//        //if (EditorPath.HasGuoShen)
//        //    PlayerBuilder.MakeAbc();
//        //return;
//        //xử lý sdk trước khi build
//        EditorUtility.DisplayProgressBar("SDK", "Đang đăng ký thông tin SDK", 0.95f);
//        PlayerBuilder.SDKDeal(ref fullPath, curSdk, true, target);
//        //AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
//        ////整理sdk的时候才拷贝的dll，这一步需要放在最后否则前面打包会有编译错误
//        //if (androidUseSoFix && androidBuild)
//        //    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "SO_FIX");
//        //else
//        //    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");

//        //打包
//        BuildOptions option = BuildOptions.CompressWithLz4;
//        if (developmentBuild)
//            option |= BuildOptions.Development;
//        if (autoProfiler)
//            option |= BuildOptions.ConnectWithProfiler;

//#if UNITY_EDITOR_OSX
//        if (target == BuildTarget.iOS)
//            option |= BuildOptions.SymlinkLibraries;
//#endif

//        //etc2软解类型
//        EditorUserBuildSettings.androidETC2Fallback = AndroidETC2Fallback.Quality32BitDownscaled;
//        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
//        //场景
//        List<string> scenes = new List<string>();
//        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
//        {
//            if (e != null && e.enabled)
//                scenes.Add(e.path);
//        }
//        var ret = BuildPipeline.BuildPlayer(scenes.ToArray(), fullPath, target, option);
//        AssetDatabase.Refresh();
//        //xử lý sdk sau khi build
//        PlayerBuilder.SDKDeal(ref fullPath, curSdk, false, target);
//        //if (androidUseSoFix && androidBuild)
//        //{
//        //    updateGradleProjectSetting(fullPath);
//        //    buildGradleApk(fullPath);
//        //}
//        //else
//        //{
//        System.Diagnostics.Process.Start(outputPath);
//        //}

//        //Delete copied files
//        //if (Directory.Exists(Application.dataPath + "/StreamingAssets/"))
//        //    Directory.Delete(Application.dataPath + "/StreamingAssets/", true);
//        if (Directory.Exists(PlayerBuilder.PlayerBuildTempPath))
//            Directory.Delete(PlayerBuilder.PlayerBuildTempPath, true);
//        if (File.Exists(Application.dataPath + "/Plugins/PGameLogic.dll"))
//            File.Delete(Application.dataPath + "/Plugins/PGameLogic.dll");
//        //PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");
//        EditorUtility.ClearProgressBar();
//        AssetDatabase.Refresh();
//        Debuger.Log("Packaged...", ret);
//    }

//    private void buildGradleApk(string projectPath)
//    {
//        string fullPath = projectPath.Replace(Application.productName, "project");
//        Directory.Move(projectPath, fullPath);
//        File.Move(fullPath + "/" + Application.productName, fullPath + "/androidProject");
//        fullPath = fullPath + "/androidProject";

//        var path = fullPath + "/buildApk.bat";
//        File.WriteAllText(path, "gradle assembleRelease\nping 127.0.0.1 -n 5 >nul");
//        var buildBat = new System.Diagnostics.Process();
//        buildBat.StartInfo.WorkingDirectory = fullPath;
//        buildBat.StartInfo.FileName = path;
//        buildBat.Start();
//        buildBat.WaitForExit();

//        path = projectPath + "/build/outputs/apk/release";
//        if (File.Exists(path))
//            System.Diagnostics.Process.Start(path);
//    }

//    private void updateGradleProjectSetting(string projectPath)
//    {
//        var ta = Resources.Load<TextAsset>("channel");
//        var json = SimpleJSON.JSONClass.Parse(ta.text) as SimpleJSON.JSONClass;
//        var channelType = json["current"].Value;
//        var channel = json[channelType].Value;

//        ta = Resources.Load<TextAsset>("LocalConfig");
//        json = SimpleJSON.JSONClass.Parse(ta.text) as SimpleJSON.JSONClass;
//        json = json["formal"] as SimpleJSON.JSONClass;
//        var tag = json["configTag"].Value;
//        var cdnUrl = json["channelUrl"][channelType][0].Value;

//        var path = projectPath + "/" + Application.productName + "/src/main/res";
//        //var path = Application.dataPath + "/../channel/so_project/src/main/res";
//        foreach (var file in Directory.GetFiles(path, "check_activity_string.xml", SearchOption.AllDirectories))
//        {
//            var xml = new System.Xml.XmlDocument();
//            xml.Load(file);
//            var list = xml["resources"].ChildNodes;
//            for (int i = 0; i < list.Count; ++i)
//            {
//                var node = list[i] as System.Xml.XmlElement;
//                if (node.Name != "string")
//                    continue;

//                if (node.GetAttribute("name") == "configUrl")
//                    node.InnerText = cdnUrl + "/res/" + channel + "/" + configTag + "/config/version_config.json";
//                if (node.GetAttribute("name") == "soUrl")
//                    node.InnerText = cdnUrl + "/res/" + channel + "/" + configTag + "/force/%d/so_patch.%s";
//                if (node.GetAttribute("name") == "soRes")
//                    node.InnerText = forceVer + "";
//            }
//            xml.Save(file);
//        }

//        //xml xóa số phiên bản sdk
//        path = projectPath + "/" + Application.productName + "/src/main/AndroidManifest.xml";
//        var strlist = new List<string>(File.ReadAllLines(path));
//        for (int i = 0; i < strlist.Count; ++i)
//        {
//            if (strlist[i].Contains("android:minSdkVersion") && strlist[i].Contains("android:targetSdkVersion"))
//            {
//                strlist.RemoveAt(i);
//                break;
//            }
//        }
//        File.WriteAllLines(path, strlist.ToArray());

//        //Modify sdk packageName /Add checkActivity
//        string launcherActivity = "";
//        var xmlDoc = new System.Xml.XmlDocument();
//        xmlDoc.Load(path);
//        var xmlList = xmlDoc["manifest"]["application"].ChildNodes;
//        for (int i = 0; i < xmlList.Count; ++i)
//        {
//            var node = xmlList[i] as System.Xml.XmlElement;
//            if (node == null)
//                continue;
//            if (node.Name != "activity")
//                continue;
//            if (node.GetAttribute("android:name").Contains(".MainActivity"))
//            {
//                launcherActivity = node.GetAttribute("android:name");
//                //Add Activity
//                var checkNode = node.CloneNode(true) as System.Xml.XmlElement;
//                checkNode.SetAttribute("android:name", "com.unity3d.hookplayer.CheckActivity");

//                //Remove startup attributes
//                var launch = node["intent-filter"];
//                node.RemoveChild(launch);
//                xmlDoc["manifest"]["application"].InsertBefore(checkNode, node);
//                xmlDoc.Save(path);
//                break;
//            }
//        }

//        if (!string.IsNullOrEmpty(launcherActivity))
//        {
//            path = projectPath + "/" + Application.productName + "src/main/java/com/unity3d/hookplayer/CheckActivity.java";
//            if (File.Exists(path))
//            {
//                strlist = new List<string>(File.ReadAllLines(path));
//                for (int i = 0; i < strlist.Count; ++i)
//                {
//                    if (strlist[i].Contains(".MainActivity") && strlist[i].Contains("import"))
//                    {
//                        strlist[i] = "import " + launcherActivity + ";";
//                        File.WriteAllLines(path, strlist.ToArray());
//                        break;
//                    }
//                }
//            }
//        }
//    }

//    private void soBuild(string curSdk, BuildTarget target)
//    {

//        //googleplay 64-bit support
//#if UNITY_ANDROID
//        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
//        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
//        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
//#endif

//        string outputPath = Application.dataPath + "/../build_out/android/";
//        DirectoryInfo di = new DirectoryInfo(outputPath);
//        if (!di.Exists)
//            di.Create();
//        string timeStr = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm");
//        string fullPath = di.FullName + "_so_build_" + timeStr;
//        //打包前sdk处理
//        EditorUtility.DisplayProgressBar("打包", "整理sdk", 0.95f);
//        //PlayerBuilder.SDKDeal(ref fullPath, curSdk, true, target);
//        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
//        //Dll được sao chép khi sdk được tổ chức, bước này cần được đặt ở cuối, nếu không sẽ có lỗi biên dịch trong gói trước
//        //PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "SO_FIX");
//        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "FIX_FGUI;SKIP_SDK");

//        //场景
//        List<string> scenes = new List<string>();
//        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
//        {
//            if (e != null && e.enabled)
//                scenes.Add(e.path);
//        }
//        string productName = PlayerSettings.productName;
//        PlayerSettings.productName = "soProject";
//        BuildOptions option = BuildOptions.CompressWithLz4;
//        var ret = BuildPipeline.BuildPlayer(scenes.ToArray(), fullPath, target, option);
//        AssetDatabase.Refresh();
//        //sdk处理
//        //PlayerBuilder.SDKDeal(ref fullPath, curSdk, false, target);

//        var patchPath = fullPath + "/patch";
//        fullPath = fullPath + "/" + Application.productName;

//        if (Directory.Exists(patchPath))
//            Directory.Delete(patchPath, true);
//        Debug.Log("fullPath " + (fullPath + "/src/main/jniLibs"));
//        if (Directory.Exists(fullPath + "/src/main/jniLibs"))
//        {
//            StringBuilder cmdBuilder = new StringBuilder();
//            var soPath = fullPath + "/src/main/jniLibs";
//            Directory.CreateDirectory(patchPath);
//            Directory.CreateDirectory(patchPath + "/data");
//            Directory.CreateDirectory(patchPath + "/data/assets_bin_Data");

//            File.Copy(Application.dataPath + "/../channel/so_project/fix/zip.exe", patchPath + "/zip.exe");
//            cmdBuilder.Append("cd " + fullPath + "/src/main\n");

//            string[] allDataFiles = Directory.GetFiles(fullPath + "/src/main/assets", "*", SearchOption.AllDirectories);
//            foreach (var dataFile in allDataFiles)
//            {
//                var file = dataFile.Replace("\\", "/");
//                if (!file.Contains("assets/bin/Data"))
//                    continue;

//                string relativePathHeader = "assets/bin/Data/";
//                int relativePathStart = file.IndexOf(relativePathHeader);
//                string filenameInZip = file.Substring(relativePathStart);//assets/bin/Data/xxx/xxx
//                string relativePath = file.Substring(relativePathStart + relativePathHeader.Length).Replace('\\', '/'); //xxx/xxx
//                string zipFileName = relativePath.Replace("/", "__").Replace("\\", "__") + ".bin";//xxx__xxx.bin

//                cmdBuilder.AppendFormat("{0} -8 \"{1}\" \"{2}\"\n", patchPath + "/zip.exe", patchPath + "/data/assets_bin_Data/" + zipFileName, filenameInZip);
//            }
//            cmdBuilder.Append("ping 127.0.0.1 -n 2 >nul\n");
//            cmdBuilder.Append(string.Format("copy {0} {1}\n", soPath + "/arm64-v8a/libil2cpp.so", patchPath + "/data\n"));
//            cmdBuilder.Append(string.Format("ren {0} {1}\n", patchPath + "/data/libil2cpp.so", "libil2cpp.so.new"));
//            cmdBuilder.Append(string.Format("cd {0} && {1} -8 -r \"{2}\" \"{3}\"\n", patchPath + "/data", patchPath + "/zip.exe", patchPath + "/so_patch.v8", "*"));
//            cmdBuilder.Append("ping 127.0.0.1 -n 2 >nul\n");

//            cmdBuilder.Append(string.Format("del {0}\n", patchPath + "/data/libil2cpp.so.new"));
//            cmdBuilder.Append(string.Format("copy {0} {1}\n", soPath + "/armeabi-v7a/libil2cpp.so", patchPath + "/data"));
//            cmdBuilder.Append(string.Format("ren {0} {1}\n", patchPath + "/data/libil2cpp.so", "libil2cpp.so.new"));
//            cmdBuilder.Append(string.Format("cd {0} && {1} -8 -r \"{2}\" \"{3}\"\n", patchPath + "/data", patchPath + "/zip.exe", patchPath + "/so_patch.v7", "*"));
//            File.WriteAllText(patchPath + "/zip.bat", cmdBuilder.ToString().Replace("/", "\\"));
//            var zipBat = new System.Diagnostics.Process();
//            zipBat.StartInfo.WorkingDirectory = patchPath;
//            zipBat.StartInfo.FileName = patchPath + "/zip.bat";
//            zipBat.Start();
//            zipBat.WaitForExit();
//            System.Diagnostics.Process.Start(patchPath);
//        }
//        else
//        {
//            Debug.Log("start fullPath " + fullPath);
//            System.Diagnostics.Process.Start(fullPath);
//        }

//        //Delete copied files
//        if (Directory.Exists(PlayerBuilder.PlayerBuildTempPath))
//            Directory.Delete(PlayerBuilder.PlayerBuildTempPath, true);
//        if (File.Exists(Application.dataPath + "/Plugins/PGameLogic.dll"))
//            File.Delete(Application.dataPath + "/Plugins/PGameLogic.dll");
//        PlayerSettings.productName = productName;
//        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "FIX_FGUI;SKIP_SDK;");
//        EditorUtility.ClearProgressBar();
//        AssetDatabase.Refresh();
//        Debuger.Log("Packaged...", ret);
//    }


//#region Common

//#region Common
//    private void GuiLine(int i_height = 1)
//    {
//        GUILayout.Space(5);
//        Rect rect = EditorGUILayout.GetControlRect(false, i_height);
//        rect.height = i_height;
//        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
//        GUILayout.Space(5);
//    }
//    private void GuiBoldToggle(string text, ref bool isValue, bool isTrue = true)
//    {
//        var origFontStyle = EditorStyles.label.fontStyle;
//        EditorStyles.label.fontStyle = FontStyle.Bold;
//        if (isTrue)
//        {
//            isValue = EditorGUILayout.ToggleLeft(text, isValue);
//        }
//        else
//        {
//            isValue = EditorGUILayout.ToggleLeft(text, !isValue);
//        }
//        EditorStyles.label.fontStyle = origFontStyle;
//    }
//    private void GuiNormalToggle(string text, ref bool isValue, bool isTrue = true)
//    {
//        var origFontStyle = EditorStyles.label.fontStyle;
//        EditorStyles.label.fontStyle = FontStyle.Normal;
//        if (isTrue)
//        {
//            isValue = EditorGUILayout.ToggleLeft(text, isValue);
//        }
//        else
//        {
//            isValue = EditorGUILayout.ToggleLeft(text, !isValue);
//        }
//        EditorStyles.label.fontStyle = origFontStyle;
//    }
//    private void GuiLabelBold(string text)
//    {
//        var origFontStyle = EditorStyles.label.fontStyle;
//        EditorStyles.label.fontStyle = FontStyle.Bold;
//        EditorGUILayout.LabelField(text);
//        EditorStyles.label.fontStyle = origFontStyle;
//    }
//#endregion

//#endregion
//}