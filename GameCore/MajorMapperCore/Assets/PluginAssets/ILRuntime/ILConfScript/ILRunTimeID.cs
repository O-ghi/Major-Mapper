using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ILRunTimeID
{
    private static Dictionary<string, string> nameMap = new Dictionary<string, string>();
    private static int nameIdx = -1;

    public static string Rename(string name, int idx)
    {
#if UNITY_EDITOR
        if (nameIdx < 0)
            nameIdx = UnityEngine.Random.Range(0, 15);

        if (!nameMap.ContainsKey(name + idx))
        {
            int insertIdx = nameIdx;
            if (name.Length <= insertIdx)
                insertIdx = name.Length;
            var newName = name.Insert(insertIdx, (((DateTime.Now - new DateTime(2018, 7, 11)).Ticks) / 10000000).ToString("F0"));
            newName = getRandomName();
            nameMap[name + idx] = newName + idx;
        }
        return nameMap[name + idx];
#else
        return name + "_" + idx;
#endif
    }

#if UNITY_EDITOR
    private static SimpleJSON.JSONClass nameArrJson;
    public static string getRandomName()
    {
        if (nameArrJson == null)
        {
            var path = Application.dataPath + "/PluginAssets/Resource/Editor/mix_word_list.json";
            if (File.Exists(path))
                nameArrJson = SimpleJSON.JSONArray.Parse(File.ReadAllText(path)) as SimpleJSON.JSONClass;
        }

        var ret = "";
        int num = UnityEngine.Random.Range(2, 5);
        //º¯ÊýÃû
        for (int i = 0; i < num; ++i)
        {
            int len = nameArrJson["name"].Count;
            int idx = UnityEngine.Random.Range(0, len);

            var w = nameArrJson["name"][idx].Value;
            if (string.IsNullOrEmpty(ret))
                ret = w;
            else
                ret += w[0].ToString().ToUpper() + w.Substring(1).ToLower();//ÍÕ·å
        }
        return ret;
    }
#endif
}