using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MonoBehaviourLogic.CreateInstance();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject gameObject = new GameObject();

            EntityBase entity = null;
            entity = new PlayerEntity(gameObject) as EntityBase;
            EntityBaseLogic.Singleton.AddComponentEntity(entity.gameObject.GetInstanceID(), entity);
        }
    }
}
