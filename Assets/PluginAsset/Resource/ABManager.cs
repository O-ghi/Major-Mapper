/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2017.10.25
*/

using System;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;

public class ABManager
{
    private static ABManager instance;
    public static ABManager Singleton
    {
        get
        {
            if(instance == null)
                instance = new ABManager();
            return instance;
        }
    }

    //key都包含资源语言目录
    private JSONClass jsonForce;
    private JSONClass jsonLocal;
    private JSONClass jsonServer;

    private JSONClass jsonLocalSave;
    private JSONClass jsonServerSave;
    private JSONArray jsonChange;

    private string localPath = PathUtil.BackLocalConfigPath;
    private string serverPath = PathUtil.BackServerConfigPath;
    private Dictionary<string, string> buildInMD5Map;
    public ABManager()
    {
        ABDownLoader.Singleton.OnWriteComplete = RemoveServerToLocal;
        jsonLocal = new JSONClass();
        jsonServer = new JSONClass();
        jsonForce = new JSONClass();

        jsonLocalSave = new JSONClass();
        jsonServerSave = new JSONClass();
        jsonChange = new JSONArray();

        LoadVersionConf();
    }

    private void LoadVersionConf()
    {
        if(System.IO.File.Exists(localPath))
        {
            //已更新资源
            try { 
                jsonLocal = JSONNode.LoadFromFile(localPath) as JSONClass;
                jsonLocalSave = JSONNode.LoadFromFile(localPath) as JSONClass;
            }catch(Exception e)
            {
                Debuger.Err(e.Message, e.StackTrace);
            }
        }
        if(jsonLocal == null)
        {
            jsonLocal = new JSONClass();
            jsonLocalSave = new JSONClass();
        }

        if(System.IO.File.Exists(serverPath))
        {
            //未更新资源
            try
            {
                jsonServer = JSONNode.LoadFromFile(serverPath) as JSONClass;
                jsonServerSave = JSONNode.LoadFromFile(serverPath) as JSONClass;
            }catch(Exception e)
            {
                Debuger.Err(e.Message, e.StackTrace);
            }
        }
        if(jsonServer == null)
        {
            jsonServer = new JSONClass();
            jsonServerSave = new JSONClass();
        }

        try
        {
            //强更资源
            if(System.IO.File.Exists(PathUtil.ForceFileListOutPath))
                jsonForce = JSONNode.LoadFromFile(PathUtil.ForceFileListOutPath) as JSONClass;
        }catch(Exception e)
        {
            Debuger.Err(e.Message, e.StackTrace);
        }
        if(jsonForce == null)
            jsonForce = new JSONClass();

        //首包资源信息
        try
        {
            //Debuger.Err("Kwang try load BibManager"+ PathUtil.BuildInFileListName);

            //包内资源
            var ta = Resources.Load<TextAsset>(System.IO.Path.GetFileNameWithoutExtension(PathUtil.BuildInFileListName));
            if (ta == null)
                return;
            //Debuger.Err("Kwang try load ta length" + ta.bytes.Length);

            int offset = 0;
            var bytes = ta.bytes;
            buildInMD5Map = new Dictionary<string, string>();
            var bibOffsetMap = new Dictionary<string, int>();

            //bib名字
            string bibName = XBuffer.ReadString(bytes, ref offset);
            //abcConf名字
            string abcConfName = XBuffer.ReadString(bytes, ref offset);
        //Lớp phủ và cập nhật thông tin MD5
        int addCount = XBuffer.ReadInt(bytes, ref offset);
            for (int i = 0; i < addCount; ++i)
            {
                var name = XBuffer.ReadString(bytes, ref offset);
                var md5 = XBuffer.ReadString(bytes, ref offset);
                buildInMD5Map[name] = md5;
            }

        //Thông tin tài nguyên trong gói
        int count = XBuffer.ReadInt(bytes, ref offset);
            //Debuger.Log("Kwang count size:" + count);

            for (int i = 0; i < count; ++i)
            {
                var name = XBuffer.ReadString(bytes, ref offset);
                var md5 = XBuffer.ReadString(bytes, ref offset);
                var bOffset = XBuffer.ReadInt(bytes, ref offset);
                buildInMD5Map[name] = md5;
                bibOffsetMap[name] = bOffset;
            }
            //Debuger.Log("Kwang bibOffsetMap size:" + bibOffsetMap.Count);
            BibManager.Singleton.Init(PathUtil.GetABBuildinPath(bibName), bibOffsetMap);
            AbcManager.Singleton.SetConfigName(abcConfName);
        }
        catch (Exception e)
        {
            Debuger.Err(e.Message, e.StackTrace);
        }
        if (buildInMD5Map == null)
            buildInMD5Map = new Dictionary<string, string>();
    }

