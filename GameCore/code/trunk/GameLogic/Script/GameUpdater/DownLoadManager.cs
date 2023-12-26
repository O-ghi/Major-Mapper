using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System.Threading;

public enum UpdaterState
{
    None,
    LocalChecking,
    RemoteChecking,
    LoadServerList,
    LoadPatches,
    ResReady,                       //资源验证准备完成
    ObbResReady,
    ObbDownLoading,
    ObbDownEnded,
    DownLoading,                    //更新中
    DownLoadPause,
    DownEnded,
    UnPacking,
    UnPackEnded,
    Complete,                       //更新器完成
    Error,
    LoadJsonError,
}

public class DownLoadManager : ManagerTemplate<DownLoadManager>
{
    private static UpdaterState m_state = UpdaterState.None;
    private static string m_log = "";                     
    
    public static string totalSize { get; private set; }
    public static float Progress { get; private set; }
    public static UpdaterState State { get { return m_state; } }
    public static string errorLog { get { return m_log; } }

    private static RequestFileInfo m_requestFileInfo;

	private static string m_remotePath = "";
	private static string m_localPath = "";

    protected override void Update()
    {
        switch (m_state)
        {
            case UpdaterState.DownLoading:
                if (HttpRequestManager.isError)
                {
                    Debug.LogError("httpRequestManager is error : "+ HttpRequestManager.errorMessage);
                    SetState(UpdaterState.DownLoadPause, HttpRequestManager.errorMessage);
                }
                else if (HttpRequestManager.isDone == false)
                {
                    Progress = (float)HttpRequestManager.loadedSize / (float)HttpRequestManager.totalSize;
                }
                else if (HttpRequestManager.isDone == true)
                {
                    SetState(UpdaterState.DownEnded);
                }
                break;
            case UpdaterState.DownEnded:
                ZipTools.isNeedSleep = true;
                SetState(UpdaterState.UnPacking);
                StartCoroutine(StartUnPack());
                break;
            case UpdaterState.UnPacking:
                if (ZipManager.isError)
                {
                    SetState(UpdaterState.Error, ZipManager.message);
                }
                else if (ZipManager.isDone == false)
                {
                    Progress = (float)ZipManager.loadedSize / (float)ZipManager.totalSize;
                }
                else if (ZipManager.isDone == true)
                {
                    SetState(UpdaterState.UnPackEnded);
                }
                break;
            case UpdaterState.UnPackEnded:
                ZipTools.isNeedSleep = false;
                SetState(UpdaterState.Complete);
                //开启个线程删除文件
                ThreadStart threadStart = new ThreadStart(AssetComplete);
                Thread t = new Thread(threadStart);
                t.IsBackground = true;
                t.Start();

                break;
            case UpdaterState.Error:
                Debug.LogError(m_log);
                SetState(UpdaterState.None);
                break;
        }
    }

    private void AssetComplete()
    {
		GameInfoManager.IsSmallClient = false;
		GameInfoManager.SaveClientConfig();

		//清除zip文件
		if (Directory.Exists(m_localPath))
            Directory.Delete(m_localPath, true);
    }

    IEnumerator StartUnPack()
    {
        yield return null;
		string patchLocal = GameInfoManager.GetAttibute("patch_local");
		ZipManager.StartUnZips(m_localPath, patchLocal, false);
    }


    private static void SetState(UpdaterState state, string msg = "")
    {
		Debug.LogFormat("DownLoad SetState:{0} msg:{1}", state, msg);

        m_state = state;
		m_log = msg;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        HttpRequestManager.Clear();
        ZipManager.Clear();
    }
    
    public static void StartDownLoad()
    {
        if (m_state != UpdaterState.ResReady)
            return;
     
        SetState(UpdaterState.DownLoading);
        HttpRequestManager.StartRequest();
    }

    public static void PauseDownLoad()
    {
        if (m_state == UpdaterState.DownLoadPause)
            return;
        SetState(UpdaterState.DownLoadPause, "主动暂停");
        HttpRequestManager.Clear();
    }

    public static void ContinueDownLoad()
    {
        if (m_state == UpdaterState.DownLoading)
            return;
        SetState(UpdaterState.ResReady);

        HttpRequestManager.EnqueueRequest(m_requestFileInfo, m_remotePath, m_localPath);

        if (HttpRequestManager.isDone)
        {
            SetState(UpdaterState.DownEnded);
        }
        //else if (PlatformManager.networkRechability == NetworkReachability.ReachableViaLocalAreaNetwork) // cần check xem có đang connect mạng không
        {
            Debug.Log("continueDownload ====");
            StartDownLoad();
        }
    }
    
    public static void DownLoadSubAssets()
    {
		m_remotePath = GameInfoManager.GetAttibute("patch_url");
		string subAssetsJsonPath = string.Format("{0}/SubAssets.json?{1}", m_remotePath, System.DateTime.Now.Ticks);
		m_localPath = GameInfoManager.GetAttibute("patch_cache");

		Instance.StartCoroutine(LoadsubAssetsJson(subAssetsJsonPath));
    }

    static IEnumerator LoadsubAssetsJson(string path)
    {
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(path);

		Debug.LogFormat("DownLoad SubAssets:{0}", path);

		yield return www.SendWebRequest();
        try
        {
            if (www.error != null || !www.isDone || string.IsNullOrEmpty(www.downloadHandler.text))
            {
                SetState(UpdaterState.Error, www.error);
            }

            //m_requestFileInfo = JsonUtility.FromJson<RequestFileInfo>(www.downloadHandler.text);
            m_requestFileInfo = new RequestFileInfo();
            JsonUtility.FromJsonOverwrite(www.downloadHandler.text, m_requestFileInfo);
            SetState(UpdaterState.ResReady);


            ZipManager.Enqueue(m_requestFileInfo);
			HttpRequestManager.EnqueueRequest(m_requestFileInfo, m_remotePath, m_localPath);
            totalSize = (m_requestFileInfo.size/1024).ToString();
            if (HttpRequestManager.totalSize<=HttpRequestManager.loadedSize)
            {
                SetState(UpdaterState.DownEnded);
            }
            //else if(PlatformManager.networkRechability == NetworkReachability.ReachableViaLocalAreaNetwork) // cần check xem có connect mạng không
            {
                StartDownLoad();
            }
        }
        catch (System.Exception ex)
        {
            SetState(UpdaterState.Error, ex.Message);
        }
    }

    protected override void InitManager()
	{
	}
}
