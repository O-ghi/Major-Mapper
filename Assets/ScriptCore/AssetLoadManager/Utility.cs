using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


public class Utility
{
    public const string AssetBundlesOutputPath = "AssetBundles";

    /// <summary>
    /// 获取带本地传输协议的assetbundle文件路径
    /// </summary>
    /// <returns>The asset bundle LPD isk path.</returns>
    /// <param name="assetBundleName">Asset bundle name.</param>
    /// <param name="sync">If set to <c>true</c> sync.</param>
    public static string GetAssetBundleLPDiskPath(string assetBundleName, bool sync = false)
    {
        //Duong thay doi path persistentDataPath sang cho b92
        string url = PathUtil.GetForceABPath(assetBundleName);
        //string url = string.Format("{0}/{1}/{2}", persistentDataPath, PlatformName, assetBundleName);

#if UNITY_ANDROID
        bool inpersistent = false;
#endif

        if (!File.Exists(url))
        {
            url = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, PlatformName, assetBundleName);
        }
#if UNITY_ANDROID
        else
        {
            inpersistent = true;
        }
#endif

#if UNITY_ANDROID
        if (!sync && inpersistent)
            return "file://" + url;
        return url;
#else
        if (sync)
            return url;
        return "file://" + url;
#endif

    }

    /// <summary>
    /// 获取本地磁盘文件路径
    /// </summary>
    /// <returns>The asset bundle disk path.</returns>
    /// <param name="assetBundleName">Asset bundle name.</param>
    public static string GetAssetBundleDiskPath(string assetBundleName)
    {
        //Duong thay doi path persistentDataPath sang cho b92
        //string path = string.Format("{0}/{1}/{2}", persistentDataPath, PlatformName, assetBundleName);
        string path = PathUtil.GetForceABPath(assetBundleName);

        if (!File.Exists(path))
        {
            path = string.Format("{0}/{1}", Application.streamingAssetsPath, assetBundleName);
		}

		return path;
    }


	public static string PlatformName
	{
        get
        {
#if UNITY_EDITOR
			return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
            return GetPlatformForAssetBundles(Application.platform);
#endif
        }

    }

#if UNITY_EDITOR
	private static string GetPlatformForAssetBundles(BuildTarget target)
	{
		switch (target)
		{
			case BuildTarget.Android:
				return "android";
			case BuildTarget.iOS:
				return "ios";
			case BuildTarget.WebGL:
				return "webgl";
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "windows";
			//case BuildTarget.StandaloneOSX:
			//	return "OSX";
			// Add more build targets for your own.
			// If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
			default:
				return null;
		}
	}
#endif

	private static string GetPlatformForAssetBundles(RuntimePlatform platform)
	{
#if UNITY_ANDROID && !Main
        return "android";
#endif

        switch (platform)
		{
			case RuntimePlatform.Android:
				return "android";
			case RuntimePlatform.IPhonePlayer:
				return "ios";
			case RuntimePlatform.WebGLPlayer:
				return "webgl";
			case RuntimePlatform.WindowsPlayer:
				return "windows";
			case RuntimePlatform.OSXPlayer:
				return "osx";
			// Add more build targets for your own.
			// If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
			default:
				return null;
		}
	}

    /// <summary>
    /// persistentDataPath 持久文件在各平台的路径 没有“File://”
    /// </summary>
    public static string persistentDataPath
    {
        get
        {
            string url = "";
#if UNITY_EDITOR
#if Main
            url = Application.dataPath + "/../persistentDataPath";
            if (!Directory.Exists(url))
                Directory.CreateDirectory(url);
#else
            url = Application.dataPath + "/../../../persistentDataPath";
            if (!Directory.Exists(url))
                Directory.CreateDirectory(url);
#endif
#elif UNITY_STANDALONE_WIN
            url = Application.dataPath + "/../persistentDataPath";
            if (!Directory.Exists(url))
                Directory.CreateDirectory(url);
#else
            url = Application.persistentDataPath;
#endif
            return url;
        }
    }

    //删除这个目录下的所有资源
    public static void DeleteDir(string dir)
    {
        foreach (string var in Directory.GetDirectories(dir))
        {
            Directory.Delete(var, true);
        }
        foreach (string var in Directory.GetFiles(dir))
        {
            File.Delete(var);
        }
    }
}

