using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneBase
{
    public GameObject _bornPos;
    public SceneData _sceneData;
    public TaskCoreCfg _taskCoreCfg;
    public string _sceneName;
    //Transform - Instance

    public Transform transform()
    {
        if (transformDic != null)
        {
            return transformDic;
        }
        return null;
    }
    private static Transform transformDic;
    


    public SceneBase(string sceneName, TaskCoreCfg taskCoreCfg)
    {
        sceneName = this.ToString();
        _sceneName = sceneName.ToLower();

        _taskCoreCfg = taskCoreCfg;
        Init(sceneName);
    }
    protected virtual void Init(string sceneName) 
    {
        GameObject obj = new GameObject(string.Format("_{0}", sceneName));
        GameObject.DontDestroyOnLoad(obj);
        transformDic = obj.transform;
        _sceneData = new SceneData(_taskCoreCfg.ID, new HollandPersonality());

    }

    
    protected virtual void OnStart() { }
}
