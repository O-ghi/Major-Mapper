/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2017.10.24
*/

using System;
using System.IO;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;

public class LocalConfig
{
    private static LocalConfig instance;
    public static LocalConfig Singleton
    {
        get
        {
            if(instance == null)
                instance = new LocalConfig();
            return instance;
        }
    }
    /// <summary>
    /// Is it a full package
    /// </summary>
    public bool FullPackage { get; private set; }
    /// <summary>
    /// Local version number
    /// </summary>
    public int LocalAppVersion { get; private set; }
    /// <summary>
    /// Configuration control version number
    /// </summary>
    public string ConfigTag { get; private set; }
    /// <summary>
    /// Local resource version number
    /// </summary>
    public int LocalForceResVersion { get; private set; }
    /// <summary>
    /// The version number in the package is greater than the version number outside the package
    /// Explain that there is an overwrite installation, and it may be necessary to unzip all the resources in the old package
    /// Clean up the previous stronger resources
    /// </summary>
    public bool IsNewApp { get; private set; }

    ///The version number in the package is less than the version number in the package
    ///Overwrite installation of old packages, in order to avoid accidents, all local resources need to be cleaned up
    public bool IsOldApp { get; private set; }

    //Current configuration
    private JSONClass usingJson = null;
    //所有配置
    private JSONClass json = null;

    private const string FORMAL_KEY = "formal";
    private const string LOCAL_CONF_KEY = "LC_URL_ADREE_Key"; 

    private bool localFake;
    private string cachePath = "";
    public List<string> ConfigUrlList = new List<string>();
    private List<string> channelUrlList = new List<string>();
    private List<string> loginUrlList = new List<string>();

    public string BuildID { get; private set; }

    public void Init()
    {

        json = null;
        IsNewApp = false;
        // 默认情况下为全量包
        FullPackage = true;

        //初始化路径
        string confPath = PathUtil.GetConfigPath();
        cachePath = confPath + PathUtil.BuildInVersionConfigName;
        if(!Directory.Exists(confPath))
            Directory.CreateDirectory(confPath);

        //Initialize the json configuration inside and outside the package
        JSONClass jsonOut = null;
        JSONClass jsonIn = null;

        if (File.Exists(cachePath))
        {
            try
            {
                //The files outside the package may be modified, you need to try
                jsonOut = JSONClass.LoadFromFile(cachePath) as JSONClass;
            }
            catch (Exception e)
            {
                Debuger.Err(e.Message, e.StackTrace);
            }
        }
        TextAsset ta = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(PathUtil.BuildInVersionConfigName));
        if(ta != null)
            jsonIn = JSONClass.Parse(ta.text) as JSONClass;

        //获取当前配置
        string fakeName = PlayerPrefs.GetString(LOCAL_CONF_KEY, FORMAL_KEY);
        localFake = fakeName != FORMAL_KEY;
        JSONClass confJsonOut = null;

        if(jsonOut != null)
            confJsonOut = jsonOut[fakeName] as JSONClass;
        JSONClass confJsonIn = null;
        if(jsonIn != null)
            confJsonIn = jsonIn[fakeName] as JSONClass;

        //Judge whether to use the outside or the inside
        if (confJsonOut == null || confJsonOut["appVer"] == null)
        {
            // No external, the first package starts for the first time
            json = jsonIn;
            usingJson = confJsonIn;
        }else if(VersionCompare.CompareVersion(confJsonIn["appVer"].Value, confJsonOut["appVer"].Value) != 0)
        {
            //The external version number is lower than the internal version number, the first start after the update
            //The external version number is higher than the internal version number, the old version is overwritten and installed to replace the new version
            json = jsonIn;
            usingJson = confJsonIn;
            if (VersionCompare.CompareVersion(confJsonIn["appVer"].Value, confJsonOut["appVer"].Value) < 0)
                IsOldApp = true;
            else
                IsNewApp = true;
        }else
        {
            //一般情况
            json = jsonOut;
            usingJson = confJsonOut;
        }

        // The internal resource version number is higher than that of the external resource
        //, which is also handled as a new package        
        if (confJsonIn != null && confJsonOut != null)
        {
            // The internal resource version number is higher than the external one,
            // you need to unzip them all
            var ret = VersionCompare.CompareVersion(confJsonIn["forceResVer"].Value, confJsonOut["forceResVer"].Value);
            if (ret > 0)
            {
                LocalForceResVersion = confJsonIn["forceResVer"].AsInt;
                IsNewApp = true;
            }
        }
        Debug.Log("json " + json.ToString());
#if UNITY_EDITOR
        var platform_data = (PlatformData)PlayerPrefs.GetInt("Editor PlatformData", 1);
#else
               //Channel cnd address list
        var channelUrl = json[FORMAL_KEY]["channelUrl"];
        if(channelUrl[ChannelManager.ChannelName] != null)
        {
            var urlsArr = channelUrl[ChannelManager.ChannelName] as JSONArray;
            for (int i = 0; i < urlsArr.Count; ++i)
                channelUrlList.Add(urlsArr[i].Value);
        }
#endif


