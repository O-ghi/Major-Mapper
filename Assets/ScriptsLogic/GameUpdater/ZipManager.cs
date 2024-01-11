using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading;

public class ZipManager : ManagerTemplate<ZipManager>
{
    private static long totalsize = 0;                                               
    private static long loadedsize = 0;
    private static bool isdone = false;
    private static bool iserror = false;
    private static string msg = "";
    
    public static bool isError { get { return iserror; } }
    public static bool isDone { get { return isdone; } }
    public static long loadedSize { get { return loadedsize; } }
    public static long totalSize { get { return totalsize; } }
    public static string message { get { return msg; } }

    private static Queue<RequestFileInfo> m_fileinfoQueue;
    private static RequestFileInfo m_currPatche;
    private static string srcPath;
    private static string destPath;
    private static bool m_replace = true;

    public ZipManager()
    {
        InitManager();
    }
    protected override void InitManager()
    {
        if(m_fileinfoQueue == null)
        {
            m_fileinfoQueue = new Queue<RequestFileInfo>();
        }
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Clear();
    }

    public static void Clear()
    {
        m_fileinfoQueue.Clear();
        iserror = false;
        isdone = false;
        loadedsize = 0;
        totalsize = 0;
        msg = "";
    }

    public static void Enqueue(RequestFileInfo fileinfo)
    {
        m_fileinfoQueue.Enqueue(fileinfo);
    }

    public static RequestFileInfo DequeuePatches()
    {
        if (m_fileinfoQueue.Count > 0)
            return m_fileinfoQueue.Dequeue();
        return null;
    }

    /// <summary>
    /// 解压线程
    /// </summary>
    public static void StartUnZips(string srcpath, string destpath, bool replace = true)
    {
        srcPath = srcpath;
        destPath = destpath;
        m_replace = replace;

        //开启个线程解压文件
        ThreadStart threadStart = new ThreadStart(UnpackThread);
        Thread t = new Thread(threadStart);
        t.IsBackground = true;
        t.Start();
    }

    private static void UnpackThread()
    {
        UnZip();
    }

    private static void UnZip()
    {
        m_currPatche = DequeuePatches();
        if (m_currPatche != null)
        {
            string path = srcPath + "/" + m_currPatche.name;
            if (!File.Exists(path))
                return;
            ZipTools.UnZipDir(path, destPath, Progress, Complete,Error,m_replace);
        }
        else
        {
            isdone = true;
        }
    }

    private static void Error(string error)
    {
        iserror = true;
        msg = error;
    }

    private static void Progress(string filename,long size,long tsize)
    {
        loadedsize = size;
        totalsize = tsize;
        msg = filename;
    }

    private static void Complete()
    {
        UnZip();
    }

}
