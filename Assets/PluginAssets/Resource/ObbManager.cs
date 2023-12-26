/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2018.11.26
*/
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ObbManager
{
    private static ObbManager instance;
    public static ObbManager Singleton
    {
        get
        {
            if (instance == null)
                instance = new ObbManager();
            return instance;
        }
    }
    
    private int nowCoroutineNum = 0;
    private const int maxCoroutineNum = 13;
    private Dictionary<string, System.Action<string, AssetBundle>> loadingMap = new Dictionary<string, System.Action<string, AssetBundle>>();
    private Dictionary<string, ResPriority> loadingPriMap = new Dictionary<string, ResPriority>();
    private Queue<string> toLoadList = new Queue<string>();
    public AssetBundle LoadAssetBundle(string resName, int version)
    {
        initObb();
        var withFolderResName = resName;
        if (ResDepManager.Singleton.IsDefLanguage(resName))
            withFolderResName = PathUtil.resLanguage + "/" + resName;
        //obb只支持ab资源
        if (!withFolderResName.EndsWith(PathUtil.abSuffix))
            withFolderResName += PathUtil.abSuffix;

        if (!obbMap.ContainsKey(withFolderResName))
            return null;

        var obb = obbMap[withFolderResName];
        if (obb.version < version)
            return null;
        var ab = AssetBundle.LoadFromFile(obbFilePath, 0, (ulong)obb.offset);
        return ab;
    }
    
    /// <summary>
    /// 同步加载ab文件
    /// </summary>
    public void LoadAssetBundle(string resName, int version, System.Action<string, AssetBundle> onCmp)
    {
        initObb();
        var withFolderResName = resName;
        if (ResDepManager.Singleton.IsDefLanguage(resName))
            withFolderResName = PathUtil.resLanguage + "/" + resName;
        //obb只支持ab资源
        if (!withFolderResName.EndsWith(PathUtil.abSuffix))
            withFolderResName += PathUtil.abSuffix;

        AssetBundle ab = null;
        if (obbMap.ContainsKey(withFolderResName))
        {
            var obb = obbMap[withFolderResName];
            if (obb.version >= version)
                ab = AssetBundle.LoadFromFile(obbFilePath, 0, (ulong)obb.offset);
        }

        if (onCmp != null)
        {
            try
            {
                onCmp(resName, ab);
            }
            catch (System.Exception e)
            {
                Debuger.Err(e.Message, e.StackTrace);
            }
        }
    }

    /// <summary>
    /// 异步加载ab文件
    /// </summary>
    public void LoadAssetBundleAsync(string resName, int version, System.Action<string, AssetBundle> onCmp, ResPriority priority)
    {
        initObb();
        var withFolderResName = resName;
        if (ResDepManager.Singleton.IsDefLanguage(resName))
            withFolderResName = PathUtil.resLanguage + "/" + resName;
        //obb只支持ab资源
        if (!withFolderResName.EndsWith(PathUtil.abSuffix))
            withFolderResName += PathUtil.abSuffix;
        
        if (priority == ResPriority.Sync || GetVersion(withFolderResName) < version)
        {
            //改同步加载
            if (loadingMap.ContainsKey(withFolderResName))
            {
                onCmp += loadingMap[withFolderResName];
                loadingMap.Remove(withFolderResName);
            }
            LoadAssetBundle(resName, version, onCmp);
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

    //限制协程数量
    private IEnumerator loadABAsyncLimited()
    {
        nowCoroutineNum++;
        while (true)
        {
            if (toLoadList.Count > 0)
            {
                var resName = toLoadList.Dequeue();
                yield return loadABAsync(resName);
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
        //obb只支持ab资源
        if (!withFolderResName.EndsWith(PathUtil.abSuffix))
            withFolderResName += PathUtil.abSuffix;

        AssetBundle ab = null;
        if(obbMap.ContainsKey(withFolderResName))
        {
            var obb = obbMap[withFolderResName];
            var abc = AssetBundle.LoadFromFileAsync(obbFilePath, 0, (ulong)obb.offset);
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
                Debug.LogError("ObbManager加载ab文件，怎么可能没有回调：" + withFolderResName);
            }

            if (loadingMap.ContainsKey(withFolderResName))
                loadingMap.Remove(withFolderResName);
        }
    }

    //如为多语言则包含语言文件件名字
    public int GetVersion(string withFolderResName)
    {
        initObb();
        //obb只支持ab资源
        if (!withFolderResName.EndsWith(PathUtil.abSuffix))
            withFolderResName += PathUtil.abSuffix;
        if (obbMap.ContainsKey(withFolderResName))
            return obbMap[withFolderResName].version;
        return -1;
    }


    private class Obb
    {
        public int version;
        public int offset;
    }

    private bool inited;
    private string obbFilePath;
    private Dictionary<string, Obb> obbMap = new Dictionary<string, Obb>();

    private void initObb()
    {
        if (inited) return;
        inited = true;

        if (Application.platform != RuntimePlatform.Android)
            return;

#if UNITY_ANDROID
        try
        {
            getObbPath();
            Debuger.Wrn("obbFilePath:" + obbFilePath);
            if (!File.Exists(obbFilePath))
            {
                Debuger.Err("obb file not exist");
                return;
            }

            //读取头文件
            int intLength = sizeof(int);
            var bytes = new byte[intLength];
            var stream = new FileInfo(obbFilePath).OpenRead();
            if (stream.Length < intLength)
            {
                stream.Close();
                Debuger.Err("obb文件异常，stream.Length < intLength");
                return;
            }
            stream.Read(bytes, 0, bytes.Length);
            int temp = 0;
            int length = XBuffer.ReadInt(bytes, ref temp);
            if (stream.Length < length)
            {
                stream.Close();
                Debuger.Err("obb文件异常，stream.Length < length");
                return;
            }

            bytes = new byte[length];
            stream.Position = 0;
            stream.Read(bytes, 0, length);
            stream.Close();
            stream.Dispose();

            //读取文件配置
            int offset = 0;
            XBuffer.ReadInt(bytes, ref offset); //总长度
            var code = XBuffer.ReadString(bytes, ref offset); //md5码

            //还原计算md5的数据
            offset = 0;
            XBuffer.WriteInt(0, bytes, ref offset);
            XBuffer.WriteString("ABCEDFGHIJKLMNOPQRSTUVWXYZ123456", bytes, ref offset);
            //计算真正的md5
            byte[] codeBytes = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(bytes);
            string realCode = System.BitConverter.ToString(codeBytes).Replace("-", "");
            if (code != realCode)
            {
                Debuger.Err("obb文件异常，校验失败");
                return;
            }

            XBuffer.ReadString(bytes, ref offset); //包名
            XBuffer.ReadLong(bytes, ref offset); //时间戳
            int size = XBuffer.ReadInt(bytes, ref offset);
            for (int i = 0; i < size; ++i)
            {
                var name = XBuffer.ReadString(bytes, ref offset);
                var version = XBuffer.ReadInt(bytes, ref offset);
                var abOffset = XBuffer.ReadInt(bytes, ref offset);
                obbMap[name] = new Obb { version = version, offset = abOffset };
            }
        }
        catch (System.Exception e)
        {
            Debuger.Err(e.Message, e.StackTrace);
            obbMap.Clear();
        }
#endif
    }

#if UNITY_ANDROID
    private string getObbPath()
    {
        if (!string.IsNullOrEmpty(obbFilePath))
            return obbFilePath;

        var Environment = new AndroidJavaClass("android.os.Environment");
        var state = Environment.CallStatic<string>("getExternalStorageState");
        if (state != "mounted")
        {
            obbFilePath = "none"; 
            Debuger.Err("obb getExternalStorageState>", state);
            return obbFilePath;
        }

        var package = "";
        var version = 0;
        using (AndroidJavaClass unity_player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject current_activity = unity_player.GetStatic<AndroidJavaObject>("currentActivity");
            package = current_activity.Call<string>("getPackageName");
            AndroidJavaObject package_info = current_activity.Call<AndroidJavaObject>("getPackageManager").Call<AndroidJavaObject>("getPackageInfo", package, 0);
            version = package_info.Get<int>("versionCode");
        }

        using (AndroidJavaObject externalStorageDirectory = Environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
        {
            string root = externalStorageDirectory.Call<string>("getPath");
            string obbPath = string.Format("{0}/Android/obb/{1}", root, package);
            obbFilePath = string.Format("{0}/main.{1}.{2}.obb", obbPath, version, package);
        }
        return obbFilePath;
    }
#endif
}