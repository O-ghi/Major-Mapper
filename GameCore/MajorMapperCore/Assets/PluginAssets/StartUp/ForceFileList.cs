/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2017.10.25
*/

using System.IO;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;

public class ForceFileList : BaseDownloader
{
    private static ForceFileList instance;
    public static ForceFileList Singleton
    {
        get
        {
            if(instance == null)
                instance = new ForceFileList();
            return instance;
        }
    }

    public int ForceCheckVersion;
    /// <summary>
    /// 检测更新
    /// </summary>
    public void CheckUpdate(int localVersion, System.Action callback)
    {
#if UNITY_EDITOR 
        if(true)
        {
            Debug.Log("Simulate HotUpdate");
            checkHotUpdate(localVersion, callback);
        }
        else
        {
            Debug.Log("No HotUpdate");
            if (callback != null)
                callback();
        }
      
#else
        checkHotUpdate(localVersion,callback);
#endif
    }
    private void checkHotUpdate(int localVersion, System.Action callback)
    {
        Debuger.Log("startUp ", GetType().Name);
        mCallback = callback;
        Debug.Log("ForceFileList mVersion: " + mVersion + " ||| ForceCheckVersion: " + ForceCheckVersion);
        if (mVersion > localVersion || (ForceCheckVersion == mVersion && ForceCheckVersion > 0))
        {
            Download();
        }
        else
        {
            if (mCallback != null)
                mCallback();
        }
    }

    public override void Download()
    {
        Debug.Log("ForceFileList download: " + getDownloadUrl());
        WWWLoader.Singleton.Download(getDownloadUrl(), onLoadCmp, null);
        //UnityWebLoader.Singleton.Download(getDownloadUrl() + "?" + mVersion, onLoadCmp, null);
    }

    private JSONClass newJson;
    private JSONClass tmpJson;
    private JSONClass jsonForceUpdated;

    protected int totalSize = 0;
    protected int loadedSize = 0;

    public bool checkMD5 = true;
    protected bool hideLoading = false;
    protected List<string> downloadUrlList = new List<string>();
    protected string tmpLoadedPath = PathUtil.ForceFileTmpLoadedPath;
    protected Dictionary<string, FileEntry> fileMap = new Dictionary<string, FileEntry>();
    protected Dictionary<string, FileEntry> filePathMap = new Dictionary<string, FileEntry>();
    protected Dictionary<string, bool> loadedMap = new Dictionary<string, bool>();
    protected Dictionary<string, int> loadingSizeMap = new Dictionary<string, int>();
    protected JSONArray fileArr;

    public void SetDownloadUrl(List<string> list)
    {
        downloadUrlList = list;
    }

    protected override string getDownloadUrl()
    {
        return getOrgUrl();
    }

    /// <summary>
    /// 对应语言需要下载的文件列表
    /// </summary>
    public List<FileEntry> GetToDownloadLanguageFileList(string language)
    {
        if (fileArr == null)
            return null;

        int ver, level, sz;
        string name = "", folder = "";
        List<FileEntry> list = new List<FileEntry>();
        for (int i = 0, len = fileArr.Count; i < len; ++i)
        {
            var resPath = fileArr[i]["res"].Value;
            if (resPath.IndexOf("/") <= 0)
                continue;
            var pathArr = resPath.Split('/');
            //只读取当前语言配置
            if (language != pathArr[0])
                continue;

            folder = pathArr[0];
            name = pathArr[1];
            //添加后台需要下载的资源
            sz = fileArr[i]["size"].AsInt;
            ver = fileArr[i]["ver"].AsInt;
            level = fileArr[i]["level"].AsInt;
            
            list.Add(new FileEntry { resName = name, folder = folder, version = ver, priority = level, size = sz });
        }
        return list;
    }

