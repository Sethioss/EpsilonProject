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

    #region Display Variables
    [SerializeField]
    public enum AllowedMessageType { NORMAL = 0, LINK = 1, LEAVE = 2 };
    public AllowedMessageType allowedType;

    public Dialogue currentDialogue;
    public int currentDialogueElementId = 0;
    [HideInInspector]
    public bool isLoading = false;
    #endregion

    #region Cached variables
    private DialogueManager cachedDialogueManager;
    #endregion

    #region Dialogues timing / display variables

    private string awaitingReaction;

    private System.DateTime writingTime;
    private System.DateTime timeToStartWriting;

    //true = Initialisation Time, false = Reaction Time
    [HideInInspector]
    public bool isInitialisation = true;
    [HideInInspector]
    public bool isWaitingForReply = false;
    private bool bubbleSpawned = false;
    [HideInInspector]
    public bool jumpToMessage = false;
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
    UserSettings userSettings;

    DialogueData data = new DialogueData();

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
        cachedDialogueManager = DialogueManager.Instance;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            LoadDialogueData();
        }

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
                    DisplayMessage();
                }
                else
                {
                    DisplayReaction(awaitingReaction);
                }
            }

        }
    }

    #region Save/Load
    public void SaveDialogueData(List<Dialogue> dialogueListToSaveTo)
    {
        SaveSystem.SaveDialogue(dialogueListToSaveTo);
    }

    public void LoadDialogueData()
    {
        data = SaveSystem.LoadDialogue();

        if (data != null)
        {
            isLoading = true;
            //Création des dialogues de retour
            cachedDialogueManager.dialogueList = RecreateDialogueListFromData(data);
            DisplayDialogueData(data);
        }
    }

    public List<Dialogue> RecreateDialogueListFromData(DialogueData data)
    {
        int processedGlobalElementId = 0;
        List<Dialogue> toLoad = new List<Dialogue>();

        //Pour chaque dialogue
        for (int i = 0; i < data.dialogueFileName.Count; i++)
        {
            Dialogue templateDialogue = cachedDialogueManager.CreateDialogue(data.dialogueFileName[i]);
            Dialogue dialogueToCreate = new Dialogue();
            dialogueToCreate.fileName = data.dialogueFileName[i];
            Debug.LogWarning("Number of elements in dialogue " + data.numberOfElementsInDialogue[i]);

            //Pour chaque élément
            for (int j = 0; j < templateDialogue.elements.Count; j++)
            {
                //If the save data indicates that the player has already passed this element
                if (templateDialogue.elements[j].index < data.numberOfElementsInDialogue[i])
                {
                    //Only keeps the element if the saved element and dialogue element id match (Some cases cause the dialogues to skip one element, this if statement is here to mitigate errors)
                    if (templateDialogue.elements[j].index == data.elementId[processedGlobalElementId])
                    {

                        DialogueElement element = new DialogueElement(templateDialogue.elements[j].message,
                            templateDialogue.elements[j].index, templateDialogue.elements[j].initiationTime, null);
                        element.chosenReplyIndex = data.chosenReplyId[processedGlobalElementId];
                        element.messageType = templateDialogue.elements[j].messageType;

                        //Has the player answered the element (Also checks if the element has replies)
                        if (element.chosenReplyIndex != -1)
                        {
                            element.AddReply(templateDialogue.elements[j].replies[data.chosenReplyId[processedGlobalElementId]]);
                        }
                        else
                        {
                            foreach (Reply replyInTemplate in templateDialogue.elements[j].replies)
                            {
                                element.AddReply(replyInTemplate);
                            }
                        }

                        //Adds the created element
                        dialogueToCreate.AddDialogueElement(element);
                    }
                }
                else
                {
                    dialogueToCreate.AddDialogueElement(templateDialogue.elements[j]);
                }

                processedGlobalElementId++;
            }

            toLoad.Add(dialogueToCreate);
        }

        return toLoad;
    }

    public void DisplayDialogueData(DialogueData data)
    {
        StartDialogue(cachedDialogueManager.dialogueList[0]);
    }

    #endregion

    #region Dialogue starting methods
    private void Init()
    {
        //Set dialogue state to the start of a new element
        isWaitingForReply = true;
        isInitialisation = true;
        bubbleSpawned = false;

        timeManager = cachedDialogueManager.timeManager;
        timeManager.currentlyWaiting = false;
        userSettings = UserSettings.Instance;

        cachedDialogueManager.dialoguesToSave = new List<Dialogue>();
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

        cachedDialogueManager.dialoguesToSave.Add(new Dialogue());
        cachedDialogueManager.dialoguesToSave[cachedDialogueManager.dialoguesToSave.Count - 1].fileName = dialogue.fileName;

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
        GameObject messagePrefab = null;
        MessageBubble messageBubble = null;

        if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.LEAVE)
        {
            if (allowedType == AllowedMessageType.LEAVE)
            {
                Debug.Log("Creating leaving message bubble");
                messagePrefab = GameObject.Instantiate(leaveMessagePrefab, transform.position, Quaternion.identity, messagePanel.transform);
                messageBubble = messagePrefab.GetComponent<MessageBubble>();

                messageBubble.message.text = "";
                messagePrefab.SetActive(false);

                currentBubble = messagePrefab;
                bubbleSpawned = true;

                StartCoroutine(SetObjectHeightToBackground(messagePrefab, messageBubble.textBackground, messagePanel));
            }
            else
            {
                GoToNextElement();
            }
        }
        else if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.LINK)
        {
            if (allowedType == AllowedMessageType.LINK)
            {
                messagePrefab = GameObject.Instantiate(interlocutorBubblePrefab, transform.position, Quaternion.identity, messagePanel.transform);
                messageBubble = messagePrefab.GetComponent<MessageBubble>();

                messageBubble.message.text = "...";

                //Current bubble = The bubble which text is gonna get changed
                currentBubble = messagePrefab;
                bubbleSpawned = true;

                StartCoroutine(SetObjectHeightToBackground(messagePrefab, messageBubble.textBackground, messagePanel));
            }
            else
            {
                currentDialogue.elements[currentDialogueElementId].elementAction = null;
                GoToNextElement();
            }
        }
        else
        {
            messagePrefab = GameObject.Instantiate(interlocutorBubblePrefab, transform.position, Quaternion.identity, messagePanel.transform);
            messageBubble = messagePrefab.GetComponent<MessageBubble>();

            messageBubble.message.text = "...";

            //Current bubble = The bubble which text is gonna get changed
            currentBubble = messagePrefab;
            bubbleSpawned = true;

            StartCoroutine(SetObjectHeightToBackground(messagePrefab, messageBubble.textBackground, messagePanel));
        }

    }

    private void DisplayLinkMessage(MessageBubble messageBubble)
    {
        if (allowedType == AllowedMessageType.LINK)
        {
            Button button = messageBubble.textBackground.GetComponent<Button>();
            button.enabled = true;
            button.onClick.AddListener(currentDialogue.elements[currentDialogueElementId].elementAction);

            DialogueElement newElement = new DialogueElement(currentDialogue.elements[currentDialogueElementId].message, currentDialogue.elements[currentDialogueElementId].index,
            currentDialogue.elements[currentDialogueElementId].initiationTime, currentDialogue.elements[currentDialogueElementId].elementAction);

            newElement.messageType = currentDialogue.elements[currentDialogueElementId].messageType;
            cachedDialogueManager.dialoguesToSave[cachedDialogueManager.dialoguesToSave.Count - 1].AddDialogueElement(newElement);

            SaveDialogueData(cachedDialogueManager.dialoguesToSave);
        }

        currentDialogue.elements[currentDialogueElementId].elementAction = null;
    }

    private void DisplayLeaveMessage(MessageBubble messageBubble)
    {
        if (allowedType == AllowedMessageType.LEAVE)
        {
            currentBubble.SetActive(true);

            if (!isLoading)
            {
                StopDialogue(currentDialogue);
            }

            DialogueElement newElement = new DialogueElement(currentDialogue.elements[currentDialogueElementId].message, currentDialogue.elements[currentDialogueElementId].index,
             currentDialogue.elements[currentDialogueElementId].initiationTime, currentDialogue.elements[currentDialogueElementId].elementAction);

            newElement.messageType = currentDialogue.elements[currentDialogueElementId].messageType;
            cachedDialogueManager.dialoguesToSave[cachedDialogueManager.dialoguesToSave.Count - 1].AddDialogueElement(newElement);
        }
    }

    private void DisplayMessage()
    {
        MessageBubble messageBubble = currentBubble.GetComponent<MessageBubble>();

        messageBubble.message.text = currentDialogue.elements[currentDialogueElementId].message;

        isInitialisation = false;
        bubbleSpawned = false;
        newDialogue = false;

        //If it's a message sent with a <SCENE> keyword, minigameInvite is set to true
        if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.LINK)
        {
            DisplayLinkMessage(messageBubble);
        }
        else if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.LEAVE)
        {
            DisplayLeaveMessage(messageBubble);
        }
        else
        {
            allowedType = AllowedMessageType.NORMAL;

            DialogueElement newElement = new DialogueElement(currentDialogue.elements[currentDialogueElementId].message, currentDialogue.elements[currentDialogueElementId].index,
            currentDialogue.elements[currentDialogueElementId].initiationTime, currentDialogue.elements[currentDialogueElementId].elementAction);
            newElement.messageType = currentDialogue.elements[currentDialogueElementId].messageType;

            //Save
            cachedDialogueManager.dialoguesToSave[cachedDialogueManager.dialoguesToSave.Count - 1].AddDialogueElement(newElement);

            SaveDialogueData(cachedDialogueManager.dialoguesToSave);
        }

        DeleteReplies();
        if (isLoading && currentDialogue.elements[currentDialogueElementId].chosenReplyIndex != -1)
        {
            SendReply(currentDialogue.elements[currentDialogueElementId].replies[0]);
        }
        else
        {
            //Display replies if there are
            if (currentDialogue.elements[currentDialogueElementId].replies.Count > 0 && currentDialogue.elements[currentDialogueElementId].messageType != DialogueElement.MessageType.LINK)
            {
                DisplayPossibleReplies(currentDialogue.elements[currentDialogueElementId].replies);
            }
            else
            {
                GoToNextElement();
            }
        }


        StartCoroutine(SetObjectHeightToBackground(currentBubble, messageBubble.textBackground, messagePanel));
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

            cachedDialogueManager.dialoguesToSave[cachedDialogueManager.dialoguesToSave.Count - 1].elements[cachedDialogueManager.dialoguesToSave[cachedDialogueManager.dialoguesToSave.Count - 1].elements.Count - 1].AddReply(replies[i]);
        }
        isWaitingForReply = true;
    }

    private void SendReply(Reply reply)
    {
        //Display
        DeleteReplies();

        //Create response bubble
        GameObject responsePrefab = GameObject.Instantiate(playerBubblePrefab, playerBubblePrefab.transform.position, Quaternion.identity, messagePanel.transform);
        MessageBubble messageBubble = responsePrefab.GetComponent<MessageBubble>();

        messageBubble.message.text = reply.replyText;

        if (messageBubble.profilePictureTransform != null)
        {
            messageBubble.profilePictureTransform.GetComponent<Image>().sprite = userSettings.profilePicture;
        }

        //Create element, remove the element created in DisplayMessage, and adds the new one
        Reply newReply = new Reply(reply.replyText, reply.reaction, reply.index, reply.reactionTime, reply.replyEvent);

        DialogueElement newElement = new DialogueElement(currentDialogue.elements[currentDialogueElementId].message, newReply, currentDialogue.elements[currentDialogueElementId].index,
            currentDialogue.elements[currentDialogueElementId].initiationTime, currentDialogue.elements[currentDialogueElementId].elementAction);


        //Apply goodReplyIndex to the dialogues
        int goodReplyIndex = reply.index;

        cachedDialogueManager.dialogueList[currentDialogue.id].elements[currentDialogueElementId].chosenReplyIndex = goodReplyIndex;
        currentDialogue.elements[currentDialogueElementId].chosenReplyIndex = goodReplyIndex;
        newElement.chosenReplyIndex = goodReplyIndex;

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

        StartCoroutine(SetObjectHeightToBackground(responsePrefab, messageBubble.textBackground, messagePanel));

        //Saving
        cachedDialogueManager.dialoguesToSave[currentDialogue.id].AddDialogueElement(newElement);
        cachedDialogueManager.dialoguesToSave[currentDialogue.id].elements.RemoveAt(currentDialogueElementId);

        SaveDialogueData(cachedDialogueManager.dialoguesToSave);
    }

    private void DeleteReplies()
    {
        foreach (Transform child in repliesPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void DisplayReaction(string reaction)
    {
        MessageBubble messageBubble = currentBubble.GetComponent<MessageBubble>();

        GameObject imageBg = messageBubble.textBackground;
        TextMeshProUGUI textInBubble = messageBubble.message;

        textInBubble.text = reaction;
        StartCoroutine(SetObjectHeightToBackground(currentBubble, imageBg, messagePanel));

        isWaitingForReply = false;
        isInitialisation = true;
        bubbleSpawned = false;

        GoToNextElement();

    }

    #endregion

    #region Dialogue navigation
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
        DeleteReplies();

        currentDialogueElementId = index - 1;
        bubbleSpawned = false;

        SetWaitingTime(currentDialogue.elements[currentDialogueElementId].initiationTime);
        timeManager.StartClock(currentWaitingTime);

        writingTime = SetWritingTime(currentDialogue.elements[currentDialogueElementId].message);
        timeToStartWriting = SetTimeToStartWriting();
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
        if (userSettings.autoMode)
        {
            currentWaitingTime = userSettings.autoModeWaitingTime;
        }
        else if (isLoading)
        {
            currentWaitingTime = "00:00:00:00";
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
        try
        {
            return timeManager.currentTime >= timeManager.timeToReach && timeManager.currentlyWaiting;
        }
        catch
        {
            return false;
        }
    }
    private bool IsTimeToStartWriting()
    {
        try
        {
            return timeManager.currentTime >= timeToStartWriting && timeManager.currentlyWaiting;
        }
        catch
        {
            return false;
        }
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

    #region Misc
    IEnumerator SetObjectHeightToBackground(GameObject messagePrefab, GameObject imageBg, GameObject panel)
    {
        yield return new WaitForEndOfFrame();

        panel.GetComponent<RectTransform>().sizeDelta =
            new Vector2(messagePrefab.GetComponent<RectTransform>().sizeDelta.x, imageBg.GetComponent<RectTransform>().sizeDelta.y);

        messagePrefab.GetComponent<RectTransform>().sizeDelta =
            new Vector2(messagePrefab.GetComponent<RectTransform>().sizeDelta.x, imageBg.GetComponent<RectTransform>().sizeDelta.y);
    }
    private void SetNewDialogueToFalse()
    {
        newDialogue = false;
    }
    #endregion
}