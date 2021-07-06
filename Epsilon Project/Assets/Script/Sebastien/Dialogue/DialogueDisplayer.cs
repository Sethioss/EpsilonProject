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
    public enum AllowedMessageType { NORMAL = 0, LINK = 1, LEAVE = 2, INFO = 3, END = 4, GAMEOVER = 5 };
    public AllowedMessageType allowedType;

    public Dialogue currentDialogue;
    public int currentDialogueElementId = 0;
    [HideInInspector]
    public bool isLoading = false;
    #endregion

    #region Cached variables
    private DialogueManager cachedDialogueManager;
    #endregion

    #region SaveWriting variables
    public int second;
    public int minute;
    public int hour;
    public int day;
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
    [HideInInspector]
    public bool tempHasReplied;
    [HideInInspector]
    public bool tempHasReacted;
    [HideInInspector]
    public bool tempIsFinished;

    [HideInInspector]
    public bool isInitialisation = true;
    [HideInInspector]
    public bool isWaitingForReply = false;
    [HideInInspector]
    public bool hasReplied = false;
    [HideInInspector]
    public bool reacting = true;
    [HideInInspector]
    public bool isFinished = false;
    private bool bubbleSpawned = false;

    public bool isHackingScene = false;

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

    [Header("Confirmation window which pops up when you want to go back to the main menu")]
    public AnimBanner confirmationMessage;

    DialogueData data;

    UserSettings userSettings;

    #endregion

    #region Unity Loop
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
        //Load les dialogues précédents
        cachedDialogueManager = DialogueManager.Instance;
        if (cachedDialogueManager.timeManager == null)
        {
            cachedDialogueManager.timeManager = TimeManager.Instance;
        }
        if (cachedDialogueManager.displayer == null)
        {
            cachedDialogueManager.displayer = DialogueDisplayer.Instance;
        }

        cachedDialogueManager.timeManager.currentlyWaiting = false;
        userSettings = UserSettings.Instance;
#if UNITY_EDITOR
        //DeleteDialogueData();
