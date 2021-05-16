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

    private string awaitingReaction;

    private System.DateTime writingTime;
    private System.DateTime timeToStartWriting;

    //true = Initialisation Time, false = Reaction Time
    private bool isInitialisation = true;
    private bool isWaitingForReply = false;
    private bool bubbleSpawned = false;
    private string currentWaitingTime;

    private GameObject currentBubble;

    [Header("Message Area")]
    public GameObject messagePanel;
    public GameObject interlocutorBubblePrefab;
    public GameObject playerBubblePrefab;

    [Header("Reply area")]
    public GameObject repliesPanel;
    public GameObject replyButtonPrefab;

    TimeManager timeManager;

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
        timeManager = DialogueManager.Instance.timeManager;
        timeManager.currentlyWaiting = false;
        DialogueManager.Instance.CreateAndStartDialogue(DialogueManager.Instance.currentDialogueFile);
    }

    private void Update()
    {
        if (!isWaitingForReply)
        {
            if (!bubbleSpawned)
            {
                if (IsTimeToStartWriting())
                {
                    CreateMessageBubble();
                }
            }
            if (IsTimeOver())
            {
                timeManager.ResetClock();
                if (isInitialisation)
                {
                    DisplayMessage(currentBubble);
                }
                else
                {
                    DisplayReaction(awaitingReaction, currentBubble);
                }
            }

        }
    }

    #region Dialogue starting methods
    private void Init()
    {
        currentDialogueElementId = 0;
        if (currentDialogue != null)
        {
            StopDialogue(currentDialogue);
        }
        isInitialisation = true;
        isWaitingForReply = false;
    }
    public void StartDialogue(Dialogue dialogue)
    {
        Init();
        currentDialogue = dialogue;

        SetWaitingTime(currentDialogue.elements[currentDialogueElementId].initiationTime);
        timeManager.StartClock(currentWaitingTime);
        writingTime = SetWritingTime(currentDialogue.elements[currentDialogueElementId].message);
        timeToStartWriting = SetTimeToStartWriting();
    }
    private void StopDialogue(Dialogue dialogueToStop)
    {
        InvokeEvent(dialogueToStop.endDialogueAction);
    }

    #endregion

    #region Element display methods
    private void CreateMessageBubble()
    {
        GameObject messagePrefab = GameObject.Instantiate(interlocutorBubblePrefab, transform.position, Quaternion.identity, messagePanel.transform);
        GameObject imageBg = messagePrefab.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
        TextMeshProUGUI textInBubble = imageBg.GetComponentInChildren<TextMeshProUGUI>();

        textInBubble.text = "...";
        currentBubble = messagePrefab;
        bubbleSpawned = true;
        StartCoroutine(SetObjectHeightToBackground(messagePrefab, imageBg, messagePanel));

    }
    private void DisplayMessage(GameObject currentBubble)
    {
            GameObject imageBg = currentBubble.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
            TextMeshProUGUI textInBubble = imageBg.GetComponentInChildren<TextMeshProUGUI>();

            textInBubble.text = currentDialogue.elements[currentDialogueElementId].message;
            isInitialisation = false;
            bubbleSpawned = false;

            if (currentDialogue.elements[currentDialogueElementId].replies.Count > 0)
            {
                DisplayPossibleReplies(currentDialogue.elements[currentDialogueElementId].replies);
            }
            else
            {
                GoToNextElement();
            }
            StartCoroutine(SetObjectHeightToBackground(currentBubble, imageBg, messagePanel));
    }

    void DisplayReaction(string reaction, GameObject bubbleObject)
    {
        if (reaction.Length > 1)
        {
            GameObject imageBg = bubbleObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
            TextMeshProUGUI textInBubble = bubbleObject.GetComponentInChildren<TextMeshProUGUI>();

            textInBubble.text = reaction;
            isInitialisation = true;
            bubbleSpawned = false;

            StartCoroutine(SetObjectHeightToBackground(bubbleObject, imageBg, messagePanel));
        }

        GoToNextElement();

    }
    private void DisplayPossibleReplies(List<Reply> replies)
    {
        isWaitingForReply = true;

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

        GameObject imageBg = messagePrefab.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;

        TextMeshProUGUI textInBubble = imageBg.GetComponentInChildren<TextMeshProUGUI>();

        textInBubble.text = reply.replyText;

        awaitingReaction = reply.reaction;

        isWaitingForReply = false;

        StartCoroutine(SetObjectHeightToBackground(messagePrefab, imageBg, messagePanel));

        SetWaitingTime(reply.reactionTime);
        timeManager.StartClock(currentWaitingTime);

        writingTime = SetWritingTime(reply.reaction);
        timeToStartWriting = SetTimeToStartWriting();

        StartCoroutine(SetObjectHeightToBackground(messagePrefab, imageBg, messagePanel));
    }
    private void GoToNextElement()
    {
        InvokeEvent(currentDialogue.elements[currentDialogueElementId].elementAction);

        isInitialisation = true;
        bubbleSpawned = false;

        currentDialogueElementId++;
        if (currentDialogueElementId >= currentDialogue.elements.Count)
        {
            StopDialogue(currentDialogue);
        }
        else
        {
            SetWaitingTime(currentDialogue.elements[currentDialogueElementId].initiationTime);
            timeManager.StartClock(currentWaitingTime);

            writingTime = SetWritingTime(currentDialogue.elements[currentDialogueElementId].message);
            timeToStartWriting = SetTimeToStartWriting();
        }
    }
    IEnumerator SetObjectHeightToBackground(GameObject message, GameObject imageBg, GameObject panel)
    {
        yield return new WaitForEndOfFrame();

        panel.GetComponent<RectTransform>().sizeDelta =
            new Vector2(message.GetComponent<RectTransform>().sizeDelta.x, imageBg.GetComponent<RectTransform>().sizeDelta.y);

        message.GetComponent<RectTransform>().sizeDelta =
            new Vector2(message.GetComponent<RectTransform>().sizeDelta.x, imageBg.GetComponent<RectTransform>().sizeDelta.y);
    }
    #endregion

    #region Invoke element and dialogue event
    private void InvokeEvent(UnityAction action)
    {
        if (action != null)
        {
            UnityEvent endElementEvent = new UnityEvent();
            endElementEvent.AddListener(action);
            endElementEvent.Invoke();
        }
    }
    #endregion

    #region "..." as the hacker's writing a message
    private System.DateTime SetWritingTime(string message)
    {
        System.DateTime typingTime = System.DateTime.MinValue;
        int typingSpeed = GetTypingSpeed(message);

        typingTime = typingTime.AddSeconds(typingSpeed);

        return typingTime;
    }

    private int GetTypingSpeed(string message)
    {
        return message.Length / 3;
    }

    private System.DateTime SetTimeToStartWriting()
    {
        System.TimeSpan typingTime = System.DateTime.MinValue.Subtract(writingTime);
        System.DateTime temp = timeManager.timeToReach.Add(typingTime);

        if (temp < timeManager.currentTime)
        {
            temp = timeManager.currentTime.AddSeconds(1);
        }

        Debug.Log("An empty bubble will start to appear at : " + temp);

        return temp;
    }

    private bool IsTimeToWrite()
    {
        return timeManager.currentTime >= timeToStartWriting && timeManager.currentlyWaiting;
    }

    #endregion

    #region ChronoTime
    private void SetWaitingTime(string waitingTime)
    {
        if (DialogueManager.Instance.autoMode)
        {
            currentWaitingTime = DialogueManager.Instance.autoModeWaitingTime;
        }
        else
        {
            currentWaitingTime = waitingTime;
        }
    }
    #endregion

    #region Flags
    private bool IsTimeOver()
    {
        return timeManager.currentTime >= timeManager.timeToReach && timeManager.currentlyWaiting;
    }

    private bool IsTimeToStartWriting()
    {
        return timeManager.currentTime >= timeToStartWriting && timeManager.currentlyWaiting;
    }
    #endregion

    #region SaveWriting
    public int second;
    public int minute;
    public int hour;
    public int day;

    public void LoadTimeToStartWriting()
    {
        TimeToStartWritingData data = SaveSystem.LoadTimeToStartWriting();

        second = data.sec;
        minute = data.min;
        hour = data.hour;
        day = data.day;
    }
    public void SaveTimeToStartWriting()
    {
        second = timeToStartWriting.Second;
        minute = timeToStartWriting.Minute;
        hour = timeToStartWriting.Hour;
        day = timeToStartWriting.Day;
        SaveSystem.SaveTimeToStartWriting(this);


    }
    #endregion
}
