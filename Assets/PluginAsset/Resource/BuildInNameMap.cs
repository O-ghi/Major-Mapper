/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2019.01.10
*/
using UnityEngine;
using System.Collections.Generic;

public class BuildInNameMap
{
    //不含后缀名
    private static Dictionary<string, string> nameMap;
    private static void loadNameMap()
    {
        if (nameMap == null)
            nameMap = new Dictionary<string, string>();
        nameMap.Clear();
        var ta = Resources.Load<TextAsset>(System.IO.Path.GetFileNameWithoutExtension(PathUtil.NameRefName));
        if (ta != null)
        {
            int offset = 0;
            var bytes = ta.bytes;
            int count = XBuffer.ReadInt(bytes, ref offset);
            for (int i = 0; i < count; ++i)
            {
                var key = XBuffer.ReadString(bytes, ref offset);
                var val = XBuffer.ReadString(bytes, ref offset);
                nameMap.Add(key, val);
            }
            Resources.UnloadAsset(ta);
        }
        else
        {
            Debuger.Err("读取资源名对应关系失败");
        }
    }

    public static string GetOrgName(string realName)
    {
        if (nameMap == null)
            loadNameMap();
        if (nameMap.ContainsKey(realName))
            return nameMap[realName];
        return realName;
    }
    
    public static string GetRealName(string fakeName)
    {
        if (nameMap == null)
            loadNameMap();
        if (nameMap.ContainsValue(fakeName))
        {
            foreach(var kv in nameMap)
            {
                if (kv.Value == fakeName)
                    return kv.Key;
            }
        }
        return fakeName;
    }
}