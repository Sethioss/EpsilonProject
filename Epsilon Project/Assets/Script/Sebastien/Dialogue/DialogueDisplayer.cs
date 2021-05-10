using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueDisplayer : MonoBehaviour
{
    private static DialogueDisplayer instance;

    public static DialogueDisplayer Instance
    {
        get
        {
            return instance;
        }
    }

    private Dialogue currentDialogue;
    private int currentDialogueElementId = 0;

    #region Timing / Display des dialogues

    private float chrono = 0;
    private string awaitingReaction;
    private float timeToReach = 1;

    //true = Initialisation Time, false = Reaction Time
    private bool isInitialisation;
    private bool isWaitingForReply;

    [Header("Message Area")]
    public GameObject messagePanel;
    public GameObject interlocutorBubblePrefab;
    public GameObject playerBubblePrefab;

    [Header("Reply area")]
    public GameObject repliesPanel;
    public GameObject replyButtonPrefab;

    #endregion

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
    private void Start()
    {
        DialogueManager.Instance.CreateAndStartDialogue(DialogueManager.Instance.currentDialogueFile);
    }

    private void Update()
    {
        if (!isWaitingForReply)
        {
            chrono += Time.deltaTime;
            if (chrono >= timeToReach)
            {
                chrono = 0;
                if (isInitialisation)
                {
                    DisplayMessage(currentDialogue.elements[currentDialogueElementId].message);
                    DisplayPossibleReplies(currentDialogue.elements[currentDialogueElementId].replies);
                }
                else
                {
                    DisplayReaction(awaitingReaction);
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

    #region Dialogue starting methods

    public void StartDialogue(Dialogue dialogue)
    {
        Init();
        currentDialogue = dialogue;
        timeToReach = currentDialogue.elements[currentDialogueElementId].initiationTime;
        isInitialisation = true;
        isWaitingForReply = false;
    }
    private void StopDialogue(Dialogue dialogueToStop)
    {
        InvokeEvent(dialogueToStop.elements[currentDialogueElementId].elementAction);
        InvokeEvent(dialogueToStop.endDialogueAction);
    }

    #endregion

    #region Element display methods
    private void DisplayMessage(string message)
    {
        GameObject messagePrefab = GameObject.Instantiate(interlocutorBubblePrefab, transform.position, Quaternion.identity, messagePanel.transform);
        GameObject imageBg = messagePrefab.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
        TextMeshProUGUI textInBubble = imageBg.GetComponentInChildren<TextMeshProUGUI>();

        textInBubble.text = message;

        isWaitingForReply = true;
        isInitialisation = false;

        StartCoroutine(SetObjectHeightToBackground(messagePrefab, imageBg));
    }
    private void DisplayPossibleReplies(List<Reply> replies)
    {
        for (int i = 0; i < replies.Count; i++)
        {
            GameObject replyButton = Instantiate(replyButtonPrefab, repliesPanel.transform);
            replyButton.GetComponentInChildren<TextMeshProUGUI>().text = replies[i].replyText;

            Reply reply = replies[i];

            replyButton.GetComponent<Button>().onClick.AddListener(delegate { SendReply(reply); });

            if (replies[i].replyEvent != null)
            {
                replyButton.GetComponent<Button>().onClick.AddListener(replies[i].replyEvent);
            }
        }
    }
    private void DeleteReplies()
    {
        foreach (Transform child in repliesPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void SendReply(Reply reply)
    {
        DeleteReplies();

        GameObject messagePrefab = GameObject.Instantiate(playerBubblePrefab, playerBubblePrefab.transform.position, Quaternion.identity, messagePanel.transform);

        GameObject imageBg = messagePrefab.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;

        TextMeshProUGUI textInBubble = imageBg.GetComponentInChildren<TextMeshProUGUI>();

        textInBubble.text = reply.replyText;

        awaitingReaction = reply.reaction;
        timeToReach = reply.reactionTime;
        isWaitingForReply = false;

        StartCoroutine(SetObjectHeightToBackground(messagePrefab, imageBg));
    }
    void DisplayReaction(string reaction)
    {
        if (reaction.Length > 1)
        {
            GameObject messagePrefab = GameObject.Instantiate(interlocutorBubblePrefab, transform.position, Quaternion.identity, messagePanel.transform);
            GameObject imageBg = messagePrefab.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
            TextMeshProUGUI textInBubble = imageBg.GetComponentInChildren<TextMeshProUGUI>();

            textInBubble.text = reaction;
            isInitialisation = true;

            StartCoroutine(SetObjectHeightToBackground(messagePrefab, imageBg));
        }

        GoToNextElement();

    }
    #endregion

    private void InvokeEvent(UnityAction action)
    {
        if (action != null)
        {
            UnityEvent endElementEvent = new UnityEvent();
            endElementEvent.AddListener(action);
            endElementEvent.Invoke();
        }
    }
    private void GoToNextElement()
    {
        InvokeEvent(currentDialogue.elements[currentDialogueElementId].elementAction);

        chrono = 0;
        currentDialogueElementId++;
        if (currentDialogueElementId >= currentDialogue.elements.Count)
        {
            StopDialogue(currentDialogue);
        }

        timeToReach = currentDialogue.elements[currentDialogueElementId].initiationTime;
    }
    IEnumerator SetObjectHeightToBackground(GameObject message, GameObject imageBg)
    {
        yield return new WaitForEndOfFrame();

        message.GetComponent<RectTransform>().sizeDelta =
            new Vector2(message.GetComponent<RectTransform>().sizeDelta.x, imageBg.GetComponent<RectTransform>().sizeDelta.y);
    }
}
