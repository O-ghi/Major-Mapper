using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManagerTemplate<T> where T : class, new()
{
    protected static T instance = null;
    private static bool applicationQuiting = false;


    public Transform transform(Type type)
    {
        if (transformDic != null)
        {
            return transformDic;
        }
        return null;
    }
    private static Transform transformDic;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
                GameObject obj = new GameObject(string.Format("_{0}", typeof(T).ToString()));
                GameObject.DontDestroyOnLoad(obj);
                transformDic = obj.transform;

            }
            return instance;
        }
    }
    public static bool CreateInstance()
    {
        if (applicationQuiting)
            return false;

        if (instance != null)
            return false;

        instance = new T();
        GameObject obj = new GameObject(string.Format("_{0}", typeof(T).ToString()));
        GameObject.DontDestroyOnLoad(obj);
        transformDic = obj.transform;



        return true;
    }

    public ManagerTemplate()
    {
        InitManager();
        ManagerLogic.Singleton.AddManagerDestroy(typeof(T), OnDestroy);

    }
    protected virtual void Update()
    {

    }
    protected virtual void LateUpdate()
    {

    }
    protected virtual void FixedUpdate()
    {

    }
    protected virtual void Start()
    {

    }
    protected virtual void Awake()
    {

    }
    protected virtual void OnEnable()
    {

    }
    protected virtual void OnDisable()
    {

    }
    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            applicationQuiting = true;
        }
    }

    public static void DestorySelf()
    {
        instance = null;
    }

    public Coroutine StartCoroutine(IEnumerator coroutine)
    {
        return CoroutineManager.Singleton.StartCoroutine(coroutine);
    }
    public void StopCoroutine(IEnumerator coroutine)
    {
        CoroutineManager.Singleton.stopCoroutine(coroutine);

    }
    public void StopCoroutine(Coroutine coroutine)
    {
        CoroutineManager.Singleton.StopCoroutine(coroutine);

    }
    public void StopAllCoroutines()
    {

    }
    public void Invoke(Action action, float v2)
    {
        CoroutineManager.Singleton.delayedCall(action, v2);
    }
    public void CancelInvoke(Action action)
    {
        //CoroutineManager.Singleton.CancelInvoke(action);

    }
    protected virtual void InitManager()
    {

    }
    public virtual void DelayInit()
    {

    }

}
