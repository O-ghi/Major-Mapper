using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using LitJson;

public class ExportAndroidProject
{

    #region Packaging method 1: Export android from Unity and use ant to package it. The output package ant will compress the real package again. As a result, the package body will become smaller, and the decompression time of the resource will become longer during the loading process. (suitable for cases where a small package is required)

    public static void ExportProject(string path)
    {
        path = path.Replace("\\", "/");

        UnityEditor.Build.Reporting.BuildReport buildReport = BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);

        UnityEditor.Build.Reporting.BuildSummary buildSummary = buildReport.summary;

        if (buildSummary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            EncryptionDll(path);
            CopyMonoSo(path);
            CopyStringFile(path);
            CopyManifestFile(path);
            Project2APK(path);
            EditorUtility.DisplayDialog("ExportAndroid", "Export android project success!", "ok");
        }
    }

    public static void Project2APK(string path)
    {
        path = path.Replace("\\", "/");
        string cmd = @"cd " + path + "&" + "build_apk.bat";
        OpenConnDesk(cmd);
    }
    
    public static void EncryptionDll(string path)
    {
        path = path.Replace("\\", "/");
        string inpath = path + "/" + "project" + "/assets/bin/Data/Managed/Assembly-CSharp.dll";
        string outpath = path + "/export_dll/Assembly-CSharp.dll";

        ExportProjectTab.CreateDir(Path.GetDirectoryName(outpath));

        if (File.Exists(inpath))
        {
            byte[] bytes = File.ReadAllBytes(inpath);
            //字节偏移加密
            bytes[0] += 1;
            File.WriteAllBytes(outpath, bytes);
        }
    }

    public static void CopyMonoSo(string path)
    {
        path = path.Replace("\\", "/");
        string soPath = path + "/tools/monoLibs/android_staging/armv7a/libmono.so";
        string toPath = path + "/" + PlayerSettings.productName + "/libs/armeabi-v7a/libmono.so";
        File.Copy(soPath, toPath, true);
    }

    public static void CopyStringFile(string path)
    {
        path = path.Replace("\\", "/");
        string soPath = path + "/tools/strings.xml";
        string toPath = path + "/" + PlayerSettings.productName + "/res/values/strings.xml";
        File.Copy(soPath, toPath, true);
    }

    public static void CopyManifestFile(string path)
    {
        path = path.Replace("\\", "/");
        string soPath = path + "/tools/AndroidManifest.xml";
        string toPath = path + "/" + PlayerSettings.productName + "/AndroidManifest.xml";
        File.Copy(soPath, toPath, true);
    }
    #endregion

    #region 打包方法二：使用unity的API进行打包，打出的包没有再一次压缩，包体大小比较大。资源加载过程中解压缩时间变少。（建议打包方式）


    static string platformXF = "AndroidOfXf";

    static string platformUC = "AndroidOfUC";

    static string platformYSDK = "AndroidOfYSDK";
    public static void ExportAPK(string path, bool isDebug = false)
    {
        //copy platformSDK to andorid
        // CopyPlatformSDKToTools(path);
        //copy lssdk to android
        //string sourcePath = path + "/tools/LSSDK.jar";
        //string toPath = Application.dataPath + "/Plugins/Android/LSSDK.jar";
        //File.Copy(sourcePath, toPath, true);
        //string sourcePath = "";
        //string toPath = "";
        //替换AndroidManifest.xml
        //sourcePath = path + "/tools/AndroidManifest.xml";
        //toPath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
        //File.Copy(sourcePath, toPath, true);

        //ChangeVersionCode(path);

        //SignApk(path);
        //PlayerSettings.productName = "封神曲";
        string apkname;
        if (isDebug)
            ExportDebugProfilerAPK(path, out apkname);
        else
            ExportReleaseAPK(path, out apkname);
        //
        //copy并加密dll
        string inpath = Application.dataPath + "/../Temp/StagingArea/assets/bin/Data/Managed/Assembly-CSharp.dll";
        string outpath = path + "/export_dll/Assembly-CSharp.dll";
        ExportProjectTab.CreateDir(Path.GetDirectoryName(outpath));
        if (File.Exists(inpath))
        {
            byte[] bytes = File.ReadAllBytes(inpath);
            //字节偏移加密
            bytes[0] += 1;
            File.WriteAllBytes(outpath, bytes);
        }
        
        EditorUtility.DisplayDialog("ExportAndroid", "ExportAPK success!", "ok");

        ShowAndSelectFileInExplorer(path, apkname);
    }


    public static bool CopyPlatformSDKToTools(string path)
    {

        string platformSDKId = GameInfoManager.GetAttibute("sdk_id");

        string PlatformFolder = "";

        switch (platformSDKId)
        {
            case "xf":
                PlatformFolder = platformXF;
                break;
            case "uc":
                PlatformFolder = platformUC;
                break;
            case "ysdk":
                PlatformFolder = platformYSDK;
                break;
            default:
                string msg = "没有选择要打包的平台";
                EditorUtility.DisplayDialog("ExportAPK", msg, "ok");
                break;
        }

        if (!Directory.Exists(path+"/" + PlatformFolder))
        {
            string msg = path + "路径下不存在" + PlatformFolder;

            EditorUtility.DisplayDialog("ExportAPK", msg, "ok");

            return false;
        }
        else
        {
            try
            {
                string SourcePath = path + "/" + PlatformFolder + "/Android";

                string toPath = Application.dataPath + "/Plugins/Android";

                CopyDirectory(SourcePath, toPath, true);

                SourcePath = path + "/" + PlatformFolder + "/LSSDK.jar";

                toPath = path + "/tools/LSSDK.jar";

                File.Copy(SourcePath, toPath, true);

                SourcePath = path + "/" + PlatformFolder + "/AndroidManifest.xml";

                toPath = path + "/tools/AndroidManifest.xml";

                File.Copy(SourcePath, toPath, true);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Print("移动平台相关内容失败,平台文件夹里有Android，LSSDK.jar,AndroidManifest.xml"+ex.ToString());

                return false;
            }
         }
        return true;
    }

    private static void DeleteDirectory(string path)
    {
        FileAttributes attr = File.GetAttributes(path);

        if (attr == FileAttributes.Directory)
        {
            Directory.Delete(path, true);
        }
        else
        {
            File.Delete(path);
        }
    }

    private static bool CopyDirectory(string SourcePath, string DestinationPath, bool overwriteexisting)
    {
        if (Directory.Exists(DestinationPath))
            DeleteDirectory(DestinationPath);

        bool ret = false;
        try
        {
            SourcePath = SourcePath.EndsWith(@"/") ? SourcePath : SourcePath + @"/";
            DestinationPath = DestinationPath.EndsWith(@"/") ? DestinationPath : DestinationPath + @"/";

            if (Directory.Exists(SourcePath))
            {
                if (Directory.Exists(DestinationPath) == false)
                    Directory.CreateDirectory(DestinationPath);

                foreach (string fls in Directory.GetFiles(SourcePath))
                {
                    FileInfo flinfo = new FileInfo(fls);
                    flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);
                }
                foreach (string drs in Directory.GetDirectories(SourcePath))
                {
                    DirectoryInfo drinfo = new DirectoryInfo(drs);
                    if (CopyDirectory(drs, DestinationPath + drinfo.Name, overwriteexisting) == false)
                        ret = false;
                }
            }
            ret = true;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log(ex.ToString());
            ret = false;
        }
        return ret;
    }

    public static void ExportReleaseAPK(string path, out string apkname)
    {
        apkname = "fsq_" + DateTime.Now.ToString("yyy_MM_dd_HH_mm_ss") + ".apk";
        path = path.Replace("\\", "/");
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetBuildScenes();
        buildPlayerOptions.locationPathName = path + "/export_apk/" + apkname;
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    public static void ExportDebugProfilerAPK(string path, out string apkname)
    {
        apkname = "fsq_" + DateTime.Now.ToString("yyy_MM_dd_HH_mm_ss") + "_debug.apk";
        path = path.Replace("\\", "/");
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetBuildScenes();
        buildPlayerOptions.locationPathName = path + "/export_debugapk/" + apkname;
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.AllowDebugging | BuildOptions.Development | BuildOptions.ConnectWithProfiler;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    public static void SignApk(string path)
    {
        path = path.Replace("\\", "/");
        string keystoreName = path + "/tools/user.keystore";
        PlayerSettings.Android.androidIsGame = true;
        PlayerSettings.Android.forceInternetPermission = true;
        PlayerSettings.Android.keystoreName = keystoreName;
        PlayerSettings.Android.keystorePass = "leji1236";
        PlayerSettings.Android.keyaliasName = "fsq";
        PlayerSettings.Android.keyaliasPass = "leji1236";
    }
    #endregion

    public static void ExportCode(string path)
    {
        string productName = PlayerSettings.productName;
        //PlayerSettings.productName = "project";

        string packageName = PlayerSettings.applicationIdentifier;
        //PlayerSettings.applicationIdentifier = "com.leji.fsq";

        path = path.Replace("\\", "/");

        UnityEditor.Build.Reporting.BuildReport buildReport = BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);

        UnityEditor.Build.Reporting.BuildSummary buildSummary = buildReport.summary;

        if (buildSummary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            EncryptionDll(path);
            PlayerSettings.productName = productName;
            PlayerSettings.applicationIdentifier = packageName;
            EditorUtility.DisplayDialog("ExportCode", "Export code success!", "ok");
        }
    }

    public static void ExportObbFile(string path)
    {
        path = path.Replace("\\", "/");


        PlayerSettings.Android.useAPKExpansionFiles = true;

        UnityEditor.Build.Reporting.BuildReport buildReport = BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);

        UnityEditor.Build.Reporting.BuildSummary buildSummary = buildReport.summary;

        if (buildSummary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            string productName = PlayerSettings.productName;

            string outobbName = string.Format("{0}.main.obb", productName);

            string inpath = path + "/" + productName + "/" + outobbName;

            string obbName = string.Format("main.{0}.{1}.obb", PlayerSettings.Android.bundleVersionCode, PlayerSettings.applicationIdentifier);

            string outpath = path + "/export_obb/"+obbName;

            ExportProjectTab.CreateDir(Path.GetDirectoryName(outpath));

            if (File.Exists(inpath))
            {
                File.Move(inpath, outpath);
            }

            FileInfo fileInfo = new FileInfo(outpath);
            RequestFileInfo requestFileInfo = new RequestFileInfo();
            requestFileInfo.name = fileInfo.Name;
            requestFileInfo.size = fileInfo.Length;

            byte[] buffer = new byte[1024 * 1024];
            requestFileInfo.crc = ZipTools.GetFileCRC(outpath, buffer);

            string filejsonName = string.Format("{0}/{1}.json", path + "/export_obb", "obb");
            string filejson = JsonMapper.ToJson(requestFileInfo);
            File.WriteAllText(filejsonName, filejson);

            PlayerSettings.Android.useAPKExpansionFiles = false;
        }
    }



    public static void ChangeVersionCode(string path)
    {
        path = path.Replace("\\", "/");
        string manifestpath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
        string[] lines = File.ReadAllLines(manifestpath);
        
        string line2 = "<manifest xmlns:android=\"http://schemas.android.com/apk/res/android\" package=\"{0}\" xmlns:tools=\"http://schemas.android.com/tools\" android:versionName=\"{1}\" android:versionCode=\"{2}\" android:installLocation=\"internalOnly\">";
		string newline2 = string.Format(line2, PlayerSettings.applicationIdentifier, GameInfoManager.GetAttibute("app_version"), GameInfoManager.GetAttibute("versionCode"));
        lines[1] = newline2;
        File.WriteAllLines(manifestpath, lines);
    }

    static public void ShowAndSelectFileInExplorer(string path, string fileName)
    {
        //path = path.Replace("/", "\\");
        //string cmd = @"cd " + path + "&" + "explorer  /select,"+ fileName;
        //OpenConnDesk(cmd);
    }

    public static void OpenConnDesk(string command, int seconds = 0)
    {
        if (command != null && !command.Equals(""))
        {
            //创建进程对象 
            Process process = new Process();
            //设定需要执行的命令 
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + command;
            process.StartInfo = startInfo;
            try
            {
                //开始进程
                if (process.Start())
                {
                    if (seconds == 0)
                    {
                        //这里无限等待进程结束 
                        process.WaitForExit();
                    }
                    else
                    {
                        //等待进程结束，等待时间为指定的毫秒 
                        process.WaitForExit(seconds);
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("出现异常：" + ex.Message);
            }
            finally
            {
                if (process != null)
                {
                    process.Close();
                }
            }
        }
    }

    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
                names.Add(e.path);
        }
        return names.ToArray();
    }
}
