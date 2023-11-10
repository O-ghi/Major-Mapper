using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    //[Header("Visual Cue")]
    //[SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private bool playerInRange;

    private void Awake()
    {
        playerInRange = false;
        //visualCue.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && !DialogueManager.GetInstance().dialogueIsPlaying) 
        {
            //visualCue.SetActive(true);
            if(InputManager.GetInstance().GetInteractPressed()) 
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
            }
        }
        else
        {
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter ");
        if (other.gameObject.tag == "Player")
        {
            playerInRange = true;

        }
    }


    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit ");

        if (other.gameObject.tag == "Player")
        {
            playerInRange = false;
             StartCoroutine(DialogueManager.GetInstance().ExitDialogueMode());

        }
    }
}
