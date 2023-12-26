using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    /// <summary>
    /// 内部辅助类
    /// </summary>
    private class CoroutineTask
    {
        public Int64 Id { get; set; }
        public bool Running { get; set; }
        public bool Paused { get; set; }

        public CoroutineTask(Int64 id)
        {
            Id = id;
            Running = true;
            Paused = false;
        }

        public IEnumerator coroutineWrapper(IEnumerator co)
        {
            IEnumerator coroutine = co;
            while (Running)
            {
                if (Paused)
                    yield return null;
                else
                {
                    if (coroutine != null)
                    {
                        try
                        {
                            bool ret = coroutine.MoveNext();
                            if (ret == false)
                                Running = false;
                        }catch(Exception e)
                        {
                            Debug.LogError(e.Message + "\n" + e.StackTrace);
                        }
                        yield return coroutine.Current;
                    }
                    else
                        Running = false;
                }
            }
            mCoroutines.Remove(Id.ToString());
        }
    }

    private static Dictionary<string, CoroutineTask> mCoroutines;
    public static CoroutineManager Singleton { get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(this);
        Singleton = this;
        mCoroutines = new Dictionary<string, CoroutineTask>();
    }

    private long curId;
    private long newId()
    {
        return ++curId;
    }

    /// <summary>
    /// 启动一个协程
    /// </summary>
    /// <param name="co"></param>
    /// <returns></returns>
    public long startCoroutine(IEnumerator co)
    {
        if (this.gameObject.activeSelf)
        {
            CoroutineTask task = new CoroutineTask(newId());
            mCoroutines.Add(task.Id.ToString(), task);
            StartCoroutine(task.coroutineWrapper(co));
            return task.Id;
        }
        return -1;
    }
    public long stopCoroutine(IEnumerator co)
    {
        if (this.gameObject.activeSelf)
        {
            if(co != null)
            {
                StopCoroutine(co);
            }
        }
        return -1;
    }

    // private static Map<string, CoroutineTask> mCoroutinesPreview;

    public long startCoroutinePreviewSkill(IEnumerator co)
    {
#if SKILL_EDITOR
        if (this.gameObject.activeSelf)
        {
            CoroutineTask task = new CoroutineTask(IdAssginer.getId(IdAssginer.IdType.CoroutineId));
            mCoroutines.Add(task.Id.ToString(), task);
            StartCoroutine(task.coroutineWrapper(co));
            return task.Id;
        }
#endif
        return -1;
    }

    /// <summary>
    /// 停止一个协程
    /// </summary>
    /// <param name="id"></param>
    public void stopCoroutine(long id)
    {
        if(mCoroutines.ContainsKey(id.ToString()))
        {
            CoroutineTask task = mCoroutines[id.ToString()];
            task.Running = false;
            mCoroutines.Remove(id.ToString());
        }
    }

    /// <summary>
    /// 暂停协程的运行
    /// </summary>
    /// <param name="id"></param>
    public void pauseCoroutine(Int64 id)
    {
        if (mCoroutines.ContainsKey(id.ToString()))
        {
            CoroutineTask task = mCoroutines[id.ToString()];
            task.Paused = true;
        }else
        {
            Debug.LogError("coroutine: " + id.ToString() + " is not exist!");
        }
    }

    /// <summary>
    /// 恢复协程的运行
    /// </summary>
    /// <param name="id"></param>
    public void resumeCoroutine(Int64 id)
    {
        if (mCoroutines.ContainsKey(id.ToString()))
        {
            CoroutineTask task = mCoroutines[id.ToString()];
            task.Paused = false;
        }
        else
        {
            Debug.LogError("coroutine: " + id.ToString() + " is not exist!");
        }
    }

    public long delayedCall(float delayedTime, Action callback)
    {
        return startCoroutine(delayedCallImpl(delayedTime, callback));
    }

    public long delayedCall(Action callback, float delayedTime)
    {
        return startCoroutine(delayedCallImpl(delayedTime, callback));
    }

    private IEnumerator delayedCallImpl(float delayedTime, Action callback)
    {
        if (delayedTime >= 0)
            yield return new WaitForSeconds(delayedTime);

        if (callback != null)
            callback();
    }


    public long delayedCall(float delayedTime, Action<object> callback, object param)
    {
        return startCoroutine(delayedCallImpl(delayedTime, callback, param));
    }

    private IEnumerator delayedCallImpl(float delayedTime, Action<object> callback, object param)
    {
        if (delayedTime >= 0)
            yield return new WaitForSeconds(delayedTime);
        if (callback != null)
            callback(param);
    }

    public long realTimeDelayCall(float delayedTime, Action callback)
    {
        return startCoroutine(realdDelayedCallImpl(delayedTime, callback));
    }

    private IEnumerator realdDelayedCallImpl(float delayedTime, Action callback)
    {
        if (delayedTime >= 0)
            yield return new WaitForSecondsRealtime(delayedTime);
        if (callback != null)
            callback();
    }

    public long realTimeDelayCall(float delayedTime, Action<object> callback, object param)
    {
        return startCoroutine(realdDelayedCallImpl(delayedTime, callback, param));
    }

    private IEnumerator realdDelayedCallImpl(float delayedTime, Action<object> callback, object param)
    {
        if (delayedTime >= 0)
            yield return new WaitForSecondsRealtime(delayedTime);
        if (callback != null)
            callback(param);
    }

    private void OnDestroy()
    {
        foreach (var task in mCoroutines.Values)
            task.Running = false;
        mCoroutines.Clear();
    }

}
