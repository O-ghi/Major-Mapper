/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2019.9.29
*/
using System;
using UnityEngine;
using System.Collections.Generic;

public class AbcManager : BibManager
{
    private static AbcManager instance;
    public new static AbcManager Singleton
    {
        get
        {
            if (instance == null)
                instance = new AbcManager();
            return instance;
        }
    }
    
    public byte[] GetDll()
    {
        //todo: Duong currently return null 
        var ab = BibManager.Singleton.TryLoadAssetBundleSingle(PathUtil.DllScriptAssignBundleName);
        return ab.LoadAsset<TextAsset>(PathUtil.DllScriptAssignBundleName).bytes;
        //return dll;
    }
    private AssetBundle configgameab = null;
    public byte[] GetBean(string beanName)
    {
        if (configgameab == null)
        {
            configgameab = BibManager.Singleton.TryLoadAssetBundleSingle("configgame");
        }

        return configgameab.LoadAsset<TextAsset>(beanName).bytes;
    }

    public string[] GetDependence(string resName)
    {
        if(this.bundleManifest == null)
        {
            //Duong load dep from assetbundle
            this.LoadMainManifest();
        }
        //duong get dep from AssetBundleManifest
        return this.GetDependencies(resName);
    }
    #region Duong: Thêm xử lý mới cho dependence 

    private AssetBundleManifest bundleManifest;

    private void LoadMainManifest()
    {
        var cachePath = AssetsUtility.GetCachePath();
#if UNITY_EDITOR
        cachePath = AssetsUtility.GetCachePlatformPath();
        cachePath += AssetsUtility.ASSETBUNDLE;
        var ab = AssetBundle.LoadFromFile(cachePath);// (AssetsUtility.ASSETBUNDLE + ".unity3d", error);
#else
        //try load from force folder, if ab is null then load from bibmanager
        var ab  = BibManager.Singleton.TryLoadAssetBundleSingle(AssetsUtility.ASSETBUNDLE);   
#endif


        bundleManifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        //Debuger.Err("Kwang load assetbundle Manifest completed");
    }
    private string[] GetDependencies(string resName)
    {
        resName = resName.Replace(" ", "");
        string[] loadlist = this.bundleManifest.GetDirectDependencies(resName);

        return loadlist;
    }

    #endregion
    private string configName;
    public void SetConfigName(string confName)
    {
        configName = confName;
        offsetMap = new Dictionary<string, int>();
    }

    public void Init(bool useThis)
    {
        if (string.IsNullOrEmpty(configName))
        {
            var a = ABManager.Singleton;
        }
        if (offsetMap == null)
            offsetMap = new Dictionary<string, int>();
        if (!useThis)
        {
            //dll = null;
            //depMap = null;
            //beanMap = null;
            if (offsetMap.Count > 0)
                offsetMap.Clear();
            return;
        }
        if (offsetMap.Count > 0)
            return;

        AssetBundle ab = AssetBundle.LoadFromFile(PathUtil.GetABBuildinPath(configName));
        if (ab == null)
            return;
        //Debug.Log("ABCManager init config file " + configName);
        var ta = ab.LoadAsset<TextAsset>(configName);
        if (ta == null)
            return;
        var bytes = ta.bytes;

        Debuger.Wrn("start init AbcManager");
        int offset = 0;
        var abcName = XBuffer.ReadString(bytes, ref offset);
        bibPath = PathUtil.GetABBuildinPath(abcName);

        //ab
        int abLen = XBuffer.ReadInt(bytes, ref offset);
        for (int i = 0; i < abLen; ++i)
        {
            var name = XBuffer.ReadString(bytes, ref offset);
            var off = XBuffer.ReadInt(bytes, ref offset);
            offsetMap[name] = off;
        }
    }
}