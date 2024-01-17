/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2017.10.25
*/
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApplicationUpdater : BaseDownloader
{
    private static ApplicationUpdater instance;
    public static ApplicationUpdater Singleton
    {
        get
        {
            if(instance == null)
                instance = new ApplicationUpdater();
            return instance;
        }
    }

    private const string APP_PATH_KEY = "afu_TempAppKey";
    private const string APP_VER_KEY = "afu_TempAppVerKey";

    private string appUrl;
    private bool appDownloaded;
    public override void CheckUpdate(System.Action callback, bool useCache = true)
    {
#if UNITY_EDITOR
        if(callback != null)
            callback();
#else
        urlIdx = 0;
        mCallback = callback;
        
        if(mVersion > LocalConfig.Singleton.LocalAppVersion)
        {
            Debuger.Log("发现新版本，开始强更", mVersion);
            //弹出确认框
            appUrl = getDownloadUrl();
            //appUrl = "http://182.140.217.41/dlied5.myapp.com/myapp/1104472208/1104472208/ttcz/10022602_com.tencent.tmgp.ttcz_u127_1.1.39.apk";
            //int appSize = GameSDK.Singleton.GetURLFileSize(appUrl);
            //Debuger.Log("url & size:", appUrl, appSize);
            //StartupTip.Singleton.TipNewAppUpdate(mVersion, LocalConfig.Singleton.LocalAppVersion, appSize, onYes, onNo);
        }else
        {
            Debuger.Log("app 没有更新");
            string appPath = PlayerPrefs.GetString(APP_PATH_KEY, "");
            if(!string.IsNullOrEmpty(appPath))
            {
                if(System.IO.File.Exists(appPath))
                    System.IO.File.Delete(appPath);
                PlayerPrefs.DeleteKey(APP_PATH_KEY);
                PlayerPrefs.DeleteKey(APP_VER_KEY);
                PlayerPrefs.Save();
            }
            if(mCallback != null)
                mCallback();
        }
#endif
    }

    protected override string getDownloadUrl()
    {
        return string.Format("");
    }

    /// <summary>
    /// 确定更新
    /// </summary>
    private void onYes()
    {
        startDownload();
    }

    /// <summary>
    /// 取消更新
    /// </summary>
    private void onNo()
    {
        if(VersionConfig.Singleton.IsForceUpdateApp)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        } else if(mCallback != null)
        {
            mCallback();
        }
    }

    private void startDownload()
    {
        appDownloaded = false;
        //Global.GEvtDispatcher.RemoveEventListener(EventID.ApplicationPaused, onAppPause, this);
        //Global.GEvtDispatcher.AddEventListener(EventID.ApplicationPaused, onAppPause, this);
#if UNITY_ANDROID
        //安卓下载自己安装
        string appPath = PlayerPrefs.GetString(APP_PATH_KEY, "");
        int appVer = PlayerPrefs.GetInt(APP_VER_KEY, -1);
        if(appVer == mVersion && !string.IsNullOrEmpty(appPath) && File.Exists(appPath))
        {
            appDownloaded = true;
            onLoadUpdate(appUrl, 1, 0);
            //tryInstallApp();
        }else
        {
            if (VersionConfig.Singleton.AppStoreUpdateApp)
            {
                if (VersionConfig.Singleton.AppStoreConfig.StartsWith("http"))
                {
                    Debuger.Log("打开网页>>>");
                    Application.OpenURL(VersionConfig.Singleton.AppStoreConfig);
                }else
                {
                    //商店下载
                    AndroidAppUpdaterMonitor.GetInstance();//resume游戏处理
                    //if (!GameSDK.Singleton.JumpToAppStore())
                    //{
                    //    Debuger.Err("跳转引用商店失败>>>");
                    //    Application.OpenURL(appUrl);
                    //}
                }
            }
            else
            {
                //游戏内下载
                if (!string.IsNullOrEmpty(appPath))
                {
                    if (System.IO.File.Exists(appPath))
                        System.IO.File.Delete(appPath);
                    PlayerPrefs.DeleteKey(APP_PATH_KEY);
                    PlayerPrefs.DeleteKey(APP_VER_KEY);
                    PlayerPrefs.Save();
                }
                string name = Path.GetFileName(appUrl);
                string mPath = PathUtil.GetServerPath(PathUtil.GetBackABPath(), name);
                //下载apk
                //下载逻辑放到Android可以断点下载
                AndroidAppUpdaterMonitor.GetInstance().Init(onAppDownloaded, onAppDownloadErr);
                //GameSDK.Singleton.DownloadAndSaveApp(appUrl, mPath, AndroidAppUpdaterMonitor.GameObjectName, AndroidAppUpdaterMonitor.MonitorFuncName);
            }
        }
#elif UNITY_IOS
        //苹果直接跳转到plist地址
        appDownloaded = true;
        tryInstallApp();
#endif
    }

    //下载失败
    private void onAppDownloadErr()
    {
        //Show popup
        //StartupTip.Singleton.TipNoNetwork(GetType().Name, onYes, onNo);
    }

    //下载成功
    private void onAppDownloaded()
    {
        appDownloaded = true;
        string name = System.IO.Path.GetFileName(appUrl);
        string mPath = PathUtil.GetServerPath(PathUtil.GetBackABPath(), name);
        PlayerPrefs.SetString(APP_PATH_KEY, mPath);
        PlayerPrefs.SetInt(APP_VER_KEY, mVersion);
        PlayerPrefs.Save();
        //tryInstallApp();
    }

