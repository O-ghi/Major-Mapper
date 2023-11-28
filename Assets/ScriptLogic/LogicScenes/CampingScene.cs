using UnityEngine;

public class CampingScene : SceneBase
{
    NpcEntity _CampingNPC_1;
    NpcEntity _CampingTable_1;
    PlayerEntity player;

    public CampingScene(string sceneName, TaskCoreCfg taskCoreCfg) : base(sceneName, taskCoreCfg)
    {
    }

    protected override void Init(string sceneName)
    {
        base.Init(sceneName);
        //_bornPos = GameObject.Find("BornPos").gameObject;
        TouchPanel touchPanel = PanelManager.SetPanel("TouchPanel") as TouchPanel;
        touchPanel.ChangeMode(0);
        InitEntity();
        InitTask();
    }

    private void InitEntity()
    {
        player = new PlayerEntity(GameObject.Find("Player"));
        _CampingNPC_1 = new NpcEntity(GameObject.Find("camping_NPC_1"));
        _CampingTable_1 = new NpcEntity(GameObject.Find("camping_Table_1"));

    }

    private void InitTask()
    {
        _sceneData.AddTask(1);
    }
}
