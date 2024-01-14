///*
 
// * -----------------------------------------------
// * Copyright (c) zhou All rights reserved.
// * -----------------------------------------------
// * 
// * Coder：Zhou XiQuan
// * Time ：2017.10.25
// */
//using UnityEngine;
//using System.Collections.Generic;


//public class ServerList : BaseDownloader
//{
//    private static ServerList instance;
//    private string Api_Key = "dxT81huPeQF1TGFySdGRKvMEtOGBs9y9";
//    public static ServerList Singleton
//    {
//        get
//        {
//            if(instance == null)
//                instance = new ServerList();
//            return instance;
//        }
//    }
//    public void setApiKey(string api_Key)
//    {
//        Api_Key = api_Key;
//    }

//    public string CloseDesc { get; private set; }

//    public ServerGroup[] ServerGroups { get; private set; }
    
//    public string Content { get; private set; }

//    public Server LastServer
//    {
//        get
//        {
//            int serverID = PlayerPrefs.GetInt(Last_Server_Key, -123456);
//            if (serverID != -123456)
//                return GetServer(serverID);
//            return null;
//        }
//    }

//    private const string Last_Server_Key = "Last_Server_key";
//    private Server curServer;
//    public Server CurrentServer
//    {
//        get
//        {
//            if(curServer == null)
//            {
//                //PlayerPrefs.DeleteKey(Last_Server_Key);
//                int serverID = PlayerPrefs.GetInt(Last_Server_Key, -123456);
//                if(serverID == -123456)
//                    curServer = GetRecommendServer();
//                else
//                    curServer = GetServer(serverID);

//                if (curServer == null)
//                    curServer = GetRecommendServer();
//            }
//            return curServer;
//        }
//        set
//        {
//            curServer = value;
//            if(value == null)
//            {
//                Debuger.Err("当前服务器被设置为空");
//                PlayerPrefs.DeleteKey(Last_Server_Key);
//                PlayerPrefs.Save();
//                return;
//            }
//            PlayerPrefs.SetInt(Last_Server_Key, value.id);
//            PlayerPrefs.Save();
//        }
//    }

//    /// <summary>
//    /// 获取服务器信息
//    /// </summary>
//    public Server GetServer(int serverID)
//    {
//        if (ServerGroups == null)
//            return null;
//        for (int i = 0, len = ServerGroups.Length; i < len; ++i)
//        {
//            var arr = ServerGroups[i].servers;
//            for (int j = 0, num = arr.Length; j < num; ++j)
//            {
//                if (arr[j].id == serverID)
//                    return arr[j];
//            }
//        }
//        return null;
//    }

//    //获得状态为新的服务器信息
//    public List<Server> GetNewServers()
//    {
//        if (ServerGroups == null)
//            return null;

//        List<Server> servers = new List<Server>();
//        for (int i = 0, len = ServerGroups.Length; i < len; ++i)
//        {
//            var arr = ServerGroups[i].servers;
//            for (int j = 0, num = arr.Length; j < num; ++j)
//            {
//                if (arr[j].state == (int)ServerState.Fluency)
//                {
//                    servers.Add(arr[j]);
//                }
//            }
//        }
//        return servers;
//    }
//    public List<Server> GetListRecommendServer()
//    {
//        if (ServerGroups == null)
//            return null;

//        List<Server> servers = new List<Server>();
//        for (int i = 0, len = ServerGroups.Length; i < len; ++i)
//        {
//            var arr = ServerGroups[i].servers;
//            for (int j = 0, num = arr.Length; j < num; ++j)
//            {
//                if (arr[j].state == (int)ServerState.Recommend)
//                {
//                    servers.Add(arr[j]);
//                }
//            }
//        }
//        return servers;
//    }

//    public ServerGroup GetServerGroup(int serverID)
//    {
//        if (ServerGroups == null)
//            return null;
//        for (int i = 0, len = ServerGroups.Length; i < len; ++i)
//        {
//            var arr = ServerGroups[i].servers;
//            for (int j = 0, num = arr.Length; j < num; ++j)
//            {
//                if (arr[j].id == serverID)
//                    return ServerGroups[i];
//            }
//        }
//        return null;
//    }

//    public Server GetWeekVersion()
//    {
//        if (ServerGroups == null)
//        {
//            //Logger.err("server groups is null");
//            return null;
//        }
//        for (int i = 0, len = ServerGroups.Length; i < len; ++i)
//        {
//            Server[] arr = ServerGroups[i].servers;
//            for (int j = 0, num = arr.Length; j < num; ++j)
//            {
//                if (arr[j].name.Contains("周版本"))
//                    return arr[j];
//            }
//        }
//        return null;
//    }

//    private Server GetRandomServer()
//    {
//        if (ServerGroups == null)
//            return null;

//        int result = 0;
//        int randomTotal = 0;
//        List<Server> list = new List<Server>();
//        for (int i = 0, len = ServerGroups.Length; i < len; ++i)
//        {
//            //if (ServerGroups[i].language != PathUtil.resLanguage)
//            //    continue;
//            var arr = ServerGroups[i].servers;
//            for (int j = 0, num = arr.Length; j < num; ++j)
//            {
//                list.Add(arr[j]);
//                randomTotal += arr[j].random;
//            }
//        }

//        //没有语言控制则全选
//        if(list.Count == 0)
//        {
//            for (int i = 0, len = ServerGroups.Length; i < len; ++i)
//            {
//                var arr = ServerGroups[i].servers;
//                for (int j = 0, num = arr.Length; j < num; ++j)
//                {
//                    list.Add(arr[j]);
//                    randomTotal += arr[j].random;
//                }
//            }
//        }

