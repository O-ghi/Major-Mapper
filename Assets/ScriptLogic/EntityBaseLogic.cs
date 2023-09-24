using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityBaseLogic : SingletonTemplate<EntityBaseLogic>
{
    #region BASEENTITY 

    private Dictionary<int, EntityBase> entityDics = new Dictionary<int, EntityBase>();
    private List<EntityBase> entityList = new List<EntityBase>();
    public EntityBase GetComponentEntity(int GetInstanceID)
    {
        if (entityDics.ContainsKey(GetInstanceID))
        {
            return entityDics[GetInstanceID];
        }
        return null;

    }

    public void AddComponentEntity(int GetInstanceID, EntityBase entity)
    {
        if (!entityDics.ContainsKey(GetInstanceID))
        {
            entityDics.Add(GetInstanceID, entity);
            entityList = entityDics.Values.ToList();

        }
    }

    public void RemoveComponentEntity(int GetInstanceID)
    {
        if (entityDics.ContainsKey(GetInstanceID))
        {
            entityDics[GetInstanceID] = null;
            entityDics.Remove(GetInstanceID);
            entityList = entityDics.Values.ToList();

        }
    }
    #endregion

    #region UNITY OVERRIDE
    public void Update()
    {
        for (int i = 0; i < entityList.Count; i++)
        {
                entityList[i].Update();
            
        }
    }
    public void LateUpdate()
    {
        for (int i = 0; i < entityList.Count; i++)
        {
           
                entityList[i].LateUpdate();
            
        }
    }

    #endregion
}
