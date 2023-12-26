using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public abstract class AssetLoadOperation : IEnumerator
{
    public object Current
    {
        get
        {
            return null;
        }
    }
    public bool MoveNext()
    {
        return !IsDone();
    }

    public void Reset()
    {
    }

    public virtual float Progress
    {
        get
        {
            return 0.0f;
        }
    }

    abstract public bool Update();

    abstract public bool IsDone();

    abstract public void Unload();
}

public abstract class AssetBundleLoadAssetOperation : AssetLoadOperation
{
    public abstract T GetAsset<T>() where T : UnityEngine.Object;
}

public class AssetBundleLoadDummyAssetOperation : AssetBundleLoadAssetOperation
{
    public Object dummyObject;

    public AssetBundleLoadDummyAssetOperation(Object dummyObject)
    {
        this.dummyObject = dummyObject;
    }
    public override T GetAsset<T>()
    {
        return dummyObject as T;
    }

    public override bool Update()
    {
        return false;
    }

    public override bool IsDone()
    {
        return true;
    }

    public override void Unload()
    {
    }
}

public class AssetBundleLoadAssetSimulationOperation : AssetBundleLoadAssetOperation
{
    Object simulatedObject;

    public AssetBundleLoadAssetSimulationOperation(Object simulatedObject)
    {
        this.simulatedObject = simulatedObject;
    }

    public override T GetAsset<T>()
    {
#if UNITY_EDITOR
        {
            //Debuger.Log("kwang...2.Load asset name " + simulatedObject.name);
            var obj = simulatedObject as T;
            GameAsset.GetShaderBack(obj as GameObject);
            return obj;
        }
#else
        return simulatedObject as T;
#endif

    }

    public override bool Update()
    {
        return false;
    }

    public override bool IsDone()
    {
        return true;
    }

    public override void Unload()
    {
    }
}

public class AssetBundleLoadAssetFullOperation : AssetBundleLoadAssetOperation
{
    protected string m_AssetBundleName;
    protected string m_AssetName;
    protected string m_DownloadingError;
    protected System.Type m_Type;
    protected AssetBundleRequest m_Request = null;

    public AssetBundleLoadAssetFullOperation(string bundleName, string assetName, System.Type type)
    {
        m_AssetBundleName = bundleName;
        m_AssetName = assetName;
        m_Type = type;
    }

    public override T GetAsset<T>()
    {
        if (m_Request != null && m_Request.isDone)
#if UNITY_EDITOR
        {
            //Debuger.Log("kwang...2.Load asset name " + m_Request.asset.name);
            var obj = m_Request.asset as T;
            GameAsset.GetShaderBack(obj as GameObject);
            return obj;
        }
#else
        return m_Request.asset as T;
#endif
        else
            return null;
    }

    // Returns true if more Update calls are required.
    public override bool Update()
    {
        if (m_Request != null)
            return false;

        LoadedAssetBundle bundle = AssetLoadManager.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
        if (bundle != null)
        {
            ///@TODO: When asset bundle download fails this throws an exception...
            m_Request = bundle.assetBundle.LoadAssetAsync(m_AssetName, m_Type);
            return false;
        }
        else
        {
            return true;
        }
    }

    public override float Progress
    {
        get
        {
            if (m_Request == null)
                return 0.0f;
            return m_Request.progress;
        }
    }

    public override bool IsDone()
    {
        // Return if meeting downloading error.
        // m_DownloadingError might come from the dependency downloading.
        if (m_Request == null && m_DownloadingError != null)
        {
            Debuger.Err("[AssetLoadManager]: "+m_DownloadingError);
            return true;
        }

        return m_Request != null && m_Request.isDone;
    }

    public override void Unload()
    {
        AssetLoadManager.UnLoadAssetBundle(m_AssetBundleName);
    }
}

#if UNITY_EDITOR
public class AssetBundleLoadLevelSimulationOperation : AssetLoadOperation
{
	AsyncOperation m_Operation = null;


	public AssetBundleLoadLevelSimulationOperation(string assetBundleName, string levelName, bool isAdditive)
	{
		string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, levelName);
		if (levelPaths.Length == 0)
		{
            ///@TODO: The error needs to differentiate that an asset bundle name doesn't exist
            //        from that there right scene does not exist in the asset bundle...

            Debuger.Err("[AssetLoadManager]:There is no scene with name : "+ levelName + " in "+ assetBundleName);
			return;
		}

        LoadSceneParameters loadSceneParameters = new LoadSceneParameters();

        if (isAdditive)
            loadSceneParameters.loadSceneMode = LoadSceneMode.Additive;
        else
            loadSceneParameters.loadSceneMode = LoadSceneMode.Single;

        m_Operation = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(levelPaths[0], loadSceneParameters);   // KIET MARK 021122

		
	}

	public override bool Update()
	{
		return false;
	}

	public override bool IsDone()
	{
		return m_Operation == null || m_Operation.isDone;
	}

    public override void Unload()
    {
    }
}
#endif

public class AssetBundleLoadLevelOperation : AssetLoadOperation
{
	protected string m_AssetBundleName;
	protected string m_LevelName;
	protected bool m_IsAdditive;
	protected string m_DownloadingError;
	protected AsyncOperation m_Request;

	public AssetBundleLoadLevelOperation(string assetbundleName, string levelName, bool isAdditive)
	{
		m_AssetBundleName = assetbundleName;
		m_LevelName = levelName;
		m_IsAdditive = isAdditive;
	}

	public override bool Update()
	{
		if (m_Request != null)
			return false;

		LoadedAssetBundle bundle = AssetLoadManager.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
		if (bundle != null)
		{
            if (m_IsAdditive)
                m_Request = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_LevelName, LoadSceneMode.Additive);
			else
                m_Request = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_LevelName);
			return false;
		}
		else
			return true;
	}

	public override bool IsDone()
	{
		// Return if meeting downloading error.
		// m_DownloadingError might come from the dependency downloading.
		if (m_Request == null && m_DownloadingError != null)
		{
            Debuger.Err("[AssetLoadManager]: "+ m_DownloadingError);
			return true;
		}

#if UNITY_EDITOR
        {
            bool isDone =  m_Request != null && m_Request.isDone;
            var allObj = Object.FindObjectsOfType<GameObject>();
            foreach(var obj in allObj)
            {
                GameAsset.GetShaderBack(obj as GameObject);
            }
            return isDone;
        }
#else
        return m_Request != null && m_Request.isDone;
#endif

    }

    public override void Unload()
    {
        AssetLoadManager.UnLoadAssetBundle(m_AssetBundleName);
    }

    public override float Progress
    {
        get
        {
            if (m_Request == null)
                return 0.0f;
            return m_Request.progress;
        }
    }
}



