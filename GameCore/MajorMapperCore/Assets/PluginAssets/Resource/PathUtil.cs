/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2017.09.22
*/

///恶心的是Android下用File去判断是否存在，用WWW去加载和AssetBundle.LoadFromFile的路径不一样
using System.IO;
using UnityEngine;
using System.Collections;
using Unity.SharpZipLib.Zip;

public class PathUtil
{
    //当前资源语言,编辑器打包ab情况下(未运行时)保持为空
    //public static string resLanguage;
    public static string resLanguage = "chinese";
    ///加载路径
    public static string GetABPath(string resName, bool wwwPath = false)
    {
        //编辑器下
#if UNITY_EDITOR
        //优先使用过审资源
        string path = EditorPath.GetGuoShenPath(resName);
        if (File.Exists(path))
            return path;

        //force
        path = GetForceABPath(resName);
        if (File.Exists(path))
            return path;

        //ui ab目录
        path = EditorUIABPath + resName + abSuffix;
        if (ResDepManager.Singleton.IsDefLanguage(resName, EditorUIABPath))
            path = EditorUIABPath + resLanguage + "/" + resName + abSuffix;
        if (File.Exists(path))
            return path;

        return GetBackABPath(resName);
#else
        //手机上
        string path = GetForceABPath(resName);
        if(File.Exists(path))   //有更新/强更
            return path;

        path = GetBackABPath(resName);
        if(File.Exists(path))   //有更新/偷跑
            return path;

        if (ResDepManager.Singleton.IsDefLanguage(resName))
            resName = BuildInNameMap.GetOrgName(resLanguage + "/" + resName);
        else
            resName = BuildInNameMap.GetOrgName(resName);

        //无更新
        if (wwwPath)
            return GetWWWABBuildInPath(resName);
        else
            return GetABBuildinPath(resName);
#endif
    }

#if UNITY_EDITOR
    public static string GetGuoShenPath(string resName = null)
    {
#if UNITY_ANDROID
        string dic = Application.dataPath + "/../../../GameAB/android/guoshen/";
#elif UNITY_IOS
        string dic = Application.dataPath + "/../../../GameAB/ios/guoshen/";
#else
        string dic = Application.dataPath + "/../../../GameAB/other/guoshen/";
#endif
        if (string.IsNullOrEmpty(resName))
            return dic;

        if (!resName.EndsWith(abSuffix))
            resName += abSuffix;

        var files = Directory.GetFiles(dic, "*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (Path.GetFileName(file).ToLower() == resName)
                return file;
        }
        return dic + resName;
    }
#endif

    public static bool IsInStreamFolder(string resName)
    {
#if UNITY_EDITOR
        if (File.Exists(GetForceABPath(resName)))
            return true;
        if (File.Exists(GetBackABPath(resName)))
            return true;
        return false;
#else
        //手机上
        string path = GetForceABPath(resName);
        if (File.Exists(path))   //有更新/强更
            return true;

        path = GetBackABPath(resName);
        if (File.Exists(path))   //有更新/偷跑
            return true;
        path = GetABBuildinPath(resName);
        if (File.Exists(path))
            return true;
        return false;
#endif
    }

    ///内置版本号
    public const string BuildInVersionConfigName = "LocalConfig.json";
    ///dll bundle名字
    public const string DllScriptBundleName = "gamelogic.bytes";
    public const string DllScriptAssignBundleName = "gamelogic";

    ///lua bundle名字
    public const string LuaScriptsBundleName = "ls_pkg_711_.bytes";
    ///config bundle名字
    public const string ConfigBundleName = "cb_pkg_711_.bytes";
    ///dep ab依赖bundle名字
    public const string BuildinDepConfName = "dc_pkg_711_.bytes";
    //多语言资源资源
    public const string LanguageConfName = "lg_pkg_711_.bytes";

    ///内置资源的清单
    public const string BuildInFileListName = "_buildin_res_list.bytes";
    //名字对应关系
    public const string NameRefName = "_res_name_ref_map.bytes";

    //强更lua后缀
    public const string UpdateLuaSuffix = ".lbytes";
    //强更bean后缀
    public const string UpdateBeanSuffix = ".bbytes";
    //依赖配置文件后缀
    public const string UpdateDepSuffix = ".dbytes";
    //多语言配置文件后缀
    public const string UpdateLanguageSuffix = ".lgbytes";

    /// <summary>
    /// ab文件后缀名
    /// </summary>
    public static string abSuffix
    {
        get
        {
            //duong: ko can su dung extension cho bundle
            return "";
//#if UNITY_IOS
//            return ".8";
//#elif UNITY_ANDROID
//            return ".9";
//#else
//            Debug.LogError( "当前平台暂时不支持" );
//            return ".0";
//#endif
        }
    }

