using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class EditorBundleWindow : EditorWindow
{
    #region use ab mode
    private static bool useAssetDatabase= true;

    [MenuItem("Tools/AssetBundles/Use Asset Database",false,1)]
    public static void UseAssetDatabase()
    {
        useAssetDatabase = EditorPrefs.GetBool("useAssetDatabase_Enable", true);
        useAssetDatabase = !useAssetDatabase;
        EditorPrefs.SetBool("useAssetDatabase_Enable", useAssetDatabase);
        Menu.SetChecked("Tools/AssetBundles/Use Asset Database", useAssetDatabase);
        //Menu.SetChecked("Tools/AssetBundles/Use Assetbundles", false);

    }
    [MenuItem("Tools/AssetBundles/Use Asset Database", true, 1)]
    public static bool UseAssetDatabaseOption()
    {
        useAssetDatabase = EditorPrefs.GetBool("useAssetDatabase_Enable", true);
        Menu.SetChecked("Tools/AssetBundles/Use Asset Database", useAssetDatabase);
        return true;
    }

    #endregion
    //const
    public static readonly BuildAssetBundleOptions options =
    BuildAssetBundleOptions.DeterministicAssetBundle
    | BuildAssetBundleOptions.ChunkBasedCompression;
    #region Editor
    //params
    public int tabIndex = 0;
    public static EditorBundleWindow window;
    private static bool isUseOtherCommands = false;
    // 3/19/2022 Đại thêm biến
    static List<AssetTableInfo> mAssetbundleTable = new List<AssetTableInfo>();
    static List<AssetBundleVersionItem> mAssetbundleOldVersionList = new List<AssetBundleVersionItem>();
    static List<AssetBundleVersionItem> mAssetbundleVersionList = new List<AssetBundleVersionItem>();
    static List<AssetBundleVersionItem> mAssetbundleUpdated = new List<AssetBundleVersionItem>();
    static List<AssetBundleVersionItem> mAssetbundleWillUpdate = new List<AssetBundleVersionItem>();

    public static PlatformData mPlatform = PlatformData.android_dev;
    static AssetBundle ab = null;
    int pos1, pos2;
    int posPath1, posPath2;
    string s;
    string folderTemp = "Updates";
    string mBaseVersionName = "_forceBaseRes.txt";
    string mversionConfig = "version_config.json";
    string updateFilename = "forceRes.json";
    private string pathFilebase = null;
    private string pathVersionConfig = null;
    private JSONClass jsonVersionConfig;
    public enum PlatformType
    {
        Android = 0,
        IOS,
        Window
    }

    [Serializable]
    class AssetBundleVersionItemRoot
    {
        public string exeVer;
        public string buildVer;
        public List<AssetBundleVersionItem> assets;
    }

    [Serializable]
    class AssetBundleVersionItem
    {
        public string name;
        public string version;
        public string md5;
        public string hashAB;
        public string size;
        public string need;
    }

    [Serializable]
    class AssetTableInfo
    {
        public string namePath;
        public string assetbundleName;
    }

    [MenuItem("Tools/AssetBundles/Open Bundle Editor", false, 2)]
    public static void Init()
    {
        // Get existing open window or if none, make a new one:
        window = (EditorBundleWindow)EditorWindow.GetWindow(typeof(EditorBundleWindow));
        window.name = "Build AB Window";
        window.maxSize = new Vector2(1200, 600);
        window.Show();
    }
    private void StartWindow()
    { 
        
    }
    void OnEnable()
    {
        StartWindow();
        InitVersion();
    }
    void OnGUI()
    {
        bool switchTab = false;
        int oldTab = tabIndex;
        tabIndex = GUILayout.Toolbar(tabIndex, new string[] { "Build AB", "Assign AB" ,"Review AB"});
        if (oldTab != tabIndex)
        {
            switchTab = true;
        }
        switch (tabIndex)
        {
            case 0:
                RenderBuildABTab();
                break;
            case 1:
                RenderAssignTab();
                break;
            case 2:
                ReviewAssetBundle(switchTab);
                break;
        }
    }
    
    private void drawLine(Vector2 begin, Vector2 end)
    {
        Handles.color = Color.gray;
        Handles.DrawLine(begin, end);
        Handles.color = Color.black;

    }
    #endregion
    #region Version Config

    virtual protected void InitVersion()
    {
        reloadVersionConfig();
    }
    #endregion
    #region Build AB Tab
    private static bool isBuildAB = true;
    private static bool excludeStreamingAB = false;
    private static bool buildFullABPatch;
    private static bool buildAppSplit;
    private static bool fullStreamingAsset = false;
    //version 
    static protected int codeVersion = 1;
    static protected string codeVersionName = "1.0";
    static bool encodeDll = false;
    private void RenderBuildABTab()
    {
        EditorGUILayout.LabelField("Current platform", AssetsUtility.GetPlatformName()); ;

        GUILayout.Label("Build Selected Option", EditorStyles.boldLabel);
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("Setting Assetbundle", EditorStyles.boldLabel);
            {
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.LabelField("Build Option", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(isUseOtherCommands);
            {
                {
                    /*
                    EditorGUILayout.BeginVertical();
                    isBuildAB = EditorGUILayout.Toggle(new GUIContent("1.Build Asset Bundle", "Allow Build Asset bundle before other commands"), isBuildAB); ;
                    {
                        buildAppSplit = EditorGUILayout.Toggle(new GUIContent("2.Build AppSplit", "Allow Copy AB to Folder StreamingAsset by Data in \"Asset\\appFiles\""), buildAppSplit);
                        EditorGUI.BeginDisabledGroup(!buildAppSplit);
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.LabelField("Option Build Applsplit", EditorStyles.boldLabel);
                            //fix size toggle
                            float originalValue = EditorGUIUtility.labelWidth;
                            EditorGUIUtility.labelWidth = 200;
                            fullStreamingAsset = EditorGUILayout.Toggle(new GUIContent("Full StreamingAsset", "If True: Ingnore \"AppFiles\" and copy All AB to folder StreamingAssets"), fullStreamingAsset);
                            EditorGUIUtility.labelWidth = originalValue;
                            EditorGUI.indentLevel--;
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    {
                        buildPatchUpdate = EditorGUILayout.Toggle(new GUIContent("3.Build patch PKG", "Allow build Patch Update When AB changed"), buildPatchUpdate);
                        EditorGUI.BeginDisabledGroup(!buildPatchUpdate);
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.LabelField("Option Build Pack", EditorStyles.boldLabel);
                            //fix size toggle
                            float originalValue = EditorGUIUtility.labelWidth;
                            EditorGUIUtility.labelWidth = 200;
                            excludeStreamingAB = EditorGUILayout.Toggle(new GUIContent("Exclude StreamingAB", "Allow Exclude StreamingAB When Compare New AB Version "), excludeStreamingAB);
                            buildFullABPatch = EditorGUILayout.Toggle(new GUIContent("Full AB package", "Allow Ignore Compare Old Version bundle and use All AB for Patch Update"), buildFullABPatch);
                            EditorGUIUtility.labelWidth = originalValue;
                            EditorGUI.indentLevel--;
                        }
                        EditorGUI.EndDisabledGroup();
                    }

                    EditorGUILayout.EndVertical();
                    */
                }
                //build button
                EditorGUILayout.Space();
                encodeDll = EditorGUILayout.Toggle("Encode dll", encodeDll);
                EditorGUILayout.LabelField("Đồng bộ logic và build dll, sau đó copy vào AssetDll", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("1.Synchronized and Build Dll", GUILayout.Width(200)))
                {
                    if (EditorUtility.DisplayDialog("Synchronized and Build Dll", "Do you want Synchronized and Build Dll?", "Yes", "No"))
                    {
                        ProcessSynchronizedAndBuildDll();
                    }
                }
                EditorGUILayout.LabelField("Assign tất cả assignbundle theo folder, dll logic", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("2.Assign All Bundle", GUILayout.Width(200)))
                {
                    if (EditorUtility.DisplayDialog("Assign All Bundle", "Do you want Assign All assets bundle?", "Yes", "No"))
                    {
                        AssignAllBundle();
                    }

                }
                EditorGUILayout.LabelField("Thực hiện build assetbundle sau khi đã assign và đồng bộ", EditorStyles.miniBoldLabel);
                //build button
                if (GUILayout.Button("3.Build Assetbundle", GUILayout.Width(200)))
                {
                    if (EditorUtility.DisplayDialog("Build By Option", "Do you want Build AB with options selected?", "Yes", "No"))
                    {
                        ProcessBuildAssetBundle();
                    }
                }
                //build button
                EditorGUILayout.LabelField("Tiến hành tất cả các bước: build dll và assign bundlename và build asset bundle", EditorStyles.miniBoldLabel);

                if (GUILayout.Button("*.Build All (Ignore dll, Config)", GUILayout.Width(200)))
                {
                    if (EditorUtility.DisplayDialog("Build By Option", "Do you want Build AB with all steps?", "Yes", "No"))
                    {
                        ProcessBuildAssetBundleAllStepIgnoreDll();
                    }
                }

                //build button
                EditorGUILayout.LabelField("Tiến hành tất cả các bước: build dll và assign bundlename và build asset bundle", EditorStyles.miniBoldLabel);

                if (GUILayout.Button("*.Build All Steps", GUILayout.Width(200)))
                {
                    if (EditorUtility.DisplayDialog("Build By Option", "Do you want Build AB with all steps?", "Yes", "No"))
                    {
                        ProcessBuildAssetBundleAllStep();
                    }
                }

                //3/28/2022 Đại create base version for each platform
                EditorGUILayout.Space();
                Rect rect = EditorGUILayout.GetControlRect(false, 1);
                rect.height = 1;
                EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
                EditorGUILayout.Space();
                GUILayout.Label("Create Version Hotupdate:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("1.Platform triển khai hiện tại", EditorStyles.miniBoldLabel);
                var oldPlatform = mPlatform;
                mPlatform = (PlatformData)EditorGUILayout.EnumPopup(mPlatform, GUILayout.Width(200));
                EditorGUILayout.Space();

                //check version with platform
                bool isExistVersionConfig = false;
                if (pathFilebase == null || mPlatform != oldPlatform)
                {
                    pathFilebase = GetOutputPath(mPlatform.ToString()) + "/" + mBaseVersionName;

                }
                if (pathVersionConfig == null || mPlatform != oldPlatform)
                {
                    pathVersionConfig = GetOutputPath(mPlatform.ToString()) + "/" + mversionConfig;
            
                }
                if (mPlatform != oldPlatform)
                {
                    if(File.Exists(pathVersionConfig))
                    {
                        this.reloadVersionConfig();
                    }
                }
                if (File.Exists(pathVersionConfig) ==false)
                {
                    isExistVersionConfig = false;
                    jsonVersionConfig = null;
                    Color oldColor = GUI.color;
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Error: Cần tạo file version config trước khi tạo hotupdate", EditorStyles.miniBoldLabel);
                    GUI.color = oldColor;
                }
                else
                {
                    isExistVersionConfig = true;
                }
                //render version
                if (jsonVersionConfig != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    int currentVersion = jsonVersionConfig["forceRes"].AsInt;
                    EditorGUILayout.IntField("Current Version", currentVersion, GUILayout.Width(200));
                    EditorGUILayout.IntField("Next Update Version", currentVersion+1, GUILayout.Width(200));
                    if (GUILayout.Button("Reload", GUILayout.Width(50)))
                    {
                        this.reloadVersionConfig();

                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("Open file VersionConfig", GUILayout.Width(200)))
                    {
                        Application.OpenURL(pathVersionConfig);
                    }
                    if(GUILayout.Button("Show in Explorer", GUILayout.Width(200))){
                        EditorUtility.RevealInFinder(pathVersionConfig);
                    }

                    EditorGUILayout.EndHorizontal();
                }
               

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("2.Tạo danh sách base trước khi tạo các version để hotupdate, chỉ tạo 1 lần duy nhất cho mỗi plaftorm", EditorStyles.miniBoldLabel);
                Color defaultColor = GUI.color;
                GUI.color = Color.yellow;
                if (File.Exists(pathFilebase)) EditorGUI.BeginDisabledGroup(true);
                else EditorGUI.BeginDisabledGroup(false);

                if (GUILayout.Button("Create Base Version, be careful!", GUILayout.Width(200)))
                {
                    
                    if (EditorUtility.DisplayDialog("Create Base Version!", "Do you want Create?", "Yes", "No"))
                    {
                        ProcessCreateBaseVersion();
                    }
                }
                GUI.color = defaultColor;
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("3.Tạo 1 version mới để triển khai hotupdate", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("Create Version", GUILayout.Width(200)))
                {
                    if (isExistVersionConfig)
                    {
                        if (EditorUtility.DisplayDialog("Create Version", "Do you want Create Version for " + mPlatform.ToString() + "?", "Yes", "No"))
                        {
                            ProcessCreateVersion();
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Lỗi hotupdate", "Vui lòng tạo file versionConfig trước khi hotupdate", "Xác nhận");
                    }

                }

            }
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;

        }

        GUILayout.Space(20);
        {
            var origFontStyle = EditorStyles.label.fontStyle;
            EditorStyles.label.fontStyle = FontStyle.Bold;
            isUseOtherCommands = EditorGUILayout.Toggle("Fast Command", isUseOtherCommands, GUILayout.Width(300));
            EditorStyles.label.fontStyle = origFontStyle;
        }
        EditorGUI.BeginDisabledGroup(!isUseOtherCommands);
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Build AssetBundle", EditorStyles.boldLabel);
                {
                    if (GUILayout.Button(new GUIContent("Build All Bundle", string.Format("Option selected:\n{0}: {1}\n{2}: {3}"
                                        , "2.Build AppSplit", "False", "3.Build patch PKG", "False")), GUILayout.Width(200)))
                    {
                        if (EditorUtility.DisplayDialog("Build All Assetbundle", "Do you want Build All Assetbundle?", "Yes", "No"))
                        {
                            BuildAllAssetBundle();
                            EditorUtility.DisplayDialog("Build All Bundle", "Build AB completed!", "Done");
                        }
                    }

                    if (GUILayout.Button(new GUIContent("Force Rename Manifest", "Force Lowercase name of manifest.Ex:\n\"Window\"->\"window\""), GUILayout.Width(200)))
                    {
                        ForceLowercaseManifestName();
                        //beforeBuildAssetBundle();
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Copy Streaming Assets", EditorStyles.boldLabel);
                {
                    if (GUILayout.Button(new GUIContent("Copy Full StreamingAssets", string.Format("Option selected:\n{0}: {1}"
                                        , "Full StreamingAsset","True")), GUILayout.Width(300)))
                    {
                        if (EditorUtility.DisplayDialog("Copy Full StreamingAssets", "Do you want copy Full AB to StreamingAssets?", "Yes", "No"))
                        {
                            CopyStreamingAssets(true);
                        }
                    }
                    if (GUILayout.Button(new GUIContent("Copy StreamingAssets Use AppSplit", string.Format("Option selected:\n{0}: {1}"
                                        , "Full StreamingAsset","False")), GUILayout.Width(300)))
                    {
                        if (EditorUtility.DisplayDialog("Copy StreamingAssets", "Do you want copy AB to StreamingAssets use AppSplit?", "Yes", "No"))
                        {
                            CopyStreamingAssets(false);
                        }
                    }

                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Build Pack Update", EditorStyles.boldLabel);
                {
                    if (GUILayout.Button(new GUIContent("Build Pack Update",string.Format("Option selected:\n{0}: {1}\n{2}: {3}"
                                        , "Exclude StreamingAB","False", "Full AB package", "False")), GUILayout.Width(200)))
                    {
                        if (EditorUtility.DisplayDialog("Build Pack Update", "Do you want build Pack Update?", "Yes", "No"))
                        {
                            BuildPatchUpdate(false, false);
                        }
                    }

                }
                EditorGUILayout.EndVertical();

            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.EndDisabledGroup();
    }
    #endregion
    #region Assign Tab



    private void RenderAssignTab()
    {
        GUILayout.Label("[Single] Assign bundle in Resources", EditorStyles.boldLabel);
        {
            //3/24/2022 Đại edit tool
            //rootPath = EditorGUILayout.TextField("Root path", rootPath);
            //if(GUILayout.Button("Chose folder"))
            //{
            //    string path = EditorUtility.OpenFolderPanel("Load folder", "Assets", "");
            //    int frontPos = Application.dataPath.IndexOf("Assets");
            //    rootPath = path.Substring(frontPos);
            //    Debug.Log("update gui:" + rootPath);
            //}
            
            EditorGUILayout.TextField("Special File", specialFile);
            EditorGUILayout.TextField("Special Folder", specialFolder);

        }


        EditorGUILayout.Space();

        GUILayout.Label("Assign Commands", EditorStyles.boldLabel);
        {
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Remove All Bundle", GUILayout.Width(200)))
                {
                    if (EditorUtility.DisplayDialog("Delete Bundle", "Do you want Remove All assets bundle?", "Yes", "No"))
                    {
                        ForceRemoveAllBundle(true);
                    }
                }

                if (GUILayout.Button("Assign All Bundle", GUILayout.Width(200)))
                {
                    if (EditorUtility.DisplayDialog("Assign All Bundle", "Do you want Assign All assets bundle?", "Yes", "No"))
                    {
                        AssignAllBundle();
                    }

                }

                if (GUILayout.Button("Re-Assign All Bundle", GUILayout.Width(200)))
                {
                    if (EditorUtility.DisplayDialog("Re-Assign All Bundle", "Do you want clear and re-assign All assets bundle?", "Yes", "No"))
                    {
                        ForceRemoveAllBundle(false);
                        AssignAllBundle();
                    }

                }
                if (GUILayout.Button("Test All Bundle Group UI", GUILayout.Width(200)))
                {
                    if (EditorUtility.DisplayDialog("Re-Assign All Bundle", "Do you want clear and re-assign All Bundle Group UI?", "Yes", "No"))
                    {
                        AssignGroupCommonBundles();
                    }

                }
                if (GUILayout.Button("Remove Unused AB Names", GUILayout.Width(200)))
                {
                    AssetDatabase.RemoveUnusedAssetBundleNames();
                }
                EditorGUILayout.EndHorizontal();

            }
        }
        EditorGUILayout.Space();
    }
    #endregion
    #region Assign Bundle Name
    //3/24/2022 Đại sửa biến 
    //public static string rootPath = "Assets/Resources";
    public static string pathRoot = "Assets/ArtResources/OutPut";
    public static string pathRootBin = "Assets/Bin";
    public static string specialFile = "UIAssetsFolderStructure";
    public static string specialFolder = "GameScenes/pvp_001,GameScenes/pvp_002,GameScenes/pvp_099,Map/1001,Map/1002,UIAssetsFolderStructure";//,Map/1001,Map/1002,Cube
    public static string folderIgnoreAssignBundle = "UI/back,UI/force,UI/guoshen,UI/ios_force";

    //public static string folderIgnoreAssignBundle = "Scenes/10000_com,Scenes/10002_com,Scenes/10003_com,Scenes/10004_com,Scenes/10005_com,Scenes/10007_com" +
    //            ",Scenes/10008_com,Scenes/10010_com,Scenes/10011_com,Scenes/10012_com,Scenes/10015_com,Scenes/10016_com";
    // ingonre folder : Lua

    //3/29/2022 Đại sửa hàm
    private static List<string> listAssignName = new List<string>();
    private static int AssignAllBundleName(bool forceOnlyBuild =false, string forceOnlyFolder ="")
    {
        listAssignName.Clear();
        //split folder
        string[] folderIgnore = folderIgnoreAssignBundle.Split(',');
        string[] specialFileList = specialFile.Split(',');
        string[] specialFolderList = specialFolder.Split(',');
        //foreach(var m in folderIgnore)
        //{
        //    Debug.Log("kwang specialFolderList " + m);
        //}
        //
        var rootFind1 = forceOnlyBuild ? pathRoot + "/" + forceOnlyFolder : pathRoot;
        string[] assetList = AssetDatabase.FindAssets("t:Object", new string[] { rootFind1, pathRootBin, }); //{ rootFind1, rootFind2, rootFind3 }
        Debug.Log("Assets length " + assetList.Length);

        float countAsset = 0;
        float maxLengthAsset = assetList.Length;
        int realAssetAssign = 0;
        foreach (var guid in assetList)
        {
  
            countAsset++;
            EditorUtility.DisplayProgressBar("Assign All Bundle", string.Format("Assigning {0}/{1}", countAsset, maxLengthAsset), (countAsset / maxLengthAsset));

            var path = AssetDatabase.GUIDToAssetPath(guid);

            bool isValid = true;

            foreach (var file in folderIgnore)
            {
                if (path.Contains(file))
                {
                    isValid = false;
                    break;
                }

            }
            foreach(var file in specialFolderList)
            {
                if (path.Contains(file))
                {
                    isValid = false;
                    break;
                }
            }
            foreach (var file in specialFileList)
            {
                if (path.Contains(file))
                {
                    isValid = false;
                    break;
                }
            }

            // Trình add code 10/10/2023.
            // Bỏ qua việc gán Bundle name cho các thư mục con của new_scene.
            if (path.Contains("Assets/ArtResources/OutPut/Scene/new_scene/"))
            {
                if (path.Replace("Assets/ArtResources/OutPut/Scene/new_scene/", string.Empty).Contains("/"))
                {
                    //Debug.Log("<color=red>Bỏ qua: " + path + "</color>");
                    continue;
                }
            }


            if (isValid)
            {
                {
                    //cut path
                    string[] bundleName =  path.Replace(pathRoot + "/", "").Split('.');
                    if (bundleName.Length == 1)
                    {
                        //Debug.Log("Folder Asset " + path);
                    }
                    else if (bundleName.Length == 2)
                    {
                        realAssetAssign++;
                        //
                        var names = bundleName[0].Split('/');
                        var mainName = names[names.Length - 1];
                        //if(listAssignName.Contains(mainName) == false)
                        {
                            listAssignName.Add(mainName);
                            //
                            AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant(mainName.ToLower(), null);
                            //Debug.Log("Assign Asset name " + path + " bundle Name " + bundleName[0]);
                        }
                        //else
                        //{
                        //    Debuger.Log("Kwang trung ten:" + path);
                        //}

                    }
                    else if (bundleName.Length >= 2)
                    {
                        Debuger.Log("Kwang lot vao day " + bundleName);
                        realAssetAssign++;
                        //combine path
                        string fullName = "";
                        for (int i = 0; i < bundleName.Length - 1; i++)
                        {
                            fullName += i == 0 ? bundleName[i] : "." + bundleName[i];
                        }
                        AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant(fullName.ToLower(), null);
                    }

                }
            }
            else
            {
                //Debug.LogError("Asset name invalid" + path);
            }

        }
        EditorUtility.ClearProgressBar();
        return realAssetAssign;
    }
    //group file assign
    private static int AssignGHAtlas()
    {
        string atlasPaths = Application.dataPath + "/UI/atlases/";
        string[] dirs = Directory.GetDirectories(atlasPaths, "*", SearchOption.TopDirectoryOnly);
        //string mainPrefab = "UI_{0}Panel.prefab";
        int realAssetAssign = 0;
        int maxLengthAsset = dirs.Length;
        int countAsset = 0;
        Debug.Log("dirs " + dirs.Length);
        for (int i = dirs.Length - 1; i >= 0; i--)
        {
            countAsset++;
            if (countAsset % 10 == 0 || countAsset == 1)
            {
                EditorUtility.DisplayProgressBar("Assign GH Atlas ", string.Format("Assigning {0}/{1}", countAsset, maxLengthAsset), (float)countAsset / maxLengthAsset);
            }

            string fi = dirs[i];
            if (fi.Contains("ModuleAtlas")) continue;
            string atlasName = Path.GetFileName(fi);

            string atlasPath = "Assets/UI/atlases/" + atlasName+"/"+ atlasName + ".prefab";
            string toAtlasPath = BuildUtil.moduleAtlas + atlasName;
            Debug.Log("atlasPath " + atlasPath + "  , toAtlasPath: " + toAtlasPath);
            BuildSingleBundle(atlasPath, toAtlasPath, ref realAssetAssign);
        }
        EditorUtility.ClearProgressBar();
        return realAssetAssign;

    }
    private static void BuildSingleBundle(string rootPath, string groupName, ref int countAssets)
    {
        UnityEngine.Object obj = null;

        if (Core.FileTool.IsExistFile(rootPath))
        {
            obj = AssetDatabase.LoadMainAssetAtPath(rootPath);
        }
        float countAsset = 0;
        if (obj != null)
        {
            countAsset++;
            //cut path
            string[] bundleName = rootPath.Split('.');
            if (bundleName.Length == 1)
            {
                //Debug.Log("Folder Asset " + path);
            }
            else if (bundleName.Length == 2)
            {
                countAssets++;
                AssetImporter.GetAtPath(rootPath).SetAssetBundleNameAndVariant(groupName.ToLower(), null);
            }
            else if (bundleName.Length >= 2)
            {
                countAssets++;
                AssetImporter.GetAtPath(rootPath).SetAssetBundleNameAndVariant(groupName.ToLower(), null);
                Debug.LogError("Error Name " + rootPath);
            }
        }
    }
    //
    private static void buildBundleLogicDll()
    {
        string androidPath = (PathUtil.DestDirAndroid + PathUtil.DllScriptBundleName).Replace(Application.dataPath, "Assets");

        switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                AssetImporter.GetAtPath(androidPath)
                    .SetAssetBundleNameAndVariant(PathUtil.DllScriptAssignBundleName, null);
                
                break;
        }
    }
    //
    struct CommonBundle
    {
        public CommonBundle(string assignFolder, string readPath)
        {
            this.AssignPathName = assignFolder;
            this.ReadRootPath = readPath;
        }
        public string AssignPathName;
        public string ReadRootPath;
    }
    private static CommonBundle[] commonBundles = new CommonBundle[]
    {
        new CommonBundle("configgame","Assets/AssetDll/ConfigGame"),

        new CommonBundle("gameshaderall","Assets/ArtResources/ArtBase/Shaders"),

    };
    private static string shaderlistPath = "Assets/ArtResources/OutPut/Other/shader_list.prefab";

    private static int AssignGroupCommonBundles()
    {
        int realAssetAssign = 0;
        string uiPathroot = pathRoot + "/UI";
        DirectoryInfo info = new DirectoryInfo(uiPathroot);
        var files = info.GetFiles("*", SearchOption.TopDirectoryOnly);
        foreach (var folder in files)
        {
            string file  = folder.FullName.Replace("\\", "/");
            //Debuger.Log("kwang file:" + file);

            if (!file.Contains("UI/UIWindow") && !file.Contains("UI/UISingle"))
            {
                var child = file.Replace(".meta", "");
                DirectoryInfo infoUI = new DirectoryInfo(child);
                var allFiles = infoUI.GetFiles("*", SearchOption.TopDirectoryOnly);
                foreach(var chilFile in allFiles)
                {
                    //Debuger.Log("chilFile.DirectoryName: "+chilFile.Name.Replace(".meta", "") + "====kwang child:" + chilFile.FullName.Replace("\\", "/").Replace(".meta", "").Replace(Application.dataPath, "Assets"));
                    BuildGroupBundle(chilFile.FullName.Replace("\\", "/").Replace(".meta", "").Replace(Application.dataPath, "Assets"),
                        chilFile.Name.Replace(".meta", ""),
                        ref realAssetAssign);
                }
            }
        }
        //var shaderObj = (ShaderList)AssetDatabase.LoadAssetAtPath(shaderlistPath, typeof(ShaderList));
        //foreach(var shader in shaderObj.shaders)
        //{
        //    var path = AssetDatabase.GetAssetPath(shader);
        //    //Debuger.Log("kwang path: " + path);
        //    AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant("shaderpack", null);
        //}
        foreach (var commonF in commonBundles)
        {
            BuildGroupBundle(commonF.ReadRootPath, commonF.AssignPathName, ref realAssetAssign);
        }
        //EditorUtility.DisplayDialog("Assign Group common", string.Format("{0}: {1} {2}", "Assign Completed", realAssetAssign, "assets"), "Done");
        return realAssetAssign;
    }
    private static void BuildGroupBundle(string rootPath, string groupName, ref int countAssets)
    {
        string[] assetList = AssetDatabase.FindAssets("t:Object", new string[] { rootPath });
        //Debug.Log(rootPath + " ,Assets length " + assetList.Length);

        float countAsset = 0;
        float maxLengthAsset = assetList.Length;
        foreach (var guid in assetList)
        {
            countAsset++;
            EditorUtility.DisplayProgressBar("Assign Group " + groupName, string.Format("Assigning {0}/{1}", countAsset, maxLengthAsset), (countAsset / maxLengthAsset));

            var path = AssetDatabase.GUIDToAssetPath(guid);
            {
                //cut path
                string[] bundleName = path.Split('.');
                if (bundleName.Length == 1)
                {
                    //Debug.Log("Folder Asset " + path);
                }
                else if (bundleName.Length == 2)
                {
                    countAssets++;
                    AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant(groupName.ToLower(), null);
                }
                else if (bundleName.Length >= 2)
                {
                    countAssets++;
                    AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant(groupName.ToLower(), null);
                    Debug.LogError("Error Name " + path);
                }

            }
        }
        EditorUtility.ClearProgressBar();
    }

    #endregion

    #region Assign bundle name
    [MenuItem("Assets/GH/Remove Bundle Name")]
    public static void resetDefaultPlayerNolightKeywords()
    {
        /*  int countFixed = 0;

          UnityEngine.Object[] obs = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
          foreach (var obj in obs)
          {
              var prefabPath = AssetDatabase.GetAssetPath(obj);
              var assetBundleName = AssetDatabase.GetImplicitAssetBundleName(prefabPath);
              AssetDatabase.RemoveAssetBundleName(assetBundleName, true);
          }
          AssetDatabase.RemoveUnusedAssetBundleNames();

          AssetDatabase.SaveAssets();
          AssetDatabase.Refresh();*/
        UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        //Debug.Log("objs count " + objs.Length);
        foreach (var obj in objs)
        {

            var path = AssetDatabase.GetAssetPath(obj);

            var import = AssetImporter.GetAtPath(path);
            //Debug.Log("objs path " + path + " name: " + import.assetBundleName);
            if (string.IsNullOrEmpty(path) == false)
                import.SetAssetBundleNameAndVariant(null, null);
        }
        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();
        return;

    }
    private static void ForceRemoveAllBundle(bool showResult)
    {
        var bundleList = AssetDatabase.GetAllAssetBundleNames();
        int index = 0;
        int maxLength = bundleList.Length;
        while (index < bundleList.Length)
        {
            EditorUtility.DisplayProgressBar("Remove All AssetBundle ", string.Format("Removing {0}/{1}", index, maxLength), ((float)index / maxLength));

            var name = bundleList[index];
            //Debug.Log("Check bundle name: " + name);
            AssetDatabase.RemoveAssetBundleName(name, true);
            index++;
        }
        AssetDatabase.RemoveUnusedAssetBundleNames();
        EditorUtility.ClearProgressBar();
        if (showResult)
        {
            EditorUtility.DisplayDialog("Remove All AssetsBundle", string.Format("{0}: {1} {2}", "Removed:", maxLength, "assets"), "Done");
        }

    }

    private static void AssignAllBundle(bool isShowDialog = true)
    {
        //duong check copy lua to assetreources before assign
        //begin assign
        //create preloadfiles
        int resourceCount = 0;
        //resourceCount = AssignAllBundleName();
        //int commonCount = AssignGroupCommonBundles();
        //assign dll logic 
        buildBundleLogicDll();
        //int commonCount = 0;
        //int commonSceneCount = this.AssignCommonScenes();
        //int singleBundleCount = AssignGHAtlas();
        if (isShowDialog)
        {
            EditorUtility.DisplayDialog("Assign All Bundle", string.Format("{0}: {1} {2} \n{3}: {4} {5}"
      , "Assign Resource", resourceCount, "assets"
      , "Assign Common", " 0 ", "assets"
      ), "Done");
        }
  
    }

    private static CommonBundle luaBundle = new CommonBundle("Lua/Lua", "Assets/Resources/Lua");
    private static void AssignLuabundleWhenBuild()
    {
        int countLuaFile = 0;
        //BuildGroupBundle(luaBundle.ReadRootPath, luaBundle.AssignPathName, ref countLuaFile);

    }
    #endregion

    #region Build Asset Bundle Option
    public void ProcessBuildAssetBundleAllStep()
    {
        //Delete config folder
        if (Directory.Exists(PlayerBuilder.GameConfigPath))
            Directory.Delete(PlayerBuilder.GameConfigPath, true);

        //1.build dll
        ProcessSynchronizedAndBuildDll();
        //build configgame
        PlayerBuilder.MakeConfigGame();
        //
        AssetDatabase.Refresh();

        //2.assign bundle
        AssignAllBundle(false);
        AssetDatabase.Refresh();
        //build ab
        long time = BuildUtil.time;
        if (isBuildAB)
        {
            if (Directory.Exists(PlayerBuilder.GameConfigPath))
            {
                BuildAllAssetBundle();

            }
            else
            {
                EditorUtility.DisplayDialog("Lỗi chưa build configame","Chưa build configGame hoặc chọn Build All Step","Done");

            }
        }
        //Delete config folder
        //if (Directory.Exists(PlayerBuilder.GameConfigPath))
        //    Directory.Delete(PlayerBuilder.GameConfigPath, true);
        AssetDatabase.Refresh();

        //if (buildPatchUpdate)
        //{
        //    BuildPatchUpdate(buildFullABPatch, excludeStreamingAB);
        //}

        EditorUtility.DisplayDialog("Process Build Asset Bundle Option", "Build bundle completed in " + (BuildUtil.time - time) / 1000 + "s", "Done");
    }
    public void ProcessBuildAssetBundleAllStepIgnoreDll()
    {
        //1.build dll
        //ProcessSynchronizedAndBuildDll();
        //build configgame
        //PlayerBuilder.MakeConfigGame();
        //
        //AssetDatabase.Refresh();

        //2.assign bundle
        AssignAllBundle(false);
        AssetDatabase.Refresh();
        //build ab
        long time = BuildUtil.time;
        if (isBuildAB)
        {
            if (Directory.Exists(PlayerBuilder.GameConfigPath))
            {
                BuildAllAssetBundle();

            }
            else
            {
                EditorUtility.DisplayDialog("Lỗi chưa build configame", "Chưa build configGame hoặc chọn Build All Step", "Done");

            }
        }
        //Delete config folder
        //if (Directory.Exists(PlayerBuilder.GameConfigPath))
        //    Directory.Delete(PlayerBuilder.GameConfigPath, true);
        AssetDatabase.Refresh();

        //if (buildPatchUpdate)
        //{
        //    BuildPatchUpdate(buildFullABPatch, excludeStreamingAB);
        //}

        EditorUtility.DisplayDialog("Process Build Asset Bundle Option", "Build bundle completed in " + (BuildUtil.time - time) / 1000 + "s", "Done");
    }
    public void ProcessBuildAssetBundle()
    {
        long time = BuildUtil.time;
        if (isBuildAB)
        {
            BuildAllAssetBundle();
        }

        //if (buildPatchUpdate)
        //{
        //    BuildPatchUpdate(buildFullABPatch, excludeStreamingAB);
        //}

        EditorUtility.DisplayDialog("Process Build Asset Bundle Option", "Build bundle completed in " + (BuildUtil.time - time) / 1000 + "s", "Done");
    }

    //3/28/2022 Đại thêm hàm
    void ProcessCreateBaseVersion()
    {
        //Check if base exist, if exist will ask user
        //string pathFilebase = GetOutputPath(mPlatform.ToString()) + "/" + mBaseVersionName;
        //if (File.Exists(pathFilebase))
        //{
        //    if (EditorUtility.DisplayDialog("File exist", "File base is exist. Do you want re create?", "Yes", "No"))
        //    {
        //        MakeVersion();
        //        CreateBaseVersion(GetOutputPath(mPlatform.ToString()), mBaseVersionName);
        //    }
        //}
        //else
        //{
        //    MakeVersion();
        //    CreateBaseVersion(GetOutputPath(mPlatform.ToString()), mBaseVersionName);
        //}
        MakeVersion();
        CreateBaseVersion(GetOutputPath(mPlatform.ToString()), mBaseVersionName);
    }

    void CreateBaseVersion(string path, string fileName)
    {
        EditorUtility.DisplayProgressBar("Creating", "Creating base version...", 0.5f);
        Dictionary<string, object> data = new Dictionary<string, object>();
        //data["exeVer"] = GameVersion.EXE;
        data["exeVer"] = mPlatform.ToString();
        data["buildVer"] = GameVersion.BUILD;
        List<Dictionary<string, object>> dataAssets = new List<Dictionary<string, object>>();
        foreach (var item in mAssetbundleVersionList)
        {
            Dictionary<string, object> abvi = new Dictionary<string, object>();
            abvi["name"] = item.name;
            abvi["version"] = item.version;
            abvi["md5"] = item.md5;
            abvi["hashAB"] = item.hashAB;
            abvi["size"] = item.size;
            abvi["need"] = item.need;
            dataAssets.Add(abvi);
        }
        data["assets"] = dataAssets;
        AssetsUtility.EasySave(path + "/", fileName, false, delegate (Stream stream, string filename)
        {
            byte[] b = System.Text.Encoding.UTF8.GetBytes(OurMiniJSON.Json.Serialize(data));
            stream.Write(b, 0, b.Length);
            return true;
        });
        EditorUtility.ClearProgressBar();
    }

    //3/19/2022 Đại thêm version
    List<AssetBundleVersionItem> assetsNeedCopy = new List<AssetBundleVersionItem>();
    string pathOutput = "";
    string sVersion = "";
    string pathFileBase = "";

    void ProcessCreateVersion()
    {
        //if base not exist will create, else return
        pathFileBase = GetOutputPath(mPlatform.ToString()) + "/" + mBaseVersionName;
        if (!File.Exists(pathFileBase))
        {
            if (EditorUtility.DisplayDialog("File base not exist", "Do you want create?", "Yes", "No"))
            {
                ProcessCreateBaseVersion();
            }
            else { return; }
        }

        MakeVersion();
        CompareVersion();
        SaveAndCopyVersion();
        EditorUtility.DisplayDialog("Create Version", "Completed!", "Done");
    }

    void MakeVersion()
    {
        //Clear
        assetsNeedCopy = new List<AssetBundleVersionItem>();
        mAssetbundleVersionList.Clear();
        mAssetbundleUpdated.Clear();
        mAssetbundleOldVersionList.Clear();
        mAssetbundleWillUpdate.Clear();
        pathOutput = GetOutputPath(mPlatform.ToString()) + "/";

        ab = AssetBundle.LoadFromFile(AssetsUtility.GetCachePlatformPath() + AssetsUtility.ASSETBUNDLE);
        sVersion = "";

        AssetBundleManifest abm = null;

        if (ab == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }
        else
        {
            abm = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        AssetBundle.UnloadAllAssetBundles(false);

        if (abm != null)
        {
            string[] abNameall = abm.GetAllAssetBundles();
            //Add ab root if not exist
            List<string> termsList = new List<string>();
            for (int runs = 0; runs < abNameall.Length; runs++)
            {
                termsList.Add(abNameall[runs]);
            }
            if (termsList.FindAll(m => m == AssetsUtility.ASSETBUNDLE).Count <= 0)
            {
                termsList.Add(AssetsUtility.ASSETBUNDLE);
            }

            abNameall = termsList.ToArray();

            foreach (var item in abNameall)
            {
                //get ab from name, and load hash
                string hashS = "";
                string hashAB = "";

                byte[] b = null;
                if (AssetsUtility.EasyLoad(AssetsUtility.GetCachePlatformPath(), item, false, delegate (Stream stream, string filename)
                {
                    b = new byte[stream.Length];
                    stream.Read(b, 0, b.Length);

                    //if (item != AssetsUtility.ASSETBUNDLE)
                    //{
                    //    hashS = abm.GetAssetBundleHash(item).ToString();
                    //    hashAB = AssetsUtility.GetMd5Hash(b);


                    //}

                    //else
                    //{
                    //    hashS = AssetsUtility.GetMd5Hash(b);
                    //    hashAB = hashS;
                    //}
                    //su dung md5 get hash ab
                    hashS = AssetsUtility.GetMd5Hash(b);
                    hashAB = hashS;

                    return true;
                }))
                {
                    int year = System.DateTime.Now.Year - 2000;
                    sVersion = year.ToString() +
                        System.DateTime.Now.Month.ToString("d2") +
                        System.DateTime.Now.Day.ToString("d2") +
                        System.DateTime.Now.Hour.ToString("d2") +
                        System.DateTime.Now.Minute.ToString("d2");
                    AssetBundleVersionItem itemVersion = new AssetBundleVersionItem();
                    itemVersion.name = item;
                    itemVersion.md5 = hashS;
                    itemVersion.hashAB = hashAB;
                    itemVersion.version = sVersion;
                    itemVersion.size = b.Length.ToString();
                    itemVersion.need = "1";
                    mAssetbundleVersionList.Add(itemVersion);
                    //add to table list
                    mAssetbundleTable.Add(new AssetTableInfo() { namePath = item, assetbundleName = item });
                }
            }
        }
        else
        {
            Debug.Log("failed to load assetbundle!");
            return;
        }

    }

    void CompareVersion()
    {
        //check folder version, and get version newest
        //string pathVersionNewest = "";
        if (!Directory.Exists(pathOutput))
        {
            Directory.CreateDirectory(pathOutput);
        }

        ////load from newest version
        if (!string.IsNullOrEmpty(pathFileBase))
        {
            string s = null;
            if (AssetsUtility.EasyLoad(pathOutput, mBaseVersionName, false, delegate (Stream stream, string filename)
            {
                s = AssetsUtility.GetStreamedString(stream);
                if (!string.IsNullOrEmpty(s))
                {
                    return true;
                }
                return false;
            }))
            {
                var assetOldTable = JsonUtility.FromJson<AssetBundleVersionItemRoot>(s);
                mAssetbundleOldVersionList.AddRange(assetOldTable.assets);
            }

            //get update list
            string s2= null;
            if (AssetsUtility.EasyLoad(pathOutput, updateFilename, false, delegate (Stream stream, string filename)
            {
                s2 = AssetsUtility.GetStreamedString(stream);
                if (!string.IsNullOrEmpty(s2))
                {
                    return true;
                }
                return false;
            }))
            {
                var assetOldTable = JsonUtility.FromJson<AssetBundleVersionItemRoot>(s2);
                mAssetbundleUpdated= assetOldTable.assets;
            }
        }
        Debug.Log("mAssetbundleOldVersionList: " + mAssetbundleOldVersionList.Count);
        Debug.Log("mAssetbundleUpdated: " + mAssetbundleUpdated.Count);

        //Compare version , and copy ab newest, or changed to output path
        for (int i = 0; i < mAssetbundleVersionList.Count; i++)
        {
            bool isExistVersion = false; // kiểm tra xem có tồn tại ở base không
            bool hasHashChange = false; // kiểm tra xem có thay đổi so với base hoặc những lần update trước đó
            bool isNewABUpdated = false;// kiểm tra xem nếu có update thì có phải là asset mới không
                                         // , hay không thay đổi so với những lần update trước đó
            //Kiểm tra xem assetbundle này có nằm trong danh sách base ko
            //Nếu có thì kiểm tra xem hash có khác không, nếu khác thì asset này đã bị thay đổi
            for (int j = 0; j < mAssetbundleOldVersionList.Count; j++)
            {
                if (mAssetbundleVersionList[i].name == mAssetbundleOldVersionList[j].name)
                {
                    isExistVersion = true;
                    if (mAssetbundleVersionList[i].hashAB != mAssetbundleOldVersionList[j].hashAB)
                    {
                        hasHashChange = true;
                        break;
                    }
                }
            }
            //Trường hợp nằm trong danh sách base thì kiểm tra xem có nằm trong danh sách hotupdate(đã bị thay đổi)
            //Fix lỗi cho TH assetbundle(AB) _a1 sau khi đổi thành a1, nhưng lần 2 thì revert lại thành _a1
            //Nếu chỉ kiểm tra vs base, thì _a sẽ không được update và trên device vẫn là _a1
            if(isExistVersion == true )
            {
              
                foreach (var abUpdated in mAssetbundleUpdated)
                {
                    // Trường hợp nếu ko thay đổi vs base, kiểm tra có nằm trong sách update lần nào chưa
                    if (hasHashChange == false)
                    {
                        if (abUpdated.name == mAssetbundleVersionList[i].name)
                        {
                            hasHashChange = true;
                            break;
                        }
                    }
                    
                }
                //Trường hợp có thay đổi vs base hoặc có trong update, thì kiểm tra lại có là ab mới không
                if (hasHashChange)
                {
                    //force isNewABUpdated = true 
                    isNewABUpdated = true;
                    //loại bỏ các ab đã update những lần trước nhưng không thay đổi bản này
                    foreach (var abUpdated in mAssetbundleUpdated)
                    {
                        if (abUpdated.name == mAssetbundleVersionList[i].name)
                        {
                            if(abUpdated.hashAB == mAssetbundleVersionList[i].hashAB)
                            {
                                isNewABUpdated = false;
                            }
                            break;
                        }
                    }
                }


            }

            if (isExistVersion)
            {
                if (hasHashChange)
                {
                    AssetBundleVersionItem abvi = new AssetBundleVersionItem();
                    abvi.name = mAssetbundleVersionList[i].name;
                    abvi.version = mAssetbundleVersionList[i].version;
                    abvi.md5 = mAssetbundleVersionList[i].md5;
                    abvi.hashAB = mAssetbundleVersionList[i].hashAB;
                    abvi.size = mAssetbundleVersionList[i].size;
                    abvi.need = mAssetbundleVersionList[i].need;
                    if(isNewABUpdated)
                    {
                        assetsNeedCopy.Add(abvi);
                    }
                    mAssetbundleWillUpdate.Add(abvi);
                }
            }
            else
            {
                AssetBundleVersionItem abvi = new AssetBundleVersionItem();
                abvi.name = mAssetbundleVersionList[i].name;
                abvi.version = mAssetbundleVersionList[i].version;
                abvi.md5 = mAssetbundleVersionList[i].md5;
                abvi.hashAB = mAssetbundleVersionList[i].hashAB;
                abvi.size = mAssetbundleVersionList[i].size;
                abvi.need = mAssetbundleVersionList[i].need;

                assetsNeedCopy.Add(abvi);
                mAssetbundleWillUpdate.Add(abvi);

            }
        }
        mAssetbundleUpdated = mAssetbundleWillUpdate;
    }

    void SaveAndCopyVersion()
    {
        //Save base version, and version from needcopy 
        Dictionary<string, object> dataBase = new Dictionary<string, object>();
        Dictionary<string, object> dataVersion = new Dictionary<string, object>();
        dataBase["exeVer"] = mPlatform.ToString();
        dataBase["buildVer"] = GameVersion.BUILD;
        dataVersion["exeVer"] = mPlatform.ToString();
        dataVersion["buildVer"] = GameVersion.BUILD;
        List<Dictionary<string, object>> dataAssetsBase = new List<Dictionary<string, object>>();
        List<Dictionary<string, object>> dataAssetsVersion = new List<Dictionary<string, object>>();
        foreach (var item in mAssetbundleUpdated)
        {
            Dictionary<string, object> abvi = new Dictionary<string, object>();
            abvi["name"] = item.name;
            abvi["version"] = item.version;
            abvi["md5"] = item.md5;
            abvi["hashAB"] = item.hashAB;
            abvi["size"] = item.size;
            abvi["need"] = item.need;
            dataAssetsBase.Add(abvi);
        }
        dataBase["assets"] = dataAssetsBase;

        foreach (var item in assetsNeedCopy)
        {
            Dictionary<string, object> abvi = new Dictionary<string, object>();
            abvi["name"] = item.name;
            abvi["version"] = item.version;
            abvi["md5"] = item.md5;
            abvi["hashAB"] = item.hashAB;
            abvi["size"] = item.size;
            abvi["need"] = item.need;
            dataAssetsVersion.Add(abvi);
        }
        dataVersion["assets"] = dataAssetsVersion;
        AssetsUtility.EasySave(pathOutput, updateFilename, false, delegate (Stream stream, string filename)
        {
            byte[] b = System.Text.Encoding.UTF8.GetBytes(OurMiniJSON.Json.Serialize(dataBase));
            stream.Write(b, 0, b.Length);
            return true;
        });
        //Duong ko can save path o b29
        /*
        AssetsUtility.EasySave(pathOutput, sVersion + "." + AssetsUtility.ASSETVERSIONS, false, delegate (Stream stream, string filename)
        {
            byte[] b = System.Text.Encoding.UTF8.GetBytes(OurMiniJSON.Json.Serialize(dataVersion));
            stream.Write(b, 0, b.Length);
            return true;
        });
        SavePathTable(sVersion, pathOutput);
        */
        //copy ab change to output
        float ii = 1;
        float total = assetsNeedCopy.Count;
        //Duong them 1 folder trung gian de ignore ab

        string path = pathOutput+"abHotupdate_"+ (jsonVersionConfig["forceRes"].AsInt + 1)+ "/";
        string sourcepath = AssetsUtility.GetCachePlatformPath();

        foreach (var item in assetsNeedCopy)
        {
            ii++;
            string target = item.name;
            if (item.name == AssetsUtility.ASSETBUNDLE)
            {
                target = AssetsUtility.ASSETBUNDLE;// + ".unity3d";
            }
            string nameNewFront, nameNewEnd, pathDest, pathDestManifest, pathFolder;
            pathDest = path +target;
            //pathDestManifest = pathDest + ".manifest";
            if (target.Contains("/"))
            {
                nameNewFront = target.Substring(0, target.LastIndexOf("/") + 1);
                nameNewEnd = target.Substring(nameNewFront.Length);
                //pathDest = path + nameNewFront + item.version + "." + nameNewEnd;
                pathFolder = path + nameNewFront;
            }
            else
                pathFolder = path;

            EditorUtility.DisplayProgressBar("Resource processing", "Copy files to version directory ", ii / total);
            if (!Directory.Exists(pathFolder))
            {
                // Try to create the directory.
                Directory.CreateDirectory(pathFolder);
            }
            if (File.Exists(pathDest))
            {
                File.Delete(pathDest);
            }
            File.Copy(sourcepath + item.name, pathDest, true);
            //if (File.Exists(pathDest + ".manifest"))
            //{
            //    File.Delete(pathDest + ".manifest");
            //}
            //File.Copy(sourcepath + item.name + ".manifest", pathDestManifest, true);
        }
        EditorUtility.ClearProgressBar();

        if (ab != null) ab.Unload(false);

        //save versionConfig
        if (jsonVersionConfig != null)
        {
            int currentVersion = jsonVersionConfig["forceRes"].AsInt;
            jsonVersionConfig["forceRes"] = (++currentVersion).ToString();

            File.WriteAllText(this.pathVersionConfig, jsonVersionConfig.ToString(""));

            this.reloadVersionConfig();
        }
    }

    public static string GetOutputPath(string platform = "")
    {

        //string path = Application.dataPath +"/../";
        string path = Application.dataPath.Replace("Assets","");
        //path = path.Substring(0, path.LastIndexOf('/'));
        //path = path.Substring(0, path.LastIndexOf('/') + 1);

        path +=
#if UNITY_ANDROID
     "assetbundles_android";
#elif UNITY_IOS
     "assetbundles_ios"; 
#else
     "assetbundles_windows";
#endif

        if (platform != "")
        {
            path += "/" + platform;
        }

        return path + "/" + GameVersion.BUILD;
    }

    void SavePathTable(string sVersion, string path)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        foreach (var item in mAssetbundleTable)
        {
            item.namePath = item.namePath.ToLower();
            data[item.namePath.Contains(".") ? item.namePath.Substring(0, item.namePath.LastIndexOf(".")) : item.namePath] = item.assetbundleName;
        }
        AssetsUtility.EasySave(path, sVersion + "." + AssetsUtility.ASSETPATHTABLE, false, delegate (Stream stream, string filename)
        {
            byte[] b = System.Text.Encoding.UTF8.GetBytes(OurMiniJSON.Json.Serialize(data));
            stream.Write(b, 0, b.Length);
            return true;
        });
    }

    //build patch update
    private static void BuildPatchUpdate(bool buildFullABPatch, bool excludeStreamingAB)
    {
    }

    //3/28/2022 Đại sửa hàm
    //build all assetbundle
    public static void BuildAllAssetBundle()
    {
        if (!Directory.Exists(AssetsUtility.GetCachePlatformPath()))
        {
            Directory.CreateDirectory(AssetsUtility.GetCachePlatformPath());
        }
        //Step 1.copy logic and build dll
        
        //Step 2.Build All Asset bundle
        BuildBundle(BuildUtil.targetPlatform, options);
        //BuildUtil.ChangeBytesToLua();


    }

    //3/28/2022 Đại thêm vào hàm tạo folder riêng cho mỗi platform
    private static void BuildBundle(BuildTarget target, BuildAssetBundleOptions mode)
    {

        //BeforeBuildAssetBundle();
        string path = AssetsUtility.GetCachePlatformPath();
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        BuildPipeline.BuildAssetBundles(path, mode, target);

        //ForceLowercaseManifestName();
    }



    private static void BeforeBuildAssetBundle()
    {
        //delete file ab manifest    
        //string flatformName = AssetBundleUtil.GetPlatformName();
        //string[] listAbFile = Directory.GetFiles(AssetConst.bundleDir, "*.*", SearchOption.TopDirectoryOnly);
        //foreach (var file in listAbFile)
        //{
        //    string newManifestPath = string.Format("{0}{1}{2}", AssetConst.bundleDir, AssetBundleUtil.GetPlatformName(), Path.GetExtension(file));
        //    //Debug.Log(file + "  :  " + flatformName);
        //    if (Path.GetFileNameWithoutExtension(file) == flatformName)
        //    {
        //        File.Delete(file);
        //    }

        //}
    }
    private static void ForceLowercaseManifestName()
    {
        //Lower name of file ab manifest    
   /*     string flatformName = AssetConst.PLATFROM_DIR.Replace("/", "");
        string[] listAbFile = Directory.GetFiles(AssetConst.bundleDir, "*.*", SearchOption.TopDirectoryOnly);
        foreach (var file in listAbFile)
        {
            string newManifestPath = string.Format("{0}{1}{2}", AssetConst.bundleDir, AssetBundleUtil.GetPlatformName(),Path.GetExtension(file));
            if(Path.GetFileNameWithoutExtension(file) == flatformName)
            {
                File.Move(file, newManifestPath);
            }
          
        }*/
    }
    #endregion
    #region copy assetbundle

    public static void CopyStreamingAssets(bool isFullStreamingAsset)
    {
        //BuildApp.BuildStreamingAssets("appFiles", true, false, isFullStreamingAsset);

    }
    #endregion
    #region build dll
    public void ProcessSynchronizedAndBuildDll()
    {
        //1.dong bo logic
        //TransferAssetTools.TransferAsset();
        //2.build dll theo tung flatform
        processLogicDll();

    }
    private void processLogicDll()
    {
        var dllPath = PathUtil.EditorDllPgamemain;
#if UNITY_ANDROID
        string destDir = PathUtil.DestDirAndroid;
#elif UNITY_IOS
        string destDir = PathUtil.DestDirIos;

#endif
        string dest = destDir + PathUtil.DllScriptBundleName;

        Directory.CreateDirectory(destDir);
        File.Copy(dllPath, dest, true);
        if (encodeDll)
        {
            var bytes = File.ReadAllBytes(dest);
            File.Delete(dest);
            PathUtil.Encode(bytes);
            File.WriteAllBytes(dest, bytes);
        }
    }

    #endregion
    #region versionConfig
    private void reloadVersionConfig()
    {
        string s = null;
        if (AssetsUtility.EasyLoad(GetOutputPath(mPlatform.ToString()) + "/", mversionConfig, false, delegate (Stream stream, string filename)
        {
            s = AssetsUtility.GetStreamedString(stream);
            if (!string.IsNullOrEmpty(s))
            {
                return true;
            }
            return false;
        }))
        {
            jsonVersionConfig = JSONClass.Parse(s) as JSONClass;

        }
    }
    #endregion
    #region review ab
    private Vector2 scrollValue;
    private DirInfo folderPath = null;
    const int INDENT_LINE = 20;
    private void ReviewAssetBundle(bool isSwitchTab)
    {
        if (isSwitchTab || GUILayout.Button(new GUIContent("Refresh tab"), GUILayout.Width(200)) || folderPath == null)
        {
            var bundleList = AssetDatabase.GetAllAssetBundleNames();
            folderPath = B_TreeFolderInfo.CreateFolderTree(bundleList);
            folderPath.ReCalculateBranch();
        }

        EditorGUILayout.LabelField(string.Format("List all asset bundle names ({0} folder, {1} bundle)", folderPath.ChildCount, folderPath.AllLeafCount()), EditorStyles.boldLabel);
        //EditorGUI.indentLevel++;
        scrollValue = EditorGUILayout.BeginScrollView(scrollValue, GUILayout.Height(500));
        DrawTree(folderPath);
        EditorGUILayout.EndScrollView();
    }

    private void DrawTree(DirInfo root)
    {
        for (int i = 0; i < root.ChildCount; i++)
        {
            var child = root.GetChild(i);
            EditorGUILayout.BeginHorizontal();
            //EditorGUI.indentLevel = EditorGUI.indentLevel + child.Level;
            EditorGUILayout.LabelField("", GUILayout.Width(child.Level * INDENT_LINE));
            if (child.ChildCount > 0)
            {
                if (GUILayout.Button(new GUIContent(root.Expanded[i] ? "-" : "+"), GUILayout.Width(20)))
                {
                    root.Expanded[i] = !root.Expanded[i];
                }
            }
            else
            {
                GUILayout.Button(new GUIContent("-"), GUILayout.Width(20));
            }
            EditorGUILayout.LabelField(string.Format("{0}.{1}", i + 1, child.Name), GUILayout.Width(position.width - child.Level * INDENT_LINE - 40));
            //EditorGUI.indentLevel = EditorGUI.indentLevel - child.Level;
            EditorGUILayout.EndHorizontal();
            if (child.ChildCount > 0 && root.Expanded[i])
            {
                DrawTree(child);
            }
        }
    }


#endregion
}
