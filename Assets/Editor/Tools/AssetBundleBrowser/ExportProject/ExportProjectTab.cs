using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AssetBundles;

public class ExportProjectTab
{
    [SerializeField]
    private ExportTabData m_Data;

    private string androidExportPathKey = "androidExport";
    private string iosExportPathKey = "iosExport";

    const string k_BuildPrefPrefix = "ABBBuild:";

    const string productName = "project";

    public AssetBundleBuildTab.ValidBuildTarget m_buildTarget = AssetBundleBuildTab.ValidBuildTarget.Android;
    GUIContent m_TargetContent;

    public ExportProjectTab()
    {
    }

    public void OnEnable(Rect pos, EditorWindow parent)
    {
        new AssetBundleBuildTab.ToggleData(
           false,
           "Build SubPackage",
           "Will build SubPackage.",
           null);
        m_TargetContent = new GUIContent("Build Target", "Choose target platform to build for.");

        if (m_Data == null)
            m_Data = new ExportTabData();

        m_Data.m_AndroidExportPath = PlayerPrefs.GetString(androidExportPathKey);
        m_Data.m_XcodeExportPath = PlayerPrefs.GetString(iosExportPathKey);
        
        CreateDir(m_Data.m_AndroidExportPath);
        CreateDir(m_Data.m_XcodeExportPath);
    }

    public void OnDisable()
    {

    }



    public void OnGUI(Rect pos)
    {
        EditorGUILayout.Space();
        
        GUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        var productName = EditorGUILayout.TextField("productName", PlayerSettings.productName, GUILayout.Width(400));
        //PlayerSettings.productName = productName;
        EditorGUILayout.LabelField("游戏名");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var applicationIdentifier = EditorGUILayout.TextField("packageName", PlayerSettings.applicationIdentifier, GUILayout.Width(400));
        //PlayerSettings.applicationIdentifier = applicationIdentifier;
        EditorGUILayout.LabelField("游戏包名");
        EditorGUILayout.EndHorizontal();

        // build target
        using (new EditorGUI.DisabledScope(!UnityEngine.AssetBundles.AssetBundleModel.Model.DataSource.CanSpecifyBuildTarget))
        {
            AssetBundleBuildTab.ValidBuildTarget tgt = (AssetBundleBuildTab.ValidBuildTarget)EditorGUILayout.EnumPopup(m_TargetContent, m_buildTarget);
            m_buildTarget = tgt;
            switch(m_buildTarget)
            {
                case AssetBundleBuildTab.ValidBuildTarget.Android:
                    GUILayout.BeginHorizontal();
                    var androidExportPath = PlayerPrefs.GetString(androidExportPathKey);
                    var androidInputPath = EditorGUILayout.TextField("Export Path", m_Data.m_AndroidExportPath);
                    if (androidInputPath != androidExportPath)
                    {
                        m_Data.m_AndroidExportPath = androidInputPath;
                        PlayerPrefs.SetString(androidExportPathKey, androidInputPath);
                    }

                    if (GUILayout.Button("Browse", GUILayout.MaxWidth(75f)))
                        AndroidBrowseForFolder();
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    
                    //EditorGUILayout.Space();
                    //if (GUILayout.Button("Export Android Project & Build APK (不推荐使用)"))
                    //{
                    //    ExportAndroidProject.ExportProject(m_Data.m_AndroidExportPath);
                    //}

                    EditorGUILayout.Space();
                    
                    if (GUILayout.Button("ExportReleaseAPK"))
                    {
                        ExportAndroidProject.ExportAPK(m_Data.m_AndroidExportPath);
                    }
                    if (GUILayout.Button("ExportDebugProfilerAPK"))
                    {
                        ExportAndroidProject.ExportAPK(m_Data.m_AndroidExportPath,true);
                    }
                    if (GUILayout.Button("ExportCode"))
                    {
                        ExportAndroidProject.ExportCode(m_Data.m_AndroidExportPath);
                    }

                    break;

                case AssetBundleBuildTab.ValidBuildTarget.iOS:
                    GUILayout.BeginHorizontal();
                    var iosExportPath = PlayerPrefs.GetString(iosExportPathKey);
                    var iosInputPath = EditorGUILayout.TextField("Export Path", m_Data.m_XcodeExportPath);
                    if (iosExportPath != iosInputPath)
                    {
                        m_Data.m_XcodeExportPath = iosInputPath;
                        PlayerPrefs.SetString(iosExportPathKey, iosInputPath);
                    }

                    if (GUILayout.Button("Browse", GUILayout.MaxWidth(75f)))
                        XcodeBrowseForFolder();
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    if (GUILayout.Button("Export Xcode Project"))
                    {
                        ExportXcodeProject.ExportProject(m_Data.m_XcodeExportPath);
                    }

                    EditorGUILayout.Space();

                    if (GUILayout.Button("ExportDebugProfileXcodeProject"))
                    {
                        ExportXcodeProject.ExportDebugProject(m_Data.m_XcodeExportPath);
                    }
                    break;
            }
        }
        
        GUILayout.EndVertical();
    }
    
    private void AndroidBrowseForFolder()
    {
        var newPath = EditorUtility.OpenFolderPanel("Bundle Folder", m_Data.m_AndroidExportPath, string.Empty);
        if (!string.IsNullOrEmpty(newPath))
        {
            m_Data.m_AndroidExportPath = newPath;
        }
    }

    private void XcodeBrowseForFolder()
    {
        var newPath = EditorUtility.OpenFolderPanel("Bundle Folder", m_Data.m_XcodeExportPath, string.Empty);
        if (!string.IsNullOrEmpty(newPath))
        {
            m_Data.m_XcodeExportPath = newPath;
        }
    }

    public static void CreateDir(string dirurl, bool clear = false)
    {
        if (string.IsNullOrEmpty(dirurl))
            return;
        bool isexists = Directory.Exists(dirurl);
        if (isexists && clear)
        {
            Directory.Delete(dirurl, true);
        }
        if (!isexists)
            Directory.CreateDirectory(dirurl);
    }

    [System.Serializable]
    public class ExportTabData
    {
        public string m_AndroidExportPath = string.Empty;
        public string m_XcodeExportPath = string.Empty;
    }
}
