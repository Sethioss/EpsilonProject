using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Localisation Debug")]
    public string localisedDialogue;
    public string unlocalisedDialogue;
    [Header("Disregards all dialogue localisations (Defaults to French)")]
    public bool ignoreDialogueLocalisation;

    [HideInInspector]
    public bool onGameSceneEntered = false;

    #region Dialogue Manager Components
    [HideInInspector]
    public CSVReader reader;
    [HideInInspector]
    public DialogueDisplayer displayer;
    [HideInInspector]
    public TimeManager timeManager;

    [Header("Dialogue initialisation and list")]
    [HideInInspector]
    public TextAsset currentDialogueFile;
    public TextAsset dialogueFileToLoad;
    [HideInInspector]
    public string dialogueCheckpoint;

    public List<Dialogue> dialogueList;
    #endregion

    public List<Dialogue> mainChatDialoguesToSave = new List<Dialogue>();
    public List<Dialogue> hackingChatDialoguesToSave = new List<Dialogue>();

    #region Debug
    [Header("Debugging tools")]
    [Tooltip("Sends debug messages for each command keyword found in the dialogue file")]
    public bool debugReadCommandKeywords = false;
    [Tooltip("Sends debug messages for each function that is played when its call is made")]
    public bool debugExecutingFunction = false;

    private List<string> debugMessages { get; } = new List<string>();
    private string colorCodeStart = "";
    private string colorCodeEnd { get; } = "</color>";

    #endregion

    #region Game Variables

    public string testSet { get { return m_testSet; } set { m_testSet = value; } }
    public float testSetFloat { get { return m_testSetFloat; } set { m_testSetFloat = value; } }
    public float wentBackHome { get { return m_wentBackHome; } set { m_wentBackHome = value; } }
    public float wentToBridge { get { return m_wentToBridge; } set { m_wentToBridge = value; } }

    [Header("Accessible test variables (Changed by events)")]
    [SerializeField]
    [Tooltip("Test for string variables")]
    private string m_testSet;
    [SerializeField]
    [Tooltip("Test for float variables")]
    private float m_testSetFloat;

    [Header("0 = false, 1 = true")]
    [Header("Accessible variables that are in the game (Changed by events)")]

    [SerializeField]
    private float m_wentBackHome = 0;
    [SerializeField]
    [Header("0 = false, 1 = true")]
    private float m_wentToBridge = 0;

    public string username = "Sebilol";

    public AnimBanner confirmationMessage;

    #endregion

    private static DialogueManager instance;
    public static DialogueManager Instance
    {
        get
        {
            return instance;
        }
    }

    public string GetUnlocalisedDialogue(TextAsset dialogue)
    {
        string dialogueName = dialogue.name;
        int endOfLocalisation = dialogueName.IndexOf("-");
        string unlocalisedDialogueName = dialogueName.Substring(endOfLocalisation + 1, dialogueName.Length - endOfLocalisation - 1);

        unlocalisedDialogue = unlocalisedDialogueName;
        return unlocalisedDialogueName;
    }

    public string GetLocalisedDialogue(TextAsset dialogue)
    {
        string dialogueName = dialogue.name;

        if (!ignoreDialogueLocalisation)
        {
            dialogueName = UserSettings.Instance.languagePrefix + "-" + GetUnlocalisedDialogue(dialogue);
        }
        else
        {
            dialogueName = "FR-" + GetUnlocalisedDialogue(dialogue);
        }

        localisedDialogue = dialogueName;
        return dialogueName;
    }
    public string GetUnlocalisedDialogue(string dialogue)
    {
        string dialogueName = dialogue;
        int endOfLocalisation = dialogueName.IndexOf("-");
        string unlocalisedDialogueName = dialogueName.Substring(endOfLocalisation + 1, dialogueName.Length - endOfLocalisation - 1);

        unlocalisedDialogue = unlocalisedDialogueName;
        return unlocalisedDialogueName;
    }

    public string GetLocalisedDialogue(string dialogue)
    {
        string dialogueName = dialogue;
        if (!ignoreDialogueLocalisation)
        {
            dialogueName = UserSettings.Instance.languagePrefix + "-" + GetUnlocalisedDialogue(dialogue);
        }
        else
        {
            dialogueName = "FR-" + GetUnlocalisedDialogue(dialogue);
        }

        localisedDialogue = dialogueName;
        TextAsset temp = GetElementFileFromName(dialogueName);
        currentDialogueFile = temp;

        return dialogueName;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this.gameObject);
        }

        reader = CSVReader.Instance;
        displayer = DialogueDisplayer.Instance;
        timeManager = TimeManager.Instance;
    }

    private void Update()
    {
        if (onGameSceneEntered)
        {
            StartDialogue();
        }
    }

    public void StartDialogue()
    {
        reader = CSVReader.Instance;
        displayer = DialogueDisplayer.Instance;
        timeManager = TimeManager.Instance;
        CreateAndStartDialogue(currentDialogueFile);
        onGameSceneEntered = false;
    }

    #region Debugging
    public void DebugElement(string[] debugMessages)
    {
        foreach (string str in debugMessages)
        {
            Debug.Log(str);
            debugMessages = null;
        }
    }

    public void AddToDebugFunctionMessage(string messageToPass, List<string> listToAddTo)
    {
        if (debugExecutingFunction)
        {
            listToAddTo.Add(colorCodeStart + messageToPass + colorCodeEnd);
        }
    }
    #endregion

    public TextAsset GetElementFileFromName(string name)
    {
        return (TextAsset)Resources.Load("Tables\\" + name);
    }
    private void AddDialogueToDialogueList(Dialogue dialogueToAdd)
    {
        dialogueList.Add(dialogueToAdd);
        localisedDialogue = GetLocalisedDialogue(GetElementFileFromName(dialogueFileToLoad.name));
    }
    private void StartDialogue(Dialogue dialogueToStart)
    {
        displayer.StartDialogue(dialogueToStart);
    }

    #region BRANCH Command / Dialogue Initialisation

    public void Branch(string dialogueFileName)
    {
        if (!displayer.isLoading)
        {
            if (!displayer.cameFromBranch)
            {
                displayer.cameFromBranch = true;
                if (dialogueFileName != "")
                {
                    dialogueList.Insert(dialogueList.Count, CreateDialogue(GetLocalisedDialogue(UserSettings.Instance.languagePrefix + "-" + dialogueFileName)));
                }
            }
        }
    }

    public Dialogue CreateDialogue(string dialogueFileName)
    {
        reader = CSVReader.Instance;
        displayer = DialogueDisplayer.Instance;
        timeManager = TimeManager.Instance;

        currentDialogueFile = GetElementFileFromName(dialogueFileName);
        Dialogue dialogueToAdd = reader.CreateDialogueFromData(currentDialogueFile);
        dialogueToAdd.id = dialogueList.Count;
        dialogueToAdd.fileName = dialogueFileName;

        return dialogueToAdd;
    }

    //The text asset function executes for the starting file
    public void CreateAndStartDialogue(TextAsset dialogueFile)
    {
        reader = CSVReader.Instance;
        displayer = DialogueDisplayer.Instance;
        timeManager = FindObjectOfType<TimeManager>();

        currentDialogueFile = dialogueFile;
        Dialogue dialogueToAdd = reader.CreateDialogueFromData(dialogueFile);
        dialogueToAdd.id = dialogueList.Count;
        dialogueToAdd.fileName = dialogueFile.name;
        AddDialogueToDialogueList(dialogueToAdd);
        StartDialogue(dialogueToAdd);
    }

    //The string function executes when a BRANCH command is called
    public void CreateAndStartDialogue(string dialogueFileName)
    {

#if UNITY_EDITOR
        colorCodeStart = "<color=yellow>";
        AddToDebugFunctionMessage("=======BRANCH FUNCTION EXECUTING=======", debugMessages);
        AddToDebugFunctionMessage("Switching from branch " + currentDialogueFile.name + " to branch: " + dialogueFileName, debugMessages);

        if (debugExecutingFunction)
        {
            DebugElement(debugMessages.ToArray());
        }
#endif

        reader = CSVReader.Instance;
        displayer = DialogueDisplayer.Instance;
        timeManager = FindObjectOfType<TimeManager>();

        currentDialogueFile = (TextAsset)Resources.Load("Tables\\" + dialogueFileName);
        Dialogue dialogueToAdd = reader.CreateDialogueFromData((TextAsset)Resources.Load("Tables\\" + dialogueFileName));
        dialogueToAdd.id = dialogueList.Count;
        AddDialogueToDialogueList(dialogueToAdd);
        StartDialogue(dialogueToAdd);
    }
    #endregion

    #region CHECK Command
    public void CompareFloatVariables(string variableToCompare, string variableComparedWith, string firstCommand, string secondCommand, string op)
    {
        UnityAction eventToTrigger = null;

        System.Reflection.PropertyInfo prop = (DialogueManager.Instance.GetType().GetProperty(variableToCompare));

        float floatVariableToCompare = (float)prop.GetValue(DialogueManager.Instance);
        float floatVariableComparedWith = float.Parse(variableComparedWith);

        List<string> debugMessages = new List<string>();

#if UNITY_EDITOR
        colorCodeStart = "<color=green>";
        AddToDebugFunctionMessage("=======CHECK FUNCTION (Float) EXECUTING=======", debugMessages);
        AddToDebugFunctionMessage("First command in the delegate : " + firstCommand, debugMessages);
        AddToDebugFunctionMessage("Second command in the delegate : " + secondCommand, debugMessages);
        AddToDebugFunctionMessage("Comparison : " + floatVariableToCompare + " " + op + " " + floatVariableComparedWith, debugMessages);
#endif

        switch (op)
        {
            //Equal
            case "=":
                if (floatVariableToCompare == floatVariableComparedWith)
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("Yes, " + variableToCompare + " is the same value as " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing first command : " + firstCommand, debugMessages);

                    if (debugExecutingFunction)
                    {
                        DebugElement(debugMessages.ToArray());
                    }
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(firstCommand, true); };
                    break;
                }
                else
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("No, " + variableToCompare + " isn't the same value as " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing second command : " + secondCommand, debugMessages);
                    if (debugExecutingFunction)
                    {
                        DebugElement(debugMessages.ToArray());
                    }
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(secondCommand, true); };
                    break;
                }

            //Different
            case "!=":
                if (floatVariableToCompare != floatVariableComparedWith)
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("Yes, " + variableToCompare + " isn't the same value as " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing first command : " + firstCommand, debugMessages);
                    if (debugExecutingFunction)
                    {
                        DebugElement(debugMessages.ToArray());
                    }
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(firstCommand, true); };
                    break;
                }
                else
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("No, " + variableToCompare + " is the same value as " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing second command : " + secondCommand, debugMessages);
                    if (debugExecutingFunction)
                    {
                        DebugElement(debugMessages.ToArray());
                    }
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(secondCommand, true); };
                    break;
                }

            //Strictly superior
            case "+":
                if (floatVariableToCompare > floatVariableComparedWith)
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("Yes, " + variableToCompare + " is higher than " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing first command : " + firstCommand, debugMessages);
                    if (debugExecutingFunction)
                    {
                        DebugElement(debugMessages.ToArray());
                    }
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(firstCommand, true); };
                    break;
                }
                else
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("No, " + variableToCompare + " isn't higher than " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing second command : " + secondCommand, debugMessages);
                    if (debugExecutingFunction)
                    {
                        DebugElement(debugMessages.ToArray());
                    }
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(secondCommand, true); };
                    break;
                }

            //Strictly inferior
            case "-":
                if (floatVariableToCompare < floatVariableComparedWith)
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("Yes, " + variableToCompare + " is less than " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing first command : " + firstCommand, debugMessages);
                    if (debugExecutingFunction)
                    {
                        DebugElement(debugMessages.ToArray());
                    }
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(firstCommand, true); };
                    break;
                }
                else
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("No, " + variableToCompare + " isn't less than " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing second command : " + secondCommand, debugMessages);
                    DebugElement(debugMessages.ToArray());
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(secondCommand, true); };
                    break;
                }

            //Superior or equal
            case "+=":
                if (floatVariableToCompare >= floatVariableComparedWith)
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("Yes, " + variableToCompare + " is higher or equal to " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing first command : " + firstCommand, debugMessages);
                    DebugElement(debugMessages.ToArray());
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(firstCommand, true); };
                    break;
                }
                else
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("No, " + variableToCompare + " isn't higher or equal to " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing second command : " + secondCommand, debugMessages);
                    DebugElement(debugMessages.ToArray());
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(secondCommand, true); };
                    break;
                }

            //Inferior or equal
            case "-=":
                if (floatVariableToCompare <= floatVariableComparedWith)
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("Yes, " + variableToCompare + " is less or equal to " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing first command : " + firstCommand, debugMessages);
                    DebugElement(debugMessages.ToArray());
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(firstCommand, true); };
                    break;
                }
                else
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("No, " + variableToCompare + " isn't less or equal to " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing second command : " + secondCommand, debugMessages);
                    DebugElement(debugMessages.ToArray());
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(secondCommand, true); };
                    break;
                }

            default:
