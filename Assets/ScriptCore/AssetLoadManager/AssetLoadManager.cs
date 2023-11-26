using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class AssetBundleLoadTask
{
    public string assetBundleName;
    public string[] dependencies;
    public AssetBundleCreateRequest request;
    public int referencedCount;

    public AssetBundleLoadTask(string assetBundleName, string[] dependencies)
    {
        this.assetBundleName = assetBundleName;
        this.dependencies = dependencies;
        this.referencedCount = 1;
    }
}

public class LoadedAssetBundle
{
    public AssetBundle assetBundle;
    public int referencedCount;

    public LoadedAssetBundle(AssetBundle assetBundle)
    {
        this.assetBundle = assetBundle;
        this.referencedCount = 1;
    }
}

public class AssetLoadManager : MonoBehaviour
{
    static AssetBundleManifest m_AssetBundleManifest = null;
#if Main
    static bool m_SimulateAssetBundleInEditor = true;
#else
    static bool m_SimulateAssetBundleInEditor = false;
#endif

    static Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles;
    static Dictionary<string, AssetBundleLoadTask> m_LoadAssetBundleTasks;
    static Dictionary<string, string> m_DownloadingErrors;
    static List<AssetLoadOperation> m_InProgressOperations;
    static Dictionary<string, string[]> m_Dependencies;

    static List<string> m_PresaveAssetBundleNames;
    static List<string> m_keysToRemove;

    static float m_gcTimer = 0;
    const float GC_CHECK_TIME = 90;


