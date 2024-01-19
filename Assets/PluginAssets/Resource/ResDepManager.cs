/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2017.10.18
*/

using UnityEngine;
using System.Collections.Generic;

public class ResDepManager
{
    private static ResDepManager instance;
    public static ResDepManager Singleton
    {
        get
        {
            if (instance == null)
                instance = new ResDepManager();
            return instance;
        }
    }

    /// <summary>
    /// 初始化依赖信息，手机上才需要
    /// </summary>
    public void LoadDeps(bool forceReload = false)
    {
#if UNITY_EDITOR && Main
        //chi load dependence khi su dung mode assetbundle
        if (UnityEditor.EditorPrefs.GetBool("useAssetDatabase_Enable", true) == false)
        {
            LoadDepsAssets(forceReload);

        }

#else
        LoadDepsAssets(forceReload);
#endif
    }



    private void LoadDepsAssets(bool forceReload)
    {
        //Duong: Su dung AssetBundleManifest nen ko can load dep Assets nao khac
    }

    public bool IsDefLanguage(string resName, string path = "")
    {
#if UNITY_EDITOR
        if (!resName.Contains("."))
            resName = resName + PathUtil.abSuffix;
        if (System.IO.File.Exists(path + "/" + PathUtil.resLanguage + "/" + resName))
            return true;
#endif
        //Luon su dung asset o thu muc gốc (chi dùng 1 ngôn ngữ)
        return false;
    }

    //是否包含语言
    public bool HasLanguage(string language)
    {
#if UNITY_EDITOR
        return System.IO.Directory.Exists(PathUtil.EditorUIABPath + language);
#else
        //return languageList.Contains(language);
        return true;
#endif
    }

    ///获取依赖
    public string[] GetDependence(string resName, bool includeMark = true)
    {
#if UNITY_EDITOR && Main
        //Duong: Neu mode database thi return null, nguoc lai thi doc tu Assetbundle
        if (UnityEditor.EditorPrefs.GetBool("useAssetDatabase_Enable", true) == true)
        {
            return null;

        }
#endif
        var deps = AbcManager.Singleton.GetDependence(resName);
        if (deps != null)
            return deps;

        return null;
    }
}