#if UNITY_EDITOR
                AddToDebugFunctionMessage("String variable " + variableToCompare + " couldn't be compared with " + variableComparedWith, debugMessages);
                DebugElement(debugMessages.ToArray());
#endif
                break;
        }

        eventToTrigger.Invoke();
    }

    public void CompareStringVariables(string variableToCompare, string variableComparedWith, string firstCommand, string secondCommand, string op)
    {
        UnityAction eventToTrigger = null;

        System.Reflection.PropertyInfo prop = (DialogueManager.Instance.GetType().GetProperty(variableToCompare));
        variableToCompare = (string)prop.GetValue(DialogueManager.Instance);
        variableComparedWith = variableComparedWith.Trim(new char[] { '\'' });

#if UNITY_EDITOR
        colorCodeStart = "<color=green>";
        AddToDebugFunctionMessage("=======CHECK FUNCTION (String) EXECUTING=======", debugMessages);
        AddToDebugFunctionMessage("First command in the delegate: " + firstCommand, debugMessages);
        AddToDebugFunctionMessage("Second command in the delegate: " + secondCommand, debugMessages);
        AddToDebugFunctionMessage("Comparison : " + variableToCompare + " " + op + " " + variableComparedWith, debugMessages);
#endif

        switch (op)
        {
            case "=":
                if (variableToCompare == variableComparedWith)
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("Yes, " + variableToCompare + " is the same value as " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing first command : " + firstCommand, debugMessages);
                    DebugElement(debugMessages.ToArray());
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(firstCommand, true); };
                    break;
                }
                else
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("No, " + variableToCompare + " isn't the same value as " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing second command : " + secondCommand, debugMessages);
                    DebugElement(debugMessages.ToArray());
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(secondCommand, true); };
                    break;
                }

            case "!=":
                if (variableToCompare != variableComparedWith)
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("Yes, " + variableToCompare + " isn't the same value as " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing first command : " + firstCommand, debugMessages);
                    DebugElement(debugMessages.ToArray());
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(firstCommand, true); };
                    break;
                }
                else
                {
#if UNITY_EDITOR
                    AddToDebugFunctionMessage("No, " + variableToCompare + " is the same value as " + variableComparedWith, debugMessages);
                    AddToDebugFunctionMessage("Executing second command : " + secondCommand, debugMessages);
                    DebugElement(debugMessages.ToArray());
#endif
                    eventToTrigger += delegate { CSVReader.Instance.SetEvents(secondCommand, true); };
                    break;
                }

            default:
