using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ManagerLogic : SingletonTemplate<ManagerLogic>
{
    private Dictionary<Type, Action> listManagerUpdate;
    private Dictionary<Type, Action> listManagerLateUpdate;
    private Dictionary<Type, Action> listManagerDestroy;
    //
    private Action[] actionUpdateTempDics;
    private Action[] actionLateUpdateTempDics;
    private Action[] actionEffectTempDics;

    private Dictionary<int, Action> actionLateUpdateObj;
    private Action[] actionLateUpdateObjList;
    public void InitManager()
    {
        listManagerUpdate = new Dictionary<Type, Action>();
        listManagerLateUpdate = new Dictionary<Type, Action>();
        listManagerDestroy = new Dictionary<Type, Action>();
        actionLateUpdateObj = new Dictionary<int, Action>();
    }
    #region Manager
    //Update
    public void AddManagerUpdate(Type type, Action _update)
    {
        if (!listManagerUpdate.ContainsKey(type))
        {
            if (_update != null)
            {
                listManagerUpdate.Add(type, _update);

                actionUpdateTempDics = listManagerUpdate.Values.ToArray();

            }
            else
            {
                Debuger.Err("uiname " + type.ToString() + " is null");

            }
        }
        else
        {
            Debuger.Err("uiname " + type.ToString() + " exists in dictionary");
        }
    }
    public void RemoveManagerUpdate(Type type)
    {
        if (listManagerUpdate.ContainsKey(type))
        {
            var uiObj = listManagerUpdate[type];
            if (uiObj != null)
                uiObj = null;
            listManagerUpdate.Remove(type);

            actionUpdateTempDics = listManagerUpdate.Values.ToArray();

        }
        else
        {
            Debuger.Err("uiname " + type.ToString() + " not exists in dictionary");
        }
    }

    //LateUpdate
    public void AddManagerLateUpdate(Type type, Action _update)
    {
        if (!listManagerLateUpdate.ContainsKey(type))
        {
            if (_update != null)
            {
                listManagerLateUpdate.Add(type, _update);

                actionLateUpdateTempDics = listManagerLateUpdate.Values.ToArray();

            }
            else
            {
                Debuger.Err("Lateupdate " + type.ToString() + " is null");

            }
        }
        else
        {
            Debuger.Err("Lateupdate " + type.ToString() + " exists in dictionary");
        }
    }
    public void RemoveManagerLateUpdate(Type type)
    {
        if (listManagerLateUpdate.ContainsKey(type))
        {
            var uiObj = listManagerUpdate[type];
            if (uiObj != null)
                uiObj = null;
            listManagerLateUpdate.Remove(type);

            actionLateUpdateTempDics = listManagerLateUpdate.Values.ToArray();

        }
        else
        {
            Debuger.Err("Lateupdate " + type.ToString() + " not exists in dictionary");
        }
    }
    //Destroy
    public void AddManagerDestroy(Type type, Action _destroy)
    {
        //Debuger.Log("add type:" + type + " AddManagerDestroy");
        if (!listManagerDestroy.ContainsKey(type))
        {
            if (_destroy != null)
            {
                listManagerDestroy.Add(type, _destroy);
            }
            else
            {
                Debuger.Err("uiname " + type.ToString() + " is null");

            }
        }
        else
        {
            Debuger.Err("uiname " + type.ToString() + " exists in dictionary");
        }
    }
    #endregion
   
    #region LateUpdate Gameobject
    public void RegisterLateUpdateObj(int id, Action callback)
    {
        if (!actionLateUpdateObj.ContainsKey(id))
        {
            if (callback != null)
            {
                actionLateUpdateObj.Add(id, callback);
                actionLateUpdateObjList = actionLateUpdateObj.Values.ToArray();
            }
        }
    }
    public void UnRegisterLateUpdateObj(int id, Action callback)
    {
        if (actionLateUpdateObj.ContainsKey(id))
        {
            actionLateUpdateObj.Remove(id);
            actionLateUpdateObjList = actionLateUpdateObj.Values.ToArray();

        }
    }
    #endregion
    #region Mono behaviours function

    // Update is called once per frame
    public void Update()
    {
        if (actionUpdateTempDics != null)
        {
            //update manager
            for (int i = 0; i < actionUpdateTempDics.Length; i++)
            {
                //var uiObj = updatTempDics[i];
                if (actionUpdateTempDics[i] != null)
                {
                    actionUpdateTempDics[i].Invoke();
                }
                else
                {
                    Debuger.Err("ui" + listManagerUpdate.ElementAt(i).Key + " has been destroyed but you are still trying to dictionary");
                }
            }
        }

    }

    public void LateUpdate()
    {
        //Late update manager
        if (listManagerLateUpdate != null)
        {
            for (int i = 0; i < listManagerLateUpdate.Count; i++)
            {
                //var uiObj = updatTempDics[i];
                if (actionLateUpdateTempDics[i] != null)
                {
                    actionLateUpdateTempDics[i].Invoke();
                }
                else
                {
                    Debuger.Err("ui" + listManagerLateUpdate.ElementAt(i).Key + " has been destroyed but you are still trying to dictionary");
                }
            }
        }
        
        if (actionLateUpdateObjList != null)
        {
            for (int i = 0; i < actionLateUpdateObjList.Length; i++)
            {
                actionLateUpdateObjList[i].Invoke();
            }
        }

    }

    public void OnDestroy()
    {
#if Main
        for (int i = 0; i < listManagerDestroy.Count; i++)
        {
            var uiObj = listManagerDestroy.ElementAt(i).Value;
            if (uiObj != null)
            {
                uiObj.Invoke();
            }
        }
#endif
    }
    #endregion

}
