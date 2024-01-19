/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2017.10.25
*/

using UnityEngine;
using System.Collections.Generic;

public class BaseDownloader
{
    protected int urlIdx = 0;
    protected List<string> urlList = new List<string>();
    public bool Loaded { get; private set; }

    protected System.Action mCallback;
    protected int mVersion;
    protected bool loadCache;

    /// <summary>
    /// 检测更新，下载
    /// </summary>
    public virtual void CheckUpdate(System.Action callback, bool useCache = true)
    {
        Debug.Log("startUp " + GetType().Name);
        urlIdx = 0;
        mCallback = callback;
        loadCache = useCache;
        Download();
    }

    public virtual void ReDownload()
    {
        urlIdx = 0;
        Download();
    }

    public virtual void Download()
    {
        Debuger.Log("请重写下载接口", GetType().Name);
    }

    /// <summary>
    /// 下载完成
    /// </summary>
    protected virtual void onLoadCmp(string path, bool success, byte[] data)
    {
		Loaded = false;
        if(!success || data == null)
        {
            if(!AllUrlTried())
                Download();
            else
                Loaded = true;
        }

        if(success && data != null)
        {
            Loaded = true;
        }
    }

    /// <summary>
    /// 进度更新
    /// </summary>
    protected virtual void onLoadUpdate(string path, float progress, ulong loadedBytes)
    {
        if (loadedBytes > 0 && progress > 0)
            Debuger.Log("onLoadUpdate " + GetType().Name + " | " + progress + " | " + (long)(loadedBytes / progress));
            //StartupTip.Singleton.TipProgress(GetType().Name, progress, (long)(loadedBytes / progress));
    }

    protected virtual string getDownloadUrl()
    {
        //Debug.Log("Channel "+ ChannelManager.Channel);
        Debug.Log("ConfigTag " + LocalConfig.Singleton.ConfigTag);

        return getOrgUrl();
    }

    protected virtual string getOrgUrl()
    {
        if (urlIdx >= urlList.Count)
            urlIdx = 0;
        int idx = urlIdx;
        urlIdx++;
        if(urlList.Count > idx)
            return urlList[idx];
        Debug.Log("没有找到下载地址" + GetType().Name);
        return "";
    }

    /// <summary>
    /// 是否所有服务器都尝试过了
    /// </summary>
    public bool AllUrlTried()
    {
        return urlIdx >= urlList.Count;
    }

    /// <summary>
    /// 重置
    /// </summary>
    public virtual void Reset(int version)
    {
        urlIdx = 0;
        mVersion = version;
        urlList.Clear();
    }

    /// <summary>
    /// 设置来源服务器列表
    /// </summary>
    public void SetUrlList(List<string> list)
    {
        if(list!= null && list.Count > 0)
        {
            urlList.Clear();
            urlList.AddRange(list);
        }
    }
}