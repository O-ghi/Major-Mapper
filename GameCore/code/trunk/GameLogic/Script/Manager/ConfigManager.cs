using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Threading;
using System;

public class ConfigManager : ManagerTemplate<ConfigManager>
{

    class ThreadInfo
    {
        public object lockItem;
        public ConfigLoaderBase loader;
        public Thread thread;
        public bool isFinish;
    }

    private static Dictionary<System.Type, ConfigLoaderBase> m_configLoaders;
    private static Dictionary<System.Type, ConfigLoaderBase> m_configLoadersAfter;


    private static float totalnum = 0;
    private static float loadednum = 0;
    private static bool useThread = true;

    public T Get<T>() where T : ConfigLoaderBase
    {
        ConfigLoaderBase loader = null;
        if (m_configLoaders == null)
        {
            m_configLoaders = new Dictionary<System.Type, ConfigLoaderBase>();
        }
        if (!m_configLoaders.TryGetValue(typeof(T), out loader))
        {
            //try getloader after config
            if (!m_configLoadersAfter.TryGetValue(typeof(T), out loader))
            {
                Debuger.Err("Request load config null: " + typeof(T));

                return null;

            }

        }

        return loader as T;
    }
    private List<Type> typeLoaderBasesBefore = new List<Type>()
    {
        typeof(TaskCoreCfgLoader),
        typeof(TaskStepCfgLoader),
        typeof(ItemCfgLoader)
};
    private List<Type> typeLoaderBasesAfter = new List<Type>()
    {

};


