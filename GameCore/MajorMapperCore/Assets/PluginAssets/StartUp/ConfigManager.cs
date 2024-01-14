/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2017.10.20
*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public class ConfigManager
{
    private static ConfigManager m_instance;
    public static ConfigManager Singleton
    {
        get
        {
            if (m_instance == null)
                m_instance = new ConfigManager();
            return m_instance;
        }
    }

    private Dictionary<string, byte[]> byteMap = new Dictionary<string, byte[]>();

    public void LoadBeans(bool forceReload = false)
    {
        initbean();
        /*
#if UNITY_EDITOR
        //DUong hien tai ko xu dung bean, se loa bang bidManager
#else
        if (forceReload)
            byteMap.Clear();
        if (byteMap.Count <= 0)
        {
            AssetBundle ab = BibManager.Singleton.LoadAssetBundle(PathUtil.ConfigBundleName);
            if (ab == null)
            {
                Debuger.Err("没有配置表");
                ab.Unload(true);
                return;
            }

            TextAsset[] beans = ab.LoadAllAssets<TextAsset>();
            if (beans != null)
            {
                for (int i = 0, len = beans.Length; i < len; ++i)
                    byteMap[beans[i].name] = beans[i].bytes;
            }
            ab.Unload(false);
        }

        ForceManager.Singleton.LoadBean((name, bytes) =>{
            byteMap[name] = bytes;
        });
#endif
        */
    }

    public byte[] GetData(string name)
    {
#if UNITY_EDITOR && ONLY_PGAME
        if (UnityEditor.EditorPrefs.GetBool("useAssetDatabase_Enable", true) == true)
        {
            byte[] fData = null;
            //if (EditorPath.HasGuoShen)
            //    fData = System.IO.File.ReadAllBytes(EditorPath.GetGuoShenPath() + "/bean/" + name + ".bytes");
            //else
            fData = System.IO.File.ReadAllBytes(Application.dataPath + "/ConfigGame/" + name + ".bytes");

            if (fData != null)
                return fData;
        }

#endif
        var data = loadbean(name);
        //Debuger.Err("Kwang get bean is success name: "+ name +" is " + (data != null));

        if (data != null)
            return data;

        //Debug.LogError("Kwang toi day thi chiu");
        if (byteMap.ContainsKey(name))
            return byteMap[name];

        return null;
    }

    public void Clear()
    {
        byteMap.Clear();
    }
    
    public byte[] LoadBean(string name)
    {
        var data = loadbean(name);
        if (data != null)
            return data;

        if (byteMap.ContainsKey(name))
            return byteMap[name];

        ForceManager.Singleton.LoadBean((n, bytes) => {
            byteMap[n] = bytes;
        });

        if (byteMap.ContainsKey(name))
            return byteMap[name];

        AssetBundle ab = BibManager.Singleton.LoadAssetBundle(PathUtil.ConfigBundleName);
        if (ab == null)
        {
            ab.Unload(true);
            return null;
        }

        TextAsset bean = ab.LoadAsset<TextAsset>(name);
        if (bean != null)
            byteMap[bean.name] = bean.bytes;
        ab.Unload(false);

        if (byteMap.ContainsKey(name))
            return byteMap[name];
        return null;
    }
 
    private bool isInit = false;
    private Dictionary<string, int> buildInLength;
    protected Dictionary<string, int> offsetMap;
    protected Dictionary<string, byte[]> configMap;

    private void initbean()
    {
#if UNITY_EDITOR && ONLY_PGAME
        if (UnityEditor.EditorPrefs.GetBool("useAssetDatabase_Enable", true) == true)
        {
            return;
        }

#endif
        if (isInit)
            return;
        isInit = true;

        AssetBundle configgameab = null;
        if (configgameab == null)
        {
            configgameab = BibManager.Singleton.TryLoadAssetBundleSingle("configgame");
        }

        var listbytes = configgameab.LoadAsset<TextAsset>("buildin_config_list").bytes;
        try
        {
            int offset = 0;
            buildInLength = new Dictionary<string, int>();
            offsetMap = new Dictionary<string, int>();

      

            //Thông tin tài nguyên trong gói
            int count = XBuffer.ReadInt(listbytes, ref offset);
            //Debuger.Log("Kwang count size:" + count);

            for (int i = 0; i < count; ++i)
            {
                var name = XBuffer.ReadString(listbytes, ref offset);
                var length = XBuffer.ReadInt(listbytes, ref offset);
                var bOffset = XBuffer.ReadInt(listbytes, ref offset);
                buildInLength[name] = length;
                offsetMap[name] = bOffset;
            }
            //bib名字
            var bibName = XBuffer.ReadString(listbytes, ref offset);
            //load config
            var datagame = configgameab.LoadAsset<TextAsset>(bibName).bytes;
            //check md5
            configMap = new Dictionary<string, byte[]>();
            //Debuger.Log("bibName.Split('.')[0] " + bibName);

            //if (AssetsUtility.GetMd5Hash(datagame) == bibName.Split('.')[0])
            {
                //int length = 0;

                foreach(var key in offsetMap.Keys)
                {
                    var offsetByte = offsetMap[key];
                    var offsetLength = buildInLength[key];

                    byte[] outConfig = new byte[offsetLength];
                    Array.Copy(datagame, offsetByte, outConfig, 0, offsetLength);
                    configMap[key] = outConfig;
                    //Debuger.Log("Add " + key + " ||offsetByte :"+ offsetByte +  "  || datasize" + outConfig.Length);
                }
            }

            //
            //Debuger.Log("Kwang bibOffsetMap size:" + offsetMap.Count);
        }
        catch (Exception e)
        {
            Debuger.Err(e.Message, e.StackTrace);
        }
    }

    private byte[] loadbean(string name)
    {
        name += ".bytes";
        if (configMap.ContainsKey(name))
        {
            return configMap[name];
        }
        else
        {
            Debuger.Err("offsetMap don't contain key: " + name);
        }

        return null;
      
    }

}