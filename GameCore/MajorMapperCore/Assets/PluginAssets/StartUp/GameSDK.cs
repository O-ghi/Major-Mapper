///* 
// * -----------------------------------------------
// * Copyright (c) zhou All rights reserved.
// * -----------------------------------------------
// * 
// * Coder：Zhou XiQuan
// * Time ：2017.10.25
//*/

//using System;
//using UnityEngine;
//using System.Runtime.InteropServices;

//public class GameSDK
//{
//    private const string SDK_JAVA_DeviceUtils = "com.gihot.radteamsdk.utils.DeviceUtils";
//    private const string SDK_JAVA_AssetsUtils = "com.gihot.radteamsdk.utils.AssetsUtils";
//    private const string SDK_JAVA_ZPHTemp = "com.gihot.radteamsdk.utils.ZPHTemp";

//    private static AndroidJavaClass SDK_DeviceUtils;
//    private static AndroidJavaClass SDK_AssetsUtils;
//    private static AndroidJavaClass SDK_ZPHTemp;

//    private static GameSDK instance;
//    public static GameSDK Singleton
//    {
//        get
//        {
//            if (instance == null)
//                instance = new GameSDK();
//            return instance;
//        }
//    }

//#if UNITY_IOS && !UNITY_EDITOR
//    [DllImport("__Internal")]
//    private static extern void SetClipBoardText(string value);
//#endif

//    public static AndroidJavaClass GetAndroidJavaClass(string path)
//    {
//        if (Application.platform == RuntimePlatform.Android)
//        {
//            try
//            {
//                return new AndroidJavaClass(path);
//            }
//            catch (Exception e)
//            {
//                Logger.log(e.Message);
//            }
//        }

//        return null;
//    }
//    public GameSDK()
//    {
//#if UNITY_ANDROID && !UNITY_EDITOR
//        //Duong 18/11/23 comment call SDK
//        /*
//        SDK_DeviceUtils = GetAndroidJavaClass(SDK_JAVA_DeviceUtils);
//        SDK_AssetsUtils = GetAndroidJavaClass(SDK_JAVA_AssetsUtils);
//        SDK_ZPHTemp = GetAndroidJavaClass(SDK_JAVA_ZPHTemp);
//        */
//#endif
//    }

//    private AndroidJavaClass downloader;
//    /// <summary>
//    /// 下载保存app
//    /// </summary>
//    /// <param name="url">下载路径</param>
//    /// <param name="savePath">保存路径</param>
//    public void DownloadAndSaveApp(string url, string savePath, string gameObjName, string func)
//    {
//        //Duong 18/11/23 comment call SDK
//        /*
//#if UNITY_ANDROID && !UNITY_EDITOR
//        if(downloader == null)
//            downloader = new AndroidJavaClass("com.zhou.gamesdk.Downloader");
//        downloader.CallStatic("Download", AndroidBrigde.Singleton.GetCurrentActivity(), url, savePath, gameObjName, func);
//#else
//        Debuger.Err("GameSDK.DownloadAndSaveApp当前平台未实现");
//#endif
//        */
//    }

//    public void QuitGame()
//    {
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#else
//        Application.Quit();
//#endif
//    }

//    public int GetURLFileSize(string url)
//    {
//        //Duong 18/11/23 comment call SDK
//        /*
//#if UNITY_ANDROID && !UNITY_EDITOR
//        if(downloader == null)
//            downloader = new AndroidJavaClass("com.zhou.gamesdk.Downloader");
//        return downloader.CallStatic<int>("GetURLFileSize", url);
//#else
//        Debuger.Err("GameSDK.GetURLFileSize当前平台未实现");
//#endif
//        */
//        return -1;
//    }