#endif

        Debug.Log("Press J to erase save data");

        LoadDialogueData();

        //Need to find a way to only create dialogues once
        bool isDialogueAlreadyInList = false;

        if (!isHackingScene)
        {
            for (int i = 0; i < cachedDialogueManager.mainChatDialoguesToSave.Count; i++)
            {
                if (cachedDialogueManager.GetUnlocalisedDialogue(cachedDialogueManager.dialogueFileToLoad.name) == cachedDialogueManager.GetUnlocalisedDialogue(cachedDialogueManager.mainChatDialoguesToSave[i].fileName))
                {
                    isDialogueAlreadyInList = true;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < cachedDialogueManager.hackingChatDialoguesToSave.Count; i++)
            {
                if (cachedDialogueManager.GetUnlocalisedDialogue(cachedDialogueManager.dialogueFileToLoad.name) == cachedDialogueManager.GetUnlocalisedDialogue(cachedDialogueManager.hackingChatDialoguesToSave[i].fileName))
                {
                    isDialogueAlreadyInList = true;
                    break;
                }
            }
        }

        if (!isDialogueAlreadyInList)
        {
            cachedDialogueManager.CreateAndStartDialogue(cachedDialogueManager.GetLocalisedDialogue(cachedDialogueManager.dialogueFileToLoad.name));
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            LoadDialogueData();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            SaveSystem.EraseDialogueData();
            SaveSystem.EraseHackingDialogueData();
            Debug.LogWarning("Save files deleted");
        }

        if (!isLoading)
        {
            if (!isFinished)
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
                        if (currentBubble == null)
                        {
                            CreateMessageBubble();
                        }
                        cachedDialogueManager.timeManager.StopClock();
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
                else
                {
                    cachedDialogueManager.timeManager.StopClock();
                }
            }
            else
            {
                cachedDialogueManager.timeManager.StopClock();
            }
        }
    }
    #endregion

    public void ClearDialogueChat()
    {
        foreach (Transform child in messagePanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    #region Save/Load
    public void DeleteDialogueData()
    {
        SaveSystem.EraseDialogueData();
        SaveSystem.EraseHackingDialogueData();
        SaveSystem.EraseTimeToReachData();
        SaveSystem.EraseMinigameProgressionData();
        SaveSystem.EraseCheckpointData();

        //Reset variables to their original values (Used for when the player erases their save and launched the game again without closing the app)
        cachedDialogueManager.dialogueList = new List<Dialogue>();
        cachedDialogueManager.mainChatDialoguesToSave = new List<Dialogue>();
        cachedDialogueManager.hackingChatDialoguesToSave = new List<Dialogue>();

        GameManager.Instance.ResetMinigameProgressionValues();

        cachedDialogueManager.dialogueFileToLoad = cachedDialogueManager.GetDialogueFileFromName("FR-Intro1");
    }
    public void UpdateDialogueState()
    {
        tempWaitingForReply = isWaitingForReply;
        tempIsInitialisation = isInitialisation;
        tempHasReplied = hasReplied;
        tempHasReacted = reacting;
        tempIsFinished = isFinished;

        if (!isHackingScene)
        {
            SaveDialogueData(cachedDialogueManager.mainChatDialoguesToSave);
        }
        else
        {
            SaveDialogueData(cachedDialogueManager.hackingChatDialoguesToSave);
        }
    }
    private void AddDialogueToSave(Dialogue dialogueToAdd)
    {
        if (!isLoading)
        {
            if (!isHackingScene)
            {
                cachedDialogueManager.mainChatDialoguesToSave.Add(dialogueToAdd);
                UpdateDialogueState();
                SaveDialogueData(cachedDialogueManager.mainChatDialoguesToSave);
            }
            else
            {
                cachedDialogueManager.hackingChatDialoguesToSave.Add(dialogueToAdd);
                UpdateDialogueState();
                SaveDialogueData(cachedDialogueManager.hackingChatDialoguesToSave);

            }
        }
    }
    private void AddElementToSave(DialogueElement elementToAdd)
    {
        if (!isLoading)
        {
            if (!isHackingScene)
            {
                cachedDialogueManager.mainChatDialoguesToSave[cachedDialogueManager.mainChatDialoguesToSave.Count - 1].AddDialogueElement(elementToAdd);

                UpdateDialogueState();
                SaveDialogueData(cachedDialogueManager.mainChatDialoguesToSave);
            }
            else
            {
                cachedDialogueManager.hackingChatDialoguesToSave[cachedDialogueManager.hackingChatDialoguesToSave.Count - 1].AddDialogueElement(elementToAdd);

                UpdateDialogueState();
                SaveDialogueData(cachedDialogueManager.hackingChatDialoguesToSave);
            }
        }
    }
    private void EditLastSavedElement(DialogueElement elementToEdit)
    {
        if (!isLoading)
        {
            if (!isHackingScene)
            {
                cachedDialogueManager.mainChatDialoguesToSave[cachedDialogueManager.dialogueList.Count - 1].elements[cachedDialogueManager.mainChatDialoguesToSave
                    [cachedDialogueManager.dialogueList.Count - 1].elements.Count - 1] = elementToEdit;

                UpdateDialogueState();
                SaveDialogueData(cachedDialogueManager.mainChatDialoguesToSave);
            }
            else
            {
                //Debug.LogWarning("Did the thing!");
                //Debug.LogWarning(cachedDialogueManager.dialogueList.Count - 1);
                //Debug.LogWarning(cachedDialogueManager.hackingChatDialoguesToSave[0].elements.Count - 1);

                cachedDialogueManager.hackingChatDialoguesToSave[0].elements[cachedDialogueManager.hackingChatDialoguesToSave
                    [0].elements.Count - 1] = elementToEdit;

                UpdateDialogueState();
                SaveDialogueData(cachedDialogueManager.hackingChatDialoguesToSave);
            }
        }
    }

    public void SaveDialogueData(List<Dialogue> dialogueListToSaveTo)
    {
        if (!isLoading)
        {
            data = null;
            if (!isHackingScene)
            {
                SaveSystem.SaveDialogue(dialogueListToSaveTo);
            }
            else
            {
                SaveSystem.SaveHackingDialogue(dialogueListToSaveTo);
            }
        }
    }
    public void LoadDialogueData()
    {
        if (!isHackingScene)
        {
            data = SaveSystem.LoadDialogue();
        }
        else
        {
            data = SaveSystem.LoadHackingDialogue();
        }

        if (data != null)
        {
            isLoading = true;

            isInitialisation = IntToBool(data.initialisation);
            isWaitingForReply = IntToBool(data.waitForReply);
            hasReplied = IntToBool(data.replied);
            reacting = IntToBool(data.reacted);
            UpdateDialogueState();

            cachedDialogueManager.wentBackHome = data.wentBackHome;
            cachedDialogueManager.wentToBridge = data.wentToBridge;
            cachedDialogueManager.dialogueCheckpoint = data.checkpoint;

            //Création des dialogues de retour
            cachedDialogueManager.dialogueList = RecreateDialogueListFromData(data);

            if (!isHackingScene)
            {
                cachedDialogueManager.mainChatDialoguesToSave = RestoreSavedDialogues(cachedDialogueManager.dialogueList, data);
                SaveDialogueData(cachedDialogueManager.mainChatDialoguesToSave);
            }
            else
            {
                cachedDialogueManager.hackingChatDialoguesToSave = RestoreSavedDialogues(cachedDialogueManager.dialogueList, data);
                SaveDialogueData(cachedDialogueManager.hackingChatDialoguesToSave);
            }

            DisplayLoadedDialogue(cachedDialogueManager.dialogueList);

            if (!isWaitingForReply)
            {
                //Load the current time to reach
                TimeToReachData timeToReachData = SaveSystem.LoadTimeToReach();

                second = timeToReachData.sec;
                minute = timeToReachData.min;
                hour = timeToReachData.hour;
                day = timeToReachData.day;

                try
                {
                    SetWaitingTime(day.ToString() + ":" + hour.ToString() + ":" + minute.ToString() + ":" + second.ToString());
                    cachedDialogueManager.timeManager.SetClockGoal(currentWaitingTime);
                }
                catch
                {
                    Debug.LogError("Time couldn't be reset. Setting time to autoModeTime");
                    SetWaitingTime(userSettings.autoModeWaitingTime);
                    cachedDialogueManager.timeManager.StartClock(currentWaitingTime);
                }
            }

            if (cachedDialogueManager.dialogueList.Count > 0 && !bubbleSpawned)
            {
                //Debug.LogError(cachedDialogueManager.dialogueList.Count);
                writingTime = SetWritingTime(cachedDialogueManager.dialogueList[cachedDialogueManager.dialogueList.Count - 1].elements[data.currentElement].message);
                timeToStartWriting = SetTimeToStartWriting();
            }

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
            Dialogue templateDialogue = cachedDialogueManager.CreateDialogue(cachedDialogueManager.GetLocalisedDialogue(data.dialogueFileName[i]));
            Dialogue dialogueToCreate = new Dialogue();
            dialogueToCreate.fileName = cachedDialogueManager.GetLocalisedDialogue(data.dialogueFileName[i]);
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
                            element.minigameLinkFinished = templateDialogue.elements[j].minigameLinkFinished;
                            //Debug.LogError("minigameLink : " + element.minigameLinkFinished);

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
    private List<Dialogue> RestoreSavedDialogues(List<Dialogue> originalDialogues, DialogueData data)
    {
        List<Dialogue> dialogueListToReturn = new List<Dialogue>();

        for (int i = 0; i < originalDialogues.Count; i++)
        {

            if (i != originalDialogues.Count - 1)
            {
                //Debug.LogWarning("Not the last dialogue");

                Dialogue dialogue = new Dialogue();
                dialogue.fileName = originalDialogues[i].fileName;
                dialogue.id = originalDialogues[i].id;
                dialogue = originalDialogues[i];
                dialogueListToReturn.Add(dialogue);
            }
            else
            {
                Dialogue dialogue = new Dialogue();
                dialogue.fileName = originalDialogues[i].fileName;
                dialogue.id = originalDialogues[i].id;
                dialogueListToReturn.Add(dialogue);

                //Debug.LogError("The last dialogue");
                for (int j = 0; j <= originalDialogues[i].elements.Count; j++)
                {
                    //Debug.LogWarning("J is " + j);
                    if (j <= data.currentElement)
                    {
                        dialogueListToReturn[i].AddDialogueElement(originalDialogues[i].elements[j]);
                    }
                }
            }

        }

        return dialogueListToReturn;
    }
    public void DisplayLoadedDialogue(List<Dialogue> loadedDialogues)
    {
        cachedDialogueManager.timeManager.StopClock();

        if (!isHackingScene)
        {
            //Chaque dialogue
            for (int i = 0; i < loadedDialogues.Count; i++)
            {
                currentDialogueElementId = 0;
                currentDialogue = loadedDialogues[i];

                //Debug.LogError(i);

                //Chaque dialogue jusqu'au dernier (Partagent le même comportement)
                if (i != loadedDialogues.Count - 1)
                {
                    //Debug.LogError("PAS dernier dialogue sauvegardé");
                    //Chaque élément
                    for (int j = 0; j < loadedDialogues[i].elements.Count; j++)
                    {
                        currentDialogueElementId = j;
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
                //Dernier dialogue, donc index de sauvegarde différents
                else
                {

                    //Debug.LogError("Dernier dialogue sauvegardé");
                    //Chaque élément
                    for (int j = 0; j < cachedDialogueManager.mainChatDialoguesToSave[i].elements.Count; j++)
                    {
                        currentDialogueElementId = j;
                        //Debug.LogError(j);
                        if (j != cachedDialogueManager.mainChatDialoguesToSave[i].elements.Count - 1)
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
                        //Dernier élément de la sauvegarde
                        else
                        {
                            if (!isInitialisation && !isWaitingForReply && !reacting && !hasReplied)
                            {
                                Debug.LogWarning("Loading :: The dialogue is finished");

                                CreateMessageBubble();
                                DisplayMessage();
                                if (loadedDialogues[i].elements[j].chosenReplyIndex != -1)
                                {
                                    SendReply(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex]);
                                    if (loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction != ""
                                        || loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction != null)
                                    {
                                        Debug.Log("Sending the message's reaction");
                                        CreateMessageBubble();
                                        DisplayReaction(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction);
                                    }
                                }

                                isFinished = true;
                                UpdateDialogueState();
                                bubbleSpawned = false;
                            }
                            //Un comportement spécifique pour chaque état
                            else if (isInitialisation)
                            {
                                if (reacting)
                                {
                                    Debug.LogWarning("Loading :: The dialogue is currently in a state where there's a message pending, but the \"...\" bubble isn't sent yet");

                                    bubbleSpawned = false;

                                    /*SetWaitingTime(currentDialogue.elements[currentDialogueElementId].initiationTime);
                                    timeManager.StartClock(currentWaitingTime);*/

                                    writingTime = SetWritingTime(currentDialogue.elements[currentDialogueElementId].message);
                                    timeToStartWriting = SetTimeToStartWriting();
                                }

                                else
                                {
                                    //Attente du message
                                    Debug.LogWarning("Loading :: The dialogue is currently in a state where the message bubble has spawned but the message hasn't been written yet");
                                    CreateMessageBubble();

                                    /*SetWaitingTime(currentDialogue.elements[currentDialogueElementId].initiationTime);
                                    timeManager.StartClock(currentWaitingTime);*/

                                    writingTime = SetWritingTime(currentDialogue.elements[currentDialogueElementId].message);
                                    timeToStartWriting = SetTimeToStartWriting();
                                }
                            }
                            else if (isWaitingForReply)
                            {
                                Debug.LogWarning("Loading :: The dialogue is currently in a state where it's waiting for a reply");
                                //Attente de la réponse
                                CreateMessageBubble();
                                DisplayMessage();

                                bubbleSpawned = false;

                            }
                            else
                            {
                                if (hasReplied && !reacting)
                                {
                                    Debug.LogWarning("Loading :: The dialogue is currently in a state where there's a reaction pending, but the \"...\" bubble isn't sent yet");
                                    //Attente de la réaction
                                    bubbleSpawned = false;

                                    CreateMessageBubble();
                                    DisplayMessage();


                                    if (loadedDialogues[i].elements[j].chosenReplyIndex != -1)
                                    {
                                        SendReply(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex]);

                                        if (loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction != "" ||
                                            loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction != null)
                                        {
                                            awaitingReaction = loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction;

                                            /*SetWaitingTime(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reactionTime);
                                            timeManager.StartClock(currentWaitingTime);*/

                                            writingTime = SetWritingTime(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction);
                                            timeToStartWriting = SetTimeToStartWriting();
                                        }

                                        if (loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].replyEvent != null)
                                        {
                                            loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].replyEvent.Invoke();
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning("Loading :: The dialogue is currently in a state where the reaction bubble has spawned but the reaction hasn't been written yet");
                                    //Attente de la réaction
                                    CreateMessageBubble();
                                    DisplayMessage();
                                    if (loadedDialogues[i].elements[j].chosenReplyIndex != -1)
                                    {
                                        SendReply(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex]);

                                        if (loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction != "" ||
                                            loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction != null)
                                        {
                                            CreateMessageBubble();
                                            /*SetWaitingTime(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reactionTime);
                                            timeManager.StartClock(currentWaitingTime);*/

                                            writingTime = SetWritingTime(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction);
                                            timeToStartWriting = SetTimeToStartWriting();
                                        }
                                        if (loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].replyEvent != null)
                                        {
                                            loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].replyEvent.Invoke();
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < loadedDialogues.Count; i++)
            {
                currentDialogueElementId = 0;
                currentDialogue = loadedDialogues[i];

                //Debug.LogError(i);

                //Chaque dialogue jusqu'au dernier (Partagent le même comportement)
                if (i != loadedDialogues.Count - 1)
                {
                    //Debug.LogError("PAS dernier dialogue sauvegardé");
                    //Chaque élément
                    for (int j = 0; j < loadedDialogues[i].elements.Count; j++)
                    {
                        currentDialogueElementId = j;
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
                //Dernier dialogue, donc index de sauvegarde différents
                else
                {

                    //Debug.LogError("Dernier dialogue sauvegardé");
                    //Chaque élément
                    for (int j = 0; j < cachedDialogueManager.hackingChatDialoguesToSave[i].elements.Count; j++)
                    {
                        currentDialogueElementId = j;
                        //Debug.LogError(j);
                        if (j != cachedDialogueManager.hackingChatDialoguesToSave[i].elements.Count - 1)
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
                        //Dernier élément de la sauvegarde
                        else
                        {
                            if (!isInitialisation && !isWaitingForReply && !reacting && !hasReplied)
                            {
                                Debug.LogWarning("Loading :: The dialogue is finished");

                                CreateMessageBubble();
                                DisplayMessage();
                                if (loadedDialogues[i].elements[j].chosenReplyIndex != -1)
                                {
                                    SendReply(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex]);
                                    if (loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction != ""
                                        || loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction != null)
                                    {
                                        Debug.Log("Sending the message's reaction");
                                        CreateMessageBubble();
                                        DisplayReaction(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction);
                                    }
                                }

                                isFinished = true;
                                UpdateDialogueState();
                                bubbleSpawned = false;
                            }
                            //Un comportement spécifique pour chaque état
                            else if (isInitialisation)
                            {
                                if (reacting)
                                {
                                    Debug.LogWarning("Loading :: The dialogue is currently in a state where there's a message pending, but the \"...\" bubble isn't sent yet");

                                    bubbleSpawned = false;

                                    /*SetWaitingTime(currentDialogue.elements[currentDialogueElementId].initiationTime);
                                    timeManager.StartClock(currentWaitingTime);*/

                                    writingTime = SetWritingTime(currentDialogue.elements[currentDialogueElementId].message);
                                    timeToStartWriting = SetTimeToStartWriting();
                                }

                                else
                                {
                                    //Attente du message
                                    Debug.LogWarning("Loading :: The dialogue is currently in a state where the message bubble has spawned but the message hasn't been written yet");
                                    CreateMessageBubble();

                                    /*SetWaitingTime(currentDialogue.elements[currentDialogueElementId].initiationTime);
                                    timeManager.StartClock(currentWaitingTime);*/

                                    writingTime = SetWritingTime(currentDialogue.elements[currentDialogueElementId].message);
                                    timeToStartWriting = SetTimeToStartWriting();
                                }
                            }
                            else if (isWaitingForReply)
                            {
                                Debug.LogWarning("Loading :: The dialogue is currently in a state where it's waiting for a reply");
                                //Attente de la réponse
                                CreateMessageBubble();
                                DisplayMessage();

                                bubbleSpawned = false;

                            }
                            else
                            {
                                if (hasReplied && !reacting)
                                {
                                    Debug.LogWarning("Loading :: The dialogue is currently in a state where there's a reaction pending, but the \"...\" bubble isn't sent yet");
                                    //Attente de la réaction
                                    bubbleSpawned = false;

                                    CreateMessageBubble();
                                    DisplayMessage();


                                    if (loadedDialogues[i].elements[j].chosenReplyIndex != -1)
                                    {
                                        SendReply(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex]);

                                        if (loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction != "" ||
                                            loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction != null)
                                        {
                                            awaitingReaction = loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction;

                                            /*SetWaitingTime(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reactionTime);
                                            timeManager.StartClock(currentWaitingTime);*/

                                            writingTime = SetWritingTime(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction);
                                            timeToStartWriting = SetTimeToStartWriting();
                                        }

                                        if (loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].replyEvent != null)
                                        {
                                            loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].replyEvent.Invoke();
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning("Loading :: The dialogue is currently in a state where the reaction bubble has spawned but the reaction hasn't been written yet");
                                    //Attente de la réaction
                                    CreateMessageBubble();
                                    DisplayMessage();
                                    if (loadedDialogues[i].elements[j].chosenReplyIndex != -1)
                                    {
                                        SendReply(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex]);

                                        if (loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction != "" ||
                                            loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction != null)
                                        {
                                            CreateMessageBubble();
                                            /*SetWaitingTime(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reactionTime);
                                            timeManager.StartClock(currentWaitingTime);*/

                                            writingTime = SetWritingTime(loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].reaction);
                                            timeToStartWriting = SetTimeToStartWriting();
                                        }
                                        if (loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].replyEvent != null)
                                        {
                                            loadedDialogues[i].elements[j].replies[loadedDialogues[i].elements[j].chosenReplyIndex].replyEvent.Invoke();
                                        }
                                    }
                                }
                            }
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

        //Set the pointer to the first dialog element
        currentDialogueElementId = 0;
        if (currentDialogue != null)
        {
            StopDialogue(currentDialogue);
        }

        //Set dialogue state to the start of a new element
        isWaitingForReply = false;
        isInitialisation = true;
        bubbleSpawned = false;
        cameFromBranch = false;
        hasReplied = false;
        reacting = true;
        isFinished = false;

        UpdateDialogueState();

    }
    public void StartDialogue(Dialogue dialogue)
    {
        Init();
        currentDialogue = dialogue;

        AddDialogueToSave(new Dialogue());
        if (!isHackingScene)
        {
            cachedDialogueManager.mainChatDialoguesToSave[cachedDialogueManager.mainChatDialoguesToSave.Count - 1].fileName = dialogue.fileName;
            cachedDialogueManager.mainChatDialoguesToSave[cachedDialogueManager.mainChatDialoguesToSave.Count - 1].id = dialogue.id;

            SaveDialogueData(cachedDialogueManager.mainChatDialoguesToSave);
        }
        else
        {
            cachedDialogueManager.hackingChatDialoguesToSave[cachedDialogueManager.hackingChatDialoguesToSave.Count - 1].fileName = dialogue.fileName;
            cachedDialogueManager.hackingChatDialoguesToSave[cachedDialogueManager.hackingChatDialoguesToSave.Count - 1].id = dialogue.id;

            SaveDialogueData(cachedDialogueManager.hackingChatDialoguesToSave);
        }

        //Set clock and writing time to the initiation time of the current element
        SetWaitingTime(currentDialogue.elements[currentDialogueElementId].initiationTime);
        cachedDialogueManager.timeManager.StartClock(currentWaitingTime);

        writingTime = SetWritingTime(currentDialogue.elements[currentDialogueElementId].message);
        timeToStartWriting = SetTimeToStartWriting();
    }
    public void StopDialogue(Dialogue dialogueToStop)
    {
        isWaitingForReply = false;
        isInitialisation = false;
        bubbleSpawned = false;
        cameFromBranch = false;
        hasReplied = false;
        reacting = false;
        isFinished = true;

        UpdateDialogueState();
        if (currentBubble != null)
        {
            currentBubble = null;
        }
        cachedDialogueManager.timeManager.StopClock();
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
                allowedType = AllowedMessageType.LEAVE;
                Debug.Log("Creating leaving message bubble");
                messagePrefab = GameObject.Instantiate(leaveMessagePrefab, transform.position, Quaternion.identity, messagePanel.transform);
                messageBubble = messagePrefab.GetComponent<MessageBubble>();

                messageBubble.message.text = "";
                messageBubble.gameObject.SetActive(false);

                currentBubble = messagePrefab;
                bubbleSpawned = true;

                StartCoroutine(SetObjectHeightToBackground(messagePrefab, messageBubble.textBackground, messagePanel));
            }
            else if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.INFO)
            {
                allowedType = AllowedMessageType.INFO;
                Debug.Log("Creating info message bubble");
                messagePrefab = GameObject.Instantiate(leaveMessagePrefab, transform.position, Quaternion.identity, messagePanel.transform);
                messageBubble = messagePrefab.GetComponent<MessageBubble>();

                messageBubble.message.text = "";
                messageBubble.gameObject.SetActive(false);

                currentBubble = messagePrefab;
                bubbleSpawned = true;

                StartCoroutine(SetObjectHeightToBackground(messagePrefab, messageBubble.textBackground, messagePanel));
            }
            else if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.GAMEOVER)
            {
                allowedType = AllowedMessageType.GAMEOVER;
                Debug.Log("Creating game over message bubble");
                messagePrefab = GameObject.Instantiate(leaveMessagePrefab, transform.position, Quaternion.identity, messagePanel.transform);
                messageBubble = messagePrefab.GetComponent<MessageBubble>();

                messageBubble.textBackground.GetComponent<Image>().color = messageBubble.gameOverColor;
                messageBubble.message.text = "";
                messageBubble.gameObject.SetActive(false);

                currentBubble = messagePrefab;
                bubbleSpawned = true;

                StartCoroutine(SetObjectHeightToBackground(messagePrefab, messageBubble.textBackground, messagePanel));
            }
            else if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.END)
            {
                allowedType = AllowedMessageType.END;
                messagePrefab = GameObject.Instantiate(leaveMessagePrefab, transform.position, Quaternion.identity, messagePanel.transform);
                messageBubble = messagePrefab.GetComponent<MessageBubble>();

                messageBubble.message.text = "";
                messageBubble.gameObject.SetActive(false);

                currentBubble = messagePrefab;
                bubbleSpawned = true;

                StartCoroutine(SetObjectHeightToBackground(messagePrefab, messageBubble.textBackground, messagePanel));
            }
            else if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.LINK)
            {
                allowedType = AllowedMessageType.LINK;
                messagePrefab = GameObject.Instantiate(interlocutorBubblePrefab, transform.position, Quaternion.identity, messagePanel.transform);
                messageBubble = messagePrefab.GetComponent<MessageBubble>();
                Button button = messageBubble.textBackground.GetComponent<Button>();

                try
                {
                    button.enabled = true;
                    button.onClick.AddListener(currentDialogue.elements[currentDialogueElementId].elementAction);
                }
                catch
                {

                }

                currentDialogue.elements[currentDialogueElementId].elementAction = null;
                messageBubble.message.text = "...";

                //Current bubble = The bubble which text is gonna get changed
                currentBubble = messagePrefab;
                bubbleSpawned = true;

                StartCoroutine(SetObjectHeightToBackground(messagePrefab, messageBubble.textBackground, messagePanel));

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
            if (isInitialisation)
            {
                reacting = false;
                UpdateDialogueState();
            }
            else
            {
                reacting = true;
                UpdateDialogueState();
            }

            DialogueElement newElement = new DialogueElement();

            if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.LEAVE)
            {
                if (allowedType == AllowedMessageType.LEAVE)
                {
                    messagePrefab = GameObject.Instantiate(leaveMessagePrefab, transform.position, Quaternion.identity, messagePanel.transform);
                    messageBubble = messagePrefab.GetComponent<MessageBubble>();

                    messageBubble.message.text = "";
                    messagePrefab.gameObject.SetActive(false);

                    currentBubble = messagePrefab;
                    bubbleSpawned = true;

                    allowedType = AllowedMessageType.LEAVE;

                    if (isInitialisation)
                    {
                        newElement = CreateLeaveMessageBubble(messageBubble);
                        AddElementToSave(newElement);
                    }
                    else
                    {
                        currentDialogue.elements[currentDialogueElementId].elementAction = null;
                    }

                    StartCoroutine(SetObjectHeightToBackground(messagePrefab, messageBubble.textBackground, messagePanel));
                }
                else
                {
                    GoToNextElement();
                }
            }
            else if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.INFO)
            {
                if (allowedType == AllowedMessageType.INFO || allowedType == AllowedMessageType.GAMEOVER)
                {
                    messagePrefab = GameObject.Instantiate(leaveMessagePrefab, transform.position, Quaternion.identity, messagePanel.transform);
                    messageBubble = messagePrefab.GetComponent<MessageBubble>();

                    messageBubble.message.text = "";
                    messagePrefab.gameObject.SetActive(false);

                    currentBubble = messagePrefab;
                    bubbleSpawned = true;

                    if (allowedType != AllowedMessageType.GAMEOVER)
                    {
                        allowedType = AllowedMessageType.INFO;
                    }
                    else
                    {
                        allowedType = AllowedMessageType.GAMEOVER;
                    }

                    if (isInitialisation)
                    {
                        newElement = CreateLeaveMessageBubble(messageBubble);
                        AddElementToSave(newElement);
                    }
                    else
                    {
                        currentDialogue.elements[currentDialogueElementId].elementAction = null;
                    }

                    StartCoroutine(SetObjectHeightToBackground(messagePrefab, messageBubble.textBackground, messagePanel));
                }
                else
                {
                    GoToNextElement();
                }
            }
            else if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.END)
            {
                if (allowedType == AllowedMessageType.END)
                {
                    UserSettings.Instance.hasUnlockedAutoMode = true;
                    SaveSystem.SaveSettings(userSettings);
                    messagePrefab = GameObject.Instantiate(leaveMessagePrefab, transform.position, Quaternion.identity, messagePanel.transform);
                    messageBubble = messagePrefab.GetComponent<MessageBubble>();

                    messageBubble.message.text = "";
                    messagePrefab.gameObject.SetActive(false);

                    currentBubble = messagePrefab;
                    bubbleSpawned = true;

                    allowedType = AllowedMessageType.END;

                    if (isInitialisation)
                    {
                        newElement = CreateLeaveMessageBubble(messageBubble);
                        AddElementToSave(newElement);
                    }
                    else
                    {
                        currentDialogue.elements[currentDialogueElementId].elementAction = null;
                    }

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

                    if (isInitialisation)
                    {
                        newElement = CreateLinkMessageBubble(messageBubble);
                        AddElementToSave(newElement);
                    }

                    StartCoroutine(SetObjectHeightToBackground(messagePrefab, messageBubble.textBackground, messagePanel));
                }
                else
                {
                    currentDialogue.elements[currentDialogueElementId].elementAction = null;
                    GoToNextElement();
                }
            }
            else if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.GAMEOVER)
            {
                if (allowedType == AllowedMessageType.GAMEOVER)
                {
                    messagePrefab = GameObject.Instantiate(leaveMessagePrefab, transform.position, Quaternion.identity, messagePanel.transform);
                    messageBubble = messagePrefab.GetComponent<MessageBubble>();

                    messageBubble.textBackground.GetComponent<Image>().color = messageBubble.gameOverColor;
                    messageBubble.message.text = "";
                    messagePrefab.gameObject.SetActive(false);

                    currentBubble = messagePrefab;
                    bubbleSpawned = true;

                    allowedType = AllowedMessageType.GAMEOVER;

                    if (isInitialisation)
                    {
                        newElement = CreateLeaveMessageBubble(messageBubble);
                        AddElementToSave(newElement);
                    }
                    else
                    {
                        currentDialogue.elements[currentDialogueElementId].elementAction = null;
                    }

                    StartCoroutine(SetObjectHeightToBackground(messagePrefab, messageBubble.textBackground, messagePanel));
                }
                else
                {
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

                if (isInitialisation)
                {
                    newElement = new DialogueElement(currentDialogue.elements[currentDialogueElementId].message, currentDialogue.elements[currentDialogueElementId].index,
                            currentDialogue.elements[currentDialogueElementId].initiationTime, currentDialogue.elements[currentDialogueElementId].elementAction);

                    AddElementToSave(newElement);
                }
            }
        }

    }
    private void DisplayMessage()
    {
        if (isLoading)
        {
            //Tous les types de message
            MessageBubble messageBubble = currentBubble.GetComponent<MessageBubble>();
            if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.LINK)
            {
                if (!currentDialogue.elements[currentDialogueElementId].minigameLinkFinished)
                {
                    Button button = messageBubble.textBackground.GetComponent<Button>();
                    button.enabled = true;
                    button.onClick.AddListener(currentDialogue.elements[currentDialogueElementId].elementAction);
                    currentDialogue.elements[currentDialogueElementId].elementAction = null;

                    DialogueElement newElement = new DialogueElement();

                    messageBubble.gameObject.SetActive(true);
                    messageBubble.message.text = currentDialogue.elements[currentDialogueElementId].message;
                }
                else
                {
                    Button button = messageBubble.textBackground.GetComponent<Button>();
                    button.enabled = false;
                    DialogueElement newElement = new DialogueElement();

                    messageBubble.gameObject.SetActive(true);
                    messageBubble.message.text = XMLManager.Instance.GetTagValue("unavailableLink");
                }
                currentDialogue.elements[currentDialogueElementId].elementAction = null;

            }
            else
            {
                DialogueElement newElement = new DialogueElement();

                messageBubble.gameObject.SetActive(true);

                messageBubble.message.text = currentDialogue.elements[currentDialogueElementId].message;
            }


            if (currentDialogue.elements[currentDialogueElementId].replies.Count > 0)
            {
                //Debug.LogWarning("Loading replies");
                DisplayPossibleReplies(currentDialogue.elements[currentDialogueElementId].replies);
            }
            else
            {
                GoToNextElement();
            }

        }
        else
        {
            //Tous les types de message
            MessageBubble messageBubble = currentBubble.GetComponent<MessageBubble>();

            if (currentDialogue.elements[currentDialogueElementId].messageType == DialogueElement.MessageType.LINK)
            {
                if (!currentDialogue.elements[currentDialogueElementId].minigameLinkFinished)
                {
                    Button button = messageBubble.textBackground.GetComponent<Button>();
                    button.enabled = true;
                    button.onClick.AddListener(currentDialogue.elements[currentDialogueElementId].elementAction);

                    DialogueElement newElement = new DialogueElement();

                    messageBubble.gameObject.SetActive(true);
                    messageBubble.message.text = currentDialogue.elements[currentDialogueElementId].message;
                }
                else
                {
                    Button button = messageBubble.textBackground.GetComponent<Button>();
                    button.enabled = false;
                    DialogueElement newElement = new DialogueElement();

                    messageBubble.gameObject.SetActive(true);
                    messageBubble.message.text = XMLManager.Instance.GetTagValue("unavailableLink");
                }
                currentDialogue.elements[currentDialogueElementId].elementAction = null;
            }
            else
            {
                DialogueElement newElement = new DialogueElement();

                messageBubble.gameObject.SetActive(true);

                messageBubble.message.text = currentDialogue.elements[currentDialogueElementId].message;
            }


            isInitialisation = false;
            bubbleSpawned = false;
            UpdateDialogueState();

            StartCoroutine(SetObjectHeightToBackground(currentBubble, messageBubble.textBackground, messagePanel));

            //Display replies if there are
            if (currentDialogue.elements[currentDialogueElementId].replies.Count > 0)
            {
                DisplayPossibleReplies(currentDialogue.elements[currentDialogueElementId].replies);
            }
            else
            {
                GoToNextElement();
            }
        }
    }
    private DialogueElement CreateLinkMessageBubble(MessageBubble messageBubble)
    {
        DialogueElement newElement = new DialogueElement(currentDialogue.elements[currentDialogueElementId].message, currentDialogue.elements[currentDialogueElementId].index,
        currentDialogue.elements[currentDialogueElementId].initiationTime, currentDialogue.elements[currentDialogueElementId].elementAction);

        newElement.messageType = currentDialogue.elements[currentDialogueElementId].messageType;
        newElement.minigameLinkFinished = currentDialogue.elements[currentDialogueElementId].minigameLinkFinished;

        return newElement;
    }
    private DialogueElement CreateLeaveMessageBubble(MessageBubble messageBubble)
    {
        DialogueElement newElement = new DialogueElement(XMLManager.Instance.GetTagValue("leaveMessage"), currentDialogue.elements[currentDialogueElementId].index,
         currentDialogue.elements[currentDialogueElementId].initiationTime, currentDialogue.elements[currentDialogueElementId].elementAction);

        newElement.messageType = currentDialogue.elements[currentDialogueElementId].messageType;

        return newElement;
    }
    private void DisplayPossibleReplies(List<Reply> replies)
    {
        DeleteReplies();

        if (isLoading)
        {

            //Create the buttons corresponding to each answer
            for (int i = 0; i < replies.Count; i++)
            {
                GameObject replyButton = Instantiate(replyButtonPrefab, repliesPanel.transform);
                replyButton.GetComponentInChildren<TextMeshProUGUI>().text = replies[i].replyText;

                Reply reply = replies[i];

                replyButton.GetComponent<Button>().onClick.AddListener(delegate { SendReply(reply); });
            }
        }
        else
        {
            //Create the buttons corresponding to each answer
            for (int i = 0; i < replies.Count; i++)
            {
                GameObject replyButton = Instantiate(replyButtonPrefab, repliesPanel.transform);
                replyButton.GetComponentInChildren<TextMeshProUGUI>().text = replies[i].replyText;

                Reply reply = replies[i];

                replyButton.GetComponent<Button>().onClick.AddListener(delegate { SendReply(reply); });
            }

            isWaitingForReply = true;
            isInitialisation = false;
            UpdateDialogueState();
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
        if (isLoading)
        {
            if (currentDialogue.elements[currentDialogueElementId].messageType != DialogueElement.MessageType.LEAVE)
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

                StartCoroutine(SetObjectHeightToBackground(responsePrefab, messageBubble.textBackground, messagePanel));
            }

        }
        else
        {
            if (currentDialogue.elements[currentDialogueElementId].messageType != DialogueElement.MessageType.LEAVE &&
                currentDialogue.elements[currentDialogueElementId].messageType != DialogueElement.MessageType.GAMEOVER &&
                currentDialogue.elements[currentDialogueElementId].messageType != DialogueElement.MessageType.END)
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

                //Apply goodReplyIndex to the dialogues
                int goodReplyIndex = reply.index;
                //Debug.LogError("Good reply index = " + goodReplyIndex);

                cachedDialogueManager.dialogueList[currentDialogue.id].elements[currentDialogueElementId].chosenReplyIndex = goodReplyIndex;
                currentDialogue.elements[currentDialogueElementId].chosenReplyIndex = goodReplyIndex;

                //Create element, remove the element created in DisplayMessage, and adds the new one
                Reply newReply = new Reply(reply.replyText, reply.reaction, goodReplyIndex, reply.reactionTime, reply.replyEvent);

                DialogueElement newElement = currentDialogue.elements[currentDialogueElementId];
                //Debug.LogError("Set new element");

                //Save
                EditLastSavedElement(newElement);
                //Debug.LogError("EditedElement");

                //Clock
                SetWaitingTime(reply.reactionTime);
                cachedDialogueManager.timeManager.StartClock(currentWaitingTime);

                writingTime = SetWritingTime(reply.reaction);
                timeToStartWriting = SetTimeToStartWriting();

                if (currentDialogue.elements[currentDialogueElementId].chosenReplyIndex != -1)
                {
                    if (currentDialogue.elements[currentDialogueElementId].replies[currentDialogue.elements[currentDialogueElementId].chosenReplyIndex].replyEvent != null)
                    {
                        currentDialogue.elements[currentDialogueElementId].replies[currentDialogue.elements[currentDialogueElementId].chosenReplyIndex].replyEvent.Invoke();
                    }
                }

                hasReplied = true;
                isWaitingForReply = false;
                UpdateDialogueState();

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
            else
            {
                //Apply goodReplyIndex to the dialogues
                int goodReplyIndex = reply.index;

                cachedDialogueManager.dialogueList[currentDialogue.id].elements[currentDialogueElementId].chosenReplyIndex = goodReplyIndex;
                currentDialogue.elements[currentDialogueElementId].chosenReplyIndex = goodReplyIndex;

                currentDialogue.elements[currentDialogueElementId].replies[currentDialogue.elements[currentDialogueElementId].chosenReplyIndex].replyEvent.Invoke();
            }
        }
    }
    private void DisplayReaction(string reaction)
    {
        if (isLoading)
        {
            MessageBubble messageBubble = currentBubble.GetComponent<MessageBubble>();

            GameObject imageBg = messageBubble.textBackground;
            TextMeshProUGUI textInBubble = messageBubble.message;

            textInBubble.text = reaction;
            StartCoroutine(SetObjectHeightToBackground(currentBubble, imageBg, messagePanel));
        }
        else
        {
            MessageBubble messageBubble = currentBubble.GetComponent<MessageBubble>();

            GameObject imageBg = messageBubble.textBackground;
            TextMeshProUGUI textInBubble = messageBubble.message;

            textInBubble.text = reaction;
            StartCoroutine(SetObjectHeightToBackground(currentBubble, imageBg, messagePanel));

            isWaitingForReply = false;
            isInitialisation = true;
            hasReplied = false;
            bubbleSpawned = false;
            UpdateDialogueState();

            GoToNextElement();
        }
    }
    #endregion

    #region Dialogue navigation
    private void GoToNextElement()
    {
        if (!isLoading)
        {
            hasReplied = false;

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
                isWaitingForReply = false;
                UpdateDialogueState();

                SetWaitingTime(currentDialogue.elements[currentDialogueElementId].initiationTime);
                cachedDialogueManager.timeManager.StartClock(currentWaitingTime);

                writingTime = SetWritingTime(currentDialogue.elements[currentDialogueElementId].message);
                timeToStartWriting = SetTimeToStartWriting();
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
        if (message != null || message != "")
        {
            try
            {
                int typingSpeed = GetTypingSpeed(message);

                typingTime = typingTime.AddSeconds(typingSpeed);
            }
            catch
            {

            }

        }
        return typingTime;
    }

    private int GetTypingSpeed(string message)
    {
        return message.Length / 3;
    }

    private System.DateTime SetTimeToStartWriting()
    {
        System.DateTime temp = new System.DateTime();
        try
        {
            System.TimeSpan typingTime = System.DateTime.MinValue.Subtract(writingTime);
            temp = cachedDialogueManager.timeManager.timeToReach.Add(typingTime);
            //Debug.Log(timeManager.timeToReach);

            if (temp < cachedDialogueManager.timeManager.currentTime)
            {
                temp = cachedDialogueManager.timeManager.currentTime;
            }
        }
        catch
        {
            temp = cachedDialogueManager.timeManager.timeToReach.AddSeconds(1);
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
        //Debug.LogError("Time saved");

    }
    #endregion

    #region Flags
    private bool IsTimeOver()
    {
        try
        {
            return cachedDialogueManager.timeManager.currentTime >= cachedDialogueManager.timeManager.timeToReach && cachedDialogueManager.timeManager.currentlyWaiting;
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
            return cachedDialogueManager.timeManager.currentTime >= timeToStartWriting && cachedDialogueManager.timeManager.currentlyWaiting;
        }
        catch
        {
            return false;
        }
    }
    #endregion

    #region SaveWriting functions

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

    private bool IntToBool(int originalBool)
    {
        return originalBool == 1;
    }
    #endregion
}