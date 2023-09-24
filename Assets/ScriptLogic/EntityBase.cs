using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase
{
    public Transform transform;
    public GameObject gameObject;
    public string name;
    public int instanceId;

    public EntityBase(GameObject _obj)
    {
        this.transform = _obj.transform;
        this.gameObject = _obj;
        this.name = _obj.name;
        this.instanceId = _obj.GetInstanceID();
    }
    //Override
    protected virtual void OnUpdate()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update() 
    {
        OnUpdate();
    }


    public void LateUpdate()
    {

    }
}
