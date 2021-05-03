using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public TextAsset dialogueFile;
    public CSVReader reader;
    public DialogueHolder holder;
    public DialogueDisplayer displayer;

    public string testSet { get { return m_testSet; } set { m_testSet = value; } }
    [SerializeField]
    private string m_testSet;

    public float testSetFloat { get { return m_testSetFloat; } set { m_testSetFloat = value; } }
    [SerializeField]
    private float m_testSetFloat;

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

    // Start is called before the first frame update
    private void Start()
    {
        reader = CSVReader.Instance;
        holder = DialogueHolder.Instance;
        displayer = DialogueDisplayer.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CreateAndStartDialogue(dialogueFile.name);
        }
    }

    public void CreateAndStartDialogue(string dialogueFileName)
    {
        Dialogue dialogueToAdd = reader.CreateDialogueFromData((TextAsset)Resources.Load("Tables\\" + dialogueFileName.Trim()));
        holder.AddDialogue(dialogueToAdd);
        displayer.StartDialogue(dialogueToAdd);
        dialogueToAdd.id = holder.dialogueList.Count - 1;
    }

    public void CreateAndStartDialogue(TextAsset dialogueFile)
    {
        Dialogue dialogueToAdd = reader.CreateDialogueFromData(dialogueFile);
        holder.AddDialogue(dialogueToAdd);
        displayer.StartDialogue(dialogueToAdd);
        dialogueToAdd.id = holder.dialogueList.Count - 1;
    }

    public void SetStringVariable(string variable, string newValue)
    {
        System.Reflection.PropertyInfo prop = (DialogueManager.Instance.GetType().GetProperty(variable));

        if (newValue[0] == '+')
        {
            newValue = prop.GetValue(DialogueManager.Instance) + newValue.Trim(new char[] {'+', '\''});
        }
        else
        {
            newValue = newValue.Trim(new char[] { '+', '\'' });
        }

        prop.SetValue(DialogueManager.Instance, newValue);
    }

    public void SetFloatVariable(string variable, string newValue)
    {
        System.Reflection.PropertyInfo prop = (DialogueManager.Instance.GetType().GetProperty(variable));

        float newFloatValue = float.Parse(newValue.Trim(new char[] { '+', '-', '*', '\'' }));
        Debug.Log(newFloatValue);

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

        prop.SetValue(DialogueManager.Instance, propValue);
    }

    public void ChangeScene(string sceneToChangeTo)
    {
        string sceneName = sceneToChangeTo;
        SceneManager.LoadScene(sceneName.Trim());
    }
}