    public static bool SimulateAssetBundleInEditor
    {
        get
        {
#if !UNITY_EDITOR
            m_SimulateAssetBundleInEditor = true;
#endif
            return m_SimulateAssetBundleInEditor;
        }
        set
        {
            m_SimulateAssetBundleInEditor = value;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        InitManager();
    }
    private void InitManager()
    {
        m_PresaveAssetBundleNames = new List<string>();
        m_keysToRemove = new List<string>();



        m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
        m_LoadAssetBundleTasks = new Dictionary<string, AssetBundleLoadTask>();
        m_DownloadingErrors = new Dictionary<string, string>();
        m_InProgressOperations = new List<AssetLoadOperation>();
        m_Dependencies = new Dictionary<string, string[]>();

        CryptTools.Init();

#if UNITY_EDITOR
        if (SimulateAssetBundleInEditor)
        {

        }
        else
#endif
        {
            if (GameInfoManager.IsSmallClientCache)
            {
                InitRecordAssetData();
            }

            //Utility.GetAssetBundleDiskPath(Utility.PlatformName + ".manifest");

            m_AssetBundleManifest = LoadAsset(Utility.PlatformName, "AssetBundleManifest", typeof(AssetBundleManifest)) as AssetBundleManifest;

            #region 小包专用代码
            if (GameInfoManager.IsSmallClientCache)
            {
#if UNITY_EDITOR
                if (isRecording == 1)
                {
                    int count = m_recordAssets.Count;
                    for (int i = 0; i < count; i++)
                    {
                        CollectionAssets(m_recordAssets[i]);
                    }
                }
#endif
                string[] allbundles = m_AssetBundleManifest.GetAllAssetBundles();
                int len = allbundles.Length;
                for (int i = 0; i < len; i++)
                {
                    if (m_assets.ContainsKey(allbundles[i]))
                        continue;
                    m_assets.Add(allbundles[i], false);
                }
            }
            #endregion

        }
    }

    #region  小包专用代码
#if UNITY_EDITOR
    public static int isRecording = 0;
    private static List<string> m_recordAssets = new List<string>();
#endif

    private static Dictionary<string, bool> m_assets = new Dictionary<string, bool>();

    void InitRecordAssetData()
    {
        RecordAssetData assets = Resources.Load<RecordAssetData>("recordAsset");
        for (int i = 0; i < assets.assetsLength; i++)
        {
            if (m_assets.ContainsKey(assets.assets[i]))
                continue;
            m_assets.Add(assets.assets[i], true);
        }
    }

    private static bool BundleFileExists(string bundleName)
    {
        if (!GameInfoManager.IsSmallClientCache)
            return true;
        if (m_assets.ContainsKey(bundleName))
            return m_assets[bundleName];
        return false;
    }

    public static bool AssetBundleFileExists(string assetBundleName)
    {
#if UNITY_EDITOR
        if (SimulateAssetBundleInEditor)
        {
            return true;
        }
        else
#endif
        {
            bool isExists = BundleFileExists(assetBundleName);
            if (!isExists)
            {
                string persistentDataPath = string.Format("{0}{1}/{2}", UpdaterUtils.persistentDataPath, ResourceUtil.GetPlatformName(), assetBundleName);
                bool ppexists = File.Exists(persistentDataPath);
                m_assets[assetBundleName] = ppexists;
                return ppexists;
            }
            return isExists;
        }
    }

#if UNITY_EDITOR
    private static void CollectionAssets(string assetBundleName)
    {
        if (isRecording == 0)
            return;

        string[] dependencies = GetDependencies(assetBundleName);
        if (dependencies != null)
        {
            for (int i = 0; i < dependencies.Length; i++)
            {
                if (!m_recordAssets.Contains(dependencies[i]))
                {
                    m_recordAssets.Add(dependencies[i]);
                }
            }
        }

        if (!m_recordAssets.Contains(assetBundleName))
        {
            m_recordAssets.Add(assetBundleName);
        }
    }

    public static List<string> GetRecordAssets()
    {
        if (isRecording == 0)
            return null;
        return m_recordAssets;
    }

    public static string[] GetDependencies(string assetBundleName)
    {
        if (m_AssetBundleManifest == null)
            return null;
        return m_AssetBundleManifest.GetAllDependencies(assetBundleName);
    }
#endif
    #endregion


    /// <summary>
    /// PreSave load asset async.
    /// </summary>
    /// <returns>The save load asset async.</returns>
    /// <param name="assetBundleName">Asset bundle name.</param>
    /// <param name="assetName">Asset name.</param>
    /// <param name="type">Type.</param>
    public static AssetBundleLoadAssetOperation PreSaveLoadAssetAsync(string assetBundleName, string assetName, System.Type type)
    {
        PreSaveCollectionDependencies(assetBundleName);
        return LoadAssetAsync(assetBundleName, assetName, type);
    }

    /// <summary>
    /// PreSave load asset.
    /// </summary>
    /// <returns>The save load asset.</returns>
    /// <param name="assetBundleName">Asset bundle name.</param>
    /// <param name="assetName">Asset name.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static T PreSaveLoadAsset<T>(string assetBundleName, string assetName) where T : Object
    {
        PreSaveCollectionDependencies(assetBundleName);
        return LoadAsset<T>(assetBundleName, assetName);
    }

    /// <summary>
    /// PreSave load assetBundle.
    /// </summary>
    /// <returns>The save load assetBundle.</returns>
    /// <param name="assetBundleName">Asset bundle name.</param>
    public static void PreSaveLoadAssetBundle(string assetBundleName)
    {
        PreSaveCollectionDependencies(assetBundleName);
        LoadAssetBundle(assetBundleName);
    }

    private static void PreSaveCollectionDependencies(string assetBundleName)
    {
        if (!m_PresaveAssetBundleNames.Contains(assetBundleName))
            m_PresaveAssetBundleNames.Add(assetBundleName);

        string[] dependencies = null;
        if (m_AssetBundleManifest != null)
        {
            if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
            {
                dependencies = m_AssetBundleManifest.GetDirectDependencies(assetBundleName);
                m_Dependencies.Add(assetBundleName, dependencies);
            }
        }

        if (dependencies != null)
        {
            int length = dependencies.Length;
            for (int i = 0; i < length; i++)
            {
                PreSaveCollectionDependencies(dependencies[i]);
            }
        }
    }

    public static void LoadAssetBundle(string assetBundleName)
    {
#if UNITY_EDITOR
        CollectionAssets(assetBundleName);
        if (SimulateAssetBundleInEditor)
        {
        }
        else
#endif
        {
            CollectionDependenciesLoad(assetBundleName);
            string error;
            LoadedAssetBundle loadedAssetBundle = GetLoadedAssetBundle(assetBundleName, out error);
            if (loadedAssetBundle == null || !string.IsNullOrEmpty(error))
            {
                Debuger.Err("[AssetLoadManager]:Failed to load assetbundle. " + error);
            }
        }
    }

    /// <summary>
    /// Loads the level async.
    /// </summary>
    /// <returns>The level async.</returns>
    /// <param name="assetBundleName">Asset bundle name.</param>
    /// <param name="levelName">Level name.</param>
    /// <param name="isAdditive">If set to <c>true</c> is additive.</param>
    public static AssetLoadOperation LoadLevelAsync(string assetBundleName, string levelName, bool isAdditive)
    {
        AssetLoadOperation operation = null;
#if UNITY_EDITOR
        CollectionAssets(assetBundleName);
        if (SimulateAssetBundleInEditor)
        {
            operation = new AssetBundleLoadLevelSimulationOperation(assetBundleName, levelName, isAdditive);
        }
        else
#endif
        {
            CollectionDependenciesLoadTask(assetBundleName);

            operation = new AssetBundleLoadLevelOperation(assetBundleName, levelName, isAdditive);

            m_InProgressOperations.Add(operation);
        }

        return operation;
    }

    /// <summary>
    /// Loads the asset async.
    /// </summary>
    /// <returns>The asset async.</returns>
    /// <param name="assetBundleName">Asset bundle name.</param>
    /// <param name="assetName">Asset name.</param>
    /// <param name="type">Type.</param>
    public static AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, System.Type type)
    {

        AssetBundleLoadAssetOperation operation = null;
#if UNITY_EDITOR
        CollectionAssets(assetBundleName);
        if (SimulateAssetBundleInEditor)
        {
            string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
            if (assetPaths.Length == 0)
            {
                Debuger.Err("[AssetLoadManager]:There is no asset with name \"" + assetName + "\" in " + assetBundleName);
                return null;
            }

            // @TODO: Now we only get the main object from the first asset. Should consider type also.
            Object target = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPaths[0], type);
            operation = new AssetBundleLoadAssetSimulationOperation(target);
        }
        else
#endif
        {
            CollectionDependenciesLoadTask(assetBundleName);

            operation = new AssetBundleLoadAssetFullOperation(assetBundleName, assetName, type);

            m_InProgressOperations.Add(operation);

        }
        return operation;
    }

    /// <summary>
    /// Loads the asset.
    /// </summary>
    /// <returns>The asset.</returns>
    /// <param name="assetBundleName">Asset bundle name.</param>
    /// <param name="assetName">Asset name.</param>
    /// <param name="type">Type.</param>
    public static Object LoadAsset(string assetBundleName, string assetName, System.Type type)
    {
        //Debuger.Log("kwang...LoadAsset " + assetBundleName);

#if UNITY_EDITOR
        CollectionAssets(assetBundleName);
        if (SimulateAssetBundleInEditor)
        {
            string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
            if (assetPaths.Length == 0)
            {
                Debuger.Err("[AssetLoadManager]:There is no asset with name \"" + assetName + "\" in " + assetBundleName);
                return null;
            }

            // @TODO: Now we only get the main object from the first asset. Should consider type also.
            Object target = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPaths[0], type);
            
            return target;
        }
        else