//    /// <summary>
//    /// 安装app
//    /// </summary>
//    /// <param name="path">文件路径</param>
//    public void InstallApp(string path)
//    {
//        //Duong 18/11/23 comment call SDK
//        /*
//#if UNITY_ANDROID && !UNITY_EDITOR
//        AndroidJavaClass installer = new AndroidJavaClass("com.zhou.gamesdk.Global");
//        installer.CallStatic("InstallApp", AndroidBrigde.Singleton.GetCurrentActivity(), path);
//        installer.Dispose();
//#else
//        Application.OpenURL(path);
//#endif
//        */
//    }

//    public bool JoinQQGroup(string key)
//    {
//        //Duong 18/11/23 comment call SDK
//        /*
//#if UNITY_ANDROID && !UNITY_EDITOR
//        AndroidJavaClass installer = new AndroidJavaClass("com.zhou.gamesdk.Global");
//        return installer.CallStatic<bool>("JoinQQGroup", AndroidBrigde.Singleton.GetCurrentActivity(), key);
//#elif UNITY_IOS && !UNITY_EDITOR
//        //if (!string.IsNullOrEmpty(key))
//        //{
//        //    var arr = key.Split('+');
//        //    if(arr.Length >=2)
//        //        return JoinQQGroup(arr[0], arr[1]);
//        //}
//        return false;
//#else
//        var arr = key.Split('+');
        
//        Debuger.Err("暂未实现JoinQQGroup");
//        return false;
//#endif
//        */
//        return false;
//    }

//    public bool SetClipBoard(string text)
//    {
//        //Duong 18/11/23 comment call SDK
//        /*
//#if UNITY_ANDROID && !UNITY_EDITOR
//		SDK_ZPHTemp.CallStatic("setTextToClipboard", text); 
//        return true;
//#elif UNITY_IOS && !UNITY_EDITOR
//        SetClipBoardText(text);
//        return true;
//#else
//        GUIUtility.systemCopyBuffer = text;
//        return true;
//#endif
//        */
//        return false;
//    }

//    public string GetMetaValue(string key, string type = "string")
//    {
//        //Duong 18/11/23 comment call SDK
//        /*
//#if UNITY_ANDROID && !UNITY_EDITOR
//        AndroidJavaClass global = new AndroidJavaClass("com.zhou.gamesdk.Global");
//        var ret = global.CallStatic<string>("GetMetaValue", AndroidBrigde.Singleton.GetCurrentActivity(), key, type);
//        global.Dispose();
//        return ret;
//#else
//        return "";
//#endif
//        */
//        return "";
//    }

//    public string GetStringValue(string key, string type = "string")
//    {
//        //Duong 18/11/23 comment call SDK
//        /*
//#if UNITY_ANDROID && !UNITY_EDITOR
//        AndroidJavaClass global = new AndroidJavaClass("com.zhou.gamesdk.Global");
//        var ret = global.CallStatic<string>("GetStringValue", AndroidBrigde.Singleton.GetCurrentActivity(), key, type);
//        global.Dispose();
//        return ret;
//#else
//        return "";
//#endif
//        */
//        return "";
//    }

//    public bool JumpToAppStore()
//    {
//        //Duong 18/11/23 comment call SDK
//        /*
//#if UNITY_ANDROID && !UNITY_EDITOR
//        AndroidJavaClass jump = new AndroidJavaClass("com.zhou.gamesdk.Global");
//        var success = jump.CallStatic<bool>("JumpToAppStore", AndroidBrigde.Singleton.GetCurrentActivity());
//        jump.Dispose();
//        return success;
//#else
//        return false;
//#endif
//        */
//        return false;
//    }

//    /// <summary>
//    /// 初始化本地推送
//    /// </summary>
//    public void InitLocalNotify(string content)
//    {
//        Debuger.Err("Gihot Skip InitLocalNotify");
////#if UNITY_ANDROID && !UNITY_EDITOR
////        try{
////            AndroidJavaClass notify = new AndroidJavaClass("com.zhou.gamesdk.Notify");
////            notify.CallStatic("Init", AndroidBrigde.Singleton.GetCurrentActivity(), content);
////            notify.Dispose();
////        }catch(Exception e)
////        {
////            Debuger.Err(e.Message, e.StackTrace);
////        }
////#elif UNITY_IOS && !UNITY_EDITOR
////        try{
////            addNotificationIOS(content);
////        }catch(Exception e)
////        {
////            Debuger.Err(e.Message, e.StackTrace);
////        }
////#else
////        Debuger.Err("GameSDK.InitLocalNotify当前平台未实现");
////#endif
//    }

