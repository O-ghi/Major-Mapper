/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2017.10.20
*/

using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ForceManager
{
    private static ForceManager m_instance;
    public static ForceManager Singleton
    {
        get
        {
            if (m_instance == null)
                m_instance = new ForceManager();
            return m_instance;
        }
    }

    private SimpleJSON.JSONClass json;
    private ForceManager()
    {
        string path = PathUtil.UpdatedConfPath;
        if (File.Exists(path))
            json = SimpleJSON.JSONClass.LoadFromFile(path) as SimpleJSON.JSONClass;

        if (json == null)
            json = new SimpleJSON.JSONClass();

        if (json[PathUtil.UpdateLuaSuffix] == null)
            json[PathUtil.UpdateLuaSuffix] = new SimpleJSON.JSONArray();

        if (json[PathUtil.UpdateBeanSuffix] == null)
            json[PathUtil.UpdateBeanSuffix] = new SimpleJSON.JSONArray();

        if (json[PathUtil.UpdateDepSuffix] == null)
            json[PathUtil.UpdateDepSuffix] = new SimpleJSON.JSONArray();

        if (json[PathUtil.UpdateLanguageSuffix] == null)
            json[PathUtil.UpdateLanguageSuffix] = new SimpleJSON.JSONArray();
    }

    /// <summary>
    /// 添加更新文件
    /// </summary>
    public void AddUpdateFile(string name)
    {
        string key = "";
        if (name.EndsWith(PathUtil.UpdateLuaSuffix))
            key = PathUtil.UpdateLuaSuffix;
        else if (name.EndsWith(PathUtil.UpdateBeanSuffix))
            key = PathUtil.UpdateBeanSuffix;
        else if (name.EndsWith(PathUtil.UpdateDepSuffix))
            key = PathUtil.UpdateDepSuffix;
        else if (name.EndsWith(PathUtil.UpdateLanguageSuffix))
            key = PathUtil.UpdateLanguageSuffix;
        else
            return;

        var arr = json[key];
        if (arr != null)
        {
            //名字去掉后缀名
            name = name.Replace(key, "");
            for (int i = 0; i < arr.Count; ++i)
            {
                //已经添加了
                if (arr[i].Value == name)
                    return;
            }
            json[key].Add(name);
        }
    }

    public void AddUpdateLanguage(string language)
    {
        var arr = json[PathUtil.LanguageConfName];
        arr.Add(language);
    }

    /// <summary>
    /// 更新结束
    /// </summary>
    public void UpdateEnd()
    {
        json.SaveToFile(PathUtil.UpdatedConfPath);
    }

    /// <summary>
    /// 清理更新文件
    /// </summary>
    public void Clear()
    {
        string path = "";
        var enu = json.Childs.GetEnumerator();
        while (enu.MoveNext())
        {
            var arr = enu.Current as SimpleJSON.JSONArray;
            if (arr == null)
                continue;
            for (int i = 0, len = arr.Count; i < len; ++i)
            {
                path = PathUtil.GetForceABPath(arr[i].Value);
                if (File.Exists(path))
                    File.Delete(path);
            }
        }
        enu.Dispose();

        json = new SimpleJSON.JSONClass();
        json.SaveToFile(PathUtil.UpdatedConfPath);
    }

    public void LoadLua(System.Action<string, byte[]> func, string name = null)
    {
        load(PathUtil.UpdateLuaSuffix, func, name);
    }

    public void LoadBean(System.Action<string, byte[]> func, string name = null)
    {
        load(PathUtil.UpdateBeanSuffix, func, name);
    }

    public void LoadDep(System.Action<string, byte[]> func, string name = null)
    {
        load(PathUtil.UpdateDepSuffix, func, name);
    }

    public void LoadLanguage(System.Action<string, byte[]> func, string name = null)
    {
        load(PathUtil.UpdateLanguageSuffix, func, name);
    }

    public List<string> LoadLanguageList()
    {
        var arr = json[PathUtil.LanguageConfName] as SimpleJSON.JSONArray;
        List<string> list = new List<string>();
        if (arr != null)
        {
            for (int i = 0; i < arr.Count; ++i)
                list.Add(arr[i].Value);
        }
        return list;
    }

    private void load(string suffix, System.Action<string, byte[]> func, string rName = null)
    {
        if (func == null)
            return;

        var arr = json[suffix] as SimpleJSON.JSONArray;
        if (arr == null)
            return;

        string name = "";
        for (int i = 0, len = arr.Count; i < len; ++i)
        {
            name = arr[i].Value;
            if (!string.IsNullOrEmpty(rName) && rName != name)
                continue;

            string path = PathUtil.GetForceABPath(name + suffix);
            if (!File.Exists(path))
                continue;
            UnityEngine.Debug.Log("load res path: " + path);

            //var data = File.ReadAllBytes(path);
            ////PathUtil.Decode(data);
            ////data = PathUtil.Unzip(data);
            //func(name, data);
            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            if (assetBundle == null)
            {
                continue;
            }
            TextAsset[] array = assetBundle.LoadAllAssets<TextAsset>();
            if (array != null)
            {
                int j = 0;
                for (int num = array.Length; j < num; j++)
                {
                    func(array[j].name, array[j].bytes);
                }
            }
            assetBundle.Unload(unloadAllLoadedObjects: true);
        }
    }
}