    //更新强更文件
    public void UpdateForceFile()
    {
        try
        {
            //强更资源
            if (System.IO.File.Exists(PathUtil.ForceFileListOutPath))
            {
                var jsonArr = JSONNode.LoadFromFile(PathUtil.ForceFileListOutPath) as JSONClass;
                jsonForce = jsonArr;
            }
        }
        catch (Exception e)
        {
            Debuger.Err(e.Message, e.StackTrace);
        }
        if (jsonForce == null)
            jsonForce = new JSONClass();
    }

    /// <summary>
    /// 获取强更资源MD5码(包含多语言目录)
    /// </summary>
    public string GetForceFileMD5(string withFolderResName)
    {
        if (!withFolderResName.Contains("."))
            withFolderResName += PathUtil.abSuffix;
        if (jsonForce[withFolderResName] != null)
            return jsonForce[withFolderResName]["md5"].Value;
        if (buildInMD5Map == null || !buildInMD5Map.ContainsKey(withFolderResName))
            return null;
        return buildInMD5Map[withFolderResName];
    }

    /// <summary>
    /// 修改服务器资源版本号
    /// </summary>
    public void SetServerVersion(string resName, string folder, int version)
    {
        if (!string.IsNullOrEmpty(folder))
            resName = folder + "/" + resName;

        jsonServer[resName].AsInt = version;
        JSONClass node = new JSONClass();
        node["name"] = resName;
        node["type"].AsInt = 1; //server add
        node["ver"].AsInt = version;
        jsonChange.Add(node);
    }

    /// <summary>
    /// 本地版本号
    /// </summary>
    public int GetLocalVersion(string resName, string folder, bool includeObb = true)
    {
        string withFolderName = resName;
        if (!string.IsNullOrEmpty(folder))
            withFolderName = folder + "/" + resName;

        int localVersion = -1;
        if (jsonLocal[withFolderName] != null)
            localVersion = jsonLocal[withFolderName].AsInt;

        if (includeObb)
        {
            int obbVersion = ObbManager.Singleton.GetVersion(withFolderName);
            if (obbVersion > localVersion)
                return obbVersion;
        }
        return localVersion;
    }

    /// <summary>
    /// 服务器版本号
    /// </summary>
    private int GetServerVersion(string resName, string folder)
    {
        if (!string.IsNullOrEmpty(folder))
            resName = folder + "/" + resName;

        if(jsonServer[resName] != null)
            return jsonServer[resName].AsInt;
        return -1;
    }
    
    private long lastSaveTime;
    private const long saveCoolTime = 10000 * 1000 * 30;//30秒写一次
    /// <summary>
    /// 服务器资源移动到本地资源列表中
    /// </summary>
    public void RemoveServerToLocal(string withFolderName)
    {
        withFolderName = withFolderName.ToLower();
        JSONNode serverVer = jsonServer[withFolderName];
        ///有后缀
        if(serverVer == null)
        {
            if(!withFolderName.Contains("."))
                withFolderName += PathUtil.abSuffix;
            serverVer = jsonServer[withFolderName];
        }
        if(serverVer != null)
        {
            jsonLocal[withFolderName] = serverVer;
            jsonServer.Remove(withFolderName);

            JSONClass node = new JSONClass();
            node["name"] = withFolderName;
            node["type"].AsInt = 2; //server remove to local
            node["ver"].AsInt = serverVer.AsInt;
            jsonChange.Add(node);
        }

        _SaveToLocalJson();
    }

    private void _SaveToLocalJson()
    {
        long now = System.DateTime.Now.Ticks;
        bool allLoaded = ABDownLoader.Singleton.IsFree;
        if(allLoaded || now - lastSaveTime > saveCoolTime)
        {
            lastSaveTime = now;
            //存磁盘
            saveJson(jsonChange);
            jsonChange = new JSONArray();
        }
    }

    /// <summary>
    /// 保存列表
    /// </summary>
    public void SaveVersionToDisk()
    {
        //同步存
        saveJson(jsonChange);
        jsonChange = new JSONArray();
    }

