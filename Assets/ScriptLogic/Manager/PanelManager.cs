using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PanelManager : ManagerTemplate<PanelManager>
{
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
        GameObject uiCanvas = GameObject.Find("_UICanvas");
        if (uiCanvas == null)
        {
            UnityEngine.Object uisysprefab = Resources.Load("_UICanvas", typeof(GameObject));
            uiCanvas = GameObject.Instantiate(uisysprefab) as GameObject;
            uiCanvas.name = "_UICanvas";
        }
        GameObject.DontDestroyOnLoad(uiCanvas);
        parent = uiCanvas.transform;
    }

    public static GameObject LoadPanel (string uiName)
    {
        string uipath = Path.Combine(Application.streamingAssetsPath ,string.Format( "ui/{0}", uiName.ToLower()));
        //Debug.Log("_________Chay tu day! + uipath| " + uipath);
        AssetBundle assetBundle = AssetBundle.LoadFromFile(uipath);
        GameObject prefab = assetBundle.LoadAsset<GameObject>(uiName);
        if (prefab == null)
            return null;
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        //Debuger.Log("Setpanel uipath :" + uipath + "  ||| go is " + (go != null));
        go.transform.SetParent(Parent);
        go.transform.position = Vector3.zero;
        return go ;
    }
}
