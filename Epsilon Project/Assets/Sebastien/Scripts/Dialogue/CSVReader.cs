using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CSVReader : MonoBehaviour
{
    private static CSVReader instance;
    public static CSVReader Instance
    {
        get
        {
            return instance;
        }
    }

    public string userName = "Sebilol";

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

    public Dialogue CreateDialogueFromData(TextAsset data)
    {
        return CreateDialogue(data);
    }
    private float GetFloat(string stringValue, float defaultValue)
    {
        float result = defaultValue;
        float.TryParse(stringValue, out result);
        return result;
    }

    private UnityAction SetEvents(string cellColumn)
    {
        UnityAction events = null;

        string[] tagArea = cellColumn.Split(new char[] { '<', '>', ' ' });
        int jump = 1;

        for (int i = 0; i < tagArea.Length; i += jump)
        {
            /*if (tagArea[i] == "BRANCH")
            {
                Debug.Log("Branch keyword detected");
                Debug.Log("Event: Lead to branch " + tagArea[i + 1]);
                string fileName = tagArea[i + 1][0].ToString();

                events += delegate { DialogueManager.Instance.CreateAndStartDialogue(fileName); };
                jump = 2;
            }
            else if (tagArea[i] == "SET")
            {
                string variable = tagArea[i + 1];
                string newValue = tagArea[i + 2];

                Debug.Log("Set keyword detected");
                Debug.Log("Event: Set " + variable + " to " + newValue);

                //Has an operator before it
                if(newValue[0] == '+' || newValue[0] == '-' || newValue[0] == '*')
                {
                    Debug.Log(newValue);
                    //Starts and ends with "'" (ChangedVariable is a string)
                    if (newValue[1] == '\'' && newValue[newValue.Length - 1] == '\'')
                    {
                        events += delegate { DialogueManager.Instance.SetStringVariable(variable, newValue); };
                    }
                    //Doesn't contain "'" (ChangedVariable is a float)
                    else
                    {
                        events += delegate { DialogueManager.Instance.SetFloatVariable(variable, newValue); };
                    }
                }
                //Has no operator before it
                else
                {
                    //Starts and ends with "'" (ChangedVariable is a string)
                    if (newValue[0] == '\'' && newValue[newValue.Length - 1] == '\'')
                    {
                        events += delegate { DialogueManager.Instance.SetStringVariable(variable, newValue); };
                    }
                    //Doesn't contain "'" (ChangedVariable is a float)
                    else
                    {
                        events += delegate { DialogueManager.Instance.SetFloatVariable(variable, newValue); };
                    }
                }
                
                jump = 3;
            }

        else if(tagArea[i] == "SCENE")
        {
            string variable = tagArea[i + 1];
            events += delegate { DialogueManager.Instance.ChangeScene(variable); };
        }

        else if(tagArea[i] == "CHECK")
        {
            string variable = tagArea[i + 1];
        }*/

            string tempStrVar = "";

            switch (tagArea[i])
            {
                case "BRANCH":
                    Debug.Log("Branch keyword detected");
                    Debug.Log("Event: Lead to branch " + tagArea[i + 1]);
                    string fileName = tagArea[i + 1][0].ToString();

                    events += delegate { DialogueManager.Instance.CreateAndStartDialogue(fileName); };
                    jump = 2;
                    break;

                case "SET":
                    string parameterVariable = tagArea[i + 1];
                    string newValue = tagArea[i + 2];

                    Debug.Log("Set keyword detected");
                    Debug.Log("Event: Set " + parameterVariable + " to " + newValue);

                    //Has an operator before it
                    if (newValue[0] == '+' || newValue[0] == '-' || newValue[0] == '*')
                    {
                        Debug.Log(newValue);
                        //Starts and ends with "'" (ChangedVariable is a string)
                        if (newValue[1] == '\'' && newValue[newValue.Length - 1] == '\'')
                        {
                            events += delegate { DialogueManager.Instance.SetStringVariable(parameterVariable, newValue); };
                        }
                        //Doesn't contain "'" (ChangedVariable is a float)
                        else
                        {
                            events += delegate { DialogueManager.Instance.SetFloatVariable(parameterVariable, newValue); };
                        }
                    }
                    //Has no operator before it
                    else
                    {
                        //Starts and ends with "'" (ChangedVariable is a string)
                        if (newValue[0] == '\'' && newValue[newValue.Length - 1] == '\'')
                        {
                            events += delegate { DialogueManager.Instance.SetStringVariable(parameterVariable, newValue); };
                        }
                        //Doesn't contain "'" (ChangedVariable is a float)
                        else
                        {
                            events += delegate { DialogueManager.Instance.SetFloatVariable(parameterVariable, newValue); };
                        }
                    }
                    jump = 3;
                    break;

                case "SCENE":
                    tempStrVar = tagArea[i + 1];
                    events += delegate { DialogueManager.Instance.ChangeScene(tempStrVar); };
                    break;

                case "CHECK":
                    tempStrVar = tagArea[i + 1];
                    break;

                default:
                    break;
            }
        }

        return events;
    }

    private string ProcessMessage(string message)
    {
        string[] splitMessage = message.Split(new char[] { '<', '>' });
        string finalMessage = "";

        if (splitMessage.Length > 1)
        {
            for (int i = 0; i < splitMessage.Length; i++)
            {
                if ((i & 1) == 0)
                {
                    finalMessage += splitMessage[i];
                }
                else
                {
                    finalMessage += ReplaceWithTagValue(splitMessage[i]);
                }
            }
            return finalMessage;
        }

        return message;
    }

    private string ReplaceWithTagValue(string originalString)
    {
        if (originalString[0] == '\\')
        {
            string stringToReturn = "<" + originalString.Substring(1, originalString.Length - 1) + ">";
            return stringToReturn;
        }
        else
        {
            switch (originalString)
            {
                case "\\":
                    string stringToReturn = "<" + originalString.Substring(1, originalString.Length - 1) + ">";
                    return stringToReturn;

                case "USERNAME":
                    return userName;

                case "DMODEL":
                    return SystemInfo.deviceModel;
                default:
                    return "<" + originalString + ">";
            }
        }

        /*else if (originalString == "USERNAME")
        {
            return userName;
        }
        else if (originalString == "DMODEL")
        {
            return SystemInfo.deviceModel;
        }
        else
        {
            return "<" + originalString + ">";
        }*/
    }

    private Dialogue CreateDialogue(TextAsset data)
    {
        string[] rows = data.text.Split(new char[] { '\n' });
        Dialogue dialogue = new Dialogue();
        UnityAction tempDialogueEvents = null;
        DialogueElement dialogueElement;

        int jump = 1;

        for (int i = 1; i < rows.Length - 1; i += jump)
        {
            string[] row = rows[i].Split(new char[] { ';' });

            if (i == 1)
            {
                tempDialogueEvents = SetEvents(row[7]);
            }

            if (row[0] != "")
            {
                int dialogueStart = i;
                int index = dialogueStart + 1;

                UnityAction replyEvents = SetEvents(row[5]);
                UnityAction elementEvents = SetEvents(row[6]);

                for (int h = 0; h < 3; h++)
                {
                    row[h] = ProcessMessage(row[h]);
                }

                Reply reply = new Reply(row[1], row[2], dialogue.elements.Count, GetFloat(row[4], row[4].Length / 5), replyEvents);

                dialogueElement = new DialogueElement(row[0], reply, dialogue.elements.Count, GetFloat(row[3], 0), elementEvents);

                for (int j = index; j < rows.Length - 1; j++)
                {
                    row = rows[j].Split(new char[] { ';' });

                    if (row[0] == "")
                    {
                        if (row[1] != "")
                        {
                            for (int h = 0; h < 3; h++)
                            {
                                row[h] = ProcessMessage(row[h]);
                            }

                            replyEvents = SetEvents(row[5]);

                            Reply additionalReply = new Reply(row[1], row[2], dialogueElement.replies.Count, GetFloat(row[4], 2f), replyEvents);
                            dialogueElement.AddReply(additionalReply);
                        }

                        index++;
                    }
                    else
                    {
                        break;
                    }
                }

                jump = index - dialogueStart;

                dialogue.AddDialogueElement(dialogueElement);
            }
        }
        dialogue.endDialogueAction = tempDialogueEvents;
        return dialogue;
    }
}
