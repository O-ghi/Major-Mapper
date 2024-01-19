using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;


public delegate void PanelCompleteHandler(UIBase uibase);

public class PanelManager : ManagerTemplate<PanelManager>
{
    private static List<UIBase> m_panels;
    private static Transform parent;
    public static Transform Parent
    {
        get
        {
            return parent;
        }
    }
    protected override void InitManager()
    {
        base.InitManager();
        ManagerLogic.Singleton.AddManagerUpdate(this.GetType(), Update);

        GameObject uiCanvas = GameObject.Find("_UICanvas");
        if (uiCanvas == null)
        {
            UnityEngine.Object uisysprefab = Resources.Load("_UICanvas", typeof(GameObject));
            uiCanvas = GameObject.Instantiate(uisysprefab) as GameObject;
            uiCanvas.name = "_UICanvas";
        }
        GameObject.DontDestroyOnLoad(uiCanvas);
        parent = uiCanvas.transform;
        m_panels = new List<UIBase>();
    }

    protected override void Update()
    {
        base.Update();
        for (int i = 0; i < m_panels.Count; i++)
        {
            m_panels[i].GameUpdate();
        }
    }
    public static UIBase SetPanel(string uiName, QuickVarList varlist = null)
    {
        if (PanelExits(uiName))
        {
            return OpenPanel(uiName, varlist);
        }
        GameObject panel = LoadPanel(uiName);

        var panelLogic = System.Activator.CreateInstance(Type.GetType(uiName)) as UIBase;
        panelLogic.gameObject = panel;
        panelLogic.GameAwake(panel.transform);
        panelLogic.Initialization(varlist);

        m_panels.Add(panelLogic);
        return panelLogic;
    }
    public static GameObject LoadPanel(string uiName)
    {

        string uipath = Path.Combine(Application.streamingAssetsPath, string.Format("ui/{0}", uiName.ToLower()));
        //Debug.Log("_________Chay tu day! + uipath| " + uipath);
        AssetBundle assetBundle = AssetBundle.LoadFromFile(uipath);
        GameObject prefab = assetBundle.LoadAsset<GameObject>(uiName);
        if (prefab == null)
            return null;
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        //Debuger.Log("Setpanel uipath :" + uipath + "  ||| go is " + (go != null));
        go.name = uiName;
        go.transform.SetParent(Parent);
        go.transform.localPosition = Vector3.zero;

        return go;
    }

    public static UIBase OpenPanel(string uiName, QuickVarList varList)
    {
        var panel = GetPanel(uiName);
        if (panel == null)
            return SetPanel(uiName, varList);
        panel.gameObject.SetActive(true);
        panel.Refresh(varList);
        return panel;
    }
    private static bool PanelExits(string uiName)
    {
        for (int i = 0; i < m_panels.Count; i++)
        {
            if (m_panels[i].name.Equals(uiName))
            {
                return true;
            }
        }
        return false;
    }
    public static UIBase GetPanel(string uiName)
    {
        int len = m_panels.Count;
        for (int i = 0; i < len; i++)
        {
            UIBase ui = m_panels[i];
            if (ui.uiName.Equals(uiName, StringComparison.Ordinal))
            {
                return ui;
            }
        }
        return null;
    }

    public static T GetPanel<T>(string uiName) where T : UIBase
    {
        UIBase ui = GetPanel(uiName);
        if (ui != null)
        {
            return ui as T;
        }
        return null;
    }

    public static Transform GetPanelTransform(string uiName)
    {
        UIBase ui = GetPanel(uiName);
        if (ui != null)
        {
            return ui.cacheTransform;
        }
        return null;
    }
    public static void ClosePanel(string uiName)
    {

    }
    public static void DestroyPanel(string uiName, bool show = true)
    {

    }
    private static void OnExitScene()
    {
        Clear();
        //Debuger.Log("OnExitScene PanelManager");
        QuickVarList varlist = QuickVarList.Get();
        varlist.AddInt((int)LoadingEnum.Scene);
        varlist.AddString(" ");
        //OpenPanel("LoadingPanel", varlist);

        //if (coroutineShowSysMsg != null)
        //{
        //    Instance.StopCoroutine(coroutineShowSysMsg);

        //}
    }

    public static void Clear()
    {

        for (int i = 0; i < m_panels.Count; i++)
        {
            m_panels[i].gameObject.SetActive(false);
        }


    }
}
