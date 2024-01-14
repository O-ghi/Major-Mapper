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

public class BackFileList : BaseDownloader
{
    public const string DownloadType_Back = "back"; //后台下载
    public const string DownloadType_hand = "hand"; //手动下载
    public const string DownloadType_Notice = "notice"; //数据流量下载时提示

    private static BackFileList instance;
    public static BackFileList Singleton
    {
        get
        {
            if (instance == null)
                instance = new BackFileList();
            return instance;
        }
    }

    public override void CheckUpdate(System.Action callback, bool useCache = true)
    {
#if UNITY_EDITOR
        if (callback != null)
            callback();
#else
        base.CheckUpdate(callback, useCache);
#endif
    }

    public override void Download()
    {
        WWWLoader.Singleton.Download(getDownloadUrl() + "?" + DateTime.Now.Ticks, onLoadCmp, null);
    }

    /// <summary>
    /// 对应语言需要下载的文件列表
    /// </summary>
    public List<FileEntry> GetToDownloadLanguageFileList(string language)
    {
        if (fileArr == null)
            return null;

        int ver, level, sz;
        string name = "", folder = "";
        List<FileEntry> list = new List<FileEntry>();
        for (int i = 0, len = fileArr.Count; i < len; ++i)
        {
            var resPath = fileArr[i]["res"].Value;
            if (resPath.IndexOf("/") <= 0)
                continue;
            var pathArr = resPath.Split('/');
            //只读取当前语言配置
            if (language != pathArr[0])
                continue;

            folder = pathArr[0];
            name = pathArr[1];
            //添加后台需要下载的资源
            sz = fileArr[i]["size"].AsInt;
            ver = fileArr[i]["ver"].AsInt;
            level = fileArr[i]["level"].AsInt;

            //需要偷跑的加入下载列表
            ABManager.Singleton.SetServerVersion(name, folder, ver);
            list.Add(new FileEntry { resName = name, folder = folder, version = ver, priority = level, size = sz });
        }
        return list;
    }

    protected List<FileEntry> fileList = new List<FileEntry>();
    protected List<string> downloadUrlList = new List<string>();
    protected JSONArray fileArr;
    protected int totalCount;
    protected long m_totalSize = -1;

    public string downloadType;
    public bool IsDownloading
    {
        get
        {
            return downloadStarted && ABDownLoader.Singleton.downloadEnable;
        }
    }

    private bool downloadStarted;
    public void SetDownloadUrl(List<string> list)
    {
        downloadUrlList = list;
    }

    protected override void onLoadCmp(string path, bool success, byte[] data)
    {
        base.onLoadCmp(path, success, data);
        if (Loaded)
        {
            fileList.Clear();
            if (success && data != null)
            {
                string str = System.Text.Encoding.Default.GetString(data);
                JSONClass json = JSONClass.Parse(str) as JSONClass;
                if (json != null && json["files"] != null && json["urls"] != null)
                {
                    fileArr = json["files"] as JSONArray;

                    //ab下载服务器设置
                    ABManager.Singleton.SaveVersionToDisk();
                    ABDownLoader.Singleton.SetDownLoadServers(downloadUrlList);

                    if (mCallback != null)
                        mCallback();
                }
                else
                {
                    Debuger.Err("偷跑列表下载失败，请检查网络链接", path);
                    CoroutineManager.Singleton.delayedCall(3, ReDownload);
                }
            }
            else
            {
                Debuger.Err("偷跑列表下载失败，请检查网络链接", path);
                CoroutineManager.Singleton.delayedCall(3, ReDownload);
            }
        }
    }

    private void compareFiles()
    {
        if (fileArr == null)
            return;

        m_totalSize = 0;
        lastSizeFrame = 0;
        cacheLoadedSize = 0;
        fileList.Clear();
        int ver, level, sz;
        string name = "", folder = "";
        for (int i = 0, len = fileArr.Count; i < len; ++i)
        {
            var resPath = fileArr[i]["res"].Value;
            if (resPath.IndexOf("/") > 0)
            {
                var pathArr = resPath.Split('/');
                //只读取当前语言配置
                if (PathUtil.resLanguage != pathArr[0])
                    continue;
                folder = pathArr[0];
                name = pathArr[1];
            }
            else
            {
                folder = null;
                name = resPath;
            }
            //添加后台需要下载的资源
            sz = fileArr[i]["size"].AsInt;
            ver = fileArr[i]["ver"].AsInt;
            level = fileArr[i]["level"].AsInt;

            //需要偷跑的加入下载列表
            ABManager.Singleton.SetServerVersion(name, folder, ver);
            if (ver > ABManager.Singleton.GetLocalVersion(name, folder))
            {
                m_totalSize += sz;
                fileList.Add(new FileEntry { resName = name, folder = folder, version = ver, priority = level, size = sz });
            }
        }
    }

