using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using System.Linq;
using System;


public abstract class ConfigLoaderBase
{
    public bool IsLoaded
    {
        get { return m_isLoaded; }
    }
    private bool m_isLoaded = false;

    public void Load()
    {
        if (IsLoaded)
            return;

        OnLoad();
    }

    public void Unload(bool force = false)
    {
        if (!force && !IsLoaded)
            return;

        OnUnload();
    }

    protected abstract void OnLoad();

    protected abstract void OnUnload();
    public static int loaderConfigSize;
    public static int loaderConfigCount;
    //public static int loaderConfigMainThreadCount;
    ////Duong: xu ly da luong 
    public static int MAX_THREAD;
    public static int currentThread;
    private static Queue<Action> readConfigWaits = new Queue<Action>();
    private object lockObj = new object();
    public static void registerReadThread(Action action, string path)
    {

        loaderConfigSize++;

        lock (readConfigWaits)
        {
            readConfigWaits.Enqueue(action);
            if (!readConfigDics.ContainsKey(path))
            {
                readConfigDics.Add(path, false);
            }
        }
    }
    public static Dictionary<string, bool> readConfigDics = new Dictionary<string, bool>();
    public static void completedReadThread(string path)
    {
        loaderConfigCount++;
        // tạm ẩn mấy cái thread
        currentThread--;
        lock (readConfigDics)
        {
            if (readConfigDics.ContainsKey(path))
            {
                readConfigDics[path] = true;
            }
        }
    }
    public static void StartUpdateLoadConfig()
    {
        CoroutineManager.Singleton.startCoroutine(doStartLoadConfig());
    }
    public static bool complete = false;
    private static IEnumerator doStartLoadConfig()
    {
        while (true)
        {
            //Debuger.Log("currentThread " + currentThread + " ||| MAX_THREAD: " + MAX_THREAD);
            if (currentThread < MAX_THREAD)
            {
                {
                    Action action = null;
                    if (readConfigWaits.Count > 0)
                    {
                        // tạm ẩn mấy cái thread
                        ++currentThread;
                        Debuger.Log("pick ation to list " + readConfigWaits.Count);
                        lock (readConfigWaits)
                        {
                            action = readConfigWaits.Dequeue();
                        }
                    }
                    action?.Invoke();
                }
            }
            //Action action = null;
            //if (readConfigWaits.Count > 0)
            //{
            //	// tạm ẩn mấy cái thread
            //	//++currentThread;
            //	//Debuger.Log("pick ation to list " + readConfigWaits.Count);
            //	lock (readConfigWaits)
            //	{
            //		action = readConfigWaits.Dequeue();
            //	}
            //}
            //action?.Invoke();
            if (complete)
            {
                Debuger.Log("_________________________Done load config");
                yield break;
            }
            //Debug.Log("________________numberload| ");
            yield return null;
        }

    }
    //private static object rowHandlerobject = new object();
    //private static object rowHandlerobject2 = new object();
    //private static object objectLock = new object();

