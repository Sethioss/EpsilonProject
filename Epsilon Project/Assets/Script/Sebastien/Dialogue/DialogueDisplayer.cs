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

    public Dialogue currentDialogue;
    public int currentDialogueElementId = 0;

    #region Timing / Display des dialogues

    private string awaitingReaction;

    private System.DateTime writingTime;
    private System.DateTime timeToStartWriting;

    //true = Initialisation Time, false = Reaction Time
    private bool isInitialisation = true;
    private bool isWaitingForReply = false;
    private bool bubbleSpawned = false;
    private bool jumpToMessage = true;
    private string currentWaitingTime;
    private bool readingDialogue = true;

    private bool newDialogue = true;
    [HideInInspector]
    public bool cameFromBranch = false;

    private GameObject currentBubble;

    [Header("Message Area")]
    public GameObject messagePanel;
    public GameObject interlocutorBubblePrefab;
    public GameObject playerBubblePrefab;
    public GameObject leaveMessagePrefab;

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
        DialogueManager.Instance.onGameSceneEntered = true;
    }

    private void Update()
    {
        if (!isWaitingForReply && readingDialogue)
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
        //Set dialogue state to the start of a new element
        isWaitingForReply = true;
        isInitialisation = true;
        bubbleSpawned = false;

        timeManager = DialogueManager.Instance.timeManager;
        timeManager.currentlyWaiting = false;

        //Set the pointer to the first dialog element
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
        isWaitingForReply = false;
        readingDialogue = true;
        newDialogue = true;

        //Set clock and writing time to the initiation time of the current element
        SetWaitingTime(currentDialogue.elements[currentDialogueElementId].initiationTime);
        timeManager.StartClock(currentWaitingTime);

        writingTime = SetWritingTime(currentDialogue.elements[currentDialogueElementId].message);
        timeToStartWriting = SetTimeToStartWriting();

    }
    private void StopDialogue(Dialogue dialogueToStop)
    {
        timeManager.ResetClock();
        InvokeEvent(dialogueToStop.endDialogueAction);
        readingDialogue = false;
    }

    #endregion

    #region Element display methods
    private void CreateMessageBubble()
    {
        GameObject messagePrefab = GameObject.Instantiate(interlocutorBubblePrefab, transform.position, Quaternion.identity, messagePanel.transform);
        GameObject imageBg = messagePrefab.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;
        TextMeshProUGUI textInBubble = imageBg.GetComponentInChildren<TextMeshProUGUI>();

        textInBubble.text = "...";
        //Current bubble = The bubble which text is gonna get changed
        currentBubble = messagePrefab;
        bubbleSpawned = true;
        StartCoroutine(SetObjectHeightToBackground(messagePrefab, imageBg, messagePanel));

    }
    private void DisplayMessage(GameObject currentBubble)
    {
        GameObject imageBg = currentBubble.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;
        TextMeshProUGUI textInBubble = imageBg.GetComponentInChildren<TextMeshProUGUI>();

        //If it's a message sent with a <SCENE> keyword, minigameInvite is set to true
        if (currentDialogue.elements[currentDialogueElementId].minigameInvite)
        {
            Button button = imageBg.GetComponent<Button>();
            button.enabled = true;
            button.onClick.AddListener(currentDialogue.elements[currentDialogueElementId].replies[0].replyEvent);
        }

        textInBubble.text = currentDialogue.elements[currentDialogueElementId].message;
        isInitialisation = false;
        bubbleSpawned = false;

        newDialogue = false;

        //Display replies if there are
        if (currentDialogue.elements[currentDialogueElementId].replies.Count > 0 && !currentDialogue.elements[currentDialogueElementId].minigameInvite)
        {
            DisplayPossibleReplies(currentDialogue.elements[currentDialogueElementId].replies);
        }
        else
        {
            GoToNextElement();
        }

        StartCoroutine(SetObjectHeightToBackground(currentBubble, imageBg, messagePanel));
    }
    private void DisplayPossibleReplies(List<Reply> replies)
    {
        //Create the buttons corresponding to each answer
        for (int i = 0; i < replies.Count; i++)
        {
            GameObject replyButton = Instantiate(replyButtonPrefab, repliesPanel.transform);
            replyButton.GetComponentInChildren<TextMeshProUGUI>().text = replies[i].replyText;

            Reply reply = replies[i];

            replyButton.GetComponent<Button>().onClick.AddListener(delegate { SendReply(reply); });
            replyButton.GetComponent<Button>().onClick.AddListener(delegate { SetNewDialogueToFalse(); });

            if (replies[i].replyEvent != null)
            {
                replyButton.GetComponent<Button>().onClick.AddListener(replies[i].replyEvent);
            }
        }
        isWaitingForReply = true;
    }

    private void SetNewDialogueToFalse()
    {
        newDialogue = false;
    }
    private void SendReply(Reply reply)
    {
        //Display
        DeleteReplies();

        if (!reply.isLeaveMessage)
        {
            //Create response bubble
            GameObject responsePrefab = GameObject.Instantiate(playerBubblePrefab, playerBubblePrefab.transform.position, Quaternion.identity, messagePanel.transform);
            GameObject messagePrefab = responsePrefab.transform.GetChild(0).gameObject;

            GameObject imageBg = messagePrefab.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;
            TextMeshProUGUI textInBubble = imageBg.GetComponentInChildren<TextMeshProUGUI>();
            textInBubble.text = reply.replyText;

            //Set clock to reaction time
            isWaitingForReply = false;
            SetWaitingTime(reply.reactionTime);
            timeManager.StartClock(currentWaitingTime);

            if (reply.reaction == "" || reply.reaction == null)
            {
                GoToNextElement();
            }
            else
            {
                awaitingReaction = reply.reaction;
            }

            StartCoroutine(SetObjectHeightToBackground(responsePrefab, imageBg, messagePanel));
        }
    }

    private void DeleteReplies()
    {
        foreach (Transform child in repliesPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void DisplayReaction(string reaction, GameObject bubbleObject)
    {
        GameObject imageBg = bubbleObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;
        TextMeshProUGUI textInBubble = bubbleObject.GetComponentInChildren<TextMeshProUGUI>();

        textInBubble.text = reaction;
        StartCoroutine(SetObjectHeightToBackground(bubbleObject, imageBg, messagePanel));

        isWaitingForReply = false;
        isInitialisation = true;
        bubbleSpawned = false;

        GoToNextElement();

    }

    private void GoToNextElement()
    {
        InvokeEvent(currentDialogue.elements[currentDialogueElementId].elementAction);

        if (currentDialogueElementId + 1 >= currentDialogue.elements.Count)
        {
            StopDialogue(currentDialogue);
        }
        else
        {
            if (!cameFromBranch)
            {
                currentDialogueElementId++;
            }
            else if (!newDialogue && cameFromBranch)
            {
                currentDialogueElementId++;
            }
            else
            {
                cameFromBranch = false;
            }

            isInitialisation = true;
            bubbleSpawned = false;

            SetWaitingTime(currentDialogue.elements[currentDialogueElementId].initiationTime);
            timeManager.StartClock(currentWaitingTime);

            writingTime = SetWritingTime(currentDialogue.elements[currentDialogueElementId].message);
            timeToStartWriting = SetTimeToStartWriting();
        }
    }

    private void GoToElement(int index)
    {
        currentDialogueElementId = index - 1;
        bubbleSpawned = false;

        SetWaitingTime(currentDialogue.elements[currentDialogueElementId].initiationTime);
        timeManager.StartClock(currentWaitingTime);

        writingTime = SetWritingTime(currentDialogue.elements[currentDialogueElementId].message);
        timeToStartWriting = SetTimeToStartWriting();

    }
    IEnumerator SetObjectHeightToBackground(GameObject messagePrefab, GameObject imageBg, GameObject panel)
    {
        yield return new WaitForEndOfFrame();

        panel.GetComponent<RectTransform>().sizeDelta =
            new Vector2(messagePrefab.GetComponent<RectTransform>().sizeDelta.x, imageBg.GetComponent<RectTransform>().sizeDelta.y);

        messagePrefab.GetComponent<RectTransform>().sizeDelta =
            new Vector2(messagePrefab.GetComponent<RectTransform>().sizeDelta.x, imageBg.GetComponent<RectTransform>().sizeDelta.y);
    }
    #endregion

    #region Invoke element and dialogue event
    private void InvokeEvent(UnityAction action)
    {
        if (action != null)
        {
            action.Invoke();
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
        //Debug.Log(timeManager.timeToReach);

        if (temp < timeManager.currentTime)
        {
            temp = timeManager.currentTime;
        }

        //Debug.Log("An empty bubble will start to appear at : " + temp);

        return temp;
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

    #region ElementCreation

    public void CreateLinkElement(string sceneToChangeTo, string inviteMessage)
    {
        string message = "";

        if (inviteMessage != "")
        {
            message = inviteMessage + "\n" + GenerateRandomLink();
        }
        else
        {
            message = GenerateRandomLink();
        }

        UnityAction changeSceneAction = null;
        changeSceneAction += delegate { GameManager.Instance.GoToScene(sceneToChangeTo); };
        Reply newReply = new Reply("", "", 0, "00:00:00:00", changeSceneAction);

        DialogueElement newElement = new DialogueElement(message, newReply, currentDialogue.elements.Count,
            currentDialogue.elements[currentDialogueElementId].initiationTime, null, true);
        newElement.AddReply(newReply);

        currentDialogue.AddDialogueElement(newElement);

        if (jumpToMessage)
        {
            GoToElement(currentDialogue.elements.Count - 1);
            jumpToMessage = false;
        }

    }
    private string GenerateRandomLink()
    {
        string randomLinkStart = "<color=blue><u>https://";
        string randomLetterChain = "";
        string randomLinkEnd = ".xyz</u></color>";

        for (int i = 0; i < 34; i++)
        {
            int randomNumber = Random.Range(65, 122);
            randomLetterChain += (char)randomNumber;
        }

        return randomLinkStart + randomLetterChain + randomLinkEnd;
    }

    public void CreateLeaveElement()
    {
        GameObject leavePrefab = GameObject.Instantiate(leaveMessagePrefab, transform.position, Quaternion.identity, messagePanel.transform);
        GameObject imageBg = leavePrefab.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        TextMeshProUGUI textInBubble = imageBg.GetComponentInChildren<TextMeshProUGUI>();

        StartCoroutine(SetObjectHeightToBackground(leavePrefab, imageBg, messagePanel));

        UnityAction leaveActions = null;
        leaveActions += delegate { DialogueManager.Instance.ChangeScene("Game"); };
        Reply leaveReply = new Reply("[Partir]", null, 0, "00:00:00:01", leaveActions, true);

        DisplayPossibleReplies(new List<Reply> { leaveReply });
        StopDialogue(currentDialogue);
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