    /// <summary>
    /// 加载资源, 此处从游戏逻辑过来，resName不能重名,不处理重名情况
    /// 除非为特殊资源，普通ab资源不要后缀
    /// 非ab后缀的资源请求需带后缀，且不支持Obb和多语言
    /// </summary>
    /// <param name="resName">资源名字</param>
    /// <param name="callback">回调函数</param>
    /// <param name="async">是否异步</param>
    public void LoadAB(string resName, Action<string, AssetBundle> callback, ResPriority priority)
    {
#if UNITY_EDITOR
        ABLoader.Singleton.LoadAssetBundleAsync(resName, callback, priority);
#else    
        JSONNode serverVer = jsonServer[resName];
        JSONNode localVer  = jsonLocal[resName];
        bool isDefLan = ResDepManager.Singleton.IsDefLanguage(resName);
        var folder = isDefLan ? PathUtil.resLanguage : "";
        string withFolderResName = isDefLan ? (PathUtil.resLanguage + "/" + resName) : resName;

        ///有后缀
        if (serverVer == null && localVer == null)
        {
            string webResName = withFolderResName;
            if(!webResName.Contains("."))
                webResName += PathUtil.abSuffix;
            serverVer = jsonServer[webResName];
            localVer = jsonLocal[webResName];
        }

        var isInAbc = AbcManager.Singleton.Contains(resName);
        if(isInAbc)
        {
            //abc绝对优先
            AbcManager.Singleton.LoadAssetBundleAsync(resName, callback, priority);
            return;
        }

        bool fromServer = true;
        if (!string.IsNullOrEmpty(GetForceFileMD5(withFolderResName)))
        {
            //强更资源走本地加载
            fromServer = false;
        }else
        {
            //偷跑资源
            if(serverVer == null)
                fromServer = false;     //未下载清单没有，从本地下载(都没有则从本地读取)
            else if(localVer == null)
                fromServer = true;      //本地没有从服务器下载
            else
                fromServer = serverVer.AsInt > localVer.AsInt; //下载版本号大的(一样大从本地读取)

            //整包只从本地读取
            //if (LocalConfig.Singleton.FullPackage)
            //    fromServer = false;
        }

        if (callback != null)
        {
            if(callbackMap.ContainsKey(resName))
                callbackMap[resName] += callback;
            else
                callbackMap.Add(resName, callback);
        }
        
        if (fromServer)
        {
            var obbVersion = ObbManager.Singleton.GetVersion(withFolderResName);
            if(obbVersion >= serverVer.AsInt)
            {
                //偷跑先从obb资源中查找
                ObbManager.Singleton.LoadAssetBundleAsync(resName, serverVer.AsInt, (res, ab) => {
                    onLocalABLoaded(resName, ab, priority);
                }, priority);
            }
            else
            {
                if (priority == ResPriority.Sync)
                {
                    Debuger.Err("服务器资源无法同步加载, 先从本地读取替代", resName);
                    if (callbackMap.ContainsKey(resName))
                    {
                        //先从本地读取
                        AssetBundle forceAb = null;
                        if (PathUtil.IsInStreamFolder(resName))
                            forceAb = ABLoader.Singleton.LoadAssetBundle(resName);
                        else
                            forceAb = BibManager.Singleton.LoadAssetBundle(resName);

                        if (forceAb == null)
                        {
                            //本地没有，读取obb资源中的旧版本
                            var obbAb = ObbManager.Singleton.LoadAssetBundle(resName, 0);
                            if (obbAb != null)
                                onLocalABLoaded(resName, obbAb, priority);
                            else
                                onServerFailed(resName, isDefLan ? PathUtil.resLanguage : null, null);
                        }
                        else
                        {
                            onLocalABLoaded(resName, forceAb, priority);
                        }
                    }
                }

                priMap[resName] = priority;
                //优先级需要提到最高
                if (isDefLan)
                    ABDownLoader.Singleton.Load(resName, PathUtil.resLanguage, onServerSuccess, onServerFailed, GetServerVersion(resName, PathUtil.resLanguage), true);
                else
                    ABDownLoader.Singleton.Load(resName, null, onServerSuccess, onServerFailed, GetServerVersion(resName, null), true);
            }
        }else
        {
            //容错机制，本地读取失败则从obb中加载，obb中再失败则强行从服务器读取
            if (PathUtil.IsInStreamFolder(resName))
            {
                ABLoader.Singleton.LoadAssetBundleAsync(resName, (res, ab) => {
                    checkLocalABLoaded(resName, (localVer != null ? localVer.AsInt : 0), ab, priority);
                }, priority);
            }else
            {
                BibManager.Singleton.LoadAssetBundleAsync(resName, (res, ab) => {
                    checkLocalABLoaded(resName, (localVer != null ? localVer.AsInt : 0), ab, priority);
                }, priority);
            }
        }
#endif
    }