//    private void tryInstallApp()
//    {
//        installTimerID = -1;
//        if(!appDownloaded)
//        {
//            if (VersionConfig.Singleton.AppStoreUpdateApp)
//            {
//                if (VersionConfig.Singleton.AppStoreConfig.StartsWith("http"))
//                {
//                    Debuger.Log("打开网页>>>");
//                    Application.OpenURL(VersionConfig.Singleton.AppStoreConfig);
//                }else if (!GameSDK.Singleton.JumpToAppStore())
//                {
//                    //商店下载
//                    Debuger.Err("跳转应用商店失败>>>");
//                    Application.OpenURL(appUrl);
//                }
//            }
//            return;
//        }

//        Debuger.Log("开始安装app");
//#if UNITY_ANDROID
//        string appPath = PlayerPrefs.GetString(APP_PATH_KEY, "");
//        if(!string.IsNullOrEmpty(appPath) && System.IO.File.Exists(appPath))
//        {
//            //安装
//            GameSDK.Singleton.InstallApp(appPath);
//        }
//        else
//        {
//            //重新下载
//        }
//#elif UNITY_IOS
//        //苹果直接跳转到plist地址
//        string url = getDownloadUrl() + "?" + mVersion;
//        Application.OpenURL(url);
//#endif
//    }

    private long installTimerID = -1;
    public void OnAppPause(bool pause)
    {
        if(pause)
        {
            if(installTimerID != -1)
                CoroutineManager.Singleton.stopCoroutine(installTimerID);
        } else
        {
            //installTimerID = CoroutineManager.Singleton.delayedCall(3, tryInstallApp);
        }
    }
}

#if UNITY_ANDROID
public class AndroidAppUpdaterMonitor : MonoBehaviour
{
    public const string MonitorFuncName = "DownloadMonitor";
    public const string GameObjectName = "AndroidAppUpdaterMonitor";

    private static AndroidAppUpdaterMonitor instance;
    public static AndroidAppUpdaterMonitor GetInstance()
    {
        if(instance == null)
        {
            var obj = new GameObject(GameObjectName);
            instance = obj.AddComponent<AndroidAppUpdaterMonitor>();
        }
        return instance;
    }

    private System.Action errorCb;
    private System.Action downloadCb;
    public void Init(System.Action onDownloaed, System.Action onErr)
    {
        errorCb = onErr;
        downloadCb = onDownloaed;
    }

    void OnApplicationPause(bool paused)
    {
        ApplicationUpdater.Singleton.OnAppPause(paused);
    }

    public void DownloadMonitor(string msg)
    {
        string[] arr = msg.Split(':');
        if(arr == null || arr.Length < 2)
            return;

        string code = arr[0];
        string para = arr[1];

        if(code == "progress")
        {
            string[] data = para.Split('+');
            float loaded = float.Parse(data[0]);
            long all = long.Parse(data[1]);
            Debuger.Log("TipProgress " + typeof(ApplicationUpdater).Name + " | " + loaded / all + " | " + all);
            //StartupTip.Singleton.TipProgress(typeof(ApplicationUpdater).Name, loaded / all, all);
        } else if(code == "complete")
        {
            string[] data = para.Split('+');
            float loaded = float.Parse(data[0]);
            long all = long.Parse(data[1]);
            Debuger.Log("TipProgress " + typeof(ApplicationUpdater).Name + " | " + loaded / all + " | " + all);

            //StartupTip.Singleton.TipProgress(typeof(ApplicationUpdater).Name, loaded / all, all);
            if(downloadCb != null)
                downloadCb();
        } else if(code == "failed")
        {
            Debuger.Log("app没有下载成功，请检查网络连接\n", msg);
            if(errorCb != null)
                errorCb();
        }
    }
}
#endif