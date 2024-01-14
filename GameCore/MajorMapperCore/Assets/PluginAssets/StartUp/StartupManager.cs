/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2017.10.24
*/
using UnityEngine;

public class StartupManager
{
    private static StartupManager instance;
    public static StartupManager Singleton
    {
        get
        {
            if(instance == null)
                instance = new StartupManager();
            return instance;
        }
    }

    private System.Action callBack;
    /// <summary>
    /// 启动更新流程
    /// </summary>
    /// <param name="callback">回调</param>
    public void Start(System.Action callback)
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Debuger.Log("System language > " + Application.systemLanguage.ToString());
        //LanguageConfig.LoadLanguage();
        //ChannelManager.Init();

        // New update structure, compatible processing
        if (GameLauncherCore.CompatibleFlag >= 14)
        {
            int value = PlayerPrefs.GetInt("local_config_version", -1);
            if (value < 0)
            {
                PlayerPrefs.SetInt("local_config_version", 1);
                PlayerPrefs.Save();
                string confPath = PathUtil.GetConfigPath() + PathUtil.BuildInVersionConfigName;
                if (System.IO.File.Exists(confPath))
                {
                    var fs = System.IO.File.OpenRead(confPath);
                    var br = new System.IO.BinaryReader(fs);
                    var b = br.ReadByte();
                    br.Close();
                    fs.Close();
                    fs.Dispose();
                    if (b < (byte)SimpleJSON.JSONBinaryTag.Array || b > (byte)SimpleJSON.JSONBinaryTag.FloatValue)
                    {
                        System.IO.File.Delete(confPath);
                        if (System.IO.Directory.Exists(PathUtil.GetConfigPath()))
                            System.IO.Directory.Delete(PathUtil.GetConfigPath(), true);
#if !UNITY_EDITOR
                        if (System.IO.Directory.Exists(PathUtil.GetForceABPath()))
                            System.IO.Directory.Delete(PathUtil.GetForceABPath(), true);
                        if (System.IO.Directory.Exists(PathUtil.GetBackABPath()))
                            System.IO.Directory.Delete(PathUtil.GetBackABPath(), true);
#endif
                    }
                }
            }
        }

#if UNITY_ANDROID
        //兼容处理
        int val = PlayerPrefs.GetInt("_ForceFileList_SaveType_", -1);
        if (val < 0)
        {
            PlayerPrefs.SetInt("_ForceFileList_SaveType_", 1);
            PlayerPrefs.Save();
            if (System.IO.File.Exists(PathUtil.ForceFileListOutPath))
            {
                var fs = System.IO.File.OpenRead(PathUtil.ForceFileListOutPath);
                var br = new System.IO.BinaryReader(fs);
                var b = br.ReadByte();
                br.Close();
                fs.Close();
                fs.Dispose();
                if (b < (byte)SimpleJSON.JSONBinaryTag.Array || b > (byte)SimpleJSON.JSONBinaryTag.FloatValue)
                {
                    if (System.IO.File.Exists(PathUtil.ForceFileTmpLoadedPath))
                        System.IO.File.Delete(PathUtil.ForceFileTmpLoadedPath);
                    var str = System.IO.File.ReadAllText(PathUtil.ForceFileListOutPath);
                    var json = SimpleJSON.JSONNode.Parse(str);
                    json.SaveToFile(PathUtil.ForceFileListOutPath);
                }
            }
        }
#endif

        callBack = callback;
#if !UNITY_EDITOR
        PathUtil.InitPath();
#endif

        LocalConfig.Singleton.Init();
        if (LocalConfig.Singleton.IsNewApp)
        {
            //The new app needs to clean up the previous stronger resources
            Debuger.Log("这是新包");
            PathUtil.ClearForce();
            ForceManager.Singleton.Clear();
        }
        else if (LocalConfig.Singleton.IsOldApp)
        {
            //The old app needs to clean up the previous stronger resources
            Debuger.Log("这是老包");
            PathUtil.ClearConfigAndForceAndBack();
        }

