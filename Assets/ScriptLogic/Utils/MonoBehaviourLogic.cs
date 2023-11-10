using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourLogic : MonoBehaviourSingleton<MonoBehaviourLogic>
{
    private EntityBaseLogic entityBaseLogic;
    private ManagerLogic managerLogic;

    protected override void Awake()
    {

        entityBaseLogic = EntityBaseLogic.Singleton;
        managerLogic = ManagerLogic.Singleton;
        managerLogic.InitManager();
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
