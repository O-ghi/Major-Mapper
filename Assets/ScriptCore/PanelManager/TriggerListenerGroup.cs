using UnityEngine;
using System.Collections;


using System.Collections.Generic;
using System;
using UnityEngine.Events;

namespace UnityEngine.EventSystems
{
    [AddComponentMenu("Event/TriggerListener Group")]
    
    public class TriggerListenerGroup : MonoBehaviour
    {
        public TriggerListener selectedTrigger;
		[HideInInspector]
		public List<Transform> deselctTrasList;

        private List<TriggerListener> m_triggers = new List<TriggerListener>();
        
        public void SelectTrigger(TriggerListener trigger)
        {
            if (selectedTrigger != null && selectedTrigger != trigger)
            {
				selectedTrigger.DoDeselect();
            }

			if (null != deselctTrasList)
				deselctTrasList.ForEach(t => SetActive(t,false));

			selectedTrigger = trigger;
        }
        public static void SetActive(Transform trans, bool state)
        {
            if (!trans)
                return;

            trans.gameObject.SetActive(state);
        }

        [System.Serializable]
        public class OnTriggerFinishEvent : UnityEvent<object> { }
        public OnTriggerFinishEvent OnFinishHandler = new OnTriggerFinishEvent();
        

        public void DoFinish()
        {
            foreach (var trigger in m_triggers)
            {
                if (trigger != null)
                {
                    int count = trigger.triggerArg.Count;
                    for (int i = 0; i < count; i++ )
                    {
                        OnFinishHandler.Invoke(trigger.triggerArg[i]);
                    }
                }
            }
        }

        public void AddTrigger(TriggerListener trigger)
        {
            if (!m_triggers.Contains(trigger))
                m_triggers.Add(trigger);
        }

        public void RemoveTrigger(TriggerListener trigger)
        {
            if (m_triggers.Contains(trigger))
                m_triggers.Remove(trigger);
        }
    }
}
