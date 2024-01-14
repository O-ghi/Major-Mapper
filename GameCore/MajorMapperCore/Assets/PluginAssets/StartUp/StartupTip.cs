///* 
// * -----------------------------------------------
// * Copyright (c) zhou All rights reserved.
// * -----------------------------------------------
// * 
// * Coder：Zhou XiQuan
// * Time ：2017.10.25
//*/

//using System;
//public class StartupTip
//{
//    private static StartupTip instance;
//    public static StartupTip Singleton
//    {
//        get
//        {
//            if(instance == null)
//                instance = new StartupTip();
//            return instance;
//        }
//    }

//    private void quitGame()
//    {
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#else
//        UnityEngine.Application.Quit();
//#endif
//    }

//    /// <summary>
//    /// 提示没有网络
//    /// </summary>
//    /// <param name="seg">模块</param>
//    /// <param name="callback">回调</param>
//    public void TipNoNetwork(string seg, Action cbYes, Action cbNo = null)
//    {
//        if (PopupPanel.Singleton == null)
//            return;

//#if UNITY_IOS
//        if (seg == typeof(VersionConfig).Name)
//        {
//            PopupPanel.Singleton.ShowTip(LanguageConfig.neterror, cbYes, cbNo);
//            PopupPanel.Singleton.SetBtnText("", "", LanguageConfig.retryStr);
//            return;
//        }
//#endif

//        if (cbNo == null) cbNo = quitGame;
//        //AgainConfirmWindow.Singleton.ShowTip("网络连接失败，请检查您的网络后重试", cbYes, cbNo, true);
//        PopupPanel.Singleton.ShowTip(LanguageConfig.neterror, cbYes, cbNo);
//    }

//    //提示只能确定
//    public void TipForRestartGame(string tip, Action cb = null)
//    {
//        if (PopupPanel.Singleton == null)
//            return;
//        if (cb == null)
//            cb = quitGame;
//        PopupPanel.Singleton.ShowTip(tip, cb);
//    }
    
//    /// <summary>
//    /// 提示写文件错误
//    /// </summary>
//    /// <param name="seg">模块</param>
//    /// <param name="callback">回调</param>
//    public void TipWriteFileError(string seg, Action cbYes, Action cbNo = null)
//    {
//        if (PopupPanel.Singleton == null)
//            return;
//        if (cbNo == null) cbNo = quitGame;
//        //AgainConfirmWindow.Singleton.ShowTip("写入本地文件失败，请检查磁盘空间后重试", cbYes, cbNo, true);
//        //UGuiMgr.singleton.loadingWnd.ShowTip();
//        PopupPanel.Singleton.ShowTip(LanguageConfig.writeFileFailed, cbYes, cbNo);
//    }

//    /// <summary>
//    /// 提示有新App更新
//    /// </summary>
//    /// <param name="curVer">新app版本号</param>
//    /// <param name="oldVer">当前版本号</param>
//    /// <param name="size">新app大小kb</param>
//    /// <param name="yesCb">确定更新</param>
//    /// <param name="noCb">不更新</param>
//    public void TipNewAppUpdate(int curVer, int oldVer, long size, Action yesCb, Action noCb)
//    {
//        if (PopupPanel.Singleton == null)
//            return;
//        //AgainConfirmWindow.Singleton.ShowTip(string.Format("发现新版本:{0}，当前版本号:{1}，点击更新", curVer, oldVer), yesCb, noCb, true);
//        PopupPanel.Singleton.ShowTip(string.Format(LanguageConfig.foundNewVersion, curVer, oldVer), yesCb, noCb);
//    }

//    /// <summary>
//    /// 提示下载进度
//    /// </summary>
//    /// <param name="seg">模块</param>
//    /// <param name="progress">进度</param>
//    /// <param name="loadedLength">下载量</param>
//    public void TipProgress(string seg, float progress, long totalSize, bool hideProgress = false)
//    {
//        if (LoadingPanel.Singleton == null)
//            return;
//        //UGuiMgr.singleton.loadingWnd.ToggleProgress(!hideProgress);