//    /// <summary>
//    /// 磁盘内存不足
//    /// </summary>
//    public bool IsDiskLowMemory()
//    {
//        //Duong 18/11/23 comment call SDK
//        /*
//#if UNITY_ANDROID && !UNITY_EDITOR
//        AndroidJavaClass mem = new AndroidJavaClass("com.zhou.gamesdk.Global");
//        long leftMem = mem.CallStatic<long>("GetAvailbaleMemory", AndroidBrigde.Singleton.GetCurrentActivity());
//        mem.Dispose();
//        return leftMem < 1024 * 1024 * 3;//5M
////#elif UNITY_IOS && !UNITY_EDITOR
//        //todo: Tam thoi luon luon du dung luong, back file hien tai ko su dung nen chua can sua tinh nang nay
//        return false;
//        //return IsIOSDiskLowMemory();
//#else
//        return false;
//#endif
//        */
//        return false;
//    }

//    private AndroidJavaClass net;
//    /// <summary>
//    /// 网络类型
//    /// </summary>
//    public int GetNetworkType()
//    {
//        /*#if UNITY_ANDROID && !UNITY_EDITOR
//                if (net == null)
//                {
//                    net = new AndroidJavaClass("com.zhou.gamesdk.Network");
//                    net.CallStatic("Regist", AndroidBrigde.Singleton.GetCurrentActivity());
//                }
//                int type = net.CallStatic<int>("GetNetInfo", AndroidBrigde.Singleton.GetCurrentActivity());
//                return type;
//#elif UNITY_IOS && !UNITY_EDITOR
//                return int.Parse(GetIOSNetInfo());
//#else
//        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
//                    return 1; //wifi或者有线
//                if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
//                    return 2; //流量
//                return 0;
//#endif*/
//        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
//            return 1; //wifi/有线
//        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
//            return 2; //流量
//        return 0; //无网络
//    }

//    private AndroidJavaClass battery;
//    public float GetBattery()
//    {
//        return SystemInfo.batteryLevel;
//        /*
//        #if UNITY_ANDROID && !UNITY_EDITOR
//                if (battery == null)
//                {
//                    battery = new AndroidJavaClass("com.zhou.gamesdk.Battery");
//                    battery.CallStatic("Regist", AndroidBrigde.Singleton.GetCurrentActivity());
//                }
//                string bat = battery.CallStatic<string>("GetBatteryInfo");
//                return bat;
//        #elif UNITY_IOS && !UNITY_EDITOR
//                return _GetBatteryLevel() + "+100";
//        #else
//                return "100+100";
//        #endif*/
//    }

//    /// <summary>
//    /// 是否是wifi
//    /// </summary>
//    public bool IsWifi()
//    {
//        return GetNetworkType() == 1;
//    }

//    #region IOS 推送
//#if UNITY_IOS && !UNITY_EDITOR
//    private int[] splitStringToIntArray(string src, char sign = '+')
//    {
//        if (string.IsNullOrEmpty(src))
//        {
//            return new int[0];
//        }
//        else
//        {
//            string[] strs = src.Split(sign);
//            int[] ret = new int[strs.Length];
//            for (int i = 0; i < strs.Length; i++)
//            {
//                if (!int.TryParse(strs[i], out ret[i]))
//                {
//                    Debug.LogError("字符串转int出错！" + src);
//                    continue;
//                }
//            }
//            return ret;
//        }
//    }

//    private void addNotificationIOS(string content)
//    {
//        UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();

//        if(string.IsNullOrEmpty(content))
//            return;

