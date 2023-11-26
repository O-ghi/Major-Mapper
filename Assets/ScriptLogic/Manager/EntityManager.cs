using System.Collections.Generic;
using UnityEngine;

public class EntityManager : ManagerTemplate<EntityManager>
{
    private static Dictionary<string, EntityBase> entityMap;

    protected override void InitManager()
    {
        ManagerLogic.Singleton.AddManagerUpdate(this.GetType(), Update);
        entityMap = new Dictionary<string, EntityBase>();
        
        //GameEventManager.RegisterEvent(GameEventTypes.EnterScene, OnEnterScene);
        //GameEventManager.RegisterEvent(GameEventTypes.ExitScene, OnExitScene);
    }
    public static void AddEntity(EntityBase entity)
    {
        
        if (entityMap.ContainsKey(entity.name))
        {
            Debug.Log("Đã có Entity " + entity.name);
            return;
        }
        entity.InitEntity();
        entityMap.Add(entity.name, entity);
    }
    public static PlayerEntity GetMainPlayerEntity()
    {
        return GetEntity<PlayerEntity>("Player");
    }

    public static T GetEntity<T>(string name) where T : EntityBase
    {
        EntityBase entity = null;
        entityMap.TryGetValue(name, out entity);
        return entity as T;
    }
}