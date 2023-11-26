using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcEntity : EntityBase
{
    private bool playerIsNear = false;
    public NpcEntity(GameObject _obj) : base(_obj)
    {
        GameEventManager.Singleton.onInteract += InteractPressed;
    }



    protected override void OnUpdate()
    {
        base.OnUpdate();
        CheckCollisions();
    }

    protected void CheckCollisions()
    {
        // Perform your collision checks here
        // Example: Check for collisions with all colliders in the scene
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.name.Equals("Player"))
            {
                playerIsNear = true;
                return;
            }
        }
        playerIsNear = false;
    }

    private void InteractPressed()
    {
        if (!playerIsNear)
        {
            return;
        }
        SceneData sceneData = SceneDataManager.currentScene._sceneData;
        TaskData taskData = sceneData.GetCurrentTask();
        if (taskData == null)
        {
            Debug.Log("Chưa có task");
            return;
        }
        if (taskData.task.SubmitNPC.Equals(this.name))
        {
            if (taskData.statusTask == StatusTask.CanAccept)
            {
                if (string.IsNullOrEmpty(taskData.task.ItemNeeds))
                {
                    Debug.Log("Mở Dialouge " + taskData.task.Dialogue);
                    DialogueManager.GetInstance().EnterDialogueMode(transform.GetComponent<DialogueTrigger>().inkJSON, taskData);
                    //DialogueManager.Instance.EnterDialogueMode();
                }

            }
            else if (taskData.statusTask == StatusTask.CanSubmit)
            {
                //SubmitEvent
                GameEventManager.Singleton.SubmitTask(taskData);

            } else if (taskData.statusTask == StatusTask.InProgress)
            {
                //Mở dialogue inprogress
            }
        } else
        {
            Debug.Log("Mở Dialogue bình thường");
        }

    }
}
