using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : ManagerTemplate<TaskManager>
{

    protected override void InitManager()
    {
        ManagerLogic.Singleton.AddManagerUpdate(this.GetType(), Update);
        InitEvent();
    }

    public void InitEvent()
    {
        GameEventManager.Singleton.onAcceptTask += AcceptTask;
        GameEventManager.Singleton.onSubmitTask += SubmitTask;
        GameEventManager.Singleton.onUpdateItem += CheckItem;
    }

    public void AcceptTask(TaskData task)
    {
        Debug.Log("AcceptTask " + task.task.ID + " | " + task.task.Step);
        TaskShowPanel taskShowPanel = PanelManager.SetPanel("TaskShowPanel") as TaskShowPanel;
        if (string.IsNullOrEmpty(task.task.ItemNeeds))
        {
            taskShowPanel.SetTitle(task.task.Name);

            ChangeStatusTask(task, StatusTask.CanSubmit);

        }
        else
        {
            string[] itemNeed = task.task.ItemNeeds.Split(',');

            taskShowPanel.SetTitle(task.task.Name + string.Format("{0}/{1}", SceneDataManager.currentScene._sceneData.GetItemCount(Int32.Parse(itemNeed[0])), itemNeed[1]));

            ChangeStatusTask(task, StatusTask.InProgress);

        }
    }
    public void SubmitTask(TaskData task)
    {
        Debug.Log("SubmitTask " + task.task.ID + " | " + task.task.Step);
        //GetReward
        ChangeStatusTask(task, StatusTask.Complete);


        SceneDataManager.currentScene._sceneData.AddTask(++task.task.Step);
    }

    public void CheckItem(int id)
    {
        var task = SceneDataManager.currentScene._sceneData.GetCurrentTask();
        if (!string.IsNullOrEmpty(task.task.ItemNeeds))
        {
            string[] itemNeed = task.task.ItemNeeds.Split(',');
            if (Int32.Parse(itemNeed[0]) == id)
            {
                TaskShowPanel taskShowPanel = PanelManager.SetPanel("TaskShowPanel") as TaskShowPanel;
                taskShowPanel.SetTitle(task.task.Name + " " + string.Format("{0}/{1}", SceneDataManager.currentScene._sceneData.GetItemCount(id), itemNeed[1]));
                if (SceneDataManager.currentScene._sceneData.GetItemCount(id) >= Int32.Parse(itemNeed[1]))
                {
                    task.statusTask = StatusTask.CanSubmit;
                    GameEventManager.Singleton.ChangeStatusTask(task);
                }
            }
        }
    }
    public void ChangeStatusTask(TaskData task, StatusTask status)
    {
        task.statusTask = status;
        GameEventManager.Singleton.ChangeStatusTask(task);
    }

}
