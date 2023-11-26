using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDataManager : ManagerTemplate<SceneDataManager> 
{
    public static SceneBase currentScene;
    protected override void InitManager()
    {
        ManagerLogic.Singleton.AddManagerUpdate(this.GetType(), Update);

    }

    public void SetScene(SceneBase scene, QuickVarList varList = null)
    {
        currentScene = scene;
        if (varList != null)
        {
            scene._taskCoreCfg = (TaskCoreCfg) varList.GetObject(0);
        }
    }

    protected override void Update()
    {
    }
}
