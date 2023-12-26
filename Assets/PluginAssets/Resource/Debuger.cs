//------------------------------------------------------------------------------
// <auto-generated>
//	@Author Zhou XiQuan
//  @Time 2016-01-22
// 一般来说只需要调用Debuger.SetTrigger(...)来设置触发密码就可以完成初始化了
// 也可以不初始化因为有默认的触发密码
// 推荐注册Application.RegisterLogCallback( Debuger.OnSystemLog );方便查看更多错误信息
// </auto-generated>
//------------------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class Debuger : MonoBehaviour
{
    private class DebugInfo
    {
        internal DebugInfo(Color color, string log)
        {
            msg = log;
            style = new GUIStyle();
            style.fontSize = 18;
            style.wordWrap = true;
            style.normal.textColor = color;
        }
        internal string msg;
        internal GUIStyle style;
    }

    private static Debuger instance;
    private static bool debugWorking = false;
    private static void createInstance()
    {
        if (!Enabled) return;

        if (instance == null)
        {
            if (!Application.isPlaying)
            {
                logToConsole = true;
                return;
            }

            getShouldDebug();
            GameObject obj = new GameObject("ZUI_Debuger");
            GameObject.DontDestroyOnLoad(obj);
            instance = obj.AddComponent<Debuger>();
            instance.screenRect = new Rect(0, 0, Screen.width, Screen.height);
            int full = Mathf.Min(Screen.height, Screen.width);
            instance.windRect = new Rect(10, 10, full / 1.1f, full / 1.1f);
            //默认正3反3
            instance.triggerArr = "1,2,3,4,1,2,3,4,1,2,3,4,3,2,1,4,3,2,1,4,3,2,1".Split(',');
#if UNITY_EDITOR
            logToConsole = true;
#else
			logToConsole = true;
#endif
        }
    }
    private const int saveLogCout = 0908;
    private static bool logLock = false;
    private static bool logToGUI = false;
    private static bool logToConsole = false;

    private static bool able = true;
    /// <summary>
    /// 总开关
    /// </summary>
    public static bool Enabled
    {
        get
        {
            return able;
        }
        set
        {
            able = value;
            if (!value)
            {
                if (instance != null)
                    GameObject.Destroy(instance.gameObject);
            }
            else
            {
                createInstance();
            }
        }
    }

    private static bool isLastLogSame(string log)
    {
        if (toLogQueue.Count > 0)
            return toLogQueue.Peek().Key == log;
        if (logList.Count > 0)
            return logList[logList.Count - 1].msg == log;
        return false;
    }

    public static void Log(params object[] msg)
    {
        //#if !UNITY_EDITOR
        //        if(false == able)
        //            return;
        //        if(false == debugWorking)
        //            return;
        //#endif

        createInstance();
        logLock = true;
        string log = "";
        for (int i = 0; i < msg.Length; ++i)
        {
            log += msg[i] == null ? "Null" : msg[i].ToString();
            if (i != msg.Length - 1) log += ",";
        }

        if (logToConsole)
            UnityEngine.Debug.Log(log);
        if (!isLastLogSame(log))
            toLogQueue.Enqueue(new KeyValuePair<string, Color>(log, colorLog));

        logLock = false;
    }

    public static void Wrn(params object[] msg)
    {
#if !UNITY_EDITOR
        if (false == able)
            return;
        if (false == debugWorking)
            return;
#endif

        createInstance();
        logLock = true;
        string log = "";
        for (int i = 0; i < msg.Length; ++i)
        {
            log += msg[i] == null ? "Null" : msg[i].ToString();
            if (i != msg.Length - 1) log += ",";
        }

        if (logToConsole)
            UnityEngine.Debug.LogWarning(log);
        if (!isLastLogSame(log))
            toLogQueue.Enqueue(new KeyValuePair<string, Color>(log, colorWrn));

        logLock = false;
    }

    public static void Err(params object[] msg)
    {
        string log = "";
        for (int i = 0; i < msg.Length; ++i)
        {
            log += msg[i] == null ? "Null" : msg[i].ToString();
            if (i != msg.Length - 1) log += ",";
        }
#if !UNITY_EDITOR
        if(!able || !debugWorking)
        {
            //Err始终打印
            UnityEngine.Debug.LogError(log);
            return;
        }
#endif

        createInstance();
        logLock = true;

        if (logToConsole)
            UnityEngine.Debug.LogError(log);

        if (!isLastLogSame(log))
            toLogQueue.Enqueue(new KeyValuePair<string, Color>(log, colorErr));

        logLock = false;
    }

    /// <summary>
    /// 系统日志接口
    /// </summary>
    public static void OnSystemLog(string logString, string stackTrace, LogType type)
    {
        //当前正在打印函数则返回，说明这里是从打印函数中过来的
        if (logLock) return;

        ///系统日志会自动打印，不需要再次打印
        bool lastConsole = logToConsole;
        logToConsole = false;

        switch (type)
        {
            case LogType.Log:
                Log(logString);
                break;
            case LogType.Warning:
                Wrn(logString);
                break;
            default:
                Err(logString);
                if (logString.Contains("Exception"))
                    Err(stackTrace);
                break;
        }

        logToConsole = lastConsole;
    }

    private static Queue<KeyValuePair<string, Color>> toLogQueue = new Queue<KeyValuePair<string, Color>>();
    private static List<DebugInfo> logList = new List<DebugInfo>();
    private static Color colorLog = Color.white;
    private static Color colorWrn = Color.yellow;
    private static Color colorErr = Color.red;

    private Rect windRect;
    private Rect screenRect;
    private bool isOpen;
    private Vector2 scrollPos;
    private float offset = 10;

    private bool newBarStyle = false;
    void OnGUI()
    {
        if (!Enabled)
            return;
        if (!logToGUI)
            return;

        if (isOpen)
        {
            if (!newBarStyle)
            {
                newBarStyle = true;
                //重写滚动条大小
                var style = new GUIStyle(GUI.skin.verticalScrollbar);
                style.fixedWidth = 50;
                style.stretchWidth = true;
                GUI.skin.verticalScrollbar = style;
                style = new GUIStyle(GUI.skin.verticalScrollbarThumb);
                style.fixedWidth = 48;
                style.stretchWidth = true;
                style.alignment = TextAnchor.MiddleCenter;
                GUI.skin.verticalScrollbarThumb = style;
            }

            while (toLogQueue.Count > 0)
            {
                var log = toLogQueue.Dequeue();
                logList.Add(new DebugInfo(log.Value, log.Key));
            }
            if (logList.Count > saveLogCout)
                logList.RemoveAt(0);

            windRect = GUI.Window(0, windRect, debugWindow, string.Format("DebugWindow(fps:{0})", frameStr));
        }
        else
        {
            if (GUILayout.Button("Open", GUILayout.MinWidth(100), GUILayout.MinHeight(60)))
            {
                isOpen = true;
                scrollPos.y = int.MaxValue;
            }
        }
    }

    private void debugWindow(int id)
    {
        GUILayout.BeginArea(new Rect(offset, offset, windRect.width - offset * 2, windRect.height - offset * 2));
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Close", GUILayout.MinWidth(60), GUILayout.MinHeight(60)))
            isOpen = false;
        GUILayout.Space(5);
        if (GUILayout.Button("Clear", GUILayout.MinWidth(60), GUILayout.MinHeight(60)))
            logList.Clear();
        GUILayout.Space(5);
        if (GUILayout.Button("Hide", GUILayout.MinWidth(60), GUILayout.MinHeight(60)))
            logToGUI = false;
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(windRect.width - offset * 2), GUILayout.Width(windRect.height - offset * 2));

        for (int i = 0, len = logList.Count; i < len; ++i)
            GUILayout.Label(logList[i].msg, logList[i].style);
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save", GUILayout.MaxWidth(80), GUILayout.MinHeight(60)))
            saveLogToDisk();
        GUILayout.Space(50);
        if (GUILayout.Button("Up", GUILayout.MinWidth(60), GUILayout.MinHeight(60)))
            scrollPos.y -= windRect.height * 0.7f;
        if (GUILayout.Button("Down", GUILayout.MinWidth(60), GUILayout.MinHeight(60)))
            scrollPos.y += windRect.height * 0.7f;
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
        GUI.DragWindow(screenRect);

    }

    private void saveLogToDisk()
    {
        string log = Application.identifier + "\n";
        log += System.DateTime.Now.ToString() + "\n\n";
        for (int i = 0, len = logList.Count; i < len; ++i)
        {
            string tag = "log:";
            if (logList[i].style.normal.textColor == colorErr)
                tag = "error:";
            else if (logList[i].style.normal.textColor == colorWrn)
                tag = "warning:";
            log += tag + ">" + logList[i].msg + "\n";
        }
        string path = Application.dataPath + "/log" + System.DateTime.Now.Ticks + ".txt";
        if (Application.isMobilePlatform)
            path = Application.persistentDataPath + "/log" + System.DateTime.Now.Ticks + ".txt";
        System.IO.File.WriteAllText(path, log);
        Log("日志已保存到>" + path);
    }
    private List<string> triggerNow = new List<string>();
    /// <summary>
    /// 打开秘籍默认正3圈反3圈
    /// 密码中只能有1234四个数字，分别对应数学中的四个象限
    /// </summary>
    private string[] triggerArr;

    /// <summary>
    /// 设置触发密码
    /// </summary>
    public static void SetTrigger(string[] trigger)
    {
        if (!Enabled) return;
        createInstance();
        if (trigger != null && trigger.Length > 0)
            instance.triggerArr = trigger;
    }

    float updateInterval = 0.3f;
    private float accum = 0.0f;
    private float frames = 0;
    private float timeleft;
    private string frameStr = "";

    private Dictionary<string, System.Action<string>> triggerMap = new Dictionary<string, System.Action<string>>();
    public static void AddTrigger(string trigger, System.Action<string> callback)
    {
        if (!Enabled) return;
        createInstance();

        if (instance.triggerMap.ContainsKey(trigger))
            instance.triggerMap[trigger] += callback;
        else
            instance.triggerMap.Add(trigger, callback);
    }

    /// <summary>
    /// 触发秘籍
    /// </summary>
    private void Update()
    {
        if (logToGUI)
        {
            //帧率检测
            timeleft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            ++frames;

            if (timeleft <= 0.0)
            {
                frameStr = (accum / frames).ToString("f2");
                timeleft += updateInterval;
                accum = 0.0f;
                frames = 0;
            }
        }

        if (Input.touchSupported)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended)
                    checkTriggerShowGUI();
                else if (touch.phase == TouchPhase.Moved)
                    addXiangXian(touch.position);
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
                checkTriggerShowGUI();
            else if (Input.GetMouseButton(0))
                addXiangXian(Input.mousePosition);
        }
    }

    private void addXiangXian(Vector2 pos)
    {
        //当前触摸在第几象限
        string xiangxian = "0";
        if (pos.y > Screen.height / 2)
        {
            if (pos.x > Screen.width / 2)
                xiangxian = "1";	//第1象限
            else
                xiangxian = "2";	//第2象限
        }
        else
        {
            if (pos.x > Screen.width / 2)
                xiangxian = "4";	//第4象限
            else
                xiangxian = "3";	//第3象限
        }

        if (triggerNow.Count == 0 || triggerNow[triggerNow.Count - 1] != xiangxian)
            triggerNow.Add(xiangxian);
    }

    private void checkTriggerShowGUI()
    {
        if (logToGUI)
        {
            string now = string.Join(",", triggerNow.ToArray());
            if (triggerMap.ContainsKey(now))
                triggerMap[now](now);
            triggerNow.Clear();
        }
        else
        {
            if (triggerArr.Length != triggerNow.Count)
            {
                triggerNow.Clear();
                return;
            }

            for (int i = 0; i < triggerArr.Length; ++i)
            {
                if (triggerArr[i] != triggerNow[i])
                {
                    triggerNow.Clear();
                    return;
                }
            }
            triggerNow.Clear();
            logToConsole = true;
            logToGUI = true;
            setShouldDebug();
        }
    }

    private void setShouldDebug()
    {
        PlayerPrefs.SetInt("Debuger_doDebug", 1);
        logToConsole = true;
        debugWorking = true;
    }

    public static void SetWorking()
    {
        createInstance();
        if (instance != null)
            instance.setShouldDebug();
    }

    public static bool IsWorking()
    {
        return debugWorking;
    }

    private static void getShouldDebug()
    {
#if UNITY_EDITOR
        debugWorking = true;
#else
        int value = PlayerPrefs.GetInt("Debuger_doDebug", -1);
        debugWorking = value > 0;
#endif
    }

    void OnDestroy()
    {
        //停止游戏时，其它脚本调用log可能清除不掉instance的GameObject
        //需要关掉log
        if (able && instance == this)
            able = false;
    }
}
