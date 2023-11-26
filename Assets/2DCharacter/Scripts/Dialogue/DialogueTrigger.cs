using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    //[Header("Visual Cue")]
    //[SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] public TextAsset inkJSON;

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
            }
        }
        else
        {
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter ");
        if (collision.gameObject.tag == "Player")
        {
            playerInRange = true;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("OnTriggerExit ");

        if (collision.gameObject.tag == "Player")
        {
            playerInRange = false;
            if (DialogueManager.GetInstance().dialogueIsPlaying)
            StartCoroutine(DialogueManager.GetInstance().ExitDialogueMode());

        }
    }

}