        //var loginUrl = json[FORMAL_KEY]["loginUrl"];
        //if(loginUrl[ChannelManager.ChannelName] != null)
        //{
        //    var urlsArr = loginUrl[ChannelManager.ChannelName] as JSONArray;
        //    for (int i = 0; i < urlsArr.Count; ++i)
        //        loginUrlList.Add(urlsArr[i].Value);
        //}

        //解析ConfigUrl
        if (usingJson != null && usingJson["appVer"] != null)
        {
            ConfigUrlList.Clear();
            if(localFake)
                Debuger.Log("You found a great secret, the current fake configuration", fakeName);

            LocalAppVersion = usingJson["appVer"].AsInt;
            FullPackage = usingJson["fullPackage"].AsBool;
#if UNITY_EDITOR
            ConfigTag = ((PlatformData)PlayerPrefs.GetInt("Editor PlatformData", 1)).ToString();
#else
            //Luon su dung tu file LocalConfig.json
            ConfigTag = confJsonIn["configTag"].Value;
#endif
            LocalForceResVersion = usingJson["forceResVer"].AsInt;
            var url = usingJson["configUrl"].Value;
            Debuger.Log("channelUrlList length " + channelUrlList.Count);
            MakeUrlList(url, ConfigUrlList);
            Debuger.Log("ConfigUrlList length " + ConfigUrlList.Count);
            //foreach (var key in ConfigUrlList)
            //{
            //    string keyStr = "";
            //    var by = System.Text.Encoding.UTF8.GetBytes(key);

            //    foreach (var b in by)
            //    {
            //        keyStr += b;
            //    }
            //    Debuger.Log("kwang key:" + key);

            //    Debuger.Log("kwang key bytes:" + keyStr + " ||||||||| by count:" + by.Length);

            //}
        }

        var enu = json.GetEnumerator();
        while(enu.MoveNext())
        {
            var kv = (KeyValuePair<string, JSONNode>)enu.Current;
            Debuger.AddTrigger(kv.Value["cmd"].Value, onFakeTriggerd);
        }
        var dis = enu as IDisposable;
        if(dis != null) dis.Dispose();

        //打包信息
        if(jsonIn[FORMAL_KEY]["buildInfo"] != null)
        {
            Debuger.Log(jsonIn[FORMAL_KEY]["buildInfo"].ToString());
            BuildID = jsonIn[FORMAL_KEY]["buildInfo"]["buildID"].Value;
        }

//#if UNITY_EDITOR
//        ConfigTag = "debug";
//#endif
    }

    private void onFakeTriggerd(string trigger)
    {
        var enu = json.GetEnumerator();
        while(enu.MoveNext())
        {
            var kv = (KeyValuePair<string, JSONNode>)enu.Current;
            if(kv.Value["cmd"].Value == trigger)
            {
                if(kv.Key == FORMAL_KEY)
                    Debuger.Log("你已经切回Formal配置，重启生效");
                else
                    Debuger.Log("你发现了一个秘密，已切换至fake配置，重启生效", kv.Key);
                PlayerPrefs.SetString(LOCAL_CONF_KEY, kv.Key);
                PlayerPrefs.Save();
                break;
            }
        }
        var dis = enu as IDisposable;
        if(dis != null) dis.Dispose();
    }

    public void MakeUrlList(string urlSuffix, List<string> list)
    {
        list.Clear();
        for (int i = 0; i < channelUrlList.Count; ++i)
        {
            if (urlSuffix.StartsWith("http"))
            {
                list.Add(urlSuffix);
                break;
            }
            else
            {
                list.Add(channelUrlList[i] + urlSuffix);
            }
        }
    }
    public void MakeHistoryUrlList(string urlSuffix, List<string> list)
    {
        list.Clear();
        list.Add(urlSuffix);
    }
    public void MakeLoginUrlList(string urlSuffix, List<string> list)
    {
        list.Clear();
        for (int i = 0; i < loginUrlList.Count; ++i)
        {
            if (urlSuffix.StartsWith("http"))
            {
                list.Add(urlSuffix);
                break;
            }else
            {
                list.Add(loginUrlList[i] + urlSuffix);
            }
        }
    }

    public string GetChannelUrl(int idx = 0)
    {
        if (idx < 0)
            idx = 0;
        if (idx >= channelUrlList.Count)
            idx = 0;
        return channelUrlList[idx];
    }

    /// <summary>
    /// 更新本地配置
    /// </summary>
    /// <param name="confUrlList"></param>
    /// <param name="appVer"></param>
    /// <param name="forceResVer"></param>
    public void SaveToLocal(int forceResVer)
    {
        if(json != null)
        {
            LocalForceResVersion = forceResVer;
            usingJson["forceResVer"].AsInt = forceResVer;
            try
            {
                json.SaveToFile(cachePath);
            }catch(Exception e)
            {
                Debuger.Err("写入磁盘出错 LocalConfig", e.Message, e.StackTrace);
                //StartupTip.Singleton.TipWriteFileError(GetType().Name, null);
            }
        }
    }
}