//        UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound);
//        DateTime now = DateTime.Now;
//        string[] list = content.Split(';');
//        for(int i=0, len = list.Length; i<len; ++i)
//        {
//            bool triggered = true;
//            string tip = "", days = "";
//            string[] arr = list[i].Split('+');
//            int hour = 0, minute = 0, second = 0;
//            for(int ii = 0, num = arr.Length; ii < num; ++ii)
//            {
//                string[] conf = arr[ii].Split(':');
//                switch(conf[0])
//                {
//                    case "id":
//                        break;
//                    case "time":
//                        string[] ts = conf[1].Split('-');
//                        hour = int.Parse(ts[0]);
//                        minute = int.Parse(ts[1]);
//                        second = int.Parse(ts[2]);
//                        break;
//                    case "title":
//                        break;
//                    case "content":
//                        tip = conf[1];
//                        break;
//                    case "day":
//                        days = conf[1];
//                        break;
//                    case "section":
//                        string[] sec = conf[1].Split('-');
//                        long start = long.Parse(sec[0]);
//                        long end = long.Parse(sec[1]);
//                        triggered = TimeUtils.javaTimeToCSharpTime(start) > now && TimeUtils.javaTimeToCSharpTime(end) < now;
//                        break;
//                    default:
//                        Debuger.Log("推送错误类型");
//                        break;
//                }
//            }
//            if(triggered)
//                sendWeekNotify(hour, minute, second, tip, splitStringToIntArray(days, '-'));
//        }
//    }
    
//    /// <summary>
//    /// 每周的推送，循环的
//    /// </summary>
//    private void sendWeekNotify(int hour, int min, int second, string content, int[] showDays)
//    {
//        if (showDays != null && showDays.Length >= 7)
//        {
//            sendNotify(hour, min, second, content, true);
//            return;
//        }

//        System.DateTime now = System.DateTime.Now;
//        int today = (int)now.DayOfWeek;
//        foreach (int day in showDays)
//        {
//            System.DateTime dt = new DateTime(now.Year, now.Month, now.Day, hour, min, second);
//            int addDay = day - today;
//            dt = dt.AddDays(addDay);
//            if (dt < now)
//                dt = dt.AddDays(7);
            
//            UnityEngine.iOS.LocalNotification localNotification = new UnityEngine.iOS.LocalNotification();
//            localNotification.fireDate = dt;
//            localNotification.alertBody = content;
//            localNotification.applicationIconBadgeNumber = 1;
//            localNotification.hasAction = true;

//            //每周定期循环
//            localNotification.repeatCalendar = UnityEngine.iOS.CalendarIdentifier.ChineseCalendar;
//            localNotification.repeatInterval = UnityEngine.iOS.CalendarUnit.Week;
//            localNotification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
//            UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(localNotification);
//        }
//    }

//    /// 每天推送
//    private void sendNotify(int hour, int min, int second, string content, bool isRepeat = true, int offsetDay = 0)
//    {
//        int year = System.DateTime.Now.Year;
//        int month = System.DateTime.Now.Month;
//        int day = System.DateTime.Now.Day;

//        System.DateTime newDate = new System.DateTime(year, month, day, hour, min, second);

//        if (isRepeat && newDate < System.DateTime.Now)
//            newDate = newDate.AddDays(1);

//        if (offsetDay > 0)
//            newDate = newDate.AddDays(offsetDay);

//        //推送时间需要大于当前时间
//        if (newDate >= System.DateTime.Now)
//        {
//            UnityEngine.iOS.LocalNotification localNotification = new UnityEngine.iOS.LocalNotification();
//            localNotification.fireDate = newDate;
//            localNotification.alertBody = content;
//            localNotification.applicationIconBadgeNumber = 1;
//            localNotification.hasAction = true;

//            //每天定期循环
//            if (isRepeat)
//            {
//                localNotification.repeatCalendar = UnityEngine.iOS.CalendarIdentifier.ChineseCalendar;
//                localNotification.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
//            }

//            localNotification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
//            UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(localNotification);
//        }
//    }
//#endif
//    #endregion
//}