#if UNITY_EDITOR
                AddToDebugFunctionMessage("String variable " + variableToCompare + " couldn't be compared with " + variableComparedWith, debugMessages);
#endif
                break;
        }

        eventToTrigger.Invoke();
    }
    #endregion

    #region SET Command
    public void SetStringVariable(string variable, string newValue)
    {

        System.Reflection.PropertyInfo prop = (DialogueManager.Instance.GetType().GetProperty(variable));
        string newValueDebug = newValue;

        if (newValue[0] == '+')
        {
            newValue = prop.GetValue(DialogueManager.Instance) + newValue.Trim(new char[] { '+', '\'' });
        }
        else
        {
            newValue = newValue.Trim(new char[] { '+', '\'' });
        }

#if UNITY_EDITOR
        colorCodeStart = "<color=red>";
        AddToDebugFunctionMessage("=======SET FUNCTION (String) EXECUTING=======", debugMessages);
        AddToDebugFunctionMessage(colorCodeStart + "SET FUNCTION (String) :: Inside the delegate, newValue is " + newValueDebug + colorCodeEnd, debugMessages);
        AddToDebugFunctionMessage(colorCodeStart + "SET FUNCTION (String) :: Setting " + variable + " to " + newValue + colorCodeEnd, debugMessages);
        DebugElement(debugMessages.ToArray());
#endif

        prop.SetValue(DialogueManager.Instance, newValue);
    }

    public void SetFloatVariable(string variable, string newValue)
    {
        System.Reflection.PropertyInfo prop = (DialogueManager.Instance.GetType().GetProperty(variable));

        float newFloatValue = float.Parse(newValue.Trim(new char[] { '+', '-', '*', '\'' }));

        float propValue = (float)prop.GetValue(DialogueManager.Instance);

        if (newValue[0] == '+')
        {
            propValue += newFloatValue;
        }
        else if (newValue[0] == '-')
        {
            propValue -= newFloatValue;
        }
        else if (newValue[0] == '*')
        {
            propValue *= newFloatValue;
        }
        else
        {
            propValue = newFloatValue;
        }

#if UNITY_EDITOR
        colorCodeStart = "<color=red>";
        AddToDebugFunctionMessage("=======SET FUNCTION (Float) EXECUTING=======", debugMessages);
        AddToDebugFunctionMessage("SET FUNCTION (Float) :: Inside the delegate, newValue is " + newFloatValue, debugMessages);
        AddToDebugFunctionMessage("SET FUNCTION (Float) :: Setting " + variable + " to " + newValue, debugMessages);
        DebugElement(debugMessages.ToArray());
#endif

        prop.SetValue(DialogueManager.Instance, propValue);
    }
    #endregion

    #region LINK/LEAVE/INFO Command
    public void SpecialMessage(string sceneToChangeTo, int allowedType)
    {
        displayer.allowedType = (DialogueDisplayer.AllowedMessageType)allowedType;

#if UNITY_EDITOR
        if (displayer.allowedType == DialogueDisplayer.AllowedMessageType.LINK)
        {
            colorCodeStart = "<color=blue>";
            AddToDebugFunctionMessage("=======LINK FUNCTION EXECUTING=======", debugMessages);
            AddToDebugFunctionMessage(colorCodeStart + "Sending a link that goes to Scene " + sceneToChangeTo + colorCodeEnd, debugMessages);
            DebugElement(debugMessages.ToArray());
        }
        else if (displayer.allowedType == DialogueDisplayer.AllowedMessageType.LEAVE)
        {
            colorCodeStart = "<color=blue>";
            AddToDebugFunctionMessage("=======LEAVE FUNCTION EXECUTING=======", debugMessages);
            AddToDebugFunctionMessage(colorCodeStart + "Sending a leave message that leads to Scene " + sceneToChangeTo + colorCodeEnd, debugMessages);
            DebugElement(debugMessages.ToArray());
        }
#endif
    }
    #endregion

    #region SCENE Command

    public void ChangeScene(string sceneToChangeTo)
    {
        if (!DialogueDisplayer.Instance.isLoading)
        {
            if (sceneToChangeTo == "")
            {
                sceneToChangeTo = GameManager.Instance.gameSceneName;
            }

            SceneManager.LoadScene(sceneToChangeTo);
        }
    }
    #endregion

    #region CHECKPOINT Command
    public void SetCheckpoint()
    {
        dialogueCheckpoint = currentDialogueFile.name;
        SaveSystem.SaveDialogue(mainChatDialoguesToSave);
        SaveSystem.SaveDialogue(hackingChatDialoguesToSave);
        SaveSystem.SaveCheckpoint(GameManager.Instance.minigameProgressionList, this);
        SaveSystem.SaveMinigameProgression(GameManager.Instance.minigameProgressionList);
    }
    #endregion

    #region GAMEOVER Command
    public void GoToCheckpoint()
    {
        for (int i = mainChatDialoguesToSave.Count - 1; i >= 0; i--)
        {
            if (mainChatDialoguesToSave[i].fileName == dialogueCheckpoint)
            {
                dialogueFileToLoad = GetElementFileFromName(dialogueCheckpoint);
                displayer.SaveDialogueData(mainChatDialoguesToSave);
                displayer.currentDialogueElementId = 1;
                //Debug.LogError(displayer.currentDialogueElementId);

                displayer.isInitialisation = true;
                displayer.isFinished = false;
                displayer.isWaitingForReply = false;
                displayer.UpdateDialogueState();
                break;
            }
            else
            {
                mainChatDialoguesToSave.RemoveAt(i);
                displayer.SaveDialogueData(mainChatDialoguesToSave);
            }
        }
        GameManager.Instance.LoadChekpoint();
        displayer.SaveDialogueData(mainChatDialoguesToSave);
        GameManager.Instance.GoToChatScene();
    }

    public void AskForConfirmation()
    {
        confirmationMessage.ChangeAnim();
    }
    #endregion
}