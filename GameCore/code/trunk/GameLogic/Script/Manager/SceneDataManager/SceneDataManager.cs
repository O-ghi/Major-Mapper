using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDataManager : ManagerTemplate<SceneDataManager> 
{
    public static SceneBase currentScene;

    public Dictionary<int , SceneBase> sceneList;
    protected override void InitManager()
    {
        ManagerLogic.Singleton.AddManagerUpdate(this.GetType(), Update);
        sceneList = new Dictionary<int, SceneBase>();
    }

    public void SetScene(SceneBase scene, QuickVarList varList = null)
    {
        currentScene = scene;
        if (varList != null)
        {
            scene._taskCoreCfg = (TaskCoreCfg) varList.GetObject(0);
        }
        sceneList.Add(scene._taskCoreCfg.ID, scene);

    }

    protected override void Update()
    {
    }
}
