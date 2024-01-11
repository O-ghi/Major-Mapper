using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SceneData
{
    public Dictionary<int, Item> Inventory { get; set; }
    public List<TaskData> Tasks { get; set; }
    public bool SceneStatus = false;
    public int _TaskCore;
    public PersonalityBase _Personality;
    public SceneData(int taskCore, PersonalityBase personality)
    {
        Inventory = new Dictionary<int, Item>();
        Tasks = new List<TaskData>();
        _TaskCore = taskCore;
        _Personality = personality;
    }
    public void AddItem(ItemCfg itemcfg)
    {
        Item item;
        if (!Inventory.TryGetValue(itemcfg.Id, out item))
        {
            item = new Item();
            item.item = itemcfg;
            Inventory.Add(itemcfg.Id, item);
        }
        item.total++;
    }
    public void AddItem(int id)
    {
        Item item;
        if (!Inventory.TryGetValue(id, out item))
        {
            item = new Item();
            item.item = ConfigManager.Instance.Get<ItemCfgLoader>().GetCfg(id);
            Inventory.Add(id, item);
        }
        item.total++;
        GameEventManager.Singleton.UpdateItem(id);
    }

    public int GetItemCount(int id)
    {
        Item item;
        if (Inventory.TryGetValue(id, out item))
        {
            return item.total;
        }
        return 0;
    }
    public void ReduceItem(int id, int num)
    {
        Item item;
        if (Inventory.TryGetValue(id, out item))
        {
            item.total = item.total - num;
        }
    }

    public void RemoveItem(int id)
    {
        Item item;
        if (Inventory.TryGetValue(id, out item))
            Inventory.Remove(id);
    }

    public void AddTask(int id)
    {
        TaskStepCfgLoader taskStepCfgLoader = ConfigManager.Instance.Get<TaskStepCfgLoader>();
        TaskStepCfg task = taskStepCfgLoader.GetCfg(_TaskCore, id);
        if (task != null)
        {
            TaskData taskData = new TaskData()
            {
                task = task,
                statusTask = StatusTask.CanAccept,
            };
            Tasks.Add(taskData);
            TaskShowPanel taskShowPanel = PanelManager.SetPanel("TaskShowPanel") as TaskShowPanel;
            taskShowPanel.SetTitle(task.Name + " (Chưa nhận)");

        }
        else
        {
            TaskShowPanel taskShowPanel = PanelManager.SetPanel("TaskShowPanel") as TaskShowPanel;
            taskShowPanel.SetTitle("Đã hoàn thành màn chơi");
        }
    }
    public TaskData GetCurrentTask()
    {
        if (Tasks.Count == 0)
        {
            return null;
        }
        return Tasks[Tasks.Count - 1];
    }

    public void AddPoint(int point, int personalityType, PersonalityMethod method)
    {
        switch (method)
        {
            case PersonalityMethod.Holland:
                {
                    switch (personalityType)
                    {
                        case (int)HollandType.REALISTIC:
                            {
                                (_Personality as HollandPersonality).Realistic++;
                                break;
                            }
                        case (int)HollandType.INVESTIGATIVE:
                            {
                                (_Personality as HollandPersonality).Investigative++;
                                break;
                            }
                        case (int)HollandType.ARTISTIC:
                            {
                                (_Personality as HollandPersonality).Artistic++;
                                break;
                            }
                        case (int)HollandType.SOCIAL:
                            {
                                (_Personality as HollandPersonality).Social++;
                                break;
                            }
                        case (int)HollandType.ENTERPRISING:
                            {
                                (_Personality as HollandPersonality).Enterprising++;
                                break;
                            }
                        case (int)HollandType.CONVENTIONAL:
                            {
                                (_Personality as HollandPersonality).Conventional++;
                                break;
                            }
                    }
                    break;
                }
            case PersonalityMethod.MBTI:
                {

                    break;
                }
        }
    }

}

public class Item
{
    public ItemCfg item;
    public int total;
}

public class TaskData
{
    public TaskStepCfg task;
    public StatusTask statusTask;
}

public enum StatusTask
{
    CanAccept = 0,
    InProgress,
    CanSubmit,
    Complete,
}