    private int sortFileList(FileEntry f1, FileEntry f2)
    {
        return f2.priority.CompareTo(f1.priority);
    }

    private long checkId;
    /// <summary>
    /// 开始后台下载，优先级大于0才会在后台下载
    /// </summary>
    public void BeginBackDownload()
    {
        if (downloadStarted)
            return;

        downloadStarted = true;
        compareFiles();
        fileList.Sort(sortFileList);
        ABDownLoader.Singleton.SetDownloadEnable(true);
        CoroutineManager.Singleton.delayedCall(3f, downloadBackList);
    }

    public bool IsFileListLoaded()
    {
        return fileArr != null;
    }

    /// <summary>
    /// 取消下载
    /// </summary>
    public void CancelDownload()
    {
        downloadStarted = false;
        ABDownLoader.Singleton.ClearAll();
        CoroutineManager.Singleton.stopCoroutine(checkId);
    }

    private void downloadBackList()
    {
        Debuger.Log("-----------偷跑长度", fileList.Count);
        if (fileList.Count > 0)
        {
            for (int i = 0, len = fileList.Count; i < len; ++i)
            {
                ABDownLoader.Singleton.Load(fileList[i].resName, fileList[i].folder, null, null, fileList[i].version, false);
            }
            checkId = CoroutineManager.Singleton.delayedCall(10f, onCheckFailed);
        }
        else
        {
            if (checkId > 0)
                CoroutineManager.Singleton.stopCoroutine(checkId);
            checkId = 0;
        }
    }

    public bool isAllLoaded()
    {
        return getLoadedSize() > totalSize() - 1L;
    }

    public long totalSize()
    {
        if (fileArr == null)
            return -1L;
        if (m_totalSize < 0L)
            compareFiles();
        return m_totalSize;
    }

    private int lastSizeFrame;
    private long cacheLoadedSize;
    public long getLoadedSize()
    {
        if (m_totalSize < 0L)
            compareFiles();

        if (cacheLoadedSize >= m_totalSize)
            return m_totalSize;

        if (Time.frameCount - lastSizeFrame < 5)
            return cacheLoadedSize;

        lastSizeFrame = Time.frameCount;
        long loadedSize = 0;
        for (int i = 0, len = fileList.Count; i < len; ++i)
        {
            var file = fileList[i];
            if (file.version <= ABManager.Singleton.GetLocalVersion(file.resName, file.folder))
                loadedSize += file.size;
        }
        cacheLoadedSize = loadedSize;
        return loadedSize;
    }

    private long lastLeftSize;
    private void onCheckFailed()
    {
        //偷跑已暂停
        if (!ABDownLoader.Singleton.downloadEnable)
        {
            checkId = CoroutineManager.Singleton.delayedCall(10f, onCheckFailed);
            return;
        }

        long leftSize = totalSize() - getLoadedSize();
        bool isEquial = leftSize == lastLeftSize;
        lastLeftSize = leftSize;

        checkId = 0;
        //下载量没变化了,或者下载完了
        if (ABDownLoader.Singleton.IsFree || isEquial)
        {
            //异常情况，取消下载重新下
            if (isEquial && ABDownLoader.Singleton.downloadEnable)
            {
                ABDownLoader.Singleton.ClearAll();
                ABDownLoader.Singleton.SetDownloadEnable(true);
            }

            //if (GameSDK.Singleton.IsDiskLowMemory())
            //{
            //    //磁盘写满，等待磁盘清空
            //    Debuger.Log("-----------磁盘空间不足，偷跑稍后再作尝试");
            //    checkId = CoroutineManager.Singleton.delayedCall(15f, onCheckFailed);
            //}
            else
            {
                //中途断网，重新开始下载
                compareFiles();
                if (fileList.Count > 0)
                    downloadBackList();
                else
                    Debuger.Log("-----------偷跑已完成");
            }
        }
        else
        {
            checkId = CoroutineManager.Singleton.delayedCall(10f, onCheckFailed);
        }
    }
}