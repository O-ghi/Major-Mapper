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
    }

    public void AcceptTask(TaskData task)
    {
        Debug.Log("AcceptTask " + task.task.ID + " | " +  task.task.Step);
        ChangeStatusTask(task, StatusTask.InProgress);
    }
    public void SubmitTask(TaskData task)
    {
        Debug.Log("SubmitTask " + task.task.ID + " | " + task.task.Step);

    }

    public void ChangeStatusTask(TaskData task, StatusTask status)
    {
        task.statusTask = status;
        GameEventManager.Singleton.ChangeStatusTask(task);
    }

}
