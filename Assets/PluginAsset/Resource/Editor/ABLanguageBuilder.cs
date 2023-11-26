using SimpleJSON;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class ABLanguageBuilder : EditorWindow
{
    [MenuItem("Tools/AB/Multilingual Packaging", false, 300)]
    public static void BuildLanguageAB()
    {
        var window = GetWindow<ABLanguageBuilder>();
        window.Show();
    }

    private void OnEnable()
    {
        curLanIndex = -1;
        var arr = System.Enum.GetNames(typeof(SystemLanguage));
        for (int i = 0; i < arr.Length; ++i)
        {
            if(arr[i].ToLower() == "chinese")
            {
                curLanIndex = i;
                break;
            }
        }
    }

    private int curLanIndex;
    private void OnGUI()
    {
        GUILayout.Space(10);
        curLanIndex = EditorGUILayout.Popup("language", curLanIndex, System.Enum.GetNames(typeof(SystemLanguage)));
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("force replacement package"))
        {
            var selectLanguage = System.Enum.GetNames(typeof(SystemLanguage))[curLanIndex].ToLower();
            var prePath = Application.dataPath + "/../language/";
            build(prePath, selectLanguage, "force");
        }
        GUILayout.Space(10);
        if (GUILayout.Button("back replacement package"))
        {
            var selectLanguage = System.Enum.GetNames(typeof(SystemLanguage))[curLanIndex].ToLower();
            var prePath = Application.dataPath + "/../language/";
            build(prePath, selectLanguage, "back");
        }
        GUILayout.EndHorizontal();
    }

    private static void build(string path, string language, string folder)
    {
        var dic = new DirectoryInfo(path + language + "/" + folder);
        if (!dic.Exists)
        {
            Debug.LogError("The language directory does not exist>" + dic.FullName);
            return;
        }

        var backMap = new Dictionary<string, string>();
        var backPath = Application.dataPath + "/../_lan_back_tmp_/";
        if (Directory.Exists(backPath))
            Directory.Delete(backPath, true);
        Directory.CreateDirectory(backPath);

        int preLen = dic.FullName.Length;
        List<string> toBuildList = new List<string>();
        int num = 0;
        EditorUtility.DisplayProgressBar("Packaging", "Copying...", 0.1f);
        foreach (var file in dic.GetFiles("*.*", SearchOption.AllDirectories))
        {
            //Backup
            num++;
            string aPath = file.FullName.Substring(preLen);
            File.Copy(Application.dataPath + aPath, backPath + num + "_" + file.Name);
            backMap[Application.dataPath + aPath] = backPath + num + "_" + file.Name;

            //Multilingual coverage
            file.CopyTo(Application.dataPath + aPath, true);
            toBuildList.Add(("Assets" + file.FullName.Substring(preLen)).Replace("\\", "/"));
        }
        AssetDatabase.Refresh();

        //ui is separated from others, ui may be packaged by folder
        string uiPathRoot = "Assets/ArtResources/OutPut/UI";
        List<Object> uiBuildObjList = new List<Object>();
        for (int i = toBuildList.Count - 1; i >= 0; --i)
        {
            if (toBuildList[i].StartsWith(uiPathRoot))
            {
                var uiFolder = Path.GetDirectoryName(toBuildList[i]);
                var folderObj = AssetDatabase.LoadAssetAtPath<Object>(uiFolder);
                if (!uiBuildObjList.Contains(folderObj))
                    uiBuildObjList.Add(folderObj);

                var obj = AssetDatabase.LoadAssetAtPath<Object>(toBuildList[i]);
                uiBuildObjList.Add(obj);
                toBuildList.RemoveAt(i);
            }
        }

        if (uiBuildObjList.Count > 0)
        {
            EditorUtility.DisplayProgressBar("Package", "Start to package UI related", 0.3f);
            var log = new StringBuilder();
            ABBuilder.BuildOneAB(uiBuildObjList.ToArray(),true, ref log, ABBuilder.outPath + "/" + language);
        }

        var outputPath = ABBuilder.outPath + "/" + language;
        if (toBuildList.Count > 0)
        {
            if (!Directory.Exists(ABBuilder.outPath))
                Directory.CreateDirectory(ABBuilder.outPath);
            if (!Directory.Exists(ABBuilder.outPath + "/" + language))
                Directory.CreateDirectory(ABBuilder.outPath + "/" + language);

            EditorUtility.DisplayProgressBar("Package", "Start to pack other pictures", 0.6f);
            List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();
            foreach (var file in toBuildList)
            {
                AssetBundleBuild abBuild = new AssetBundleBuild();
                abBuild.assetBundleName = Path.GetFileNameWithoutExtension(file).ToLower() + PathUtil.abSuffix;
                abBuild.assetNames = new string[] { file };
                buildList.Add(abBuild);
            }
            var buildOptions = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;
            BuildPipeline.BuildAssetBundles(ABBuilder.outPath + "/" + language, buildList.ToArray(), buildOptions, EditorUserBuildSettings.activeBuildTarget);

            //Delete redundant files
            var abPath = outputPath + "/" + Path.GetFileName(outputPath).ToLower();
            File.Delete(abPath);
            File.Delete(abPath + ".manifest");

        }

        if (File.Exists(outputPath + "/shader_list" + PathUtil.abSuffix))
        {
            File.Delete(outputPath + "/shader_list" + PathUtil.abSuffix);
            File.Delete(outputPath + "/shader_list" + PathUtil.abSuffix + ".manifest");
        }
        EditorUtility.ClearProgressBar();

        foreach (var kv in backMap)
            File.Copy(kv.Value, kv.Key, true);
        if (Directory.Exists(backPath))
            Directory.Delete(backPath, true);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        System.Diagnostics.Process.Start(ABBuilder.outPath + "/" + language);
        Debug.Log("Multi-language packaging completed >>>" + dic.FullName);
    }
}