/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2017.10.25
*/

using SimpleJSON;
using System.Collections.Generic;
using System.Diagnostics;

public class VersionConfig : BaseDownloader
{
    private static VersionConfig instance;
    public static VersionConfig Singleton
    {
        get
        {
            if (instance == null)
                instance = new VersionConfig();
            return instance;
        }
    }

    /// app版本号
    public int AppVersion { get; private set; }

    /// 是否为强更版本（app）
    public bool IsForceUpdateApp { get; private set; }
    /// 强更App时是否跳转到引用商店（不是则内部下载）
    public bool AppStoreUpdateApp { get; private set; }

    public string AppStoreConfig { get; private set; }

    ///后台资源版本号
    public int BackResVersion { get; private set; }
    ///强更资源版本号
    public int ForceResVersion { get; private set; }
    public int ForceCheckForceVersion { get; private set; }
    ///服务器列表版本号
    public int ServerListVersion { get; private set; }
    ///公告版本号
    public int NoticeVersion { get; private set; }
    /// 是否在审核中[总开关]
    public static bool IsInAuditing { get; set; }
    public bool doMarkCheck { get; private set; }

    /// 强制语言
    public string ForceLanguage { get; private set; }
    //更选多语言信息
    public JSONArray MultiLanguageList { get; private set; }

    //是否后台偷跑
    public string BackDownloadType { get; private set; }

    //维护
    public string MaintainDesc { get; private set; }
    //修复客户端
    public JSONClass RepairJson { get; private set; }

    //CDN路径
    public string CDNPath { get; private set; }

    //2021/07/30 HRP TOAN ADD STT
    public string LogPath { get; private set; }
    //2021/07/30 HRP TOAN ADD END

    //2021/08/24 LINH ADD STT
    public string ApiKeyGetSvList { get; private set; } // Key get List Server
    //2021/08/24 LINH ADD END

    public List<string> AppUrlList = new List<string>();
    public List<string> BackResUrlList = new List<string>();
    public List<string> ForceResUrlList = new List<string>();
    public List<string> ServerListUrlList = new List<string>();
    public List<string> NoticeUrlList = new List<string>();
    public List<string> LoginUrlList = new List<string>();       //登录历史服的拉取地址
    public List<string> MarkUrlList = new List<string>(); //审核状态地址

    public List<string> ForceDownloadUrlList = new List<string>();
    public List<string> BackDownloadUrlList = new List<string>();

    private int failedTimes = 0;
    public override void Download()
    {
        string url = getDownloadUrl();
        UnityEngine.Debug.Log("version config url " + url);
        //StartupTip.Singleton.TipProgress(GetType().Name, 0, 0, true);
        WWWLoader.Singleton.Download(url, onLoadCmp, onLoadUpdate, System.DateTime.Now.Ticks.ToString(), loadCache);
        //UnityWebLoader.Singleton.Download(url, onLoadCmp, onLoadUpdate, System.DateTime.Now.Ticks.ToString(), true);
    }

