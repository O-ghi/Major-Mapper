using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : ManagerTemplate<DialogueManager>
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text dialogueText;
    private List<TextAsset> textAssets;
    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private Text[] choicesText;

    private Story currentStory;

    private DialogueFunctionBinding dialogueFunctionBinding;

    public bool dialogueIsPlaying { get; private set; }


    protected override void InitManager()
    {
        ManagerLogic.Singleton.AddManagerUpdate(this.GetType(), Update);
        dialogueFunctionBinding = new DialogueFunctionBinding();
        m_eventTriggerListeners = new List<TriggerListener>();
        textAssets = new List<TextAsset>();
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    protected override void Start()
    {
        dialogueIsPlaying = false;


    }

    protected override void Update()
    {
        // return right away if dialogue isn't playing
        if (!dialogueIsPlaying)
        {
            return;
        }

        // handle continuing to the next line in the dialogue when submit is pressed
        // NOTE: The 'currentStory.currentChoiecs.Count == 0' part was to fix a bug after the Youtube video was made
        if (canContinueToNextLine
            && currentStory.currentChoices.Count == 0
            && InputManager.GetInstance().GetInteractPressed())
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON, TaskData taskData = null)
    {

        dialoguePanel = PanelManager.SetPanel("DialoguePanel").gameObject;
        dialoguePanel.transform.localPosition = Vector3.zero;
        dialoguePanel.SetActive(false);
        choices = new GameObject[] { dialoguePanel.transform.Find("ChoiceList/1").gameObject,
            dialoguePanel.transform.Find("ChoiceList/2").gameObject ,
            dialoguePanel.transform.Find("ChoiceList/3").gameObject };
        dialogueText = dialoguePanel.transform.Find("StoryBackground/Text (Legacy)").GetComponent<Text>();

        foreach (var choice in choices)
        {
            SetButton(choice.transform.GetOrAddComponent<TriggerListener>(), EventTriggerType.PointerClick, MakeChoice);
        }
        currentStory = new Story(inkJSON.text);

        if (textAssets.Contains(inkJSON))
        {
            currentStory.ChoosePathString("openagain");
        } else
        {
            textAssets.Add(inkJSON);

        }

        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        //Get all the choices
        choicesText = new Text[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<Text>();
            index++;
        }
        dialogueFunctionBinding.AcceptTaskBind(currentStory, taskData);

        ContinueStory();
    }

    public IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);
        dialogueFunctionBinding.AcceptTaskUnBind(currentStory);
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }
    private Coroutine displayLineCoroutine;

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            // set text for the current dialogue line
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            string nextLine = currentStory.Continue();
            // handle case where the last line is an external function
            if (nextLine.Equals("") && !currentStory.canContinue)
            {
                StartCoroutine(ExitDialogueMode());
            }
            // otherwise, handle the normal case for continuing the story
            else
            {
                // handle tags
                displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
            }
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }
    private bool canContinueToNextLine = false;
    [SerializeField] private float typingSpeed = 0.04f;

    private IEnumerator DisplayLine(string line)
    {
        // set the text to the full line, but set the visible characters to 0
        dialogueText.text = line;
        // hide items while text is typing
        

        canContinueToNextLine = false;

        bool isAddingRichTextTag = false;

        // display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            // if the submit button is pressed, finish up displaying the line right away
            if (InputManager.GetInstance().GetSubmitPressed())
            {
                break;
            }

            // check for rich text tag, if found, add it without waiting
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            // if not rich text, add the next letter and wait a small time
            else
            {
                yield return new WaitForSeconds(0f);
            }
        }

        // actions to take after the entire line has finished displaying
        DisplayChoices();
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < choices.Length; i++)
        {
            choices[i].GetComponent<HorizontalLayoutGroup>().enabled = false;
            yield return new WaitForEndOfFrame();
            choices[i].GetComponent<HorizontalLayoutGroup>().enabled = true;
        }

        canContinueToNextLine = true;
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        // defensive check to make sure our UI can support the number of choices coming in
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: "
                + currentChoices.Count);
        }

        int index = 0;
        // enable and initialize the choices up to the amount of choices for this line of dialogue
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;

            choices[index].GetComponent<HorizontalLayoutGroup>().enabled = false;

            choices[index].GetComponent<HorizontalLayoutGroup>().enabled = true;

            index++;

        }
        // go through the remaining choices the UI supports and make sure they're hidden
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        // Event System requires we clear it first, then wait
        // for at least one frame before we set the current selected object.
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }


    public void MakeChoice(EventTriggerType _event, GameObject gameObject)
    {

        if (canContinueToNextLine)
        {
            int choiceIndex = int.Parse(gameObject.name) - 1;
            currentStory.ChooseChoiceIndex(choiceIndex);

            // NOTE: The below two lines were added to fix a bug after the Youtube video was made
            InputManager.GetInstance().RegisterSubmitPressed(); // this is specific to my InputManager script
            ContinueStory();
        }

    }

    private List<TriggerListener> m_eventTriggerListeners;

    public void SetButton(TriggerListener trigger, EventTriggerType triggerType, UnityAction<EventTriggerType, GameObject> triggerAction, object triggerArg = null)
    {
        TriggerListener.Entry entry = trigger.triggers.Find(p => p.eventID == triggerType);
        if (entry == null)
        {
            entry = new TriggerListener.Entry();
            entry.eventID = triggerType;
            trigger.triggers.Add(entry);
        }
        trigger.triggerArg.Add(triggerArg);
        entry.AddEventListener(triggerType, triggerAction);
        if (!m_eventTriggerListeners.Contains(trigger))
        {
            m_eventTriggerListeners.Add(trigger);
        }
    }

    public void SetButton(Component item, EventTriggerType triggerType, UnityAction<EventTriggerType, GameObject> triggerEvent, object triggerArg = null)
    {
        if (item == null)
            return;
        TriggerListener trigger = item.GetComponent<TriggerListener>();
        if (trigger != null)
            SetButton(trigger, triggerType, triggerEvent, triggerArg);
    }

    public void SetButton(Transform parent, string targetPath, EventTriggerType triggerType, UnityAction<EventTriggerType, GameObject> triggerEvent, object triggerArg = null)
    {
        Transform trans = parent.Find(targetPath);
        if (trans != null)
            SetButton(trans, triggerType, triggerEvent, triggerArg);
    }
}