    public static string bytesSuffix
    {
        get
        {
            return ".bytes";
        }
    }

#if UNITY_EDITOR
    private static string abBuildinPath = Application.dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
    private static string abBuildinPath = Application.dataPath + "!assets/";
#elif UNITY_IOS
    private static string abBuildinPath = Application.dataPath + "/Raw/";
#endif

    ///ab包内路径, AssetBundle api读取
    public static string GetABBuildinPath(string resName = null)
    {
        if (string.IsNullOrEmpty(resName))
            return abBuildinPath;

        var dic = abBuildinPath;
        if (ResDepManager.Singleton.IsDefLanguage(resName, dic))
        {
            //判断是否需要区分语言
            if (!string.IsNullOrEmpty(resLanguage))
                dic = dic + resLanguage + "/";
        }

        if (resName.Contains("."))
            return dic + resName;
        else
            return dic + resName + abSuffix;
    }

    ///ab包内路径 WWW读取
    public static string GetWWWABBuildInPath(string resName = null)
    {
        string dic = "";
#if UNITY_EDITOR
#if UNITY_EDITOR_WIN
        dic = "file://" + Application.dataPath + "/StreamingAssets/ab/";
#else
        dic = Application.dataPath + "/StreamingAssets/ab/";
#endif
#elif UNITY_ANDROID
        dic = "jar:file://" + Application.dataPath + "!/assets/ab/";
#elif UNITY_IOS
        dic = Application.dataPath + "/Raw/ab/";
#endif

        if (string.IsNullOrEmpty(resName))
            return dic;

        if (ResDepManager.Singleton.IsDefLanguage(resName, dic))
        {
            //判断是否需要区分语言
            if (!string.IsNullOrEmpty(resLanguage))
                dic = dic + resLanguage + "/";
        }

        if (resName.Contains("."))
            return dic + resName;
        else
            return dic + resName + abSuffix;
    }

    /// <summary>
    /// 初始化目录
    /// </summary>
    public static void InitPath()
    {
        if (!Directory.Exists(GetConfigPath()))
            Directory.CreateDirectory(GetConfigPath());
        if (!Directory.Exists(GetBackABPath()))
            Directory.CreateDirectory(GetBackABPath());
        if (!Directory.Exists(GetForceABPath()))
            Directory.CreateDirectory(GetForceABPath());
    }

