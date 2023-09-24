using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourLogic : MonoBehaviourSingleton<MonoBehaviourLogic>
{
    private EntityBaseLogic entityBaseLogic;

    // Start is called before the first frame update
    void Start()
    {

        entityBaseLogic = EntityBaseLogic.Singleton;
    }

    // Update is called once per frame


    void Update()
    {
        if (entityBaseLogic != null)
        {
            entityBaseLogic.Update();
        }
    }
}
