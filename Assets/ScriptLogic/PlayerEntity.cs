using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : EntityBase
{
    public PlayerEntity(GameObject _obj) : base(_obj)
    {
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        Debug.Log("Player Entity");
    }
}
