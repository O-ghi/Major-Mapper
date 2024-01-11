using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonTemplate<T> where T : class, new()
{
    protected static T mSingleton = null;

    public static T Singleton
    {
        get
        {
            if (mSingleton == null)
            {
                mSingleton = new T();

            }
            return mSingleton;
        }
    }
    public SingletonTemplate()
    {
        OnCreateInstance();
    }

    public static void DestorySelf()
    {
        mSingleton = null;
    }
    protected virtual void OnCreateInstance()
    {

    }
}
