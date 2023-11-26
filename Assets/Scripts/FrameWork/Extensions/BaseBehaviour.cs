/*
 * file BaseBehaviour.cs
 *
 * author: Pengmian
 * date:   2014/09/16 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于所有的Behavior类继承
/// </summary>
public class BaseBehaviour : MonoBehaviour
{
    // 缓存的Component
    protected Map<string, Component> mComponents = new Map<string, Component>();
    // 启动的协程列表
    protected List<long> mCoroutines = new List<long>();
    //5.x版本引擎已经自动缓存
    protected Transform trans;
    // 缓存的GameObject
    protected GameObject mGameObject = null;
    
    /// <summary>
    /// 缓存的Transform
    /// </summary>
    public Transform TransformExt
    {
        get
        {
            if (trans == null)
                trans = transform;
            return trans;
        }
    }

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
 
    }

    protected virtual void OnDestroy()
    {
        stopAllCoroutine();
    }

    protected void stopAllCoroutine()
    {
        foreach (long id in mCoroutines)
        {
            CoroutineManager.Singleton.stopCoroutine(id);
        }
        mCoroutines.Clear();
    }

    /// <summary>
    /// 延迟调用
    /// </summary>
    /// <param name="delayedTime"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public long delayCall(float delayedTime, Action callback)
    {
        long ret = CoroutineManager.Singleton.delayedCall(delayedTime, callback);
        mCoroutines.Add(ret);
        return ret;
    }
    
    /// <summary>
    /// 停止一个协程
    /// </summary>
    /// <param name="id"></param>
    public void stopCoroutine(long id)
    {
        CoroutineManager.Singleton.stopCoroutine(id);
        int idx = mCoroutines.IndexOf(id);
        if (idx >= 0)
        {
            mCoroutines.RemoveAt(idx);
        }
    }
}


