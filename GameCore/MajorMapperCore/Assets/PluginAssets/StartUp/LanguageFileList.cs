/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2018.12.27
*/

using System;
using System.Collections.Generic;

public class LanguageFileList
{
    private static LanguageFileList instance;
    public static LanguageFileList Singleton
    {
        get
        {
            if (instance == null)
                instance = new LanguageFileList();
            return instance;
        }
    }

    public bool CheckMD5 = true;
    private Action<bool> onComplete;
    private Action<long, long> onProgress;

    private string resLanguage;
    private int backStartIdx;
    private List<string> toDownloadList;
    private List<FileEntry> fileList;

    public long ServerLanguageResSize(string language, bool includeBackRes = false)
    {
        var list = ForceFileList.Singleton.GetToDownloadLanguageFileList(language);
        if (list == null)
            list = new List<FileEntry>();
        if (includeBackRes)
        {
            var backList = BackFileList.Singleton.GetToDownloadLanguageFileList(language);
            if (backList != null)
                list.AddRange(backList);
        }

        long size = 0;
        for (int i = list.Count - 1; i >= 0; --i)
        {
            if (!string.IsNullOrEmpty(list[i].md5))
            {
                if (list[i].md5 == ABManager.Singleton.GetForceFileMD5(list[i].folder + "/" + list[i].resName))
                    continue;
            }else
            {
                if (list[i].version > ABManager.Singleton.GetLocalVersion(list[i].md5, list[i].folder))
                    continue;
            }
            size += list[i].size;
        }
        return size;
    }

    public bool Download(string language, Action<bool> compleFunc, Action<long, long> updateProgressFunc, bool includeBackRes = false)
    {
        resLanguage = language;
        onComplete = compleFunc;
        onProgress = updateProgressFunc;
        fileList = new List<FileEntry>();
        toDownloadList = new List<string>();

        //force
        var list = ForceFileList.Singleton.GetToDownloadLanguageFileList(language);
        fileList.AddRange(list);

        var downloadList = VersionConfig.Singleton.ForceDownloadUrlList;
        if (downloadList == null || downloadList.Count == 0)
            return false;
        var urlFolder = downloadList[0];
        for (int i = list.Count - 1; i >= 0; --i)
            toDownloadList.Add(urlFolder + language + "/" + list[i].resName);

        backStartIdx = toDownloadList.Count;
        if(includeBackRes)
        {
            //back
            list = BackFileList.Singleton.GetToDownloadLanguageFileList(language);
            fileList.AddRange(list);

            downloadList = VersionConfig.Singleton.BackDownloadUrlList;
            if (downloadList != null && downloadList.Count > 0)
            {
                urlFolder = downloadList[0];
                for (int i = list.Count - 1; i >= 0; --i)
                    toDownloadList.Add(urlFolder + language + "/" + list[i].resName);
            }
        }
        
        //没有资源需要下载
        if (toDownloadList.Count <= 0)
            return false;

        start();
        return true;
    }

    private int loadedNum;
    private int failedNum;

    private long totalSize;
    private long loadedSize;

    private void start()
    {
        failedNum = 0;
        loadedNum = 0;

        loadedSize = 0;
        totalSize = 0;
        
        for (int i = 0, len = fileList.Count; i < len; ++i)
        {
            var file = fileList[i];
            totalSize += file.size;
            if(!string.IsNullOrEmpty(file.md5))
            {
                if (file.md5 != ABManager.Singleton.GetForceFileMD5(file.folder + "/" + file.resName))
                {
                    //force比较md5
                    WWWLoader.Singleton.Download(toDownloadList[i], onFileLoaded, null, file.md5);
                    continue;
                }
            }
            else if(file.version > ABManager.Singleton.GetLocalVersion(file.md5, file.folder))
            {
                //back比较版本号
                WWWLoader.Singleton.Download(toDownloadList[i], onFileLoaded, null, file.version + "");
                continue;
            }
            else
            {
                loadedNum++;
                loadedSize += file.size;
            }
        }
    }
    
    private void onFileLoaded(string path, bool success, byte[] data)
    {
        if(success)
        {
            int idx = toDownloadList.IndexOf(path);
            var file = fileList[idx];
            //force目录
            string savePath = PathUtil.GetForceABPath(file.folder + "/" + file.resName);
            //back目录 
            if(string.IsNullOrEmpty(file.md5))
                savePath = PathUtil.GetBackABPath(file.folder + "/" + file.resName);

            //back列表没有存MD5码
            if(CheckMD5 && !string.IsNullOrEmpty(file.md5))
            {
                byte[] hash = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(data);
                string md5 = System.BitConverter.ToString(hash).Replace("-", "");
                if (md5 != file.md5)
                {
                    Debuger.Err("LanguageFileList>md5码对不>", path);
                    failedNum++;
                    checkCmp();
                    return;
                }
            }

            try
            {
                if (System.IO.File.Exists(savePath))
                    System.IO.File.Delete(savePath);
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(savePath)))
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(savePath));
                System.IO.File.WriteAllBytes(savePath, data);

                if (string.IsNullOrEmpty(file.md5))
                {
                    ABManager.Singleton.RemoveServerToLocal(file.resName + "/" + file.folder);
                }
                else
                {
                    ForceManager.Singleton.AddUpdateFile(file.resName + "/" + file.folder);
                    if (loadedNum % 10 == 0)
                        ForceManager.Singleton.UpdateEnd();
                }
            }
            catch (System.Exception e)
            {
                Debuger.Err("写文件出错 LanguageFileList", path, e.Message);
                failedNum++;
                checkCmp();
                return;
            }
            loadedNum++;
            loadedSize += file.size;
            if (onProgress != null)
                onProgress(loadedSize, totalSize);
        }
        else
        {
            failedNum++;
        }
        checkCmp();
    }

    private void checkCmp()
    {
        if (loadedNum + failedNum >= toDownloadList.Count)
        {
            bool success = failedNum <= 0;
            if (success)
                ForceManager.Singleton.AddUpdateLanguage(resLanguage);
            ForceManager.Singleton.UpdateEnd();

            //下载已完成，
            if (onComplete != null)
                onComplete(success);
        }
    }
}