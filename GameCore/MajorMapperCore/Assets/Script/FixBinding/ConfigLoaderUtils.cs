using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Threading;
using System.IO;
using System;
using System.Reflection;

public class ConfigLoaderUtils
{
    public static Type stringType = typeof(System.String);
    public static Type intType = typeof(System.Int32);
    public static Type Int64Type = typeof(System.Int64);
    public static Type DoubleType = typeof(System.Double);
    public static Type SingleType = typeof(System.Single);
    public static Type UInt32Type = typeof(System.UInt32);
    public static Type UInt64Type = typeof(System.UInt64);
    public static Type BooleanType = typeof(System.Boolean);
    private static int currentThreadLoader = 0;
    #region After load config
    public static Thread workerThread;
    public static bool isAfterLoader;
    public static bool isAfterLoaderCompleted;
    public static void StartLoadConfig()
    {
        isAfterLoaderCompleted = false;
        isAfterLoader = true;
        var thread = new Thread(() => LoadAfterConfig());
        thread.Start();
    }
    public static void EndLoadConfig()
    {
        isAfterLoaderCompleted = true;

    }
    static bool isAfterLoading = false;
    static bool isNewRequestLoading = true;
    static string __path;
    static System.Action<object> __rowHandler;
    static string __bundleName;
    static XmlDocument __doc2;
    static Action __onCompleted;
    static string __typeName;
    static Action<List<object>> __rowListHandler;

    private static object lockObj = new object();


    private static void LoadAfterConfig()
    {
        while (!isAfterLoaderCompleted)
        {

            if (isNewRequestLoading)
            {
                isNewRequestLoading = false;
                //Debuger.Log("New request ReadPlainXml");

                ReadPlainXml(__path, __rowHandler, __bundleName, __doc2, __onCompleted, __typeName, __rowListHandler);
            }
            Thread.Sleep(0);
        }
    }
    #endregion
   

    #region support read config
    public static bool ReadPlainXmlThread(string path, System.Action<object> rowHandler, string bundleName, XmlDocument doc, Action onCompleted, string typeName, Action<List<object>> rowListHandler = null)
    {
        var doc2 = GetDocument(path, bundleName);
        if (isAfterLoader)
        {
            //Debuger.Log("New request loader config");
            __path = path;
            __rowHandler = rowHandler;
            __bundleName = bundleName;
            __doc2 = doc2;
            __onCompleted = onCompleted;
            __typeName = typeName;
            __rowListHandler = rowListHandler;
            //
            isNewRequestLoading = true;
        }
        else
        {
            //var thread = new Thread(() => ReadPlainXml(path, rowHandler, bundleName, doc2, onCompleted, typeName, rowListHandler));
            //thread.Start();
            ReadPlainXml(path, rowHandler, bundleName, doc2, onCompleted, typeName, rowListHandler);
        }
        return true;

    }
    private static void ReadPlainXml(string path, System.Action<object> rowHandler, string bundleName, XmlDocument doc, Action onCompleted, string typeName, Action<List<object>> rowListHandler = null)
    {
        var errorCount = 0;
        try { 
            if (doc == null)
            {
                //Debuger.Log("Return Error 1");
                return;

            }

            var table = doc.SelectSingleNode("Table");
            if (table == null)
            {
                //Debuger.Log("Return Error 2");
                return;
            }

            var rows = table.SelectNodes("Row");

            //Duong tạo list asset thay vì invoke
#if Main
            var type = Type.GetType(typeName);
#else
            var type = GameLauncherCore.ilrtApp.GetType(typeName).ReflectionType;
#endif

            List<object> rowResult = new List<object>();
            for (int i = 0; i < rows.Count; i++)
            {
#if Main
                //Phương án này ko hỗ trợ cho PGame, chỉ dùng cho PGameMain + ILruntime
                rowResult.Add(Activator.CreateInstance(type));
#else
                rowResult.Add(GameLauncherCore.ilrtApp.Instantiate(typeName));
#endif
            }

            for (int i = 0; i < rows.Count; i++)
            {
                var xmlNode = XmlNodeUtils.GetIndexXmlNodeList(rows, i);
                var row = xmlNode as XmlElement;
                if (row == null)
                    continue;

                var attribs = row.Attributes;

                for (int j = 0; j < XmlNodeUtils.GetCountXmlNamedNodeMap(attribs); j++)
                {
                    var notArrtribute = XmlNodeUtils.GetIndexXmlAttributeCollection(attribs, j);

                    var attribName = notArrtribute.Name;
                    var attribVal = notArrtribute.Value;

                    FieldInfo field = type.GetField(attribName);
                    if (FieldInfoFix.IsFieldTypeNull(field))
                        continue;

                    GameTools2.SetField(field, rowResult[i], attribVal);
                }

                //rowHandler(rowResult[i]);
            }
            lock (lockObj)
            {
                --currentThreadLoader;
                rowListHandler?.Invoke(rowResult);

                onCompleted?.Invoke();
            }    
  
        }catch (Exception e)
        {
            Debuger.Err("ReadPlainXml error at " + path + " count: "+ (++errorCount) + "  | " + e.Message);
            ReadPlainXml(path, rowHandler, bundleName, doc, onCompleted, typeName, rowListHandler);
        }
    }
   
    public static XmlDocument GetDocument(string path, string bundleName)
    {
        TextAsset data = AssetLoadManager.LoadAsset<TextAsset>(bundleName, path);

        if (data == null)
        {
            Debug.LogErrorFormat("Failed to load config from {0}", path);
            return null;
        }

        XmlDocument doc = new XmlDocument();
        try
        {
            doc.LoadXml(data.text);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Failed xml data: " + data);
            Debug.LogErrorFormat("Failed to read xml from {0}. ({1})", path, ex.Message);
            return null;
        }

        return doc;
    }
#endregion

}