#endif
        {
            CollectionDependenciesLoad(assetBundleName);
            string error;
            LoadedAssetBundle loadedAssetBundle = GetLoadedAssetBundle(assetBundleName, out error);
            if (loadedAssetBundle == null || !string.IsNullOrEmpty(error))
            {
                Debuger.Err("[AssetLoadManager]:Failed to load assetbundle. " + error);
                return null;
            }
#if UNITY_EDITOR
            var obj=  loadedAssetBundle.assetBundle.LoadAsset(assetName, type);
            //Debuger.Log("kwang...Load asset name " + assetBundleName);
            GameAsset.GetShaderBack(obj as GameObject);
            return obj;

#else
            return loadedAssetBundle.assetBundle.LoadAsset(assetName, type);

#endif
        }

    }

    /// <summary>
    /// Loads the asset.
    /// </summary>
    /// <returns>The asset.</returns>
    /// <param name="assetBundleName">Asset bundle name.</param>
    /// <param name="assetName">Asset name.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static T LoadAsset<T>(string assetBundleName, string assetName) where T : Object
    {
        return LoadAsset(assetBundleName, assetName, typeof(T)) as T;
    }

    /// <summary>
    /// Unload asset bundle.
    /// </summary>
    /// <param name="assetBundleName">Asset bundle name.</param>
    public static void UnLoadAssetBundle(string assetBundleName)
    {
#if UNITY_EDITOR
        // If we're in Editor simulation mode, we don't have to load the manifest assetBundle.
        if (SimulateAssetBundleInEditor)
            return;
#endif

        if (m_PresaveAssetBundleNames.Contains(assetBundleName))
            return;

        string[] dependencies = null;

        m_Dependencies.TryGetValue(assetBundleName, out dependencies);

        if (dependencies != null)
        {
            int length = dependencies.Length;
            string depend = "";
            for (int i = 0; i < length; i++)
            {
                depend = dependencies[i];
                UnLoadAssetBundle(depend);
            }
        }

        string error;

        LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error);

        if (bundle != null)
        {
            if (--bundle.referencedCount <= 0)
            {

                bundle.assetBundle.Unload(true);

                m_LoadedAssetBundles.Remove(assetBundleName);

                m_Dependencies.Remove(assetBundleName);


                //Debuger.Err("[AssetLoadManager]:" + assetBundleName + " has been unloaded successfully");
            }
            return;
        }

        AssetBundleLoadTask loadTask = null;
        if (m_LoadAssetBundleTasks.TryGetValue(assetBundleName, out loadTask))
        {
            if (--loadTask.referencedCount <= 0)
            {
                if (loadTask.request != null && loadTask.request.assetBundle != null)
                {
                    Debug.LogErrorFormat("AssetBundleName:{0},has same file assetbundle", assetBundleName);

                    loadTask.request.assetBundle.Unload(true);
                }

                m_LoadAssetBundleTasks.Remove(assetBundleName);
            }
        }
    }


    /// <summary>
    /// Gets the loaded asset bundle.
    /// </summary>
    /// <returns>The loaded asset bundle.</returns>
    /// <param name="assetBundleName">Asset bundle name.</param>
    /// <param name="error">Error.</param>
    public static LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName, out string error)
    {
        if (string.IsNullOrEmpty(assetBundleName))
        {
            error = null;
            return null;
        }

        if (m_DownloadingErrors.TryGetValue(assetBundleName, out error))
            return null;

        LoadedAssetBundle bundle = null;

        if (m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle))
            return bundle;

        return null;
    }

    /// <summary>
    /// Collections the dependencies load.
    /// </summary>
    /// <param name="assetBundleName">Asset bundle name.</param>
    private static void CollectionDependenciesLoad(string assetBundleName)
    {
        if (string.IsNullOrEmpty(assetBundleName))
            return;
        if (m_DownloadingErrors.ContainsKey(assetBundleName))
        {
            Debuger.Err("[AssetLoadManager]:load " + assetBundleName + " error");
            return;
        }

        string[] dependencies = null;
        if (m_AssetBundleManifest != null)
        {
            if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
            {
                dependencies = m_AssetBundleManifest.GetDirectDependencies(assetBundleName);
                m_Dependencies.Add(assetBundleName, dependencies);
            }
        }

        if (dependencies != null)
        {
            int length = dependencies.Length;
            for (int i = 0; i < length; i++)
            {
                CollectionDependenciesLoad(dependencies[i]);
            }
        }

        if (m_LoadedAssetBundles.ContainsKey(assetBundleName))
        {
            m_LoadedAssetBundles[assetBundleName].referencedCount++;
            return;
        }

        string url = Utility.GetAssetBundleDiskPath(assetBundleName);
        //Debuger.Log("url assetBundleName: " + url);
        AssetBundle bundle = AssetBundle.LoadFromFile(url);
        if (bundle == null)
        {
            m_DownloadingErrors.Add(assetBundleName, string.Format("[AssetLoadManager]:{0} is not a valid asset bundle.", assetBundleName));
            return;
        }

        m_LoadedAssetBundles.Add(assetBundleName, new LoadedAssetBundle(bundle));

    }

    /// <summary>
    /// Collections the dependencies load task.
    /// </summary>
    /// <param name="assetBundleName">Asset bundle name.</param>
    private static void CollectionDependenciesLoadTask(string assetBundleName)
    {
        if (m_DownloadingErrors.ContainsKey(assetBundleName))
        {
            Debuger.Err("[AssetLoadManager]:load " + assetBundleName + " error");
            return;
        }

        string[] dependencies = null;
        if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
        {
            dependencies = m_AssetBundleManifest.GetDirectDependencies(assetBundleName);
            m_Dependencies.Add(assetBundleName, dependencies);
        }

        if (dependencies != null)
        {
            int length = dependencies.Length;
            for (int i = 0; i < length; i++)
            {
                CollectionDependenciesLoadTask(dependencies[i]);
            }
        }

        if (m_LoadedAssetBundles.ContainsKey(assetBundleName))
        {
            m_LoadedAssetBundles[assetBundleName].referencedCount++;
            return;
        }

        if (m_LoadAssetBundleTasks.ContainsKey(assetBundleName))
        {
            m_LoadAssetBundleTasks[assetBundleName].referencedCount++;
            return;
        }

        m_LoadAssetBundleTasks.Add(assetBundleName, new AssetBundleLoadTask(assetBundleName, dependencies));
    }

    /// <summary>
    /// Checks all dependencies loaded.
    /// </summary>
    /// <returns><c>true</c>, if all dependencies loaded was checked, <c>false</c> otherwise.</returns>
    /// <param name="task">Task.</param>
	private static bool CheckAllDependenciesLoaded(AssetBundleLoadTask task)
    {
        if (task.dependencies == null || task.dependencies.Length == 0)
            return true;

        bool ok = true;

        for (int i = 0; i < task.dependencies.Length; i++)
        {
            if (m_LoadAssetBundleTasks.ContainsKey(task.dependencies[i]))
            {
                ok = false;
                break;
            }
        }

        return ok;
    }

    void Update()
    {
        //Debuger.Log("AssetBundleLoadTask Update");
        // Collect all the finished WWWs.
        m_keysToRemove.Clear();
        foreach (var keyValue in m_LoadAssetBundleTasks)
        {
            AssetBundleLoadTask loadTask = keyValue.Value;

            if (loadTask.request == null)
            {
                if (CheckAllDependenciesLoaded(loadTask))
                {
                    string url = Utility.GetAssetBundleDiskPath(loadTask.assetBundleName);
                    loadTask.request = AssetBundle.LoadFromFileAsync(url);
                }
            }
            else
            {
                // If downloading succeeds.
                if (loadTask.request.isDone)
                {
                    AssetBundle bundle = loadTask.request.assetBundle;
                    if (bundle == null)
                    {
                        m_DownloadingErrors.Add(keyValue.Key, string.Format("[AssetLoadManager]:{0} is not a valid asset bundle.", keyValue.Key));
                        m_keysToRemove.Add(keyValue.Key);
                        continue;
                    }

                    LoadedAssetBundle loadedAssetBundle = new LoadedAssetBundle(bundle);
                    loadedAssetBundle.referencedCount = loadTask.referencedCount;

                    m_LoadedAssetBundles.Add(keyValue.Key, loadedAssetBundle);
                    m_keysToRemove.Add(keyValue.Key);
                }
            }
        }

        // Remove the finished WWWs.
        foreach (var key in m_keysToRemove)
        {
            AssetBundleLoadTask loadTask = m_LoadAssetBundleTasks[key];
            m_LoadAssetBundleTasks.Remove(key);
        }

        m_keysToRemove.Clear();

        // Update all in progress operations
        for (int i = 0; i < m_InProgressOperations.Count;)
        {
            if (!m_InProgressOperations[i].Update())
            {
                m_InProgressOperations.RemoveAt(i);
            }
            else
                i++;
        }

        AssetLoadGC();
    }

    public static void OnDispose()
    {
        foreach (var item in m_LoadAssetBundleTasks)
        {
            if (item.Value.request != null && item.Value.request.assetBundle != null)
            {
                item.Value.request.assetBundle.Unload(true);

                Debug.LogErrorFormat("AssetBundleName:{0},has same file assetbundle", item.Value.assetBundleName);
            }
        }
        m_LoadAssetBundleTasks.Clear();
        m_InProgressOperations.Clear();
        m_keysToRemove.Clear();
    }

    private static void AssetLoadGC()
    {
        m_gcTimer += Time.deltaTime;
        if (m_gcTimer <= GC_CHECK_TIME)
            return;

        m_gcTimer = 0;

        //Resources.UnloadUnusedAssets();
    }

} // End of AssetBundleManager.



