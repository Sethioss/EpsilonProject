using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    #region Dialogue Manager Components
    private CSVReader reader;
    private DialogueDisplayer displayer;
    public TimeManager timeManager;

    [Header("Dialogue initialisation and list")]
    public TextAsset currentDialogueFile;
    public List<Dialogue> dialogueList;
    #endregion

    #region Debugging Variables
    [Header("Debugging tools")]
    [Tooltip("Sends debug messages for each command keyword found in the dialogue file")]
    public bool debugReadCommandKeywords = true;
    [Tooltip("Sends debug messages for each function that is played when its call is made")]
    public bool debugExecutingFunction = false;
    [Tooltip("Doesn't wait between dialogues")]
    public bool autoMode = false;

    private List<string> debugMessages { get; } = new List<string>();
    private string colorCodeStart = "";
    private string colorCodeEnd { get; } = "</color>";

    #endregion

    #region Game Variables

    public string testSet { get { return m_testSet; } set { m_testSet = value; } }
    public float testSetFloat { get { return m_testSetFloat; } set { m_testSetFloat = value; } }

    [Header("Accessible variables (Changed by events)")]
    [SerializeField]
    [Tooltip("Test for string variables")]
    private string m_testSet;
    [SerializeField]
    [Tooltip("Test for float variables")]
    private float m_testSetFloat;
    public string username = "Sebilol";

    #endregion

    private static DialogueManager instance;
    public static DialogueManager Instance
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
            DontDestroyOnLoad(instance);
        }

        if (this != instance)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        reader = CSVReader.Instance;
        displayer = DialogueDisplayer.Instance;
        timeManager = FindObjectOfType<TimeManager>();
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

    private void AddToDebugFunctionMessage(string messageToPass, List<string> listToAddTo)
    {
        if (debugExecutingFunction)
        {
            listToAddTo.Add(colorCodeStart + messageToPass + colorCodeEnd);
        }
    }
    #endregion

    #region BRANCH Command / Dialogue Initialisation
    //The text asset function executes for the starting file
    public void CreateAndStartDialogue(TextAsset dialogueFile)
    {
        reader = CSVReader.Instance;
        displayer = DialogueDisplayer.Instance;
        timeManager = FindObjectOfType<TimeManager>();

        currentDialogueFile = dialogueFile;
        Dialogue dialogueToAdd = reader.CreateDialogueFromData(dialogueFile);
        dialogueList.Add(dialogueToAdd);
        displayer.StartDialogue(dialogueToAdd);
        dialogueToAdd.id = dialogueList.Count - 1;
    }
    //The string function executes when a BRANCH command is called
    public void CreateAndStartDialogue(string dialogueFileName)
    {

#if UNITY_EDITOR
        colorCodeStart = "<color=yellow>";
        AddToDebugFunctionMessage("=======BRANCH FUNCTION EXECUTING=======", debugMessages);
        AddToDebugFunctionMessage(colorCodeStart + "Switching from branch " + currentDialogueFile.name + " to branch: " + dialogueFileName, debugMessages);

        if (debugExecutingFunction)
        {
            DebugElement(debugMessages.ToArray());
        }
#endif
        currentDialogueFile = (TextAsset)Resources.Load("Tables\\" + dialogueFileName);
        Dialogue dialogueToAdd = reader.CreateDialogueFromData((TextAsset)Resources.Load("Tables\\" + dialogueFileName));
        dialogueList.Add(dialogueToAdd);
        displayer.StartDialogue(dialogueToAdd);
        dialogueToAdd.id = dialogueList.Count - 1;
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

    #region SCENE Keyword
    public void ChangeScene(string sceneToChangeTo)
    {
        string sceneName = sceneToChangeTo;

#if UNITY_EDITOR
        colorCodeStart = "<color=blue>";
        AddToDebugFunctionMessage("=======SCENE FUNCTION EXECUTING=======", debugMessages);
        AddToDebugFunctionMessage(colorCodeStart + "Going to Scene " + sceneToChangeTo + colorCodeEnd, debugMessages);
        DebugElement(debugMessages.ToArray());
#endif
        SceneManager.LoadScene(sceneName.Trim());
    }
    #endregion    
}