//        if (progress > 1)
//            progress = 1f;
//        string tip = "";
//        if (seg == typeof(ApplicationUpdater).Name)
//        {
//            //下载app（Android）
//            if (progress <= 0)
//            {
//                //UpdateLoadingWindow.Singleton.ShowTip("正在下载新版本...");
//                //tip = UGuiMgr.singleton.LanConfig.GetLanguage(LanguageConfig.downloadNewVersion);
//                //UGuiMgr.singleton.loadingWnd.ShowTip(tip);
//                LoadingPanel.Singleton.ShowTip(LanguageConfig.downloadNewVersion);
//            }
//            else
//            {
//                //UpdateLoadingWindow.Singleton.ShowTip(string.Format("正在下载新版本...{0}M/{1}M", loadedLength / 1024f, (loadedLength / progress) / 1204f));
//                //tip = UGuiMgr.singleton.LanConfig.GetLanguage(LanguageConfig.updatingResParam);
//                tip = LanguageConfig.downloadNewVersion;
//                tip = string.Format(tip, totalSize * progress / 1024f, totalSize / 1024f);
//                //UGuiMgr.singleton.loadingWnd.ShowTip(tip);
//                LoadingPanel.Singleton.ShowTip(tip);
//            }
//            if (hideProgress)
//                progress = -1;
//            LoadingPanel.Singleton.SetProgress(progress);
//            //UpdateLoadingWindow.Singleton.ChangeProgress(progress);
//            //UGuiMgr.singleton.loadingWnd.ChangeProgress(progress);
//        }
//        else if(seg == typeof(ForceFileList).Name)
//        {
//            //下载强更资源
//            if(hideProgress)
//            {
//                //UpdateLoadingWindow.Singleton.ShowTip("正在加载资源...");
//                //UpdateLoadingWindow.Singleton.ChangeProgress(-1f);
//                //tip = UGuiMgr.singleton.LanConfig.GetLanguage(LanguageConfig.loadingRes);
//                //UGuiMgr.singleton.loadingWnd.ShowTip(tip);
//                //UGuiMgr.singleton.loadingWnd.ChangeProgress(-1f);
//                LoadingPanel.Singleton.ShowTip(LanguageConfig.loadingRes);
//                LoadingPanel.Singleton.SetProgress(-1);
//            }
//            else
//            {
//                //UpdateLoadingWindow.Singleton.ShowTip(string.Format("资源更新中...{0}M/{1}M", loadedLength / 1024f, (loadedLength / progress) / 1024f));
//                //tip = UGuiMgr.singleton.LanConfig.GetLanguage(LanguageConfig.updatingResParam);
//                //tip = string.Format(tip, loadedLength / 1024f, (loadedLength / progress) / 1204f);
//                tip = string.Format(LanguageConfig.updatingResParam, totalSize * progress / 1024f, totalSize / 1024f);
//                //UGuiMgr.singleton.loadingWnd.ShowTip(tip);
//                LoadingPanel.Singleton.ShowTip(tip);
//                LoadingPanel.Singleton.SetProgress(progress);
//            }
//        }
//        else if(seg == typeof(VersionConfig).Name)
//        {
//            //下载versionConfig
//            //UpdateLoadingWindow.Singleton.ShowTip("正在检测更新...");
//            //UpdateLoadingWindow.Singleton.ChangeProgress(-1f);
//            //tip = UGuiMgr.singleton.LanConfig.GetLanguage(LanguageConfig.checkForUpdate);
//            //UGuiMgr.singleton.loadingWnd.ShowTip(tip);
//            //UGuiMgr.singleton.loadingWnd.ChangeProgress(-1f);
//#if !UNITY_IOS
//            LoadingPanel.Singleton.ShowTip(LanguageConfig.checkForUpdate);
//#endif
//            //LoadingPanel.Singleton.ShowTip("");
//            LoadingPanel.Singleton.SetProgress(-1);
//        }
//    }

//    /// <summary>
//    /// 提示强更文件
//    /// </summary>
//    /// <param name="title">标题</param>
//    /// <param name="tip">提示内容</param>
//    /// <param name="size">更新文件大小KB</param>
//    /// <param name="callback">更新回调</param>
//    public void TipForceResUpdate(string title, string tip, long size, Action callback)
//    {
//        if (PopupPanel.Singleton == null)
//            return;

//        //AgainConfirmWindow.Singleton.ShowTip(string.Format(tip, size / 1024), callback, callback, true);
//        string resTip = "";
//        string defTip = "";
//        var arr = tip.Split(';');
//        foreach(var lan in arr)
//        {
//            var arr2 = lan.Split('+');
//            if (arr2.Length < 2)
//                continue;
//            if (PathUtil.resLanguage == arr2[0])
//                resTip = arr2[1];
//            else if (LanguageConfig.DefaultLanguage == arr[0])
//                defTip = arr[1];
//        }
//        if (!string.IsNullOrEmpty(resTip))
//            tip = resTip;
//        else if (!string.IsNullOrEmpty(defTip))
//            tip = defTip;
//        tip = string.Format(tip, size / 1024f);
//        PopupPanel.Singleton.ShowTip(tip, callback, quitGame);
//        //UGuiMgr.singleton.dialogWnd.Show(tip, callback);
//    }

//    //多语言选择窗口
//    public void ShowLanguageSelect(SimpleJSON.JSONArray array, Action<string> callback)
//    {
//        var defaultLanguage = PathUtil.resLanguage;
//        LanguagePanel.Singleton.Show(array, callback, defaultLanguage);
//    }

//    public void DisposeView()
//    {
//        LanguagePanel.Dispose();
//        LoadingPanel.Dispose();
//        PopupPanel.Dispose();
//    }
//}