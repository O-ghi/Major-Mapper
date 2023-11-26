using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneBase
{
    public GameObject _bornPos;
    public SceneData _sceneData;
    public TaskCoreCfg _taskCoreCfg;
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
        _taskCoreCfg = taskCoreCfg;
        Init(sceneName);
    }
    protected virtual void Init(string sceneName) 
    {
        GameObject obj = new GameObject(string.Format("_{0}", sceneName));
        GameObject.DontDestroyOnLoad(obj);
        transformDic = obj.transform;
        _sceneData = new SceneData(_taskCoreCfg.ID);

    }

    protected virtual void OnStart() { }
}