    private int failedTimes = 0;
    protected override void onLoadCmp(string path, bool success, byte[] data)
    {
        forceUrlIdx = 0;
        base.onLoadCmp(path, success, data);
        if(Loaded)
        {
            ended = false;
            totalSize = 0;
            loadedSize = 0;
            fileMap.Clear();
            loadedMap.Clear();
            filePathMap.Clear();
            loadingSizeMap.Clear();
            if (success && data != null)
            {
                newJson = new JSONClass();
                string forcePath = PathUtil.ForceFileListOutPath;
                //本地强更资源列表
                if(File.Exists(forcePath))
                    jsonForceUpdated = JSONNode.LoadFromFile(forcePath) as JSONClass;
                if(jsonForceUpdated == null)
                    jsonForceUpdated = new JSONClass();

                //临时保存列表
                if(File.Exists(tmpLoadedPath))
                    tmpJson = JSONNode.LoadFromFile(tmpLoadedPath) as JSONClass;
                if(tmpJson == null)
                    tmpJson = new JSONClass();
                
                //解析列表
                string str = System.Text.Encoding.UTF8.GetString(data);
                JSONClass json = JSONClass.Parse(str) as JSONClass;
                if(json != null && json["assets"] != null)
                {
                    JSONArray arr = json["assets"] as JSONArray;
                    Debug.Log("ForceFileList Force Update assets " + arr.Count);
                    int size, ver;
                    fileArr = arr;
                    string md5, res, folder, hashAB;
                    for (int i = 0, len = arr.Count; i < len; ++i)
                    {
                        var resPath = arr[i]["name"].Value;
                        Debuger.Log("ForceFileList " + resPath);
                        if (resPath.IndexOf("/") > 0)
                        {
                            var pathArr = resPath.Split('/');
                            //只读取当前语言配置
                            folder = pathArr[0];
                            res = pathArr[1];
                        }
                        else
                        {
                            folder = null;
                            res = resPath;
                        }
                        md5 = arr[i]["md5"].Value;
                        ver = arr[i]["ver"].AsInt;
                        size = arr[i]["size"].AsInt;
                        hashAB = arr[i]["hashAB"].Value;
                        //添加到本地列表
                        JSONClass node = new JSONClass();
                        newJson[resPath] = node;
                        node["md5"] = md5;
                        //Debug.Log("md5 " + md5 + "  old:  " + ABManager.Singleton.GetForceFileMD5(resPath));
                        //本地没有或者md5码不一样则下载
                        if (md5 != ABManager.Singleton.GetForceFileMD5(resPath))
                        {
                            if (!fileMap.ContainsKey(resPath))
                            {
                                if (tmpJson[resPath] != null && tmpJson[resPath].Value == md5)
                                    continue;
                                
                                fileMap.Add(resPath, new FileEntry { folder = folder, resName = res, md5 = md5, version = ver, size = size });
                                totalSize += size;
                            }
                            else
                            {
                                Debuger.Err("致命错误，检测到重复的强更资源", res);
                            }
                        }
                    }
                    Debug.Log("fileMap " + fileMap.Count);
                    if(fileMap.Count > 0)
                    {
                        //hideLoading = json["hideLoading"].AsBool;
                        //if(json["showPopSize"].AsInt <= totalSize / 1024)
                        //非wifi则弹窗提示
                        //if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                        //    StartupTip.Singleton.TipForceResUpdate(json["title"].Value, json["tip"].Value, totalSize / 1024, startDownload);
                        //else
                            startDownload();
                    } else
                    {
                        if(mCallback != null)
                            mCallback();
                    }
                } else
                {
                    Debuger.Log("ForceFileList Download force list error", path);
                    //StartupTip.Singleton.TipNoNetwork(GetType().Name, ReDownload);
                }
            } else
            {
                Debuger.Log("下载失败，请检查网络链接 null", path);
                //StartupTip.Singleton.TipNoNetwork(GetType().Name, ReDownload);
                if (failedTimes <= 3)
                {
                    failedTimes++;
                    Debuger.Log("失败后自动从新下载>", failedTimes);
                    ReDownload();
                }
                else
                {
                    failedTimes = 0;
                    //StartupTip.Singleton.TipNoNetwork(GetType().Name, ReDownload);
                    Debuger.Log("失败后自动从新下载>", GetType().Name);

                    ReDownload();
                }
            }
        }
    }

