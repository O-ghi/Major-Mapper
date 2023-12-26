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
using UnityEngine.Networking;

public class ABDownLoader
{
    private static ABDownLoader instance;
    public static ABDownLoader Singleton
    {
        get
        {
            if(instance == null)
                instance = new ABDownLoader();
            return instance;
        }
    }

    private ThreadHandler thHandler = new ThreadHandler(10);
    private List<string> serverUrlList = new List<string>();

    /// <summary>
    /// 服务器地址
    /// </summary>
    public void SetDownLoadServers(List<string> list)
    {
        serverUrlList = list;
    }

    protected virtual string getDownloadUrl()
    {
        if (serverUrlList.Count > 0)
            return serverUrlList[0];
        Debuger.Err("没有找到下载地址 > ABDownloader");
        return "";
    }
    
    public System.Action<string> OnWriteComplete;
    private Dictionary<string, byte[]> writingMap = new Dictionary<string, byte[]>();
    private Dictionary<string, WirteInfo> wroteMap = new Dictionary<string, WirteInfo>();
    private Dictionary<string, LoaderCb> loadingMap = new Dictionary<string, LoaderCb>();

    /// <summary>
    /// 是否处于空闲状态
    /// </summary>
    public bool IsFree
    {
        get
        {
            return writingMap.Count + loadingMap.Count + toLoadList.Count == 0;
        }
    }

    /// <summary>
    /// 下载资源，下载并写入磁盘成功后逻辑自行从磁盘加载资源，可节约内存暂用
    /// 偷跑接口，resName可能重名,需处理对应不同语言问题
    /// </summary>
    /// <param name="resName">资源名</param>
    /// <param name="onSuccess">成功回调（下载并且写入磁盘）</param>
    /// <param name="onFailed">失败回调（下载或者写入磁盘）</param>
    /// <param name="version">版本号</param>
    /// <param name="important">是否优先下载</param>
    public void Load(string resName, string folder, System.Action<string, string> onSuccess, System.Action<string, string, AssetBundle> onFailed, int version = 0, bool important = true)
    {
        if(!downloadEnable)
        {
            //不让下载则立即回调失败
            if (onFailed != null)
                onFailed(resName, folder, null);
            onSuccess = null;
            onFailed = null;
        }

        string withFolderName = resName;
        if (!string.IsNullOrEmpty(folder))
            withFolderName = folder + "/" + resName;

        if(!loadingMap.ContainsKey(withFolderName))
        {
            //添加到加载列表
            loadingMap.Add(withFolderName, new LoaderCb { failedCb = onFailed, successCb = onSuccess });
            if(important)
                toLoadList.Insert(0, new LoadInfo { resName = resName, folder = folder, version = version });
            else
                toLoadList.Add(new LoadInfo { resName = resName, folder = folder, version = version });

            if(nowCoroutineNum < MaxCoroutineNum)
                CoroutineManager.Singleton.startCoroutine(downloadResourceLimited());
        } else
        {
            //正在下载或者写磁盘
            loadingMap[withFolderName].failedCb += onFailed;
            loadingMap[withFolderName].successCb += onSuccess;
        }
    }

    private int nowCoroutineNum;
    public static int MaxCoroutineNum = 5;
    private List<LoadInfo> toLoadList = new List<LoadInfo>();

    //写文件走后台线程
    public bool WriteFileInBackThread = true;
    
    private WaitForSeconds waitOneSecond = new WaitForSeconds(1f);
    private WaitForSeconds waitPointOneSecond = new WaitForSeconds(0.1f);

    public bool downloadEnable { get; private set; }
    public void SetDownloadEnable(bool value)
    {
        Debuger.Log("资源下载开关>" + value);
        downloadEnable = value;
        if(!value)
        {
            string folder = "", res = "";
            foreach (var cb in loadingMap)
            {
                res = cb.Key;
                folder = null;

                if (cb.Key.Contains("/"))
                {
                    var arr = cb.Key.Split('/');
                    folder = arr[0];
                    res = arr[1];
                }

                if (cb.Value.failedCb != null)
                    cb.Value.failedCb(res, folder, null);
                cb.Value.failedCb = null;
                cb.Value.successCb = null;
            }
        }
    }

    //检测到异常情况下调用
    public void ClearAll()
    {
        SetDownloadEnable(false);
        writingMap.Clear();
        toLoadList.Clear();
        loadingMap.Clear();
    }

