using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourLogic : MonoBehaviourSingleton<MonoBehaviourLogic>
{
    private EntityBaseLogic entityBaseLogic;
    private ManagerLogic managerLogic;
    private GameEventManager gameEventManager;
    protected override void Awake()
    {
        base.Awake();
        entityBaseLogic = EntityBaseLogic.Singleton;
        managerLogic = ManagerLogic.Singleton;
        managerLogic.InitManager();
        gameEventManager = GameEventManager.Singleton;
    }

    // Update is called once per frame


    void Update()
    {
        if (entityBaseLogic != null)
        {
            entityBaseLogic.Update();
        }
        if (managerLogic != null)
        {
            managerLogic.Update();
        }
    }
}