    protected override void InitManager()
    {
        for (int i = 0; i < typeLoaderBasesBefore.Count; i++)
        {
            var type = typeLoaderBasesBefore[i];
            if (type.IsSubclassOf(typeof(ConfigLoaderBase)))
            {
                var loader = System.Activator.CreateInstance(type) as ConfigLoaderBase;
                if (loader == null)
                    continue;
                if (m_configLoaders == null)
                {
                    m_configLoaders = new Dictionary<System.Type, ConfigLoaderBase>();
                }
                try
                {
                    m_configLoaders.Add(typeLoaderBasesBefore[i], loader);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }

        //startTick = System.Environment.TickCount;
        if (m_configLoaders == null)
        {
            m_configLoaders = new Dictionary<System.Type, ConfigLoaderBase>();
        }
        totalnum = m_configLoaders.Count;
        configEnumor = m_configLoaders.GetEnumerator();

        //todo: Tam thoi pending dung multi thread de resolved ilruntime
        useThread = false;
        // tạm ẩn mấy cái liên quan đến thread
        ConfigLoaderBase.MAX_THREAD = SystemInfo.processorCount;
        //ConfigLoaderBase.MAX_THREAD = 1;
        /*#if Main
                useThread = (SystemInfo.processorCount >= 3);

                if (useThread)
                {

                    threadCount = SystemInfo.processorCount - 1;


                    loaderArray = new ThreadInfo[threadCount];

                    for (int i = 0; i < threadCount; i++)
                    {
                        ThreadInfo info = new ThreadInfo();
                        info.loader = GetLoader();
                        //Debuger.Log("info.loader " + info.loader.GetType().ToString());
                        info.lockItem = new object();
                        info.thread = new Thread(LoadConfigAsync);
                        info.isFinish = false;

                        loaderArray[i] = info;
                        info.thread.Start(info);
                    }
                }
        #endif*/
        startTime = Time.realtimeSinceStartup;
        Debuger.Log("Start Load Config " + startTime);
        ConfigLoaderBase.StartUpdateLoadConfig();
        StartCoroutine(LoadConfig());
    }
    private static float startTime;
    public static bool isDone = false;
    public static bool isAfterLoadDone = false;

    public static float progress
    {
        get
        {
            //Duong co 3 file cung chung 1 config
            //Debuger.Log((ConfigLoaderBase.loaderConfigCount +ConfigLoaderBase.loaderConfigMainThreadCount) +"/"+ (totalnum));
            // tạm ẩn mấy cái liên quan đến thread
            float curProgress = (ConfigLoaderBase.loaderConfigCount /*+ConfigLoaderBase.loaderConfigMainThreadCount*/) / (totalnum + 17);
            return curProgress > 1 ? 1 : curProgress;
        }
    }

    private Dictionary<System.Type, ConfigLoaderBase>.Enumerator configEnumor;
    //private int startTick = 0;
    private int threadCount = 3;
    private ThreadInfo[] loaderArray = null;
    private ConfigLoaderBase languageLoader = null;
    private ConfigLoaderBase skillActionLoader = null;

    private ConfigLoaderBase GetLoader()
    {
        if (!configEnumor.MoveNext())
        {
            AssetLoadManager.UnLoadAssetBundle("configbin.bundle");
            return null;
        }

        var loader = configEnumor.Current.Value;
        loader.Load();
        loadednum++;
        return loader;
    }

    private void LoadConfigAsync(object arg)
    {
        ThreadInfo info = (ThreadInfo)arg;

        while (true)
        {
            ConfigLoaderBase loader;
            bool isFinish = false;

            lock (info.lockItem)
            {
                loader = info.loader;
                isFinish = info.isFinish;
                info.loader = null;
            }

            if (isFinish)
                return;

            if (null == loader)
            {
                Thread.Sleep(0);
                continue;
            }

            try
            {
                loader.DoUnpacker();
            }
            catch
            {
                Debug.LogError("Error DoUnpacker");
            }
        }
    }


    IEnumerator LoadConfig()
    {
        if (!useThread)
        {
            ConfigLoaderBase loader = null;
            while ((loader = GetLoader()) != null)
            {
                //loader.DoUnpacker();
                yield return null;
            }

            while (ConfigLoaderBase.loaderConfigCount < ConfigLoaderBase.loaderConfigSize)
            {
                yield return null;
            }
            //#endif
            yield return null;
            yield return null;
            Debuger.Log("Done loader config size: " + ConfigLoaderBase.loaderConfigSize);
            Debuger.Log("Done Load Config " + (Time.realtimeSinceStartup - startTime));

            ConfigLoaderBase.complete = true;
            isDone = true;

            //Start load next config
            StartCoroutine(LoadConfigAfter());

        }
        else
        {
            bool hasFinish = false;
            while (true)
            {
                hasFinish = false;
                for (int i = 0; i < threadCount; i++)
                {
                    var info = loaderArray[i];
                    lock (info.lockItem)
                    {
                        if (null != info.loader)
                            continue;

                        info.loader = GetLoader();
                        if (null == info.loader)
                        {
                            info.isFinish = true;
                            hasFinish = true;
                        }
                    }
                }

                if (hasFinish)
                {
                    for (int i = 0; i < threadCount; i++)
                    {
                        var info = loaderArray[i];
                        lock (info.lockItem)
                        {
                            if (!info.isFinish)
                                goto WAIT_FOR_NEXT;
                        }
                    }

                    languageLoader.Load();
                    languageLoader.DoUnpacker();
                    loadednum++;
                    skillActionLoader.Load();
                    skillActionLoader.DoUnpacker();
                    loadednum++;


                    isDone = true;
                    //int delta = System.Environment.TickCount - startTick;
                    //Debug.LogErrorFormat("load config:{0}", delta);
                    yield break;
                }

            WAIT_FOR_NEXT:
                yield return null;
            }
        }
    }

    IEnumerator LoadConfigAfter()
    {
        yield return null;
        yield return null;
        yield return null;
        //ConfigLoaderUtils.StartLoadConfig();
        //new setting
        //ConfigLoaderBase.MAX_THREAD = 1;
        ConfigLoaderBase.loaderConfigCount = 0;
        ConfigLoaderBase.loaderConfigSize = 0;

        ConfigLoaderBase.complete = false;
        ConfigLoaderBase.StartUpdateLoadConfig();
        startTime = Time.realtimeSinceStartup;
        //
        for (int i = 0; i < typeLoaderBasesAfter.Count; i++)
        {
            var type = typeLoaderBasesAfter[i];
            if (type.IsSubclassOf(typeof(ConfigLoaderBase)))
            {
                var loader = System.Activator.CreateInstance(type) as ConfigLoaderBase;
                if (loader == null)
                    continue;
                if (m_configLoadersAfter == null)
                {
                    m_configLoadersAfter = new Dictionary<System.Type, ConfigLoaderBase>();
                }
                try
                {
                    m_configLoadersAfter.Add(typeLoaderBasesAfter[i], loader);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }

        //startTick = System.Environment.TickCount;
        if (m_configLoadersAfter == null)
        {
            m_configLoadersAfter = new Dictionary<System.Type, ConfigLoaderBase>();
        }
        totalnum = m_configLoadersAfter.Count;
        configEnumor = m_configLoadersAfter.GetEnumerator();


        Debuger.Log("LoadConfigAfter configEnumor size: " + m_configLoadersAfter.Count);

        if (!useThread)
        {
            ConfigLoaderBase loader = null;
            while ((loader = GetLoader()) != null)
            {
                //loader.DoUnpacker();
                yield return null;
            }
            while (ConfigLoaderBase.loaderConfigCount < ConfigLoaderBase.loaderConfigSize)
            {
                //Debuger.Log("ConfigLoaderBase loaderConfigCount: " + ConfigLoaderBase.loaderConfigCount
                //        + "  |||   loaderConfigSize: "+ ConfigLoaderBase.loaderConfigSize);

                yield return null;
            }
            //#endif
            yield return null;
            yield return null;
            Debuger.Log("After Done loader config size: " + ConfigLoaderBase.loaderConfigSize);
            Debuger.Log("After Done Load Config " + (Time.realtimeSinceStartup - startTime));

            //ConfigLoaderBase.complete = true;
            //isDone = true;
            isAfterLoadDone = true;
            //ConfigLoaderUtils.EndLoadConfig();

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
}

public enum RuleListType
{
    SCENE_RULE = 0,
    SKILL_MAIN,
    SKILL_ACTION,
    SKILL_SPHERE,
    OTHERS_RULE
}