    private IEnumerator downloadResourceLimited()
    {
        nowCoroutineNum++;
        while(true)
        {
            if(downloadEnable && toLoadList.Count > 0)
            {
                LoadInfo li = toLoadList[0];
                toLoadList.RemoveAt(0);
                //已经下载了就不用下载了(逻辑层先于偷跑下载可能出现)
                var withFolderName = li.resName;
                if (!string.IsNullOrEmpty(li.folder))
                    withFolderName = li.folder + "/" + li.resName;
                if (!writingMap.ContainsKey(withFolderName))
                    yield return downloadResource(li.folder, li.resName, li.version);
            } else
            {
                yield return waitOneSecond;
            }
        }
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    private IEnumerator downloadResource(string folder, string resName, int version)
    {
        //?用于cdn版本号
        var withFolderName = resName;
        if (!string.IsNullOrEmpty(folder))
            withFolderName = folder + "/" + resName;
        UnityWebRequest download = new UnityWebRequest(PathUtil.GetServerPath(getDownloadUrl(), withFolderName) + "?" + version);
        //UnityWebRequest download = new UnityWebRequest(PathUtil.GetServerPath(getDownloadUrl(), resName) + "?" + version, UnityWebRequest.kHttpVerbGET, new DownloadHandlerBuffer(), null);
        yield return download;

        //if (download.isError || download.responseCode != 200)
        if(!string.IsNullOrEmpty(download.error))
        {
            //下载失败
            Debuger.Err(download.error, download.url);
            if(loadingMap.ContainsKey(withFolderName))
            {
                var cb = loadingMap[withFolderName];
                try
                {
                    if(cb.failedCb != null)
                        cb.failedCb(resName, folder, null);
                } catch(System.Exception e)
                {
                    Debuger.Err(e.Message, e.StackTrace);
                }
                loadingMap.Remove(withFolderName);
            }
        } else
        {
            /*
                        byte[] data = download.bytes;
                        while(true)
                        {
                            // 等待写线程写入完成
                            if(!writingMap.ContainsKey(withFolderName))
                            {
                                // 先加到写队列，通知线程写入
                                writingMap.Add(withFolderName, data);
                                writeToDisk(withFolderName, folder, data);
                            } else
                            {
                                WirteInfo info = null;
                                wroteMap.TryGetValue(withFolderName, out info);
                                if(info != null && info.deal == false)
                                {
                                    // 写文件已完成   
                                    info.deal = true;
                                    if (writingMap.ContainsKey(withFolderName))
                                        writingMap.Remove(withFolderName);

                                    LoaderCb cb = null;
                                    if (loadingMap.ContainsKey(withFolderName))
                                    {
                                        cb = loadingMap[withFolderName];
                                        loadingMap.Remove(withFolderName);
                                    }else
                                    {
                                        cb = new LoaderCb();
                                    }

                                    try
                                    {
                                        if(info.result)
                                        {
                                            //写文件成功
                                            if(OnWriteComplete != null)
                                                OnWriteComplete(withFolderName);
                                            if (cb.successCb != null)
                                                cb.successCb(resName, folder);
                                        } else
                                        {
                                            //写文件失败
                                            if(cb.failedCb != null)
                                                cb.failedCb(resName, folder, download.assetBundle);

                                            //最后一个文件失败时给一个提示
                                            if (IsFree)
                                                StartupTip.Singleton.TipWriteFileError(GetType().Name, () => { }, () => { });
                                        }
                                    } catch(System.Exception e)
                                    {
                                        Debuger.Err(e.Message, e.StackTrace);
                                    }
                                    break;
                                } else
                                {
                                    yield return waitPointOneSecond;
                                }
                            }
                        }*/
        }
        download.Dispose();
        yield return null;
    }

    ///写磁盘异步
    private void writeToDisk(string res, string folder, byte[] downloadData)
    {
        if(!thHandler.IsRunning)
            thHandler.Start(10 * 60 * 1000); //10分钟

        if (WriteFileInBackThread)
            thHandler.PushHandler(threadSave, new SaveInfo { data = downloadData, folder = folder, res = PathUtil.GetBackABPath(res) });
        else
            threadSave(new SaveInfo { data = downloadData, folder = folder, res = PathUtil.GetBackABPath(res) });
    }

    private void threadSave(object obj)
    {
        SaveInfo si = obj as SaveInfo;
        if(si != null)
            save(si.data, si.folder, si.res);
    }

    private readonly object wirteLock = new object();
    private class SaveInfo
    {
        public byte[] data;
        public string res;
        public string folder;
    }

    private void save(byte[] data, string folder, string fullPath)
    {
        lock(wirteLock)
        {
            string resName = Path.GetFileName(fullPath);
            string resName2 = Path.GetFileNameWithoutExtension(fullPath);
            if(!string.IsNullOrEmpty(folder))
            {
                resName = folder + "/" + resName;
                resName2 = folder + "/" + resName2;
            }

            try
            {
                var dir = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllBytes(fullPath, data);
                if(wroteMap.ContainsKey(resName))
                {
                    wroteMap[resName].deal = false;
                    wroteMap[resName].result = true;

                    wroteMap[resName2].deal = false;
                    wroteMap[resName2].result = true;
                } else
                {
                    wroteMap.Add(resName, new WirteInfo { deal = false, result = true });
                    wroteMap.Add(resName2, new WirteInfo { deal = false, result = true });
                }
            } catch
            {
                if(File.Exists(fullPath))
                    File.Delete(fullPath);

                if(wroteMap.ContainsKey(resName))
                {
                    wroteMap[resName].deal = false;
                    wroteMap[resName].result = false;
                } else
                {
                    wroteMap.Add(resName, new WirteInfo { deal = false, result = false });
                }
                //Debuger.Err("写文件出错 ABDownLoader", resName, e.Message);
            }
        }
    }

    private class WirteInfo
    {
        //是否已处理
        public bool deal;
        //是否写成功
        public bool result;
    }

    private struct LoadInfo
    {
        public string folder;
        public string resName;
        public int version;
    }

    private class LoaderCb
    {
        public System.Action<string, string> successCb;
        public System.Action<string, string, AssetBundle> failedCb;
    }
}