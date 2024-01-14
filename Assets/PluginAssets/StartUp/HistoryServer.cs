using UnityEngine;
using System.Collections.Generic;

 
public class HistoryServer : BaseDownloader
{
    public class ServerHistoryInfo
    {
        public string roleName;
        public int serverId;
        public int level;
        public int iconId;
        public long login_time;
    }

    private static HistoryServer instance;
    public static HistoryServer Singleton
    {
        get
        {
            if (instance == null)
                instance = new HistoryServer();
            return instance;
        }
    }

    protected string downLoadPath = "";  //下载路径 
    
    //历史区服数据
    public string Content { get; private set; }

    public Dictionary<int, ServerHistoryInfo> serverHistoriesDic;
    private List<ServerHistoryInfo> sortServerHistoryList;


    public ServerHistoryInfo GetHistoryInfoById(int serverId)
    {
        if (serverHistoriesDic == null)
            return null;

        if (serverHistoriesDic.ContainsKey(serverId))
        {
            return serverHistoriesDic[serverId];
        }
        return null;
    }

    //获得历史区服列表
    public List<ServerHistoryInfo> GetHistoryServerList()
    {
        if (serverHistoriesDic == null || serverHistoriesDic.Count == 0)
            return null;

        if (sortServerHistoryList != null && sortServerHistoryList.Count > 0)
            return sortServerHistoryList;

        sortServerHistoryList = new List<ServerHistoryInfo>(serverHistoriesDic.Values);
        sortServerHistoryList.Sort(_SortFun);
        return sortServerHistoryList;
    }

    private int _SortFun(ServerHistoryInfo a, ServerHistoryInfo b)
    {
        return b.login_time.CompareTo(a.login_time);
    }

    //开始拉取历史区服（应该在sdk登录成功后开始拉取）
    public void StartDownLoad(string userName)
    {
        string path = string.Format(getOrgUrl(), userName);
        Debug.Log("LINH LOG  HistoryPath ----  " + path);
        if (Loaded)
        {
            if(path.Equals(downLoadPath))
                return;
        }

        downLoadPath = path;
        serverHistoriesDic = null;
        sortServerHistoryList = null;
        Download();
    }

    //重新开始拉取历史区服（此接口目的是获取最新的历史区服）
    public void RestartDownLoad(string userName)
    {        
        string path = string.Format(getOrgUrl(), userName);
        downLoadPath = path;
        serverHistoriesDic = null;
        sortServerHistoryList = null;
        Download();
        Debuger.Log("--------------->>>开始拉取历史区服", path);
    }



    public override void Download()
    {
        //Debug.Log("LINH LOG History version" + mVersion.ToString() + "---  " + downLoadPath);
        WWWLoader.Singleton.Download(downLoadPath, onLoadCmp, onLoadUpdate, mVersion.ToString(), loadCache, false);
    }

    protected override void onLoadCmp(string path, bool success, byte[] data)
    {
        base.onLoadCmp(path, success, data);
        if (!Loaded)
            return;

        if (success && data != null)
        {
            Content = System.Text.Encoding.UTF8.GetString(data);
            Debuger.Log("--------->> 历史记录拉取结果", path, Content);
        }
        else
        {
            Debuger.Err("历史区服记录获取失败!");
            //CoroutineManager.Singleton.delayedCall(10, ReDownload);
        }

        if (!string.IsNullOrEmpty(Content))
        {
            SimpleJSON.JSONArray arr = SimpleJSON.JSONArray.Parse(Content) as SimpleJSON.JSONArray;
            serverHistoriesDic = new Dictionary<int, ServerHistoryInfo>();
            for (int i = 0, len = arr.Count; i < len; ++i)
            {                
                ServerHistoryInfo histotyInfo = new ServerHistoryInfo();
                histotyInfo.roleName = arr[i]["role_name"];
                histotyInfo.serverId = arr[i]["server_id"].AsInt;
                histotyInfo.level = arr[i]["level"].AsInt;
                histotyInfo.iconId = arr[i]["icon"].AsInt;
                histotyInfo.login_time = (long)arr[i]["login_time"].AsDouble;

                if (!serverHistoriesDic.ContainsKey(histotyInfo.serverId))
                {
                    serverHistoriesDic.Add(histotyInfo.serverId, histotyInfo);
                }
                else
                {
                    serverHistoriesDic[histotyInfo.serverId] = histotyInfo;
                }
            }
        }

        if (mCallback != null)
            mCallback();

        //GED.ED.dispatchEvent(BaseEventID.LoginHistory);
    }

}