    //清除force资源及其信息
    public static void ClearForce()
    {
        if (File.Exists(UpdatedConfPath))
            File.Delete(UpdatedConfPath);
        if (File.Exists(ForceFileListOutPath))
            File.Delete(ForceFileListOutPath);
        if (File.Exists(ForceFileTmpLoadedPath))
            File.Delete(ForceFileTmpLoadedPath);
#if !UNITY_EDITOR
        //删除文件夹又立马创建可能触发异常
        //IOException lock violation on path
        if (Directory.Exists(GetForceABPath()))
        {
            //Directory.Delete(GetForceABPath(), true);
            var arr = Directory.GetFiles(GetForceABPath(), "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < arr.Length; ++i)
                File.Delete(arr[i]);
            arr = Directory.GetDirectories(GetForceABPath());
            for (int i = 0; i < arr.Length; ++i)
                Directory.Delete(arr[i], true);
        }
#endif
    }

    //清除back资源及其信息
    public static void ClearBack()
    {
        if (File.Exists(BackLocalConfigPath))
            File.Delete(BackLocalConfigPath);
        if (File.Exists(BackServerConfigPath))
            File.Delete(BackServerConfigPath);

#if !UNITY_EDITOR
        //删除文件夹又立马创建可能触发异常
        //IOException lock violation on path
        if (Directory.Exists(GetBackABPath()))
        {
            //Directory.Delete(GetBackABPath(), true);
            var arr = Directory.GetFiles(GetBackABPath(), "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < arr.Length; ++i)
                File.Delete(arr[i]);
            arr = Directory.GetDirectories(GetBackABPath());
            for (int i = 0; i < arr.Length; ++i)
                Directory.Delete(arr[i], true);
        }
#endif
    }

    /// <summary>
    /// 删除config，force，back目录
    /// </summary>
    public static void ClearConfigAndForceAndBack()
    {
        //删除文件夹又立马创建可能触发异常
        //IOException lock violation on path
        if (Directory.Exists(GetConfigPath()))
        {
            //Directory.Delete(GetConfigPath(), true);
            var arr = Directory.GetFiles(GetConfigPath(), "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < arr.Length; ++i)
                File.Delete(arr[i]);
            arr = Directory.GetDirectories(GetConfigPath());
            for (int i = 0; i < arr.Length; ++i)
                Directory.Delete(arr[i], true);
        }

#if !UNITY_EDITOR
        if (Directory.Exists(GetForceABPath()))
        {
            //Directory.Delete(GetForceABPath(), true);
            var arr = Directory.GetFiles(GetForceABPath(), "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < arr.Length; ++i)
                File.Delete(arr[i]);
            arr = Directory.GetDirectories(GetForceABPath());
            for (int i = 0; i < arr.Length; ++i)
                Directory.Delete(arr[i], true);
        }
        if (Directory.Exists(GetBackABPath()))
        {
            //Directory.Delete(GetBackABPath(), true);
            var arr = Directory.GetFiles(GetBackABPath(), "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < arr.Length; ++i)
                File.Delete(arr[i]);
            arr = Directory.GetDirectories(GetBackABPath());
            for (int i = 0; i < arr.Length; ++i)
                Directory.Delete(arr[i], true);
        }
#endif
        //InitPath();
    }

    /// <summary>
    /// 获取网络路径
    /// </summary>
    public static string GetServerPath(string url, string resName)
    {
        if (resName.Contains("."))
            return url + resName;
        else
            return url + resName + abSuffix;
    }

    private static string abBackPath = Application.persistentDataPath + "/ab/";
    public static string GetBackABPath(string resName = null)
    {
#if UNITY_EDITOR
#if UNITY_ANDROID
        string dic = Application.dataPath + "/../../code/{0}/GameAB/android/back/";
#elif UNITY_IOS
        string dic = Application.dataPath + "/../../code/{0}/GameAB/ios/back/";
#else
        string dic = Application.dataPath + "/../../code/{0}/GameAB/other/back/";
#endif
        dic = string.Format(dic, EditorPath.CurrentBranch);
#else
        string dic = abBackPath;
#endif
        if (string.IsNullOrEmpty(resName))
            return dic;

        if (ResDepManager.Singleton.IsDefLanguage(resName, dic))
        {
            //判断是否需要区分语言
            if (!string.IsNullOrEmpty(resLanguage))
                dic = dic + resLanguage + "/";
        }

        if (resName.Contains("."))
            return dic + resName;
        else
            return dic + resName + abSuffix;
    }

    private static string abforcepath = Application.persistentDataPath + "/abForce/";
    public static string GetForceABPath(string resName = null)
    {
//#if UNITY_EDITOR
//#if UNITY_ANDROID
//        string dic = Application.dataPath + "/../../code/{0}/GameAB/android/force/";
//#elif UNITY_IOS
//        string dic = Application.dataPath + "/../../code/{0}/GameAB/ios/force/";
//#else
//        string dic = Application.dataPath + "/../../code/{0}/GameAB/other/force/";
//#endif
//        dic = string.Format(dic, EditorPath.CurrentBranch);
//#else
//        string dic = abforcepath;
//#endif
#if UNITY_EDITOR
        string dic = AssetsUtility.GetCachePlatformPath();
#else
        //string dic = AssetsUtility.GetCachePath();
        string dic = abforcepath;
#endif

        if (string.IsNullOrEmpty(resName))
            return dic;

        if (ResDepManager.Singleton.IsDefLanguage(resName, dic))
        {
            //判断是否需要区分语言
            if (!string.IsNullOrEmpty(resLanguage))
                dic = dic + resLanguage + "/";
        }
        //duong: ko su dun suffix
        return dic + resName;

        //if (resName.Contains("."))
        //    return dic + resName;
        //else
        //    return dic + resName + abSuffix;
    }

    /// <summary>
    /// 配置目录
    /// </summary>
    public static string GetConfigPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/../98/config/";
#else
        return Application.persistentDataPath + "/config/";
#endif
    }

    /// 强更资源清单
    public static string ForceFileListOutPath = GetConfigPath() + "force.conf";
    /// 强更资源已下载清单
    public static string ForceFileTmpLoadedPath = GetConfigPath() + "tmpLoaded.conf";
    //强更lua,bean信息
    public static string UpdatedConfPath = GetConfigPath() + "_important.json";
    //已偷跑文件列表
    public static string BackLocalConfigPath = GetConfigPath() + "local.conf";
    //待偷跑文件列表
    public static string BackServerConfigPath = GetConfigPath() + "server.conf";

#if UNITY_EDITOR
    public static int scriptOffset = 123;
    //dll的key必须足够长，不然还是能反编译出来，要修改适当修改后面的数字即可
    public static string scriptKey = "GameDataManager.Bean";
    public static int binOffset = 123;
    public static string binKey = "GameDataManager.Bean";
#endif
    public static int codeOffset = 123;
    public static string codeKey = "GameDataManager.Bean";

    public static void Decode(byte[] data)
    {
        if (data == null)
            return;
        var enu = realDecode(data);
        while (enu.MoveNext())
        {
        }
    }

    private static IEnumerator realDecode(byte[] data)
    {
        //Debug.Log("realDecode codeKey " + codeKey);
        var keys = System.Text.Encoding.Default.GetBytes(codeKey);
        for (int i = keys.Length - 1; i >= 0; --i)
        {
            if (data.Length > codeOffset + i)
                data[i + codeOffset] -= keys[i];
        }
        yield return null;
    }

    public static byte[] LoadBytes(string name)
    {
        //Debug.Log("Loadbytes current: " + name);
        byte[] retData = null;
        string forcePath = GetForceABPath(name);
        if (File.Exists(forcePath))
        {
            //Debug.Log("forcePath exist: " + forcePath);
            //热更文件不打ab
            var orgData = File.ReadAllBytes(forcePath);
            Decode(orgData);
            retData = orgData;
            //retData = Unzip(orgData);
        }
        else
        {
            //Debug.Log("forcePath not exist: " + forcePath);
            var ab = BibManager.Singleton.LoadAssetBundle(name);
            if (ab != null)
            {
                var ta = ab.LoadAsset<TextAsset>(name);
                if (ta != null)
                    retData = ta.bytes;
            }
        }
        return retData;
    }
    public static byte[] LoadBytesUnZip(string name)
    {
        byte[] retData = null;
        string forcePath = GetForceABPath(name);
        if (File.Exists(forcePath))
        {
            //热更文件不打ab
            var orgData = File.ReadAllBytes(forcePath);
            retData = Unzip(orgData);
        }
        else
        {
            var ab = BibManager.Singleton.LoadAssetBundle(name);
            if (ab != null)
            {
                var ta = ab.LoadAsset<TextAsset>(name);
                if (ta != null)
                    retData = ta.bytes;
            }
        }
        return retData;
    }

    public static string ByteCode = "0908";
    public static byte[] Unzip(byte[] orgData)
    {
        if (orgData == null)
            return null;
        MemoryStream memStream = new MemoryStream(orgData);
        var zipStream = new ZipInputStream(memStream);
        zipStream.IsStreamOwner = true;
        zipStream.Password = ByteCode;
        var file = zipStream.GetNextEntry();
        var unzipData = new byte[file.Size];
        zipStream.Read(unzipData, 0, unzipData.Length);
        zipStream.Close();
        zipStream.Dispose();
        return unzipData;
    }

#if UNITY_EDITOR
    public static void Encode(byte[] data)
    {
        if (data == null)
            return;
        //Debug.Log("Encode codeKey " + codeKey);
        var keys = System.Text.Encoding.Default.GetBytes(codeKey);
        for (int i = keys.Length - 1; i >= 0; --i)
        {
            if (data.Length > codeOffset + i)
                data[i + codeOffset] += keys[i];
        }
    }
#if UNITY_ANDROID
    public static string EditorDllPgamemain = Application.dataPath + "/../hotupdate/PGameMain/GameDll/GameLogic.dll";
    //
    public static string EditorFixDllPath = Application.dataPath + "/../GameDll/GameFix.dll";
    public static string EditorFixPdbPath = Application.dataPath + "/../GameDll/GameFix.pdb";
    public static string EditorDllPath = Application.dataPath + "/../GameDll/GameLogic.dll";
    public static string EditorPdbPath = Application.dataPath + "/../GameDll/GameLogic.pdb";
    public static string EditorUIABPath
    {
        get
        {
            var path = Application.dataPath + "/../../code/{0}/GameAB/android/ui/";
            path = string.Format(path, EditorPath.CurrentBranch);
            return path;
        }
    }
#elif UNITY_IOS
    public static string EditorDllPgamemain = Application.dataPath + "/../../PGameMain/GameDll_IOS/GameLogic.dll";

    public static string EditorFixDllPath = Application.dataPath + "/../GameDll_IOS/PGameFix.dll";
    public static string EditorFixPdbPath = Application.dataPath + "/../GameDll_IOS/PGameFix.pdb";
    public static string EditorDllPath = Application.dataPath + "/../GameDll_IOS/PGameLogic.dll";
    public static string EditorPdbPath = Application.dataPath + "/../GameDll_IOS/PGameLogic.pdb";
    public static string EditorUIABPath {
        get
        {
            var path = Application.dataPath + "/../../code/{0}/GameAB/ios/ui/";
            path = string.Format(path, EditorPath.CurrentBranch);
            return path;
        }
    }
#endif
#endif


    public static string DestDirAndroid = Application.dataPath + "/AssetDll/Android/";
    public static string DestDirIos = Application.dataPath + "/AssetDll/Ios/";
}