//        if(randomTotal <= 0)
//        {
//            //没有概率直接随机
//            result = Random.Range(0, list.Count);
//            return list[result];
//        }else
//        {
//            result = Random.Range(0, randomTotal);
//            for (int i = 0, len = list.Count; i < len; ++i)
//            {
//                var server = list[i];
//                result -= server.random;
//                if (result < 0)
//                    return server;
//            }
//        }

//        if (ServerGroups.Length > 0 && ServerGroups[0].servers.Length > 0)
//            return ServerGroups[0].servers[0];
//        return null;
//    }
//    private Server GetRecommendServer()
//    {
//        if (ServerGroups == null)
//            return null;
//        Server res = null;
//        List<Server> list = new List<Server>();
//        for (int i = 0, len = ServerGroups.Length; i < len; ++i)
//        {
//            //if (ServerGroups[i].language != PathUtil.resLanguage)
//            //    continue;
//            var arr = ServerGroups[i].servers;
//            for (int j = 0, num = arr.Length; j < num; ++j)
//            {
//                var sv = arr[j];
//                if (sv.state == (int)ServerState.Recommend)
//                {
//                    if(res == null || res.id < sv.id)
//                    res = sv;
//                }
//            }
//        }

//        if (res == null &&  ServerGroups.Length > 0 && ServerGroups[0].servers.Length > 0)
//            return ServerGroups[0].servers[0];
//        return res;
//    }

//    public override void Download()
//    {
//        //UnityWebLoader.Singleton.Download(getDownloadUrl(), onLoadCmp, onLoadUpdate, mVersion, true);
//        //string url = "http://125.212.249.22:8090/server/server_list?ts={0}&s={1}";
//        string ts = GetTimestamp(System.DateTime.UtcNow);
//        string md5s = Utilities.getMd5str(ts + Api_Key);
//        string path = string.Format(getOrgUrl(), ts, md5s);
//        WWWLoader.Singleton.Download(path, onLoadCmp, onLoadUpdate,mVersion.ToString(), loadCache, true);
//    }

//#if UNITY_IOS
//    protected override string getDownloadUrl()
//    {
//        //Duong remove channel Zhangyu
//        //if ((ChannelManager.ChannelName == "zhangyu_ios" || ChannelManager.ChannelName == "zhangyu_ios_yueyu") && GameLauncher.CompatibleFlag >= 10)
//        //{
//        //    var channel = ZhangYuIOSCaller.doGetMetaValue("promoteid");
//        //    Debug.Log("zhangyu ios promote id>" + channel);
//        //    return string.Format(getOrgUrl(), ChannelManager.Channel, channel);
//        //}
//        //else

//         return base.getDownloadUrl();
//    }
//#endif
//    private static string GetTimestamp(System.DateTime value)
//    {
//        return value.ToString("yyyyMMddHHmmssffff");
//    }

//    protected override void onLoadCmp(string path, bool success, byte[] data)
//    {
//        base.onLoadCmp(path, success, data);
//        if(!Loaded)
//            return;
        
//        //Debug.Log("Server list download success is "+ success);
//        if (success && data != null)
//        {
//            Content = System.Text.Encoding.UTF8.GetString(data);
//        }
//        else
//        {
//            Debuger.Err("Tải xuống danh sách máy chủ không thành công, tải xuống lại trong 3 giây ");
//            CoroutineManager.Singleton.delayedCall(3, ReDownload);
//        }
//        //Debug.Log("Server list download Content " + Content);

//        if (!string.IsNullOrEmpty(Content))
//        {

//            SimpleJSON.JSONNode json = SimpleJSON.JSONClass.Parse(Content);
//            if(json["ret"].AsInt == 0)
//            {
//                CloseDesc = json["closeDesc"];
//                SimpleJSON.JSONArray arr = json["servers"].AsArray;
//                ServerGroups = new ServerGroup[arr.Count];
//                for (int i = 0, len = arr.Count; i < len; ++i)
//                {
//                    ServerGroups[i] = new ServerGroup();

//                    var group = arr[i]["servers"];
//                    int num = group.Count;
//                    ServerGroups[i].servers = new Server[num];
//                    ServerGroups[i].id = arr[i]["groupId"].AsInt;
//                    ServerGroups[i].name = arr[i]["groupName"].Value;
//                    ServerGroups[i].language = arr[i]["language"].Value;

//                    for (int j = 0; j < num; ++j)
//                    {
//                        Server server = new Server();
//                        ServerGroups[i].servers[j] = server;
//                        server.id = group[j]["id"].AsInt;
//                        server.ip = group[j]["ip"].Value;
//                        server.port = group[j]["port"].AsInt;
//                        server.name = group[j]["name"].Value;
//                        server.random = group[j]["random"].AsInt;
//                        server.state = group[j]["state"].AsInt;
//                    }
//                }
//            }
//            else
//            {
//                Debuger.Err("Call API Error Code:" + json["ret"].AsInt + " MsgError: " + json["msg"]);
//            }
           
//        }

//        if (mCallback != null)
//            mCallback();
//        GED.ED.dispatchEvent(BaseEventID.ServerListLoaded);
//    }
    
//    public class Server
//    {
//        public int id;
//        public string name;
//        public string ip;
//        public int port;
//        public int state;
//        public int random;
//    }

//    public class ServerGroup
//    {
//        public int id;
//        public string name;
//        public Server[] servers;
//        public string language;
//    }

//    public enum ServerState
//    {
//        Hide     = 0,  //隐藏
//        Bomb     = 1,  //火爆
//        Fluency  = 2,  //流畅
//        Maintain = 3,  //维护
//        Busy     = 4,  //拥挤
//        Recommend = 5, //Uu tien Init
//    }
//}