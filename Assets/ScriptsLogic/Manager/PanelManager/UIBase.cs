using UnityEngine.Profiling;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;



public interface ItfBase
{
    bool ismaxDepth { set; get; }
    string uiName { set; get; }
    int sortingOrder { set; get; }
    //UILAYER uiLayer { set; get; }
    Transform cacheTransform { set; get; }
    Canvas cacheCanvas { set; get; }
    QuickVarList uiData { set; get; }
    GameObject uiPrefab { set; get; }
    void Init();
    void OnStart();
    void OnUpdate();
    void OnLateUpdate();
    void Register();
    void UnRegister();
    void Destroy();
    void OnClose();
}

//#if Main
//public class UIBase : MonoBehaviour
//#else
public class UIBase
//#endif
{

    [HideInInspector]
    public bool ismaxDepth = false;
    [HideInInspector]
    public QuickVarList Data
    {
        get { return m_data; }
        set
        {
            //if (m_data != null)
            //	m_data.Dispose();
            m_data = value;
        }
    }
    protected QuickVarList m_data = null;

    [HideInInspector]
    public string uiName = string.Empty;
    [HideInInspector]
    public int sortingOrder = 0;
    //[HideInInspector]
    //public UILAYER uiLayer = UILAYER.NORMAL;


    private ItfBase m_luacode = null;

    private PanelCompleteHandler m_handler = null;

    [HideInInspector]
    public Transform cacheTransform;
    //Duong 21/10/22 Them cac thuoc tinh moi thay the cho monobehaviour
    //#if !Main
    public Transform transform;
    public GameObject gameObject;
    public string name;
    //#endif
    [HideInInspector]
    public Canvas cacheCanvas;

    //private LanguagePackLoader m_languageLoader;
    //private ConmandHelpCfgLoader m_conmandhelpLoader;
    private bool m_nocshape = false;
    public bool nocshape
    {
        set
        {
            m_nocshape = value;
            if (m_nocshape)
                SetLuaCode();
        }
        get { return m_nocshape; }
    }
    //Duong 21/10/22 Thay the Awake mono -> GameAwake duoc chu dong goi khi khoi tao
    //Gọi ở PanelManager
    public void GameAwake(Transform transform)
    {
        //init

        if (coroutineDics == null)
        {
            coroutineDics = new Dictionary<GameObject, List<IEnumerator>>();
        }

        this.cacheTransform = transform;

        this.transform = transform;
        this.gameObject = transform.gameObject;
        this.name = transform.name;
        //#endif
        if (m_luacode != null)
            m_luacode.cacheTransform = cacheTransform;
        m_eventTriggerListeners = new List<TriggerListener>();
        //
        sortingOrder = 0;
    }

    /// <summary>
    /// 需要的初始化
    /// </summary>
    /// <param name="args"></param>
    public void Initialization(QuickVarList args, PanelCompleteHandler handler = null)
    {
        uiName = cacheTransform.name;
        if (m_luacode != null)
            m_luacode.uiName = uiName;
        cacheCanvas = cacheTransform.GetComponent<Canvas>();
        if (m_luacode != null)
            m_luacode.cacheCanvas = cacheCanvas;
        Init();
        Data = args;
        UIStart();
        if (m_luacode != null)
        {
            m_luacode.uiData = Data;
        }
        m_handler = handler;
    }

    void UIStart()
    {
        if (m_luacode != null)
        {
            m_luacode.OnStart();
            m_luacode.Register();
        }
        OnStart();
        Register();
        if (m_handler != null)
            m_handler(this);
    }

    public void GameUpdate()
    {
        if (m_luacode != null)
            m_luacode.OnUpdate();
        UnityEngine.Profiling.Profiler.BeginSample(uiName);
        OnUpdate();
        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void GameLateUpdate()
    {
        if (m_luacode != null)
            m_luacode.OnLateUpdate();
        OnLateUpdate();
    }
    //
    private static Dictionary<GameObject, List<IEnumerator>> coroutineDics;
    //Duong support StartCoroutine
    public IEnumerator UIStartCoroutine(IEnumerator enumerator)
    {
        if (!coroutineDics.ContainsKey(this.gameObject))
        {
            var list = new List<IEnumerator>();
            coroutineDics.Add(this.gameObject, new List<IEnumerator>());
        }
        var targetList = coroutineDics[this.gameObject];
        targetList.Add(enumerator);
        CoroutineManager.Singleton.StartCoroutine(enumerator);

        return enumerator;
    }

    public void UIStopAllCoroutines()
    {
        if (coroutineDics.ContainsKey(this.gameObject))
        {
            var targetList = coroutineDics[this.gameObject];
            for (int i = 0; i < targetList.Count; i++)
            {
                if (targetList[i] != null)
                {
                    CoroutineManager.Singleton.stopCoroutine(targetList[i]);
                }
            }
            targetList = null;
            coroutineDics.Remove(this.gameObject);
        }
    }
    public void UIStopCoroutine(IEnumerator enumerator)
    {
        if (enumerator == null)
            return;
        if (coroutineDics.ContainsKey(this.gameObject))
        {
            var targetList = coroutineDics[this.gameObject];
            for (int i = 0; i < targetList.Count; i++)
            {
                if (targetList[i] == enumerator)
                {
                    CoroutineManager.Singleton.stopCoroutine(targetList[i]);
                    targetList.RemoveAt(i);
                    return;
                }
            }
        }
    }

    public void UIDestroyImmediate(UnityEngine.Object gameObject)
    {
        GameObject.DestroyImmediate(gameObject);
    }
    public void UIDestroy(GameObject gameObject)
    {
        GameObject.Destroy(gameObject);
    }

    public GameObject UIInstantiate(GameObject gameObject)
    {
        return GameObject.Instantiate(gameObject);
    }
    public Transform UIInstantiate(Transform titleGo)
    {
        return GameObject.Instantiate(titleGo);

    }

    /// <summary>
    /// 刷新
    /// </summary>
    /// <param name="args"></param>
    public void Refresh(QuickVarList args, PanelCompleteHandler handler = null)
    {
        Data = args;
        m_handler = handler;
        if (m_luacode != null)
        {
            //if (!LuaManager.isDispose)
            // m_luacode.UnRegister();
            m_luacode.uiData = Data;
            m_luacode.OnStart();
            m_luacode.Register();
        }
        UnRegister();
        OnStart();
        Register();
        if (m_handler != null)
            m_handler(this);
    }

    public void UIClose()
    {
        if (m_luacode != null)
            m_luacode.OnClose();
        OnClose();
    }
    
    public void UIDestroy()
    {
        //if (m_luacode != null && !LuaManager.isDispose)
        // m_luacode.UnRegister();
        UnRegister();
        //if (m_luacode != null && !LuaManager.isDispose)
        // m_luacode.Destroy();
        ClearAllEventTriggerListener();
        Destroy();
        Data = null;
        m_luacode = null;
        m_handler = null;
    }

    private void SetLuaCode()
    {
#if THREAD_SAFT || HOTFIX_ENABLE
        string luaCodename = gameObject.name;
        m_luacode = LuaManager.luaEnv.Global.Get<ItfBase>(luaCodename);
        if (m_luacode == null)
            return;
        m_luacode.uiPrefab = gameObject;
        if (m_luacode != null)
        {
            m_luacode.Init();
        }
#endif
    }

    protected T GetTargetComponent<T>(string targetPath, Transform tra = null) where T : Component
    {
        GameObject targetTrans = GetTargetGameObject(targetPath, tra);
        if (targetTrans == null)
            return null;
        if(targetTrans.GetComponent<T>() != null)
        {
            return targetTrans.GetComponent<T>();
        } else
        {
            return targetTrans.AddComponent<T>();
        }
    }

    protected GameObject GetTargetGameObject(string targetPath, Transform tra = null)
    {
        Transform targetTrans = ((null == tra) ? cacheTransform : tra).Find(targetPath);
        if (targetTrans == null)
            return null;
        return targetTrans.gameObject;
    }

    protected virtual void Init() { }

    protected virtual void OnStart() { }

    protected virtual void OnUpdate() { }

    protected virtual void OnLateUpdate() { }

    protected virtual void Register() { }

    protected virtual void UnRegister() { }

    protected virtual void OnClose() { }

    protected virtual void Destroy() { }


    protected string GetLanguageText(string key)
    {
        return /*m_languageLoader.GetConfig(key)*/"";
    }

    protected string GetLanguageTextFormat(string key, params object[] args)
    {
        return /*m_languageLoader.GetConfig(string.Format(key, args))*/"";
    }

    public void SetActive(Component item, bool value)
    {
        if (item.gameObject.activeSelf == value)
            return;
        SetActive(item.gameObject, value);
    }

    public void SetActive(GameObject item, bool value)
    {
        if (item.activeSelf == value)
            return;
        item.SetActive(value);
    }

    public void SetActive(Component parent, string targetPath, bool value)
    {
        Transform trans = parent.transform.Find(targetPath);
        if (trans != null)
            SetActive(trans, value);
    }

    public void SetActive(Transform parent, string targetPath, bool value)
    {
        Transform trans = parent.Find(targetPath);
        if (trans != null)
            SetActive(trans.gameObject, value);
    }

    public void PlayAudio(int id)
    {
        //AudioManager.PlayAudio(id, PanelManager.cachePanelMgrTransform);
    }

    public void SetMat(Graphic graphic, Material mat)
    {
        if (null == graphic)
            return;

        graphic.material = mat;
    }

    public void SetMat(Transform tra, Material mat)
    {
        Graphic graphic = tra.GetComponent<Graphic>();
        SetMat(graphic, mat);
    }

    public void SetMat(Transform parent, string targetPath, Material mat)
    {
        Transform trans = parent.Find(targetPath);
        if (trans != null)
            SetMat(trans, mat);
    }

    #region SetText
    public void SetText(Text item, object value)
    {
        item.text = value.ToString();
    }

    public void SetText(Component item, object value)
    {
        Text text = item.GetComponent<Text>();
        if (text != null)
            SetText(text, value);
    }

    public void SetText(Transform parent, string targetPath, object value)
    {
        Transform trans = parent.Find(targetPath);
        if (trans != null)
            SetText(trans, value);
    }

    public void SetText(Text item, string value)
    {
        if (null == item)
        {
            return;
        }

        item.text = value;
    }

    public void SetText(Text item, int value)
    {
        SetText(item, value.ToString());
    }

    public void SetText(Text item, float value)
    {
        SetText(item, value.ToString());
    }

    public void SetText(Text item, double value)
    {
        SetText(item, value.ToString());
    }

    public void SetText(Text item, long value)
    {
        SetText(item, value.ToString());
    }

    public void SetText(Component item, int value)
    {
        SetText(item, value.ToString());
    }

    public void SetText(Component item, float value)
    {
        SetText(item, value.ToString());
    }

    public void SetText(Component item, string value)
    {
        Text text = item.GetComponent<Text>();
        if (text != null)
            SetText(text, value);
    }

    public void SetText(Transform parent, string targetPath, int value)
    {
        Transform trans = parent.Find(targetPath);
        if (trans != null)
            SetText(trans, value);
    }

    public void SetText(Transform parent, string targetPath, string value)
    {
        Transform trans = parent.Find(targetPath);
        if (trans != null)
            SetText(trans, value);
    }

    public void SetText(Text item, int value, bool isNum)
    {
        if (value == 0)
            SetText(item, "0");
        else
            SetText(item, value);
    }

    public void SetText(Text item, long value, bool isNum)
    {
        if (value == 0)
            SetText(item, "0");
        else
            SetText(item, value);
    }

    public void SetText(Component item, int value, bool isNum)
    {
        Text text = item.GetComponent<Text>();
        if (text != null)
            SetText(text, value, isNum);
    }
    #endregion
    //protected void SetFormatTextLanguage(Text item, string languageKey, int count, params object[] args)
    //{
    //    if (args.Length == count)
    //        SetLanguageTextFormat(item, GetLanguageText(string.Format(languageKey, args)));
    //    else
    //    {
    //        object[] objs = new object[count];
    //        Array.Copy(args, count, objs, 0, args.Length - count);
    //        string format = string.Format(languageKey, args);
    //        SetLanguageTextFormat(item, format, objs);
    //    }
    //}

    //protected void SetFormatTextLanguage(Transform item, string languageKey, int count, params object[] args)
    //{
    //    Text text = item.GetComponent<Text>();
    //    if (text != null)
    //        SetFormatTextLanguage(text, languageKey, count, args);
    //}

    //public void SetFormatTextLanguage(Transform parent, string targetPath, string languageKey, int count, params object[] args)
    //{
    //    Transform trans = parent.Find(targetPath);
    //    if (trans != null)
    //        SetFormatTextLanguage(trans, languageKey, count, args);
    //}

    //public void SetLanguageTextFormat(Text item, string languageKey, params object[] args)
    //{
    //    if (null != args && args.Length > 0)
    //        SetText(item, GameItemUtil.GetLanguageThenFormat(languageKey, args));
    //    else
    //        SetText(item, GetLanguageText(languageKey));
    //}

    //public void SetLanguageTextFormat(Transform item, string languageKey, params object[] args)
    //{
    //    Text text = item.GetComponent<Text>();
    //    if (text != null)
    //        SetLanguageTextFormat(text, languageKey, args);
    //}

    //public void SetLanguageTextFormat(Transform parent, string targetPath, string languageKey, params object[] args)
    //{
    //    Transform trans = parent.Find(targetPath);
    //    if (trans != null)
    //        SetLanguageTextFormat(trans, languageKey, args);
    //}

    public void SetTextFormat(Text item, string format, params object[] args)
    {
        if (args != null)
        {
            try
            {
                SetText(item, string.Format(format, args));
            }
            catch
            {
                SetText(item, format);
            }
        }

        else
            SetText(item, format);
    }

    public void SetTextFormat(Component item, string format, params object[] args)
    {
        Text text = item.GetComponent<Text>();
        if (text != null)
            SetTextFormat(text, format, args);
    }

    public void SetTextFormat(Transform parent, string targetPath, string format, params object[] args)
    {
        Transform trans = parent.Find(targetPath);
        if (trans != null)
            SetTextFormat(trans, format, args);
    }

    public void SetTextColor(Text item, Color color)
    {
        item.color = color;
    }

    public void SetTextColor(Component item, Color color)
    {
        Text text = item.GetComponent<Text>();
        if (text != null)
            SetTextColor(text, color);
    }

    public void SetTextColor(Transform parent, string targetPath, Color color)
    {
        Transform trans = parent.Find(targetPath);
        if (trans != null)
            SetTextColor(trans, color);
    }

    public void SetTextForColor(Text item, string value, int color)
    {
        //string color_value = GameItemUtil.GetLanguage("color_" + color);
        //SetTextFormat(item, "<color={0}>{1}</color>", color_value, value);
    }

    public void SetTextForColor(Component item, string value, int color)
    {
        Text text = item.GetComponent<Text>();
        if (text != null)
            SetTextForColor(text, value, color);
    }

    public void SetTextForColor(Transform parent, string targetPath, string value, int color)
    {
        Transform trans = parent.Find(targetPath);
        if (trans != null)
            SetTextForColor(trans, value, color);
    }

    public void SetPhoto(Component item, string spriteName)
    {
        //SImage image = item.GetComponent<SImage>();
        //if (image != null)
        //    image.SetSpriteName(spriteName);
    }

    //public void SetPhoto(SImage item, string spriteName)
    //{
    //    if (item != null)
    //        item.SetSpriteName(spriteName);
    //}

    public void SetPhoto(Transform parent, string targetPath, string spriteName)
    {
        Transform trans = parent.Find(targetPath);
        if (trans != null)
            SetPhoto(trans, spriteName);
    }



    public void SetSlider(Transform parent, string targetPath, float value)
    {
        if (parent != null)
            SetSlider(parent.Find(targetPath), value);
    }

    public void SetSlider(Component item, float value)
    {
        if (item != null)
            SetSlider(item.GetComponent<Slider>(), value);
    }

    public void SetArrayActive(Component[] traArray, int start)
    {
        for (int i = 0; i < traArray.Length; ++i)
        {
            if (!traArray[i])
                continue;
            if (i < start)
                SetActive(traArray[i], true);
            else
                SetActive(traArray[i], false);
        }
    }

    public void SetSlider(Slider item, float value)
    {
        if (item != null)
            item.value = value;
    }

    private List<TriggerListener> m_eventTriggerListeners;

    protected void ClearAllEventTriggerListener()
    {
        for (int i = 0; i < m_eventTriggerListeners.Count; i++)
        {
            TriggerListener entry = m_eventTriggerListeners[i];
            foreach (var item in entry.triggers)
            {
                item.RemoveAllEventListener();
            }
        }
    }

    protected void ClearTriggerListener(Transform trigger, EventTriggerType triggerType = EventTriggerType.PointerClick)
    {
        if (!trigger)
            return;
        ClearTriggerListener(trigger.GetComponent<TriggerListener>());
    }

    protected void ClearTriggerListener(TriggerListener trigger, EventTriggerType triggerType = EventTriggerType.PointerClick)
    {
        if (!trigger)
            return;

        TriggerListener.Entry entry = trigger.triggers.Find(p => p.eventID == triggerType);
        if (entry == null)
            return;
        entry.RemoveAllEventListener();
    }

    protected void SetBtnClick(Component[] trigger, params UnityAction<EventTriggerType, GameObject>[] triggerAction)
    {
        for (int i = 0; i < trigger.Length; i++)
            SetButton(trigger[i], EventTriggerType.PointerClick, triggerAction[i]);
    }

    protected void SetBtnSelect(TriggerListener[] trigger, params UnityAction<EventTriggerType, GameObject>[] triggerAction)
    {
        for (int i = 0; i < trigger.Length; i++)
            SetButton(trigger[i], EventTriggerType.Select, triggerAction[i]);
    }

    protected void SetBtnClick(TriggerListener trigger, UnityAction<EventTriggerType, GameObject> triggerAction)
    {
        SetButton(trigger, EventTriggerType.PointerClick, triggerAction);
    }

    protected void SetBtnClick(Transform trigger, UnityAction<EventTriggerType, GameObject> triggerAction)
    {
        SetButton(trigger, EventTriggerType.PointerClick, triggerAction);
    }

    protected void SetBtnSelect(TriggerListener trigger, UnityAction<EventTriggerType, GameObject> triggerAction)
    {
        SetButton(trigger, EventTriggerType.Select, triggerAction);
    }

    public void SetButton(TriggerListener trigger, EventTriggerType triggerType, UnityAction<EventTriggerType, GameObject> triggerAction, object triggerArg = null)
    {
        TriggerListener.Entry entry = trigger.triggers.Find(p => p.eventID == triggerType);
        if (entry == null)
        {
            entry = new TriggerListener.Entry();
            entry.eventID = triggerType;
            trigger.triggers.Add(entry);
        }
        trigger.triggerArg.Add(triggerArg);
        entry.AddEventListener(triggerType, triggerAction);
        if (!m_eventTriggerListeners.Contains(trigger))
        {
            m_eventTriggerListeners.Add(trigger);
        }
    }

    public void SetButton(Component item, EventTriggerType triggerType, UnityAction<EventTriggerType, GameObject> triggerEvent, object triggerArg = null)
    {
        if (item == null)
            return;
        TriggerListener trigger = item.GetComponent<TriggerListener>();
        if (trigger != null)
            SetButton(trigger, triggerType, triggerEvent, triggerArg);
    }

    public void SetButton(Transform parent, string targetPath, EventTriggerType triggerType, UnityAction<EventTriggerType, GameObject> triggerEvent, object triggerArg = null)
    {
        Transform trans = parent.Find(targetPath);
        if (trans != null)
            SetButton(trans, triggerType, triggerEvent, triggerArg);
    }
    protected void SetObjectActive(Component item, bool value)
    {
        if (null == item)
            return;
        if (item.gameObject.activeSelf == value)
            return;
        item.gameObject.SetActive(value);
    }

    protected void SetObjectActive(GameObject item, bool value)
    {
        if (item.activeSelf == value)
            return;
        item.SetActive(value);
    }

    protected void SetBtnDisable(Transform tra, string path, bool state)
    {
        if (!tra)
            return;

        var child = tra.Find(path);
        if (!child)
            return;

        var trigger = child.GetComponent<TriggerListener>();
        SetBtnDisable(trigger, state);
    }

    protected void SetBtnDisable(Component tra, bool state)
    {
        if (!tra)
            return;

        var trigger = tra.GetComponent<TriggerListener>();
        SetBtnDisable(trigger, state);
    }

    protected void SetBtnDisable(TriggerListener trigger, bool state)
    {
        if (!trigger)
            return;

        trigger.disabled = state;
    }

    protected void SetBtnDisableAction(Transform tra, string path, EventTriggerType type, System.Action action)
    {
        if (!tra)
            return;

        var child = tra.Find(path);
        if (!child)
            return;

        var trigger = child.GetComponent<TriggerListener>();
        SetBtnDisableAction(trigger, type, action);
    }

    protected void SetBtnDisableAction(Component tra, EventTriggerType type, System.Action action)
    {
        if (!tra)
            return;

        var trigger = tra.GetComponent<TriggerListener>();
        SetBtnDisableAction(trigger, type, action);
    }

    protected void SetBtnDisableAction(TriggerListener trigger, EventTriggerType type, System.Action action)
    {
        if (!trigger)
            return;

        trigger.SetDisableAction(type, action);
    }

    protected void SetObjectActive(Transform item, string targetPath, bool value)
    {
        Transform trans = item.Find(targetPath);
        if (trans != null)
            SetObjectActive(trans.gameObject, value);
    }

    #region UIBase.Event
    public virtual void OnCloseEvent()
    {
        if (null != cacheTransform)
        {
            PanelManager.ClosePanel(cacheTransform.name);
        }
    }

    public virtual void OnDestroyEvent(bool show = true)
    {
        PanelManager.DestroyPanel(cacheTransform.name, show);
    }

    #endregion
}