    public bool ReadConfig<T>(string path, System.Action<T> rowHandler, string bundleName = "config", bool immediately = false, Action onCompleted = null)
    {
        //#if !Main
        //var doc = ConfigManager.GetDocument(path, bundleName);
        if (immediately)
        {
            return ReadPlainXml<T>(path, rowHandler, bundleName, true);
        }
        else
        {
            var type = typeof(T).Name;
            //var doc2 = ConfigManager.GetDocument(path, bundleName);
            Debug.Log("___________________________ReadConfig| " + type);

            Action action = (() =>
            ConfigLoaderUtils.ReadPlainXmlThread(path,
            (row) =>
            {
                //lock (rowHandlerobject)
                //{
                //	rowHandler?.Invoke((T)row);
                //}
            }
            , bundleName, null, () => {
                completedReadThread(path);
                onCompleted?.Invoke();
            }, type,
            (list) => {
                if (list != null)
                {
                    //Debuger.Log("Callback list size = " + list.Count);

                    for (int i = 0; i < list.Count; i++)
                    {
                        rowHandler?.Invoke((T)list[i]);
                    }
                }
            })
            );

            registerReadThread(action, path);

            return true;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Path"></param>
    /// <param name="rowHandler"></param>
    /// <param name="bundleName"></param>
    /// <param name="IsPlainXml">是否还是名文读取</param>
    private static bool isLoad;
 

    public bool ReadRecConfig(string path, System.Action<string, List<List<VariantLogic>>> handler, string bundleName = "config.bundle")
    {
        //XmlDocument doc = ConfigManager.GetDocument(path, bundleName);

        Action action = (() =>
        ReadRecPlainXmlThread(path, handler, bundleName, null,
             () => {
                 completedReadThread(path);
             }, (rowDic) => {
                 var keyList = rowDic.Keys.ToList();
                 for (int i = 0; i < rowDic.Count; i++)
                 {
                     handler.Invoke(keyList[i], rowDic[keyList[i]]);
                 }
             })
        );

        registerReadThread(action, path);
        return true;
    }

    public static bool ReadRecPlainXmlThread(string path, System.Action<string, List<List<VariantLogic>>> handler, string bundleName, XmlDocument doc, Action onCompleted, Action<Dictionary<string, List<List<VariantLogic>>>> rowListHandler = null)
    {
        var doc2 = ConfigManager.GetDocument(path, bundleName);

        //var thread = new Thread(() => ReadRecPlainXml(path, handler, bundleName, doc2, onCompleted, rowListHandler));
        //thread.Start();
        ReadRecPlainXml(path, handler, bundleName, doc2, onCompleted, rowListHandler);
        return true;
    }

    private static void ReadRecPlainXml(string path, System.Action<string, List<List<VariantLogic>>> handler, string bundleName, XmlDocument doc, Action onCompleted, Action<Dictionary<string, List<List<VariantLogic>>>> rowListHandler = null)
    {
        var errorCount = 0;
        try
        {
            if (doc == null)
                return;

            var table = doc.SelectSingleNode("Table");
            if (table == null)
                return;

            var rows = table.SelectNodes("Row");
            Dictionary<string, List<List<VariantLogic>>> rowResult = new Dictionary<string, List<List<VariantLogic>>>();

            for (int i = 0; i < rows.Count; i++)
            {
                var xmlNode = XmlNodeUtils.GetIndexXmlNodeList(rows, i);

                var row = xmlNode as XmlElement;
                if (row == null)
                    continue;

                string key = row.GetAttribute("ID");
                if (string.IsNullOrEmpty(key))
                    continue;

                List<List<VariantLogic>> record = new List<List<VariantLogic>>();

                int j = 0;
                while (true)
                {
                    var recRowString = row.GetAttribute(string.Format("R{0}", j.ToString()));
                    if (string.IsNullOrEmpty(recRowString))
                    {
                        break;
                    }

                    string[] recRowElems = recRowString.Split(',');

                    List<VariantLogic> recRow = new List<VariantLogic>(recRowElems.Length);

                    for (int k = 0; k < recRowElems.Length; k++)
                    {
                        recRow.Add(VariantLogic.MakeForString(recRowElems[k]));
                    }

                    record.Add(recRow);

                    j++;
                }

                //handler(key, record);
                rowResult.Add(key, record);
            }
            rowListHandler?.Invoke(rowResult);
            onCompleted?.Invoke();

        }
        catch
        {
            Debuger.Err("ReadRecPlainXml error at " + path + " count: " + (++errorCount));
            ReadRecPlainXml(path, handler, bundleName, doc, onCompleted, rowListHandler);
        }
    }


    public bool ReadNewActivityRowFromDoc<T>(XmlDocument doc, System.Action<T> rowHandler)
    {
        var type = typeof(T);

        if (doc == null)
            return false;

        var table = doc.SelectSingleNode("Table");
        if (table == null)
            return false;

        var rows = table.SelectNodes("Row");

        for (int i = 0; i < XmlNodeUtils.GetCountXmlNodeList(rows); i++)
        {
            var xmlNode = XmlNodeUtils.GetIndexXmlNodeList(rows, i);
            var row = xmlNode as XmlElement;
            if (row == null)
                continue;

            T rowInstance = (T)System.Activator.CreateInstance(typeof(T));
            if (rowInstance == null)
                return false;

            var attribs = row.Attributes;

            for (int j = 0; j < XmlNodeUtils.GetCountXmlNamedNodeMap(attribs); j++)
            {
                var notArrtribute = XmlNodeUtils.GetIndexXmlAttributeCollection(attribs, j);
                var attribName = notArrtribute.Name;
                var attribVal = notArrtribute.Value;

                FieldInfo field = type.GetField(attribName);
                if (FieldInfoFix.IsFieldTypeNull(field))
                    continue;

                GameTools2.SetField(field, rowInstance, attribVal);
            }

            rowHandler(rowInstance);
        }

        return true;
    }


    // 编辑器模式下采用明文 XML 读取
    // 发包时需要把XML转成二进制读取
#if !Main
    [ILRuntime.Runtime.ILRuntimeJIT(ILRuntime.Runtime.ILRuntimeJITFlags.JITImmediately)]
#endif
    private bool ReadPlainXml<T>(string path, System.Action<T> rowHandler, string bundleName, bool immediately)
    {
        if (actionList == null)
        {
            actionList = new List<Action>();
        }
        //const string XmlConfigBundle = "config.bundle";
        //const string XmlConfigAssetPrefix = "Assets/Config/";

        //path = string.Format("{0}{1}", XmlConfigAssetPrefix, path);
        //TextAsset data = ResourceManager.LoadAsset<TextAsset>(path);

        var type = typeof(T);

        XmlDocument doc = ConfigManager.GetDocument(path, bundleName);

        Action action = (() =>
        {
            if (doc == null)
                return;

            var table = doc.SelectSingleNode("Table");
            if (table == null)
                return;

            var rows = table.SelectNodes("Row");

            for (int i = 0; i < XmlNodeUtils.GetCountXmlNodeList(rows); i++)
            {
                var xmlNode = XmlNodeUtils.GetIndexXmlNodeList(rows, i);
                var row = xmlNode as XmlElement;
                if (row == null)
                    continue;

                T rowInstance = (T)System.Activator.CreateInstance(typeof(T));
                if (rowInstance == null)
                    return;

                var attribs = row.Attributes;

                for (int j = 0; j < XmlNodeUtils.GetCountXmlNamedNodeMap(attribs); j++)
                {
                    var notArrtribute = XmlNodeUtils.GetIndexXmlAttributeCollection(attribs, j);

                    var attribName = notArrtribute.Name;
                    var attribVal = notArrtribute.Value;

                    FieldInfo field = type.GetField(attribName);
                    if (FieldInfoFix.IsFieldTypeNull(field))
                        continue;

                    GameTools2.SetField(field, rowInstance, attribVal);
                    //Debuger.Log("Set field OK type: " + FieldInfoFix.FieldType(field).ToString());
                }

                rowHandler(rowInstance);
            }
        });

        action();
        // tạm ẩn mấy cái thread
        //loaderConfigMainThreadCount++;

        return true;
    }

    private bool ReadRecPlainXml(string path, System.Action<string, List<List<VariantLogic>>> handler, string bundleName)
    {
        if (actionList == null)
        {
            actionList = new List<Action>();
        }
        XmlDocument doc = ConfigManager.GetDocument(path, bundleName);

        Action action = (() =>
        {
            if (doc == null)
                return;

            var table = doc.SelectSingleNode("Table");
            if (table == null)
                return;

            var rows = table.SelectNodes("Row");

            for (int i = 0; i < XmlNodeUtils.GetCountXmlNodeList(rows); i++)
            {
                var xmlNode = XmlNodeUtils.GetIndexXmlNodeList(rows, i);

                var row = xmlNode as XmlElement;
                if (row == null)
                    continue;

                string key = row.GetAttribute("ID");
                if (string.IsNullOrEmpty(key))
                    continue;

                List<List<VariantLogic>> record = new List<List<VariantLogic>>();

                int j = 0;
                while (true)
                {
                    var recRowString = row.GetAttribute(string.Format("R{0}", j.ToString()));
                    if (string.IsNullOrEmpty(recRowString))
                    {
                        break;
                    }

                    string[] recRowElems = recRowString.Split(',');

                    List<VariantLogic> recRow = new List<VariantLogic>(recRowElems.Length);

                    for (int k = 0; k < recRowElems.Length; k++)
                    {
                        recRow.Add(VariantLogic.MakeForString(recRowElems[k]));
                    }

                    record.Add(recRow);

                    j++;
                }

                handler(key, record);
            }
        });

        actionList.Add(action);
        return true;
    }

    //protected static Dictionary<string, FieldInfo> fieldTypeDic = new Dictionary<string, FieldInfo>();

    List<Action> actionList = new List<Action>();

    public void DoUnpacker()
    {

        if (actionList == null)
        {
            return;
        }
        //var actionList = actionList;
        //actionList.Clear();
        if (actionList.Count > 0)
        {
            //long begin = System.Environment.TickCount;
            for (int i = 0; i < actionList.Count; i++)
            {
                if (actionList[i] != null)
                {
                    actionList[i].Invoke();
                }
            }

            actionList.Clear();
            //long end = System.Environment.TickCount;
            //Debug.LogFormat("load {0:0.00}\t{1}\t{3}:\t{2}", ConfigManager.progress, end - begin, GetType().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
        }
    }

    private void orderbyItem(ref List<MemberInfo> arr1)
    {
        for (int i = 0; i < arr1.Count; i++)
        {
            for (int j = i + 1; j < arr1.Count; j++)
            {
                if (arr1[j].Name.CompareTo(arr1[i].Name) < 0)
                {
                    //cach trao doi gia tri
                    var tmp = arr1[i];
                    arr1[i] = arr1[j];
                    arr1[j] = tmp;
                }
            }
        }
    }


}
