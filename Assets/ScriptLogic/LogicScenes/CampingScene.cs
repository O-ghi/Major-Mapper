using UnityEngine;

public class CampingScene : SceneBase
{
    NpcEntity _CampingNPC_1;
    NpcEntity _CampingNPC_2;
    PlayerEntity player;

    public CampingScene(string sceneName, TaskCoreCfg taskCoreCfg) : base(sceneName, taskCoreCfg)
    {
    }

    protected override void Init(string sceneName)
    {
        base.Init(sceneName);
        //_bornPos = GameObject.Find("BornPos").gameObject;
        InitEntity();
        InitTask();
    }

    private void InitEntity()
    {
        player = new PlayerEntity(GameObject.Find("Player"));
        _CampingNPC_1 = new NpcEntity(GameObject.Find("camping_NPC_1"));
        _CampingNPC_2 = new NpcEntity(GameObject.Find("camping_NPC_2"));

    }

    private void InitTask()
    {
        _sceneData.AddTask(1);
    }
}
