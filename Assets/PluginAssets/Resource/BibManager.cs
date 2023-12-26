/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2019.07.02
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BibManager
{
    private static BibManager instance;
    public static BibManager Singleton
    {
        get
        {
            if (instance == null)
                instance = new BibManager();
            return instance;
        }
    }

    //The file does not contain a suffix
    protected string bibPath;
    protected Dictionary<string, int> offsetMap;
    public void Init(string path, Dictionary<string, int> bibMap)
    {
        if (offsetMap != null)
        {
            Debuger.Err("BibManager has been initialized");
            return;
        }
        //Debug.Log("bibPath: " + path);
        bibPath = path;
        offsetMap = bibMap;
    }
    
    public bool Contains(string withFolderResName)
    {
        if (offsetMap == null)
        {
            //强制初始化/bib初始化在abManager构造函数
            var a = ABManager.Singleton;
        }
        
        if (!withFolderResName.Contains("."))
            withFolderResName += PathUtil.abSuffix;
        return offsetMap.ContainsKey(withFolderResName);
    }
    public AssetBundle TryLoadAssetBundleSingle(string resName)
    {
        AssetBundle ab = null;
        if (PathUtil.IsInStreamFolder(resName))
            ab = ABLoader.Singleton.LoadAssetBundle(resName);
        else
            ab = LoadAssetBundle(resName);
        //if(ab != null)
        //{
        //    Debuger.Err("Kwang TryLoadAssetBundleSingle is success name: " + resName);
        //}
        //else
        //{
        //    Debuger.Err("Kwang TryLoadAssetBundleSingle is faled name: " + resName);

        //}
        return ab;
    }
    public AssetBundle LoadAssetBundle(string resName)
    {
        resName = resName.ToLower();

        if (offsetMap == null)
        {
            // Khởi tạo bắt buộc / khởi tạo bib trong hàm tạo abManager
            var a = ABManager.Singleton;
        }
        var withFolderResName = resName;
        if (ResDepManager.Singleton.IsDefLanguage(resName))
            withFolderResName = PathUtil.resLanguage + "/" + resName;
        if (!withFolderResName.Contains("."))
            withFolderResName += PathUtil.abSuffix;



        if (!offsetMap.ContainsKey(withFolderResName))
            return null;

        var offset = offsetMap[withFolderResName];


        var ab = AssetBundle.LoadFromFile(bibPath, 0, (ulong)offset);
        return ab;
    }
    
    private int nowCoroutineNum = 0;
    private const int maxCoroutineNum = 13;
    private Dictionary<string, System.Action<string, AssetBundle>> loadingMap = new Dictionary<string, System.Action<string, AssetBundle>>();
    private Dictionary<string, ResPriority> loadingPriMap = new Dictionary<string, ResPriority>();
    private Queue<string> toLoadList = new Queue<string>();
    public void LoadAssetBundle(string resName, System.Action<string, AssetBundle> onCmp)
    {
        resName = resName.ToLower();
        if (offsetMap == null)
        {
            //强制初始化/bib初始化在abManager构造函数
            var a = ABManager.Singleton;
        }

        var withFolderResName = resName;
        if (ResDepManager.Singleton.IsDefLanguage(resName))
            withFolderResName = PathUtil.resLanguage + "/" + resName;
        if (!withFolderResName.Contains("."))
            withFolderResName += PathUtil.abSuffix;

        AssetBundle ab = null;
        if (offsetMap.ContainsKey(withFolderResName))
        {
            var offset = offsetMap[withFolderResName];
            ab = AssetBundle.LoadFromFile(bibPath, 0, (ulong)offset);
            //Debuger.Err("Kwang load ContainsKey " + withFolderResName);

        }
        else
        {
            //Debuger.Err("Kwang load not ContainsKey " + withFolderResName);

        }
        if (ab != null)
        {
            //Debuger.Err("Kwang load ab success " + withFolderResName);
        }
        else
        {
            //Debuger.Err("Kwang load ab faled " + withFolderResName);

        }
        if (onCmp != null)
        {
            try
            {
                onCmp(resName, ab);
            }catch (System.Exception e)
            {
                Debuger.Err(e.Message, e.StackTrace);
            }
        }
    }
    
    public void LoadAssetBundleAsync(string resName, System.Action<string, AssetBundle> onCmp, ResPriority priority)
    {
        resName = resName.ToLower();

        if (offsetMap == null)
        {
            //强制初始化/bib初始化在abManager构造函数
            var a = ABManager.Singleton;
        }

        var withFolderResName = resName;
        if (ResDepManager.Singleton.IsDefLanguage(resName))
            withFolderResName = PathUtil.resLanguage + "/" + resName;
        if (!withFolderResName.Contains("."))
            withFolderResName += PathUtil.abSuffix;

        if (priority == ResPriority.Sync || !offsetMap.ContainsKey(withFolderResName))
        {
            //改同步加载
            if (loadingMap.ContainsKey(withFolderResName))
            {
                onCmp += loadingMap[withFolderResName];
                loadingMap.Remove(withFolderResName);
            }
            LoadAssetBundle(resName, onCmp);
            return;
        }

        if (!loadingMap.ContainsKey(withFolderResName))
        {
            toLoadList.Enqueue(resName);
            loadingMap.Add(withFolderResName, onCmp);
            loadingPriMap[withFolderResName] = priority;
            //如果有空闲的协成不用重新开启
            if (nowCoroutineNum < maxCoroutineNum && toLoadList.Count > nowCoroutineNum)
                CoroutineManager.Singleton.startCoroutine(loadABAsyncLimited());
        }
        else
        {
            //如果已经在加载了，那么只需添加回调
            loadingMap[withFolderResName] += onCmp;
            //还没开始加载修改优先级
            if (loadingPriMap.ContainsKey(withFolderResName))
                loadingPriMap[withFolderResName] = priority;
        }
    }

    private IEnumerator loadABAsyncLimited()
    {
        nowCoroutineNum++;
        while (true)
        {
            if (toLoadList.Count > 0)
            {
                var res = toLoadList.Dequeue();
                yield return loadABAsync(res);
            }
            else
            {
                nowCoroutineNum--;
                break;
            }
        }
    }

    /// <summary>
    /// 异步加载ab文件
    /// </summary>
    private IEnumerator loadABAsync(string resName)
    {
        var withFolderResName = resName;
        if (ResDepManager.Singleton.IsDefLanguage(resName))
            withFolderResName = PathUtil.resLanguage + "/" + resName;
        if (!withFolderResName.Contains("."))
            withFolderResName += PathUtil.abSuffix;

        AssetBundle ab = null;
        if(offsetMap.ContainsKey(withFolderResName))
        {
            var offset = offsetMap[withFolderResName];
            var abc = AssetBundle.LoadFromFileAsync(bibPath, 0, (ulong)offset);
            if (loadingPriMap.ContainsKey(withFolderResName))
            {
                abc.priority = (int)loadingPriMap[withFolderResName];
                loadingPriMap.Remove(withFolderResName);
            }
            yield return abc;
            ab = abc.assetBundle;
        }

        if (loadingMap.ContainsKey(withFolderResName))
        {
            //异步加载时同步加载来了，会在同步加载时回调
            System.Action<string, AssetBundle> cb = loadingMap[withFolderResName];
            if (cb != null)
            {
                try
                {
                    cb(resName, ab);
                }
                catch (System.Exception e)
                {
                    Debuger.Err(e.Message, e.StackTrace);
                }
            }
            else
            {
                Debug.LogError("BibManager加载ab文件，怎么可能没有回调：" + withFolderResName);
            }

            if (loadingMap.ContainsKey(withFolderResName))
                loadingMap.Remove(withFolderResName);
        }
    }
}