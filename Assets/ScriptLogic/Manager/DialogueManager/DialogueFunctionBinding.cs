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
}