    /// 下载完成
    protected override void onLoadCmp(string path, bool success, byte[] data)
    {
        base.onLoadCmp(path, success, data);
        if (!Loaded)
            return;

        if (success && data != null)
        {
            string str = System.Text.Encoding.Default.GetString(data);
            Debuger.Log(str);
            JSONClass json = JSONClass.Parse(str) as JSONClass;
            if (json != null && json["app"] != null)
            {
                //解析
                AppVersion = json["app"].AsInt;
                IsForceUpdateApp = json["forceApp"].AsBool;
                AppStoreUpdateApp = json["appStore"].AsBool;
                AppStoreConfig = json["appStore"].Value;
                BackDownloadType = json["backDownloadType"].Value;

                ServerListVersion = json["serverList"].AsInt;
                ForceResVersion = json["forceRes"].AsInt;
                ForceCheckForceVersion = json["forceCheckForce"].AsInt;
                BackResVersion = json["backRes"].AsInt;
                NoticeVersion = json["notice"].AsInt;
                IsInAuditing = json["auditing"].AsBool;
#if UNITY_EDITOR
                IsInAuditing = UnityEngine.PlayerPrefs.GetInt("SimulateAuditVersion", 0) == 1 ? true : false;
#endif
                doMarkCheck = json["markCheck"].AsBool;
                if (json["log"].AsBool)
                    Debuger.SetWorking();

                ForceLanguage = json["language"].Value;
                MultiLanguageList = json["multiLanguage"] as JSONArray;
                MaintainDesc = json["maintain"].Value;

                ApiKeyGetSvList = json["ApiKeyGetSvList"].Value;


                RepairJson = json["repair"] as JSONClass;
                var ghIDArr = json["gsIDs"] as JSONArray;
                if (ghIDArr != null)
                {
                    for (int i = 0; i < ghIDArr.Count; ++i)
                    {
                        if (LocalConfig.Singleton.BuildID == ghIDArr[i].Value)
                            IsInAuditing = true;
                    }
                }

                var key = json["codeKey"].Value;
                if (!string.IsNullOrEmpty(key))
                    PathUtil.ByteCode = key;

               


                //网址
                LocalConfig.Singleton.MakeUrlList(json["appUrl"].Value, AppUrlList);
                LocalConfig.Singleton.MakeUrlList(json["backResUrl"].Value, BackResUrlList);
                LocalConfig.Singleton.MakeUrlList(json["forceResUrl"].Value, ForceResUrlList);
                LocalConfig.Singleton.MakeUrlList(json["backDownloadUrl"].Value, BackDownloadUrlList);
                LocalConfig.Singleton.MakeUrlList(json["forceDownloadUrl"].Value, ForceDownloadUrlList);
                if (IsInAuditing)
                {
                    LocalConfig.Singleton.MakeUrlList(json["serverListUrlReview"].Value, ServerListUrlList);
                }
                else
                {
                    LocalConfig.Singleton.MakeUrlList(json["serverListUrlNew"].Value, ServerListUrlList);
                }
                //LocalConfig.Singleton.MakeUrlList(json["serverListUrlNew"].Value, ServerListUrlList);

                //Logger.log("Make server list " + ServerListUrlList.Count);
                LocalConfig.Singleton.MakeUrlList(json["noticeUrl"].Value, NoticeUrlList);
                Debuger.Log("json loginUrl " + json["historyUrl"].Value);
                LocalConfig.Singleton.MakeHistoryUrlList(json["historyUrl"].Value, LoginUrlList);
                LocalConfig.Singleton.MakeLoginUrlList(json["markUrl"].Value, MarkUrlList);

                for (int i = 0; i < ForceDownloadUrlList.Count; ++i)
                {
                    Debuger.Log("ForceDownloadUrlList[i] " + ForceDownloadUrlList[i]);
                    //ForceDownloadUrlList[i] = string.Format(ForceDownloadUrlList[i], "", LocalConfig.Singleton.ConfigTag) + "{0}/";
                    ForceDownloadUrlList[i] = string.Format(ForceDownloadUrlList[i], "", LocalConfig.Singleton.ConfigTag);

                }
                for (int i = 0; i < BackDownloadUrlList.Count; ++i)
                {
                    Debuger.Log("ForceDownloadUrlList[i] " + BackDownloadUrlList[i]);

                    BackDownloadUrlList[i] = string.Format(BackDownloadUrlList[i], "", LocalConfig.Singleton.ConfigTag);

                }

                if (mCallback != null)
                    mCallback();
            }
            else
            {
                Debuger.Log("解析失败文件", GetType().ToString());
                //StartupTip.Singleton.TipNoNetwork(GetType().Name, ReDownload);
                ReDownload();
            }
        }

        if (!success || data == null)
        {
            Debuger.Err("网络错误:" + GetType().ToString());
            //StartupTip.Singleton.TipNoNetwork(GetType().Name, ReDownload);
            if (failedTimes <= 3)
            {
                failedTimes++;
                Debuger.Log("失败后自动从新下载>", failedTimes);
                ReDownload();
            }
            else
            {
                failedTimes = 0;
                Debuger.Log("failedTimes " + GetType().Name);
                //StartupTip.Singleton.TipNoNetwork(GetType().Name, ReDownload);
                ReDownload();
            }
        }
    }
}
