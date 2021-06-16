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
    public bool tempWaitingForReply;
    [HideInInspector]
    public bool tempIsInitialisation;

    //[HideInInspector]
    public bool isInitialisation = true;
    //[HideInInspector]
    public bool isWaitingForReply = false;

    private bool bubbleSpawned = false;
    private string currentWaitingTime;

    [HideInInspector]
    public bool cameFromBranch = false;

    [HideInInspector]
    public GameObject currentBubble;

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

    //DialogueData data = new DialogueData();

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

        if (!isLoading)
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
                        DisplayMessage();
                    }
                    else
                    {
                        DisplayReaction(awaitingReaction);
                    }
                }

            }
        }
    }

    #region Save/Load
    private void UpdateDialogueState()
    {
        tempWaitingForReply = isWaitingForReply;
        tempIsInitialisation = isInitialisation;

        SaveDialogueData(cachedDialogueManager.dialoguesToSave);
    }

    private void AddDialogueToSave(Dialogue dialogueToAdd)
    {
        if (!isLoading)
        {
            cachedDialogueManager.dialoguesToSave.Add(dialogueToAdd);
            UpdateDialogueState();
            SaveDialogueData(cachedDialogueManager.dialoguesToSave);
        }
    }
    private void AddElementToSave(DialogueElement elementToAdd)
    {
        if (!isLoading)
        {
            cachedDialogueManager.dialoguesToSave[cachedDialogueManager.dialoguesToSave.Count - 1].AddDialogueElement(elementToAdd);
            UpdateDialogueState();
            SaveDialogueData(cachedDialogueManager.dialoguesToSave);
        }
    }
    private void EditLastSavedElement(DialogueElement elementToEdit)
    {
        if (!isLoading)
        {
            cachedDialogueManager.dialoguesToSave[currentDialogue.id].elements[cachedDialogueManager.dialoguesToSave[currentDialogue.id].elements.Count - 1] = elementToEdit;
            UpdateDialogueState();
            SaveDialogueData(cachedDialogueManager.dialoguesToSave);
        }
    }
    public void SaveDialogueData(List<Dialogue> dialogueListToSaveTo)
    {
        if (!isLoading)
        {
            SaveSystem.SaveDialogue(dialogueListToSaveTo);
        }
    }
    public void LoadDialogueData()
    {
        DialogueData data = SaveSystem.LoadDialogue();

        if (data != null)
        {
            isLoading = true;
            //Création des dialogues de retour
            cachedDialogueManager.dialogueList = RecreateDialogueListFromData(data);
            DisplayLoadedDialogue(cachedDialogueManager.dialogueList);

            isWaitingForReply = BoolToInt(data.waitForReply);
            isInitialisation = BoolToInt(data.initialisation);

            isLoading = false;
        }
    }
    public List<Dialogue> RecreateDialogueListFromData(DialogueData data)
    {
        int processedGlobalElement = 0;
        List<Dialogue> toLoad = new List<Dialogue>();

        //Pour chaque dialogue
        for (int i = 0; i < data.dialogueFileName.Count; i++)
        {
            //Debug.LogError("Checking dialogue " + data.dialogueFileName[i]);
            Dialogue templateDialogue = cachedDialogueManager.CreateDialogue(data.dialogueFileName[i]);
            Dialogue dialogueToCreate = new Dialogue();
            dialogueToCreate.fileName = data.dialogueFileName[i];
            dialogueToCreate.id = templateDialogue.id;

            //Debug.LogWarning("Number of elements in dialogue " + data.numberOfElementsInDialogue[i]);
            dialogueToCreate.id = i;

            //Pour chaque élément
            for (int j = 0; j < templateDialogue.elements.Count; j++)
            {
                //Debug.LogError(processedGlobalElement);
                //Debug.LogError("Checking element " + currentDialogue.elements[j].index);
                //If the save data indicates that the player has already passed this element
                if (templateDialogue.elements[j].index < data.numberOfElementsInDialogue[i])
                {
                    //Checks if the element ID exists in the save dialogue
                    for (int h = 0; h < data.numberOfElementsInDialogue[i]; h++)
                    {
                        //Debug.LogWarning("Data element ID " + data.elementId[h]);
                        //Debug.LogWarning("Template dialogue element ID " + templateDialogue.elements[j].index);

                        int globalElementInLoop = h + processedGlobalElement;

                        if (data.elementId[globalElementInLoop] == templateDialogue.elements[j].index)
                        {
                            //Debug.LogError("Creating the new element");
                            DialogueElement element = new DialogueElement(templateDialogue.elements[j].message,
                                templateDialogue.elements[j].index, templateDialogue.elements[j].initiationTime, templateDialogue.elements[j].elementAction);
                            element.chosenReplyIndex = data.chosenReplyId[globalElementInLoop];

                            element.messageType = templateDialogue.elements[j].messageType;

                            //Debug.LogError("Knowing if the player has replied");
                            //Has the player answered the element (Also checks if the element has replies)
                            //Debug.LogError("Creating the reply");
                            //Debug.LogError("The player hasn't replied");
                            foreach (Reply replyInTemplate in templateDialogue.elements[j].replies)
                            {
                                element.AddReply(replyInTemplate);
                            }

                            //Adds the created element
                            dialogueToCreate.AddDialogueElement(element);
                            //Debug.LogError("The element was created successfully!");
                            break;
                        }
                    }
                }
                else
                {
                    DialogueElement newElement = templateDialogue.elements[j];
                    newElement.messageType = templateDialogue.elements[j].messageType;
                    dialogueToCreate.AddDialogueElement(templateDialogue.elements[j]);
                }
            }

            processedGlobalElement += data.numberOfElementsInDialogue[i];
            toLoad.Add(dialogueToCreate);
        }
        return toLoad;
    }

    public void DisplayLoadedDialogue(List<Dialogue> loadedDialogues)
    {
        //Chaque dialogue
        for (int i = 0; i < loadedDialogues.Count; i++)
        {
            currentDialogueElementId = 0;
            currentDialogue = loadedDialogues[i];

            //Debug.LogError(i);

            if (i != loadedDialogues.Count - 1)
            {
                //Chaque élément
                for (int j = 0; j < loadedDialogues[i].elements.Count; j++)
                {
                    allowedType = (AllowedMessageType)loadedDialogues[i].elements[j].messageType;
                    CreateMessageBubble();
                    DisplayMessage();
                    if (loadedDialogues[i].elements[j].chosenReplyIndex != -1)
                    {
                        SendReply(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex]);
                        if (loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction != "")
                        {
                            CreateMessageBubble();
                            DisplayReaction(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction);
                        }
                    }
                }
            }
            else
            {
                //Chaque élément
                for (int j = 0; j < cachedDialogueManager.dialoguesToSave[i].elements.Count; j++)
                {
                    allowedType = (AllowedMessageType)loadedDialogues[i].elements[j].messageType;
                    CreateMessageBubble();
                    DisplayMessage();
                    if (loadedDialogues[i].elements[j].chosenReplyIndex != -1)
                    {
                        SendReply(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex]);
                        if (loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction != "")
                        {
                            CreateMessageBubble();
                            DisplayReaction(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction);
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region Dialogue starting methods
    private void Init()
    {
        //Set dialogue state to the start of a new element
        isWaitingForReply = true;
        isInitialisation = true;
        bubbleSpawned = false;
        cameFromBranch = false;

        timeManager = cachedDialogueManager.timeManager;
        timeManager.currentlyWaiting = false;
        userSettings = UserSettings.Instance;

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

        AddDialogueToSave(new Dialogue());
        cachedDialogueManager.dialoguesToSave[cachedDialogueManager.dialoguesToSave.Count - 1].fileName = dialogue.fileName;
        cachedDialogueManager.dialoguesToSave[cachedDialogueManager.dialoguesToSave.Count - 1].id = dialogue.id;

        SaveDialogueData(cachedDialogueManager.dialoguesToSave);

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
    }

    #endregion

    #region Element display methods
    public void CreateMessageBubble()
    {
        GameObject messagePrefab = null;
        MessageBubble messageBubble = null;


        if (isLoading)
        {
            if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.LEAVE)
            {
                Debug.Log("Creating leaving message bubble");
                messagePrefab = GameObject.Instantiate(leaveMessagePrefab, transform.position, Quaternion.identity, messagePanel.transform);
                messageBubble = messagePrefab.GetComponent<MessageBubble>();

                messageBubble.message.text = "";
                messagePrefab.SetActive(false);

                currentBubble = messagePrefab;
                bubbleSpawned = true;

                StartCoroutine(SetObjectHeightToBackground(messagePrefab, messageBubble.textBackground, messagePanel));
                GoToNextElement();

                currentDialogue.elements[currentDialogueElementId].elementAction = null;
            }
            else if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.LINK)
            {

                Debug.Log("Creating link bubble");
                messagePrefab = GameObject.Instantiate(interlocutorBubblePrefab, transform.position, Quaternion.identity, messagePanel.transform);
                messageBubble = messagePrefab.GetComponent<MessageBubble>();

                messageBubble.message.text = "...";

                //Current bubble = The bubble which text is gonna get changed
                currentBubble = messagePrefab;
                bubbleSpawned = true;

                StartCoroutine(SetObjectHeightToBackground(messagePrefab, messageBubble.textBackground, messagePanel));
                currentDialogue.elements[currentDialogueElementId].elementAction = null;
                GoToNextElement();

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
        else
        {
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
                currentDialogue.elements[currentDialogueElementId].elementAction = null;
            }
            else if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.LINK)
            {
                if (allowedType == AllowedMessageType.LINK)
                {
                    Debug.Log("Creating link bubble");
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

    }
    private void DisplayMessage()
    {
        if (!isLoading)
        {
            //Tous les types de message
            MessageBubble messageBubble = currentBubble.GetComponent<MessageBubble>();
            DialogueElement newElement = new DialogueElement();

            messageBubble.message.text = currentDialogue.elements[currentDialogueElementId].message;

            isInitialisation = false;
            bubbleSpawned = false;

            //If it's a message sent with a <SCENE> keyword, minigameInvite is set to true
            if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.LINK && allowedType == AllowedMessageType.LINK)
            {
                newElement = CreateLinkMessageElement(messageBubble);
            }
            else if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.LEAVE && allowedType == AllowedMessageType.LEAVE)
            {
                newElement = CreateLeaveMessageElement(messageBubble);
            }
            else
            {
                allowedType = AllowedMessageType.NORMAL;

                newElement = new DialogueElement(currentDialogue.elements[currentDialogueElementId].message, currentDialogue.elements[currentDialogueElementId].index,
                currentDialogue.elements[currentDialogueElementId].initiationTime, currentDialogue.elements[currentDialogueElementId].elementAction);
                newElement.messageType = currentDialogue.elements[currentDialogueElementId].messageType;
            }

            //Save
            AddElementToSave(newElement);

            //Display replies if there are
            if (currentDialogue.elements[currentDialogueElementId].replies.Count > 0)
            {
                DisplayPossibleReplies(currentDialogue.elements[currentDialogueElementId].replies);
            }
            else
            {
                GoToNextElement();
            }

            StartCoroutine(SetObjectHeightToBackground(currentBubble, messageBubble.textBackground, messagePanel));
        }
        else
        {
            //Tous les types de message
            MessageBubble messageBubble = currentBubble.GetComponent<MessageBubble>();
            DialogueElement newElement = new DialogueElement();

            messageBubble.message.text = currentDialogue.elements[currentDialogueElementId].message;

            isInitialisation = false;
            bubbleSpawned = false;

            if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.LINK)
            {
                newElement = CreateLinkMessageElement(messageBubble);
            }
            else if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.LEAVE)
            {
                newElement = CreateLeaveMessageElement(messageBubble);
            }
            else
            {
                allowedType = AllowedMessageType.NORMAL;

                newElement = new DialogueElement(currentDialogue.elements[currentDialogueElementId].message, currentDialogue.elements[currentDialogueElementId].index,
                currentDialogue.elements[currentDialogueElementId].initiationTime, currentDialogue.elements[currentDialogueElementId].elementAction);
                newElement.messageType = currentDialogue.elements[currentDialogueElementId].messageType;
            }

            if (currentDialogue.elements[currentDialogueElementId].replies.Count < 1)
            {
                GoToNextElement();
            }
        }



    }
    private DialogueElement CreateLinkMessageElement(MessageBubble messageBubble)
    {
        Button button = messageBubble.textBackground.GetComponent<Button>();
        button.enabled = true;
        button.onClick.AddListener(currentDialogue.elements[currentDialogueElementId].elementAction);
        currentDialogue.elements[currentDialogueElementId].elementAction = null;

        DialogueElement newElement = new DialogueElement(currentDialogue.elements[currentDialogueElementId].message, currentDialogue.elements[currentDialogueElementId].index,
        currentDialogue.elements[currentDialogueElementId].initiationTime, currentDialogue.elements[currentDialogueElementId].elementAction);

        newElement.messageType = currentDialogue.elements[currentDialogueElementId].messageType;

        return newElement;
    }
    private DialogueElement CreateLeaveMessageElement(MessageBubble messageBubble)
    {
        currentBubble.SetActive(true);

        StopDialogue(currentDialogue);

        DialogueElement newElement = new DialogueElement(currentDialogue.elements[currentDialogueElementId].message, currentDialogue.elements[currentDialogueElementId].index,
         currentDialogue.elements[currentDialogueElementId].initiationTime, currentDialogue.elements[currentDialogueElementId].elementAction);

        newElement.messageType = currentDialogue.elements[currentDialogueElementId].messageType;

        return newElement;
    }
    private void DisplayPossibleReplies(List<Reply> replies)
    {
        DeleteReplies();

        //Create the buttons corresponding to each answer
        for (int i = 0; i < replies.Count; i++)
        {
            GameObject replyButton = Instantiate(replyButtonPrefab, repliesPanel.transform);
            replyButton.GetComponentInChildren<TextMeshProUGUI>().text = replies[i].replyText;

            Reply reply = replies[i];

            replyButton.GetComponent<Button>().onClick.AddListener(delegate { SendReply(reply); });

            //Save
            if (!isLoading)
            {
                cachedDialogueManager.dialoguesToSave[cachedDialogueManager.dialoguesToSave.Count - 1].
                    elements[cachedDialogueManager.dialoguesToSave[cachedDialogueManager.dialoguesToSave.Count - 1].elements.Count - 1].AddReply(replies[i]);
            }
        }
        isWaitingForReply = true;
        UpdateDialogueState();
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

        if (!isLoading)
        {
            //Apply goodReplyIndex to the dialogues
            int goodReplyIndex = reply.index;

            cachedDialogueManager.dialogueList[currentDialogue.id].elements[currentDialogueElementId].chosenReplyIndex = goodReplyIndex;
            currentDialogue.elements[currentDialogueElementId].chosenReplyIndex = goodReplyIndex;


            //Create element, remove the element created in DisplayMessage, and adds the new one
            Reply newReply = new Reply(reply.replyText, reply.reaction, goodReplyIndex, reply.reactionTime, reply.replyEvent);

            DialogueElement newElement = new DialogueElement(currentDialogue.elements[currentDialogueElementId].message, newReply, currentDialogue.elements[currentDialogueElementId].index,
                currentDialogue.elements[currentDialogueElementId].initiationTime, currentDialogue.elements[currentDialogueElementId].elementAction);
            newElement.chosenReplyIndex = goodReplyIndex;

            isWaitingForReply = false;

            //Save
            EditLastSavedElement(newElement);

            //Clock
            SetWaitingTime(reply.reactionTime);
            timeManager.StartClock(currentWaitingTime);

            if (currentDialogue.elements[currentDialogueElementId].chosenReplyIndex != -1)
            {
                if (currentDialogue.elements[currentDialogueElementId].replies[currentDialogue.elements[currentDialogueElementId].chosenReplyIndex].replyEvent != null)
                {
                    currentDialogue.elements[currentDialogueElementId].replies[currentDialogue.elements[currentDialogueElementId].chosenReplyIndex].replyEvent.Invoke();
                }
            }
        }

        if (reply.reaction == "" || reply.reaction == null)
        {
            GoToNextElement();
        }
        else
        {
            awaitingReaction = reply.reaction;
        }

        StartCoroutine(SetObjectHeightToBackground(responsePrefab, messageBubble.textBackground, messagePanel));
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
        if (!isLoading)
        {
            InvokeEvent(currentDialogue.elements[currentDialogueElementId].elementAction);

            if (cameFromBranch)
            {
                cameFromBranch = false;
                StartDialogue(cachedDialogueManager.dialogueList[cachedDialogueManager.dialogueList.Count - 1]);
            }
            else if (currentDialogueElementId + 1 >= currentDialogue.elements.Count)
            {
                StopDialogue(currentDialogue);
            }
            else
            {
                currentDialogueElementId++;

                isInitialisation = true;
                bubbleSpawned = false;

                SetWaitingTime(currentDialogue.elements[currentDialogueElementId].initiationTime);
                timeManager.StartClock(currentWaitingTime);

                writingTime = SetWritingTime(currentDialogue.elements[currentDialogueElementId].message);
                timeToStartWriting = SetTimeToStartWriting();
            }
        }
        else
        {
            if (currentDialogueElementId + 1 >= currentDialogue.elements.Count)
            {
                StopDialogue(currentDialogue);
            }
            else
            {
                //Debug.LogError("Incrementing DialogueElementId");
                currentDialogueElementId++;
            }
        }
    }

    #endregion

    #region Invoke element and dialogue event
    private void InvokeEvent(UnityAction action)
    {
        if (action != null && !isLoading)
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

    private bool BoolToInt(int originalBool)
    {
        return originalBool == 1;
    }
    #endregion
}