
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnityEngine.EventSystems
{
    [AddComponentMenu("Event/Trigger Listener")]
    public class TriggerListener : Selectable,
        IPointerClickHandler,
        IDropHandler,
        ISubmitHandler
    {
        [Serializable]
        public class TriggerEvent : UnityEvent<EventTriggerType, GameObject>
        { }

        [Serializable]
        public class Entry
        {
            public EventTriggerType eventID = EventTriggerType.PointerClick;
            [SerializeField]
            public TriggerEvent callback = new TriggerEvent();
            private Dictionary<EventTriggerType, List<int>> calls = new Dictionary<EventTriggerType, List<int>>();

            public void Invoke(EventTriggerType eventID, GameObject gameObject)
            {
                callback.Invoke(eventID, gameObject);
            }

            public void AddEventListener(EventTriggerType eventID, UnityAction<EventTriggerType, GameObject> call)
            {
                int hash = call.GetHashCode();
                List<int> list;
                if (!calls.TryGetValue(eventID, out list))
                {
                    list = new List<int>();
                    calls.Add(eventID, list);
                }

                if (list.Contains(hash))
                    return;

                list.Add(hash);
                callback.AddListener(call);
            }

            public void RemoveEventListener(EventTriggerType eventID, UnityAction<EventTriggerType, GameObject> call)
            {
                int hash = call.GetHashCode();
                List<int> list;
                if (!calls.TryGetValue(eventID, out list))
                    return;

                if (list.Contains(hash))
                    return;

                callback.RemoveListener(call);
            }

            public void RemoveAllEventListener()
            {
                calls.Clear();
                callback.RemoveAllListeners();
            }
        }

        [FormerlySerializedAs("delegates")]
        [SerializeField]
        private List<Entry> m_Delegates;

        [Obsolete("Please use triggers instead (UnityUpgradable) -> triggers", true)]
        public List<Entry> delegates;
        public List<Entry> triggers
        {
            get
            {
                if (m_Delegates == null)
                    m_Delegates = new List<Entry>();
                return m_Delegates;
            }
            set { m_Delegates = value; }
        }
        //Duong : new Action
        public Action<string> ActionQuickVarListLongPress;
        public Action<string> ActionQuickVarListClick;
        public Action ActionShowSystemByLanguage;


        private Dictionary<EventTriggerType, Action<EventTriggerType, GameObject>> m_funcs = new Dictionary<EventTriggerType, Action<EventTriggerType, GameObject>>();

        public new Image targetGraphic;
        public Sprite slideSprite;
        public Sprite selectSprite;                     //选中状态显示
        public Sprite pressedSprite;                //按下状态显示
        public Sprite disabledSprite;               //禁用状态显示

        public Transform selectTrans;
        public Transform deselectTrans;

        public Transform parentTra;

        public Transform[] selectTransArray;
        public Transform[] deselectTransArray;

        public bool istoggle = false;

        public TriggerListenerGroup triggerListenerGroup;

        private RectTransform cacheRectTransform;
        public bool isscale = false;
        public float scaling = 0.9f;

        [Range(0, 1)]
        public float eventAlphaThreshold = 0;
        public bool enableAlphaThreshold = false;

        private bool m_crossDisabled = false;
        public bool crossDisabled
        {
            set { m_crossDisabled = value; }
            get { return m_crossDisabled; }
        }

        private bool m_disabled = false;
        public bool disabled
        {
            set
            {
                m_disabled = value;
                if (m_disabled)
                    DoSpriteSwap(disabledSprite);
                else
                    DoSpriteSwap(null);
            }
            get
            {
                return m_disabled;
            }

        } //禁用
        private bool selected = false;              //是否被选中
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (value)
                {
                    DoSelect();
                }
                else
                {
                    DoDeselect();
                }
            }
        }

        public List<object> triggerArg = new List<object>();
        public int custom;

        private Dictionary<EventTriggerType, System.Action> m_DisableActionDic = new Dictionary<EventTriggerType, System.Action>();
        public void SetDisableAction(EventTriggerType type, System.Action action)
        {
            if (!m_DisableActionDic.ContainsKey(type))
                m_DisableActionDic.Add(type, action);
        }

        protected TriggerListener()
        { }

        public static TriggerListener Get(GameObject go)
        {
            TriggerListener listener = go.GetComponent<TriggerListener>();
            if (listener == null)
                listener = go.AddComponent<TriggerListener>();
            return listener;
        }

        protected override void Awake()
        {

#if Main
            if (Application.isPlaying)
            {
#endif
            var coms = transform.GetComponents<TriggerListenerObjectLogic>();
            foreach (var com in coms)
            {
                GameObject.Destroy(com);
            }
#if Main
        }
#endif

#if Main
            if (Application.isPlaying)
                //GED.ED.dispatchEvent((int)BaseEventID.CreateTriggerListenerObjectLogic, gameObject);
#else
            //GED.ED.dispatchEvent((int)BaseEventID.CreateTriggerListenerObjectLogic, gameObject);
#endif



            //
            if (targetGraphic != null)
            {
                this.transition = Transition.SpriteSwap;
            }
            else
            {
                this.transition = Transition.ColorTint;
            }
            cacheRectTransform = transform.GetComponent<RectTransform>();
            if (triggerListenerGroup != null)
                triggerListenerGroup.AddTrigger(this);

            SetEventAlphaThreshold();
        }

        public float pressdelay = 0f;               //按下连续执行时间间隔 0表示每帧执行

        private bool isPointerDown = false;     //是否按下
        private float incrementaltime = 0;

        private float longPressTotalTime = 0.5f;       //触发长按时间
        private float longpressTime = 0;
        private bool isLongPress = false;

        private float longPressTotalTimeGuide = 3.0f;   //触发长按时间用于格挡引导
        private float longpressTimeGuide = 0;
        private bool isLongPressGuide = false;

        void Update()
        {
            if (isPointerDown)
            {
                if (incrementaltime >= pressdelay)
                {
                    //借用下EventTriggerType.UpdateSelected 枚举做按下持续执行事件
                    Execute(EventTriggerType.UpdateSelected);
                }
                incrementaltime += Time.deltaTime;
            }

            if (isLongPress)
            {
                if (longpressTime > longPressTotalTime)
                {
                    //借用下EventTriggerType.Submit 枚举做长按下执行事件
                    Execute(EventTriggerType.Submit);
                    isLongPress = false;
                }
                longpressTime += Time.deltaTime;
            }

            if (isLongPressGuide)
            {
                if (longpressTimeGuide > longPressTotalTimeGuide)
                {
                    isLongPressGuide = false;
                    if (ActionQuickVarListLongPress != null)
                    {
                        ActionQuickVarListLongPress.Invoke(name);
                    }
                }
                longpressTimeGuide += Time.deltaTime;
            }
        }

        public void AddEventListener(EventTriggerType eventTriggerType, Action<EventTriggerType, GameObject> func)
        {
            if (func == null)
            {
                return;
            }

            if (m_funcs.ContainsKey(eventTriggerType))
            {
                RemoveEventListener(eventTriggerType);
            }
            m_funcs.Add(eventTriggerType, func);
        }

        public void RemoveEventListener(EventTriggerType eventTriggerType)
        {
            if (m_funcs.ContainsKey(eventTriggerType))
            {
                m_funcs.Remove(eventTriggerType);
            }
        }

        private void Execute(EventTriggerType triggerType, BaseEventData eventData = null)
        {
            if (disabled)
            {
                if (m_DisableActionDic.ContainsKey(triggerType))
                    m_DisableActionDic[triggerType]();
                return;
            }
            Action<EventTriggerType, GameObject> func = null;
            if (m_funcs.TryGetValue(triggerType, out func))
            {
                func(triggerType, gameObject);
                return;
            }

            for (int i = 0, imax = triggers.Count; i < imax; ++i)
            {
                var ent = triggers[i];
                if (ent.eventID == triggerType)
                    ent.Invoke(triggerType, gameObject);
            }
        }

        void DoSpriteSwap(Sprite newSprite)
        {
            if (targetGraphic == null)
                return;
            targetGraphic.overrideSprite = newSprite;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            Execute(EventTriggerType.PointerEnter, eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            isPointerDown = false;
            longpressTime = 0;
            longpressTimeGuide = 0;
            incrementaltime = 0;
            isLongPress = false;
            isLongPressGuide = false;
            Execute(EventTriggerType.PointerExit, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Execute(EventTriggerType.Drag, eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            Execute(EventTriggerType.Drop, eventData);
        }

        private void SetEventAlphaThreshold()
        {
            if (enableAlphaThreshold == false)
                return;
            Image image = cacheRectTransform.GetComponent<Image>();
            if (image != null)
                image.alphaHitTestMinimumThreshold = eventAlphaThreshold;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (disabled)
                return;
            if (this.transition != Transition.SpriteSwap)
                DoStateTransition(SelectionState.Pressed, false);
            else
            {
                if (istoggle && selected)
                    DoSpriteSwap(selectSprite);
                else
                    DoSpriteSwap(pressedSprite);
            }
            DoScaleButton(scaling);

            isLongPress = true;
            isLongPressGuide = true;
            isPointerDown = true;
            incrementaltime = 0;
            // Execute(EventTriggerType.UpdateSelected);

            Execute(EventTriggerType.PointerDown, eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (disabled)
                return;
            if (this.transition != Transition.SpriteSwap)
                DoStateTransition(SelectionState.Normal, false);
            else
            {
                if (istoggle && selected)
                    DoSpriteSwap(selectSprite);
                else
                    DoSpriteSwap(null);
            }
            DoScaleButton(1);
            isPointerDown = false;
            isLongPress = false;
            isLongPressGuide = false;
            incrementaltime = 0;
            Execute(EventTriggerType.PointerUp, eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_crossDisabled)
            {
                if (ActionShowSystemByLanguage != null)
                {
                    ActionShowSystemByLanguage.Invoke();
                }
                return;
            }
            if (disabled)
            {
                if (m_DisableActionDic.ContainsKey(EventTriggerType.PointerClick))
                    m_DisableActionDic[EventTriggerType.PointerClick]();
                return;
            }

            if (longpressTime > longPressTotalTime)
            {
                if (longpressTimeGuide > longPressTotalTimeGuide)
                {
                    longpressTimeGuide = 0;
                }

                longpressTime = 0;
                return;
            }
            longpressTime = 0;
            longpressTimeGuide = 0;
            if (istoggle)
            {
                if (selected == false)
                {
                    DoSelect();
                }
                else
                {
                    if (triggerListenerGroup == null)
                    {
                        DoDeselect();
                    }
                }
            }

            if (ActionQuickVarListLongPress != null)
            {
                ActionQuickVarListLongPress.Invoke(name);
            }

            Execute(EventTriggerType.PointerClick, eventData);
        }

        private void DoScaleButton(float scale)
        {
            if (isscale)
            {
                if (null == targetGraphic)
                {
                    cacheRectTransform.localScale = Vector3.one * scale;
                }
                else
                {
                    targetGraphic.GetComponent<RectTransform>().localScale = Vector3.one * scale;
                }
            }
        }

        public void DoSelect()
        {
            selected = true;
            DoSpriteSwap(selectSprite);
            if (triggerListenerGroup != null)
            {
                triggerListenerGroup.SelectTrigger(this);
            }
            ChangeSelectTrans(selected);
            Execute(EventTriggerType.Select, null);
        }

        public void DoSelectDontExcute()
        {
            selected = true;
            DoSpriteSwap(selectSprite);
            ChangeSelectTrans(selected);
        }

        public void DoDeselect()
        {
            selected = false;
            DoSpriteSwap(null);
            Execute(EventTriggerType.Deselect, null);
            ChangeSelectTrans(selected);
        }

        public void ChangeSelectTrans(bool isselected)
        {
            if (parentTra)
                SetChildActive(parentTra, 0);

            if (selectTrans != null)
                selectTrans.gameObject.SetActive(isselected);
            if (deselectTrans != null)
                deselectTrans.gameObject.SetActive(!isselected);

            for (int i = 0; i < selectTransArray.Length; i++)
                SetActive(selectTransArray[i], isselected);
            for (int i = 0; i < deselectTransArray.Length; i++)
                SetActive(deselectTransArray[i], !isselected);
        }
        public static void SetChildActive(Transform trans, int startIndex)
        {
            int count = trans.childCount;
            for (int i = 0; i < count; i++)
            {
                Transform child = trans.GetChild(i);
                if (child != null)
                {
                    if (i >= startIndex)
                        child.gameObject.SetActive(false);
                    else
                        child.gameObject.SetActive(true);
                }
            }
        }
        public static void SetActive(Transform trans, bool state)
        {
            if (!trans)
                return;

            trans.gameObject.SetActive(state);
        }
        public override void OnMove(AxisEventData eventData)
        {
            Execute(EventTriggerType.Move, eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Execute(EventTriggerType.BeginDrag, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Execute(EventTriggerType.EndDrag, eventData);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            Execute(EventTriggerType.Submit, eventData);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (triggerListenerGroup != null)
                triggerListenerGroup.RemoveTrigger(this);
            m_funcs.Clear();
            m_funcs = null;
        }

        //protected override void OnDisable()
        //{
        //    disabled = false;
        //    selected = false;
        //    DoSpriteSwap(null);
        //}
        //protected override void OnEnable()
        //{
        //    if (!disabled)
        //    {
        //        DoSpriteSwap(disabledSprite);
        //    }
        //}

    }
}

