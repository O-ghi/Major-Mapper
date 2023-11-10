using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : ManagerTemplate<DialogueManager> 
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text dialogueText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private Text[] choicesText;

    private Story currentStory;

    public bool dialogueIsPlaying { get; private set; }


    protected override void InitManager()
    {
        ManagerLogic.Singleton.AddManagerUpdate(this.GetType(), Update);

    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    protected override void Start()
    {
        dialogueIsPlaying = false;
        
        //Get all the choices
        choicesText = new Text[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<Text>();
            index++;
        }
    }

    protected override void Update()
    {
        if (!dialogueIsPlaying) 
        {
            return;
        }

        if(InputManager.GetInstance().GetSubmitPressed() ) 
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        dialoguePanel = PanelManager.LoadPanel("DialogPanel");
        dialoguePanel.transform.localPosition = Vector3.zero;
        dialoguePanel.SetActive(false);
        choices = new GameObject[] { dialoguePanel.transform.Find("ChoiceList/Choice1").gameObject, 
            dialoguePanel.transform.Find("ChoiceList/Choice2").gameObject , 
            dialoguePanel.transform.Find("ChoiceList/Choice3").gameObject };
        dialogueText = dialoguePanel.transform.Find("StoryBackground/Text (Legacy)").GetComponent<Text>();
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    public IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            //display choices
            DisplayChoices();
        }
        else
        {
             StartCoroutine(ExitDialogueMode());
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if(currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support.Number of choices given: " + currentChoices.Count);
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }
        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
    }
}