    private Dictionary<string, ResPriority> priMap = new Dictionary<string, ResPriority>();
    private Dictionary<string, Action<string, AssetBundle>> callbackMap = new Dictionary<string, Action<string, AssetBundle>>();
    /// <summary>
    /// 从服务器下载成功
    /// </summary>
    private void onServerSuccess(string resName, string folder)
    {
        var priority = ResPriority.Async;
        if (priMap.ContainsKey(resName))
        {
            priority = priMap[resName];
            priMap.Remove(resName);
        }

        if (!callbackMap.ContainsKey(resName))
            return;
        
        ABLoader.Singleton.LoadAssetBundleAsync(resName, callbackMap[resName], priority);
        callbackMap.Remove(resName);
    }

    /// <summary>
    /// 从服务器下载失败（可能写本地磁盘失败）
    /// </summary>
    private void onServerFailed(string resName, string folder, AssetBundle ab)
    {
        if(priMap.ContainsKey(resName))
            priMap.Remove(resName);

        if(!callbackMap.ContainsKey(resName))
            return;

        try
        {
            callbackMap[resName](resName, ab);
        }catch(Exception e)
        {
            Debuger.Err(e.Message + "\n" + e.StackTrace);
        }
        callbackMap.Remove(resName);
    }
    
    //本地加载失败后尝试从obb中读取
    private void checkLocalABLoaded(string resName, int abVersion, AssetBundle assetBundle, ResPriority priority)
    {
        if (assetBundle != null)
            onLocalABLoaded(resName, assetBundle, priority);
        else
            ObbManager.Singleton.LoadAssetBundleAsync(resName, abVersion, (res, ab) => { onLocalABLoaded(resName, ab, priority); }, priority);
    }

    /// <summary>
    /// 本地加载ab结果
    /// </summary>
    private void onLocalABLoaded(string resName, AssetBundle ab, ResPriority priority)
    {
        if (!callbackMap.ContainsKey(resName))
            return;
        
        if(ab != null || priority == ResPriority.Sync)
        {
            try
            {
                callbackMap[resName](resName, ab);
            } catch(Exception e)
            {
                Debuger.Err(e.Message + "\n" + e.StackTrace);
            }
            callbackMap.Remove(resName);
        } else
        {
            if (ResDepManager.Singleton.IsDefLanguage(resName))
                ABDownLoader.Singleton.Load(resName, PathUtil.resLanguage, onServerSuccess, onServerFailed, GetServerVersion(resName, PathUtil.resLanguage), true);
            else
                ABDownLoader.Singleton.Load(resName, null, onServerSuccess, onServerFailed, GetServerVersion(resName, null), true);
        }
    }






    /// 保存配置逻辑↓↓↓
    private ThreadHandler thHandler = new ThreadHandler();
    private readonly object wirteLock = new object();
    private void saveJson(JSONArray arr)
    {
         if(!thHandler.IsRunning)
             thHandler.Start(10 * 60 * 1000);//10分钟
         thHandler.PushHandler(threadSave, arr);
    }

    private void threadSave(object obj)
    {
        lock(wirteLock)
        {
            JSONArray arr = obj as JSONArray;
            if(arr != null)
            {
                string resName = "";
                int version = 0;
                var enu = arr.GetEnumerator();
                while(enu.MoveNext())
                {
                    JSONNode node = enu.Current as JSONNode;
                    resName = node["name"].Value;
                    version = node["ver"].AsInt;
                    switch(node["type"].AsInt)
                    {
                        case 1: //设置server版本号
                        jsonServerSave[resName].AsInt = version;
                        break;
                        case 2: //将server移到local
                        jsonServerSave.Remove(resName);
                        jsonLocalSave[resName].AsInt = version; 
                        break;
                    }
                }
            }

            try
            {
                if(System.IO.File.Exists(localPath))
                    System.IO.File.Delete(localPath);
                if(System.IO.File.Exists(serverPath))
                    System.IO.File.Delete(serverPath);

                jsonLocalSave.SaveToFile(localPath);
                jsonServerSave.SaveToFile(serverPath);
            } catch(System.Exception e)
            {
                if(System.IO.File.Exists(localPath))
                    System.IO.File.Delete(localPath);
                if(System.IO.File.Exists(serverPath))
                    System.IO.File.Delete(serverPath);
                try
                {
                    System.IO.File.AppendAllText(localPath + "_errLog", string.Format("时间:{0}\nab列表写入失败，已删除重新下载\nMessage:{1}\nStackTrace:{2}\n\n", System.DateTime.Now.ToString(), e.Message, e.StackTrace));
                }catch
                {

                }
                //Debuger.Err("写文件出错 ABManager", path, e.Message);
            }
        }
    }
}