        Caching.ClearCache();
        WWWLoader.Singleton.UseCache = false;
        VersionConfig.Singleton.SetUrlList(LocalConfig.Singleton.ConfigUrlList);
        VersionConfigChecker.Singleton.SetUrlList(LocalConfig.Singleton.ConfigUrlList);
        VersionConfig.Singleton.CheckUpdate(selectLanguage, false);
        //selectLanguage();
    }

    private void selectLanguage()
    {
        //bool languageSelected = LanguageConfig.LanguageSelected();
        //var arr = VersionConfig.Singleton.MultiLanguageList;
        //if (languageSelected || arr == null || arr.Count <= 0)
        //{
        //    updateApp();
        //}
        //else
        //{
        //    StartupTip.Singleton.ShowLanguageSelect(arr, (language) =>
        //    {
        //        LanguageConfig.SelectLanguage(language);
        //        ForceManager.Singleton.AddUpdateLanguage(language);
        //        ForceManager.Singleton.UpdateEnd();
        //        updateApp();
        //    });
        //}

        updateApp();

    }

    private void updateApp()
    {
        //强制多语言
        if (!string.IsNullOrEmpty(VersionConfig.Singleton.ForceLanguage))
        {
            var language = VersionConfig.Singleton.ForceLanguage;
            PathUtil.resLanguage = language;
            //LanguageConfig.SelectLanguage(language);
            //ForceManager.Singleton.AddUpdateLanguage(language);
            ForceManager.Singleton.UpdateEnd();
        }

        //app
        ApplicationUpdater.Singleton.Reset(VersionConfig.Singleton.AppVersion);
        ApplicationUpdater.Singleton.SetUrlList(VersionConfig.Singleton.AppUrlList);

        //强更
        ForceFileList.Singleton.ForceCheckVersion = VersionConfig.Singleton.ForceCheckForceVersion;
        ForceFileList.Singleton.Reset(VersionConfig.Singleton.ForceResVersion);
        ForceFileList.Singleton.SetUrlList(VersionConfig.Singleton.ForceResUrlList);
        ForceFileList.Singleton.SetDownloadUrl(VersionConfig.Singleton.ForceDownloadUrlList);

        //偷跑
        BackFileList.Singleton.downloadType = VersionConfig.Singleton.BackDownloadType;
        BackFileList.Singleton.Reset(VersionConfig.Singleton.BackResVersion);
        BackFileList.Singleton.SetUrlList(VersionConfig.Singleton.BackResUrlList);
        BackFileList.Singleton.SetDownloadUrl(VersionConfig.Singleton.BackDownloadUrlList);

        //服务器列表
        //ServerList.Singleton.Reset(VersionConfig.Singleton.ServerListVersion);
        //ServerList.Singleton.SetUrlList(VersionConfig.Singleton.ServerListUrlList);
        //ServerList.Singleton.setApiKey(VersionConfig.Singleton.ApiKeyGetSvList);
       //Debug.Log("ServerListUrlList length " + VersionConfig.Singleton.ServerListUrlList.Count);

        //公告
        //Notice.Singleton.Reset(VersionConfig.Singleton.NoticeVersion);
        //Notice.Singleton.SetUrlList(VersionConfig.Singleton.NoticeUrlList);

        //登录历史
        //HistoryServer.Singleton.Reset(-1);
        //HistoryServer.Singleton.SetUrlList(VersionConfig.Singleton.LoginUrlList);

        //状态标记
        //SpecialMarker.Singleton.Reset(-1);
        //SpecialMarker.Singleton.SetUrlList(VersionConfig.Singleton.MarkUrlList);

        //if(!VersionConfig.IsInAuditing && VersionConfig.Singleton.doMarkCheck)
        //{
        //    //总开关关闭时，审核标记检查
        //    flagTotal = 2;
        //    SpecialMarker.Singleton.CheckUpdate(endBaseCheck, false);
        //}else
        {
            flagTotal = 1;
        }
        //更新app
        ApplicationUpdater.Singleton.CheckUpdate(endBaseCheck, false);
    }

    private int flag = 0;
    private int flagTotal = 0;
    private void endBaseCheck()
    {
        flag++;
        if (flag < flagTotal)
            return;
        maintainOrRepair();
    }

    //维护或者修复客户端
    private void maintainOrRepair()
    {
        //维护
        if (!string.IsNullOrEmpty(VersionConfig.Singleton.MaintainDesc))
        {
            //StartupTip.Singleton.TipForRestartGame(VersionConfig.Singleton.MaintainDesc);
            return;
        }

        //修复客户端
        var repairJson = VersionConfig.Singleton.RepairJson;
        if (repairJson != null && repairJson["resVersion"].AsInt > LocalConfig.Singleton.LocalForceResVersion)
        {
            //修复客户端
            if (PlayerPrefs.GetInt("_StartUp_Repair_Game", -1) < 0)
            {
                PlayerPrefs.SetInt("_StartUp_Repair_Game", 1);
                PlayerPrefs.Save();
                bool all = repairJson["all"].AsBool;
                bool force = repairJson["force"].AsBool;
                bool back = repairJson["back"].AsBool;
                bool pref = repairJson["pref"].AsBool;
                Debuger.Wrn("修复客户端>", all, force, back, pref);

                if (all)
                {
                    PathUtil.ClearConfigAndForceAndBack();
                    PlayerPrefs.DeleteAll();
                }
                else
                {
                    if (force)
                        PathUtil.ClearForce();
                    if (back)
                        PathUtil.ClearBack();
                    if (pref)
                        PlayerPrefs.DeleteAll();
                }
                PlayerPrefs.Save();

                //强制重启
                if (repairJson["restart"].AsBool)
                {
                    //StartupTip.Singleton.TipForRestartGame(repairJson["tip"].Value);
                    return;
                }
            }
        }
        if(PlayerPrefs.HasKey("_StartUp_Repair_Game"))
        {
            PlayerPrefs.DeleteKey("_StartUp_Repair_Game");
            PlayerPrefs.Save();
        }

        updateForce();
    }

    private void updateForce()
    {
        //更新强更文件
        if (VersionConfig.IsInAuditing)
        {
            Debuger.Err("过审状态跳过ForceFileList更新");
            startEnd();
        }
        else
        {
            Debug.Log("Check update local version: " + LocalConfig.Singleton.LocalForceResVersion
                       +" || remote version "+ VersionConfig.Singleton.ForceResVersion);

            ForceFileList.Singleton.CheckUpdate(LocalConfig.Singleton.LocalForceResVersion, startEnd);
        }
    }

    private void startEnd()
    {
        AbcManager.Singleton.Init(VersionConfig.IsInAuditing);
        LocalConfig.Singleton.SaveToLocal(VersionConfig.Singleton.ForceResVersion);
        bool appChanged = LocalConfig.Singleton.IsNewApp || LocalConfig.Singleton.IsOldApp;
        ResDepManager.Singleton.LoadDeps(appChanged);
        ConfigManager.Singleton.LoadBeans(appChanged);

        //ServerList.Singleton.CheckUpdate(null);
        Notice.Singleton.CheckUpdate(null);

        //StartupTip.Singleton.DisposeView();

        //如果不包含当前语言则用默认语言
        //if (!ResDepManager.Singleton.HasLanguage(PathUtil.resLanguage))
        //    PathUtil.resLanguage = LanguageConfig.DefaultLanguage;
        Debug.Log("最终使用语言>" + PathUtil.resLanguage);

        //BackFileList.Singleton.CheckUpdate(BackFileList.Singleton.BeginBackDownLoad);
        //PathUtil.codeOffset--;
        if (callBack != null)
            callBack();
    }
}