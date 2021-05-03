using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueDisplayer : MonoBehaviour
{

    private bool proceed = false;
    private Dialogue currentDialogue;
    private int currentDialogueElementId = 0;
    private float timeToWait = .5f;

    [Header("Message Area")]
    public GameObject messagePanel;
    public GameObject interlocutorBubblePrefab;
    public GameObject playerBubblePrefab;

    [Header("Reply area")]
    public GameObject repliesPanel;
    public GameObject replyButtonPrefab;

    private static DialogueDisplayer instance;
    public static DialogueDisplayer Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (this != instance)
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (proceed)
        {
            proceed = false;

            if (currentDialogueElementId < currentDialogue.elements.Count)
            {
                ContinueDialogue(currentDialogue, currentDialogueElementId);
            }
            else
            {
                //Dialogue is finished, action can be taken here
                if(currentDialogue.endDialogueAction != null)
                {
                    InvokeEvent(currentDialogue.endDialogueAction);
                }
            }
        }
    }

    private void Init()
    {
        currentDialogueElementId = 0;
        if (currentDialogue != null)
        {
            StopDialogue(currentDialogue);
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Init();
        currentDialogue = dialogue;
        ContinueDialogue(currentDialogue, currentDialogueElementId);
    }

    //Proceeds elements by elements
    private void ContinueDialogue(Dialogue dialogue, int currentDialogueId)
    {
        StartCoroutine(DisplayDialogueElement(dialogue.elements[currentDialogueId]));
    }

    private void StopDialogue(Dialogue dialogueToStop)
    {
        InvokeEvent(dialogueToStop.elements[currentDialogueElementId].elementAction);
        InvokeEvent(dialogueToStop.endDialogueAction);
        StopAllCoroutines();
    }
    IEnumerator DisplayDialogueElement(DialogueElement elementToDisplay)
    {
        if (elementToDisplay.index < currentDialogue.elements.Count)
        {
            DeleteReplies();
            timeToWait = elementToDisplay.initiationTime;

            yield return new WaitForSeconds(timeToWait);
            DisplayMessage(elementToDisplay.message);

            if (elementToDisplay.replies.Count <= 0)
            {
                yield return new WaitForSeconds(timeToWait);

                GoToNextElement();
            }
            else
            {
                DisplayPossibleReplies(elementToDisplay.replies);
            }
        }
    }

    private void DisplayMessage(string message)
    {
        GameObject messagePrefab = GameObject.Instantiate(interlocutorBubblePrefab, transform.position, Quaternion.identity, messagePanel.transform);

        GameObject imageBg = messagePrefab.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;

        TextMeshProUGUI textInBubble = imageBg.GetComponentInChildren<TextMeshProUGUI>();

        textInBubble.text = message;

        StartCoroutine(SetObjectHeightToBackground(messagePrefab, imageBg));
    }

    IEnumerator SetObjectHeightToBackground(GameObject message, GameObject imageBg)
    {
        yield return new WaitForEndOfFrame();

        message.GetComponent<RectTransform>().sizeDelta =
            new Vector2(message.GetComponent<RectTransform>().sizeDelta.x, imageBg.GetComponent<RectTransform>().sizeDelta.y);
    }

    private void DeleteReplies()
    {
        foreach (Transform child in repliesPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void DisplayPossibleReplies(List<Reply> replies)
    {
        for (int i = 0; i < replies.Count; i++)
        {
            GameObject replyButton = Instantiate(replyButtonPrefab, repliesPanel.transform);
            replyButton.GetComponentInChildren<TextMeshProUGUI>().text = replies[i].reply;

            Reply reply = replies[i];

            replyButton.GetComponent<Button>().onClick.AddListener(delegate { SendReply(reply); });

            if(replies[i].replyEvent != null)
            {
                replyButton.GetComponent<Button>().onClick.AddListener(replies[i].replyEvent);
            }
        }
    }

    private void SendReply(Reply reply)
    {
        GameObject messagePrefab = GameObject.Instantiate(playerBubblePrefab, playerBubblePrefab.transform.position, Quaternion.identity, messagePanel.transform);

        GameObject imageBg = messagePrefab.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;

        TextMeshProUGUI textInBubble = imageBg.GetComponentInChildren<TextMeshProUGUI>();

        textInBubble.text = reply.reply;

        StartCoroutine(SetObjectHeightToBackground(messagePrefab, imageBg));
        StartCoroutine(DisplayReaction(reply.reaction, reply.reactionTime));
    }

    IEnumerator DisplayReaction(string reaction, float reactionTime)
    {
        yield return new WaitForEndOfFrame();
        DeleteReplies();
        yield return new WaitForSeconds(reactionTime);

        if(reaction.Length > 1)
        {
            GameObject messagePrefab = GameObject.Instantiate(interlocutorBubblePrefab, transform.position, Quaternion.identity, messagePanel.transform);
            GameObject imageBg = messagePrefab.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
            TextMeshProUGUI textInBubble = imageBg.GetComponentInChildren<TextMeshProUGUI>();

            textInBubble.text = reaction;

            StartCoroutine(SetObjectHeightToBackground(messagePrefab, imageBg));
        }

        GoToNextElement();

    }

    private void InvokeEvent(UnityAction action)
    {
        if(action != null)
        {
            UnityEvent endElementEvent = new UnityEvent();
            endElementEvent.AddListener(action);
            endElementEvent.Invoke();
        }
    }

    //Allower for the Dialogue to continue
    private void GoToNextElement()
    {
        if(currentDialogue.elements[currentDialogueElementId].elementAction != null)
        {
            InvokeEvent(currentDialogue.elements[currentDialogueElementId].elementAction);
        }
        currentDialogueElementId++;
        proceed = true;
    }
}
