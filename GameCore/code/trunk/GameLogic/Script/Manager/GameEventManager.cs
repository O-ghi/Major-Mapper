using System;

public class GameEventManager: SingletonTemplate<GameEventManager>
{
    #region TaskEvent
    public Action<TaskData> onAcceptTask;
    public void AcceptTask(TaskData taskData)
    {
        if (onAcceptTask != null)
        {
            onAcceptTask(taskData);
        }
    }

    public Action<TaskData> onSubmitTask;
    public void SubmitTask(TaskData taskData)
    {
        if (onSubmitTask != null)
        {
            onSubmitTask(taskData);
        }
    }

    public Action<TaskData> onChangeStatusTask;
    public void ChangeStatusTask(TaskData taskData)
    {
        if(onChangeStatusTask != null)
        {
            onChangeStatusTask(taskData);
        }
    }

    public Action<int> onUpdateItem;
    public void UpdateItem(int id)
    {
        if (onUpdateItem != null)
        {
            onUpdateItem(id);
        }
    }

    #endregion

    #region InputEvent
    public Action onInteract;
    public void Interact()
    {
        if (onInteract != null)
        {
            onInteract();
        }
    }
    #endregion
}
