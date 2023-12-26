using Ink.Runtime;
using UnityEngine;

public class DialogueFunctionBinding
{
	public void AcceptTaskBind(Story story, TaskData taskData)
	{
		story.BindExternalFunction("acceptTask", () => AcceptTask(taskData));
	}

	public void AcceptTaskUnBind(Story story)
	{
		story.UnbindExternalFunction("acceptTask");
	}

	public void AcceptTask(TaskData taskData)
	{
        Debug.Log("binding accepttask");
		if (taskData == null)
		{
			return;
		}
		GameEventManager.Singleton.AcceptTask(taskData);

	}

    public void SubmitTaskBind(Story story, TaskData taskData)
    {
        story.BindExternalFunction("submitTask", () => SubmitTask(taskData));
    }

    public void SubmitTaskUnBind(Story story)
    {
        story.UnbindExternalFunction("submitTask");
    }

    public void SubmitTask(TaskData taskData)
    {
        Debug.Log("binding submitTask");
        if (taskData == null)
        {
            return;
        }
        GameEventManager.Singleton.SubmitTask(taskData);

    }
}