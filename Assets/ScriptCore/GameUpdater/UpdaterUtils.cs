using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public class UpdaterUtils
{
    /// <summary>
    /// StreamingAsset 文件夹资源根目录 在各平台的路径
    /// </summary>
    /// <returns></returns>
	private static string m_streamingAsset = "";
    public static string streamingAsset
	{
        get
        {
			if (string.IsNullOrEmpty(m_streamingAsset))
			{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
				m_streamingAsset = "file://" + Application.dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
				m_streamingAsset = "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
				m_streamingAsset = "file://" + Application.dataPath + "/Raw/";
#endif
			}
			return m_streamingAsset;
        }
    }

	/// <summary>
	/// persistentDataPath 持久文件在各平台的路径 没有“File://”
	/// </summary>
	private static string m_persistentDataPath = "";
	public static string persistentDataPath
	{
        get
		{
			if (string.IsNullOrEmpty(m_persistentDataPath))
			{
#if UNITY_EDITOR
#if Main
                m_persistentDataPath = Application.dataPath + "/../persistentDataPath";
                if (!Directory.Exists(m_persistentDataPath))
                    Directory.CreateDirectory(m_persistentDataPath);
#else
                m_persistentDataPath = Application.dataPath + "/../../../persistentDataPath";
                if (!Directory.Exists(m_persistentDataPath))
                    Directory.CreateDirectory(m_persistentDataPath);
#endif

#elif UNITY_STANDALONE_WIN
                m_persistentDataPath = Application.dataPath + "/../persistentDataPath";
				if (!Directory.Exists(m_persistentDataPath))
					Directory.CreateDirectory(m_persistentDataPath);
#else
                m_persistentDataPath = Application.persistentDataPath;
#endif
                m_persistentDataPath = m_persistentDataPath + "/";
			}
			return m_persistentDataPath;
        }
    }

	public static string OS
	{
		get
		{
			switch (Application.platform)
			{
				case RuntimePlatform.Android:
					return "android";
				case RuntimePlatform.IPhonePlayer:
					return "ios";
				default:
					return "windows";
			}
		}
	}

	public static string persistentDataPathAndPlatform
	{
		get
		{
			return persistentDataPath + OS;
		}
	}

	//删除这个目录下的所有资源
	public static void DeleteDir(string dir)
    {
        foreach (string var in Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly))
        {
            Directory.Delete(var, true);
        }

        foreach (string var in Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly))
        {
            File.Delete(var);
        }
    }

    /// <summary>
    /// 比较版本号如果arg2 >= arg1 true, else false
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    public static bool CompAppVersion(string arg1, string arg2)
    {
        if (arg1 == arg2)
            return true;
        string[] l_v = arg1.Split('.');
        string[] r_v = arg2.Split('.');
        int count = l_v.Length > r_v.Length ? l_v.Length : r_v.Length;
        //默认远程的游戏版本号大于本地版本号成立
        for (int i = 0; i < count; i++)
        {
            int l = 0; int r = 0;
            if (i < l_v.Length)
                l = int.Parse(l_v[i]);
            if (i < r_v.Length)
                r = int.Parse(r_v[i]);
            if (r > l)
                return true;
            else if (r < l)
                return false;
        }
        return true;
    }

    public static int IntConvert(string value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;
        int relust = 0;
        int.TryParse(value, out relust);
        return relust;
    }

    public static long LongConvert(string value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;
        long relust = 0;
        long.TryParse(value, out relust);
        return relust;
    }

    public enum ValidBuildTarget
    {
        StandaloneOSXUniversal = 2,
        StandaloneOSXIntel = 4,
        Windows = 5,
        WebPlayer = 6,
        WebPlayerStreamed = 7,
        IOS = 9,
        PS3 = 10,
        XBOX360 = 11,
        Android = 13,
        StandaloneLinux = 17,
        StandaloneWindows64 = 19,
        WebGL = 20,
        WSAPlayer = 21,
        StandaloneLinux64 = 24,
        StandaloneLinuxUniversal = 25,
        WP8Player = 26,
        StandaloneOSXIntel64 = 27,
        BlackBerry = 28,
        Tizen = 29,
        PSP2 = 30,
        PS4 = 31,
        PSM = 32,
        XboxOne = 33,
        SamsungTV = 34,
        N3DS = 35,
        WiiU = 36,
        tvOS = 37,
        Switch = 38
    }

    public static RequestInfo ParseRequestInfo(string xmlstr)
    {
        RequestInfo info = new RequestInfo();

        XmlDocument document = new XmlDocument();
        document.LoadXml(xmlstr);
        XmlNode root = document.SelectSingleNode("patches");

        XmlElement rootNode = root as XmlElement;
        info.appversion = rootNode.GetAttribute("appVersion");
        info.packversion = GameConvert.IntConvert(rootNode.GetAttribute("maxVersion"));

        info.filelist = new List<RequestFileInfo>();
        
        foreach (XmlNode _node in root.ChildNodes)
        {
            XmlElement node = _node as XmlElement;
            RequestFileInfo fileinfo = new RequestFileInfo();
            fileinfo.name = node.GetAttribute("name");
            long.TryParse(node.GetAttribute("crc"), out fileinfo.crc);
            fileinfo.size = GameConvert.LongConvert(node.GetAttribute("size"));
            info.filelist.Add(fileinfo);
        }

        return info;
    }
}