    private int filesFailedTimes = 0;
    private bool hasFailedFile = false;
    private float lastProgressTime = 0;
    private long fileFailedDelayId;
    private void checkTipFilesFailed()
    {
        if (hasFailedFile && Time.time - lastProgressTime > 5f)
        {
            //没有下载进度改变时，下载3次失败才提示下载失败
            hasFailedFile = false;
            lastProgressTime = Time.time;
            if (filesFailedTimes <= 3)
            {
                filesFailedTimes++;
                Debuger.Wrn("失败自动重新下载>", filesFailedTimes);
                startDownload();
            }
            else
            {
                filesFailedTimes = 0;
                //StartupTip.Singleton.TipNoNetwork(GetType().Name, startDownload);
                startDownload();
            }
        }
        else
        {
            CoroutineManager.Singleton.stopCoroutine(fileFailedDelayId);
            fileFailedDelayId = CoroutineManager.Singleton.delayedCall(5f, checkTipFilesFailed);
        }
    }

    /// <summary>
    /// 更新进度
    /// </summary>
    protected override void onLoadUpdate(string path, float progress, ulong loadedBytes)
    {
        lastProgressTime = Time.time;
        int loadingLoadedSize = 0;
        if(filePathMap.ContainsKey(path))
        {
            loadingSizeMap[path] = (int)(filePathMap[path].size * progress);
            foreach (var size in loadingSizeMap.Values)
                loadingLoadedSize += size;
        }

        //if(hideLoading)
        //    StartupTip.Singleton.TipProgress(GetType().Name, (loadingLoadedSize + loadedSize) / (float)totalSize, 0, true);
        //else
        //    StartupTip.Singleton.TipProgress(GetType().Name, (loadingLoadedSize + loadedSize) / (float)totalSize, totalSize / 1024);
    }

    /// <summary>
    /// 开始下载
    /// </summary>
    protected virtual void startDownload()
    {
        Debug.Log("startDownload force list");

        checkTipFilesFailed();
        downloadFileList();
        forceUrlIdx++;
        if (forceUrlIdx >= downloadUrlList.Count)
            forceUrlIdx = 0;
    }

    /// <summary>
    /// 下载列表
    /// </summary>
    protected virtual void downloadFileList()
    {
        ended = false;
        var enu = fileMap.GetEnumerator();
        string url = getForceFileUrl();
        while(enu.MoveNext())
        {
            FileEntry fe = enu.Current.Value;
            string resName = fe.resName;
            //if (!string.IsNullOrEmpty(fe.folder))
            //    resName = fe.folder + "/" + resName;

            //已经下载了
            if (loadedMap.ContainsKey(resName))
                continue;
            //Debug.Log("url: " + url + " ||| resName: " + resName + " || Format " + PathUtil.GetServerPath(url, resName));
            var serverPath = string.Format(PathUtil.GetServerPath(url, resName), mVersion);
            filePathMap[serverPath] = fe;
            WWWLoader.Singleton.Download(serverPath, onFileLoaded, onLoadUpdate, fe.md5);
        }
        tryComplete();
    }

    private bool ended = false;
    private float lastWirteTime;
    protected virtual void tryComplete()
    {
        if (ended)
            return;

        try
        {
            int loadedNum = loadedMap.Count;
            //没有可以下载的
            if (fileMap.Count <= loadedNum)
            {
                try
                {
                    var enu = newJson.GetEnumerator();
                    while(enu.MoveNext())
                    {
                        var node = (KeyValuePair<string, JSONNode>)enu.Current;
                        ForceManager.Singleton.AddUpdateFile(node.Key);
                        jsonForceUpdated[node.Key] = node.Value;
                    }
                }catch(System.Exception e)
                {
                    Debuger.Err(e.Message, e.StackTrace);
                }
                ForceManager.Singleton.UpdateEnd();
                
                string path = PathUtil.ForceFileListOutPath;
                jsonForceUpdated.SaveToFile(path);
                ABManager.Singleton.UpdateForceFile();

                if(File.Exists(tmpLoadedPath))
                    File.Delete(tmpLoadedPath);
                dispose();
                ended = true;
                CoroutineManager.Singleton.stopCoroutine(fileFailedDelayId);
                if (mCallback != null)
                    mCallback();
            } else
            {
                float now = Time.time;
                if(loadedNum % 10 == 0 && now - lastWirteTime > 5)
                {
                    //5秒或者10个文件写一次
                    lastWirteTime = now;
                    if(File.Exists(tmpLoadedPath))
                        File.Delete(tmpLoadedPath);
                    tmpJson.SaveToFile(tmpLoadedPath);
                }
            }
        } catch(System.Exception e)
        {
            Debuger.Err(e.Message, e.StackTrace);
            //StartupTip.Singleton.TipForRestartGame("force file update end error");
        }
    }

