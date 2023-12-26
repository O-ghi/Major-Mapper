/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2017.09.26
*/
using UnityEngine;

public class GoLoader : MonoBehaviour
{
    public string layer;
    public string resName;
    public bool async;
    private bool loaded;
    private bool killed;

    private void Start()
    {
        if (string.IsNullOrEmpty(resName))
            return;

        if (async)
            ResManager.Singleton.Request(resName, onABLoaded);
        else
            onResLoaded(resName, resName, null);
    }

    private void onABLoaded(string res)
    {
        if (killed)
            return;
        ResManager.Singleton.GetLoadedObjSync(res, res, null, onResLoaded);
    }

    private void onResLoaded(string res, string dep, System.Type type)
    {
        if (killed)
            return;
        loaded = true;
        var go = ResManager.Singleton.LoadObjSync(resName) as GameObject;
        if (go != null)
        {
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        }

        if (!string.IsNullOrEmpty(layer) && go != null)
        {
            int layerInt = LayerMask.NameToLayer(layer);
            Transform[] childs = go.transform.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < childs.Length; ++i)
                childs[i].gameObject.layer = layerInt;
        }
    }

    private void OnDestroy()
    {
        killed = true;
        if (loaded)
            ResManager.Singleton.ReturnObj(resName);
    }
}