    private Dictionary<string, bool> md5FailedMap = new Dictionary<string, bool>();
    /// <summary>
    /// 下载完成
    /// </summary>
    protected virtual void onFileLoaded(string path, bool success, byte[] data)
    {
        Debuger.Log("onFileLoaded " + path);
        var fe = filePathMap[path];
        string name = fe.resName;
        if (!string.IsNullOrEmpty(fe.folder))
            name = fe.folder + "/" + name;
        if (loadingSizeMap.ContainsKey(path))
            loadingSizeMap.Remove(path);

        if (success && data != null)
        {
            string fileMd5 = "";
            bool legal = true;
            //fileMd5 = md5;

            if (checkMD5)
            {
                /// So sánh mã MD5 để xác minh xem quá trình tải xuống đã hoàn tất chưa
                string md5 = AssetsUtility.GetMd5Hash(data);
                legal = fe.md5 == md5;
                fileMd5 = md5;
            }

            if (legal)
            {
                //Lưu vào cục bộ
                string mPath = PathUtil.GetForceABPath(name);
                try
                {
					var dir = Path.GetDirectoryName(mPath);
					if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    if(File.Exists(mPath))
                        File.Delete(mPath);
                    File.WriteAllBytes(mPath, data);
                } catch(System.Exception e)
                {
                    Debuger.Err("Lỗi khi ghi tệp ForceFileList", path, e.Message);
                    //StartupTip.Singleton.TipWriteFileError(GetType().Name, startDownload);
                    startDownload();
                    return;
                }

                //Tải xuống ngay sau khi ngắt kết nối có thể được tải xuống thành công nhiều lần
                if (!loadedMap.ContainsKey(name))
                    loadedSize += fe.size;

                // Sửa đổi danh sách cục bộ, tải xuống điểm ngắt
                tmpJson[name] = fe.md5;
                loadedMap[name] = true;
                lastProgressTime = Time.time;
                CoroutineManager.Singleton.delayedCall(0.5f, tryComplete);
            } else
            {
                //Debuger.Err("File MD5 verification failed, download again", path, fe.md5, fileMd5);
                if (!md5FailedMap.ContainsKey(path))
                {
                    md5FailedMap.Add(path, true);
                    WWWLoader.Singleton.Download(path, onFileLoaded, onLoadUpdate, fe.md5 + System.DateTime.Now.Ticks);
                    //UnityWebLoader.Singleton.Download(path, onFileLoaded, onLoadUpdate, fe.md5);
                }else
                {
                    md5FailedMap.Remove(path);
                    hasFailedFile = true;
                    failedTimes = 5;
                }
            }
        } else
        {
            Debuger.Err("下载失败，请检查网络链接", path);
            hasFailedFile = true;
            //StartupTip.Singleton.TipNoNetwork(GetType().Name, startDownload);
        }
    }

    private int forceUrlIdx = 0;
    private string getForceFileUrl()
    {
        if (downloadUrlList.Count > forceUrlIdx)
            return downloadUrlList[forceUrlIdx];
        if (downloadUrlList.Count > 0)
            return downloadUrlList[0];
        Debug.Log("没有找到下载地址" + GetType().Name);
        return "";
    }

    private void dispose()
    {
        forceUrlIdx = 0;
        jsonForceUpdated = null;
        newJson = null;
        tmpJson = null;
        fileMap.Clear();
        loadedMap.Clear();
        instance = null;
    }
}