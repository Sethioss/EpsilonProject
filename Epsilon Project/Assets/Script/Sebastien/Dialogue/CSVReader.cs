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

    #region Dialogue processing
    public Dialogue CreateDialogueFromData(TextAsset data)
    {
        return CreateDialogue(data);
    }
    private Dialogue CreateDialogue(TextAsset data)
    {
        //Split table in rows
        string[] rows = data.text.Split(new char[] { '\n' });
        Dialogue dialogue = new Dialogue();
        UnityAction tempDialogueEvents = null;
        DialogueElement dialogueElement;

        //PROCESSING DIALOGUE
        //Iterate through rows (jump = Length of a dialogue)
        int jump = 1;

        for (int i = 1; i < rows.Length - 1; i += jump)
        {
            string[] currentRow = rows[i].Split(new char[] { ';' });

            if (i == 1)
            {
                tempDialogueEvents = SetEvents(currentRow[7]);
            }

            if (currentRow[0] != "")
            {
                //PROCESSING DIALOGUE ELEMENT
                //This is the start of a new dialogue

                int dialogueStart = i;
                int index = dialogueStart + 1;

                //PROCESSING FIRST REPLY AND ELEMENT EVENT
                UnityAction replyEvents = SetEvents(currentRow[5]);
                UnityAction elementEvents = SetEvents(currentRow[6]);

                for (int h = 0; h < 3; h++)
                {
                    currentRow[h] = ProcessMessage(currentRow[h]);
                }

                Reply reply = new Reply(currentRow[1], currentRow[2], dialogue.elements.Count, GetTime(currentRow[4]), replyEvents);

                dialogueElement = new DialogueElement(currentRow[0], reply, dialogue.elements.Count, GetTime(currentRow[3]), elementEvents);

                //Iterate through the rows in a nested loop until it finds a new dialogue element
                for (int j = index; j < rows.Length - 1; j++)
                {
                    currentRow = rows[j].Split(new char[] { ';' });

                    if (currentRow[0] == "")
                    {
                        //We're either still in a dialogue, or it's an empty line

                        if (currentRow[1] != "")
                        {
                            //This is a new element
                            //PROCESSING DIALOGUE ELEMENT
                            for (int h = 1; h < 3; h++)
                            {
                                //Check for keywords
                                currentRow[h] = ProcessMessage(currentRow[h]);
                            }

                            //PROCESSING REPLY
                            replyEvents = SetEvents(currentRow[5]);
                            reply = new Reply(currentRow[1], currentRow[2], dialogueElement.replies.Count, GetTime(currentRow[4]), replyEvents);
                            dialogueElement.AddReply(reply);
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
    #endregion

    #region Keyword processing
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
                    return DialogueManager.Instance.username;

                case "DMODEL":
                    return SystemInfo.deviceModel;

                /*case "LAT":
                    return GPS.Instance.latitude.ToString();

                case "LONG":
                    return GPS.Instance.longitude.ToString();*/

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
    #endregion

    #region Event/command processing
    public UnityAction SetEvents(string cellContent, bool playRightAway = false)
    {
        UnityAction events = null;

        string[] tagArea = cellContent.Split(new char[] { '<', '>', ' ' });
        string colorCodeFirst = "<color=blue>";
        string colorCodeLast = "</color>";

        int jump = 1;

        for (int i = 0; i < tagArea.Length; i += jump)
        {
            switch (tagArea[i])
            {
                case "BRANCH":
                    string fileName = tagArea[i + 1].ToString();

                    events += delegate { DialogueManager.Instance.CreateAndStartDialogue(fileName); };
                    jump = 2;

                    //The event takes place right as it is read, used for check functions
                    if (playRightAway)
                    {
                        events.Invoke();
                    }
#if UNITY_EDITOR
                    if (!playRightAway && DialogueManager.Instance.debugReadCommandKeywords)
                    {
                        //Debug
                        List<string> debugMessages = new List<string>();
                        debugMessages.Add("=======DISCOVERED A NEW EVENT=======");
                        colorCodeFirst = "<color=yellow>";
                        debugMessages.Add(colorCodeFirst + "BRANCH :: Branch keyword detected" + colorCodeLast);
                        debugMessages.Add(colorCodeFirst + "BRANCH :: Going to branch : " + fileName + colorCodeLast);

                        DialogueManager.Instance.DebugElement(debugMessages.ToArray());
                    }
#endif
                    break;

                case "SET":

                    string parameterVariable = tagArea[i + 1];
                    string valueToSetTo = tagArea[i + 2];
                    char firstLetterOfValue = valueToSetTo[0];

                    string finalNewValue = "";

                    int newValueStartId = i + 2;
                    int newValueEndId = newValueStartId;

                    //Debug.Log("Event: Set " + parameterVariable + " to " + newValue);

                    //Has an operator before it
                    if (firstLetterOfValue == '+' || firstLetterOfValue == '-' || firstLetterOfValue == '*')
                    {
                        //Starts and ends with "'" (ChangedVariable is a string)
                        if (valueToSetTo[1] == '\'')
                        {
                            string tempString = tagArea[newValueEndId];
                            while (tempString[tempString.Length - 1] != '\'')
                            {
                                newValueEndId++;
                                tempString = tagArea[newValueEndId];
                            }

                            for (int j = newValueStartId; j <= newValueEndId; j++)
                            {
                                if (j == newValueStartId)
                                {
                                    finalNewValue += tagArea[j];
                                }
                                else
                                {
                                    finalNewValue += " " + tagArea[j];
                                }
                            }

                            events += delegate { DialogueManager.Instance.SetStringVariable(parameterVariable, finalNewValue); };
                        }
                        //Doesn't contain "'" (ChangedVariable is a float)
                        else
                        {
                            newValueEndId = newValueStartId;
                            finalNewValue = valueToSetTo;
                            events += delegate { DialogueManager.Instance.SetFloatVariable(parameterVariable, finalNewValue); };
                        }
                    }
                    //Has no operator before it
                    else
                    {
                        //Starts with "'" (ChangedVariable is a string)
                        if (firstLetterOfValue == '\'')
                        {
                            string tempString = tagArea[newValueEndId];
                            while (tempString[tempString.Length - 1] != '\'')
                            {
                                tempString = tagArea[newValueEndId];
                                newValueEndId++;
                                tempString = tagArea[newValueEndId];
                            }

                            for (int j = newValueStartId; j <= newValueEndId; j++)
                            {
                                if (j == newValueStartId)
                                {
                                    finalNewValue += tagArea[j];
                                }
                                else
                                {
                                    finalNewValue += " " + tagArea[j];
                                }
                            }

                            events += delegate { DialogueManager.Instance.SetStringVariable(parameterVariable, finalNewValue); };
                        }
                        //Doesn't contain "'" (ChangedVariable is a float)
                        else
                        {
                            newValueEndId = newValueStartId;
                            finalNewValue = valueToSetTo;
                            events += delegate { DialogueManager.Instance.SetFloatVariable(parameterVariable, finalNewValue); };
                        }
                    }
#if UNITY_EDITOR
                    if (!playRightAway && DialogueManager.Instance.debugReadCommandKeywords)
                    {
                        //Debug
                        List<string> debugMessages = new List<string>();
                        debugMessages.Add("=======DISCOVERED A NEW EVENT=======");
                        colorCodeFirst = "<color=red>";
                        debugMessages.Add(colorCodeFirst + "SET :: Set keyword detected" + colorCodeLast);
                        debugMessages.Add(colorCodeFirst + "SET :: New Value Start : " + tagArea[newValueStartId] + " at " + newValueStartId + colorCodeLast);
                        debugMessages.Add(colorCodeFirst + "SET :: New Value End : " + tagArea[newValueEndId] + " at " + newValueEndId + colorCodeLast);
                        debugMessages.Add(colorCodeFirst + "SET :: Value to set to : " + finalNewValue + colorCodeLast);
                        DialogueManager.Instance.DebugElement(debugMessages.ToArray());
                    }
#endif
                    jump = (newValueEndId - i) - 1;

                    //The event takes place right as it is read, used for check functions
                    if (playRightAway)
                    {
                        events.Invoke();
                        events = null;
                    }

                    break;

                case "SCENE":

                    string sceneToChangeTo = "";
                    string inviteMessage = "";
                    sceneToChangeTo = tagArea[i + 1];

                    //Find if there's a message to go with the link
                    try
                    {
                        //Get invite message
                        inviteMessage = tagArea[i + 2];
                        int messageStartId = i + 2;
                        char firstLetterOfMessage = inviteMessage[0];
                        int messageEndId = messageStartId;

                        if (firstLetterOfMessage == '\'')
                        {
                            string tempMessage = tagArea[messageStartId];
                            while (tempMessage[tempMessage.Length - 1] != '\'')
                            {
                                tempMessage = tagArea[messageEndId];
                                messageEndId++;
                                tempMessage = tagArea[messageEndId];
                            }

                            tempMessage = "";

                            for (int j = messageStartId; j <= messageEndId; j++)
                            {
                                if (j == messageStartId)
                                {
                                    tempMessage += tagArea[j];
                                }
                                else
                                {
                                    tempMessage += " " + tagArea[j];
                                }
                            }
                            inviteMessage = tempMessage.Trim('\'');

                        }
                    }
                    catch
                    {
                        Debug.LogWarning("No message has been set in the scene keyword. The invite message will just be the link");
                    }

                    events += delegate { DialogueManager.Instance.InviteToMinigame(sceneToChangeTo, inviteMessage); };
                    jump = 2;

                    //The event takes place right as it is read, used for check functions
                    if (playRightAway)
                    {
                        Debug.Log(jump);
                        events.Invoke();
                    }
#if UNITY_EDITOR
                    if (!playRightAway && DialogueManager.Instance.debugReadCommandKeywords)
                    {
                        //Debug
                        List<string> debugMessages = new List<string>();
                        debugMessages.Add("=======DISCOVERED A NEW EVENT=======");
                        colorCodeFirst = "<color=blue>";
                        debugMessages.Add(colorCodeFirst + "SCENE :: Scene keyword detected " + colorCodeLast);
                        debugMessages.Add(colorCodeFirst + "SCENE :: Going to scene : " + sceneToChangeTo + colorCodeLast);
                        DialogueManager.Instance.DebugElement(debugMessages.ToArray());
                    }
#endif
                    break;

                case "CHECK":

                    string valueToCheck = tagArea[i + 1];
                    string op = tagArea[i + 2];
                    string valueToCheckWith = tagArea[i + 3];
                    int valueToCheckWithStartID = i + 3;
                    int valueToCheckWithEndID = valueToCheckWithStartID;

                    string finalValueToCheckWith = "";
                    string firstCommand = "";
                    string secondCommand = "";


                    //---GET FIRST COMMAND CONTENT---//
                    //First command start
                    int firstCommandStartID = i;
                    while (tagArea[firstCommandStartID] != "?")
                    {
                        firstCommandStartID++;
                    }

                    //First command end
                    int firstCommandEndID = firstCommandStartID;
                    while (tagArea[firstCommandEndID] != ":")
                    {
                        firstCommandEndID++;
                    }

                    //First command content
                    for (int j = firstCommandStartID; j < firstCommandEndID; j++)
                    {
                        firstCommand += " " + tagArea[j];
                    }

                    //---GET SECOND COMMAND CONTENT---//
                    int secondCommandStartID = firstCommandEndID + 2;

                    //Second command end
                    int secondCommandEndID = secondCommandStartID;
                    while (tagArea[secondCommandEndID] != "!")
                    {
                        secondCommandEndID++;
                    }

                    //Second command content
                    for (int j = secondCommandStartID; j < secondCommandEndID; j++)
                    {
                        secondCommand += " " + tagArea[j];
                    }

                    //Il y'a des caractères ' au début et à la fin, on compare une string
                    if (valueToCheckWith[0] == '\'')
                    {
                        char lastElementChar = tagArea[valueToCheckWithEndID][tagArea[valueToCheckWithEndID].Length - 1];
                        while (lastElementChar != '\'')
                        {
                            valueToCheckWithEndID++;
                            lastElementChar = tagArea[valueToCheckWithEndID][tagArea[valueToCheckWithEndID].Length - 1];
                        }

                        for (int j = valueToCheckWithStartID; j <= valueToCheckWithEndID; j++)
                        {
                            if (j == valueToCheckWithStartID)
                            {
                                finalValueToCheckWith += tagArea[j];
                            }
                            else
                            {
                                finalValueToCheckWith += " " + tagArea[j];
                            }
                        }

                        jump = secondCommandEndID - i;

                        //CompareStringVariables contains the logic that plays the functions inside the commands with a recursive call
                        events += delegate { DialogueManager.Instance.CompareStringVariables(valueToCheck, finalValueToCheckWith, firstCommand, secondCommand, op); };
                    }
                    else
                    {
                        events += delegate { DialogueManager.Instance.CompareFloatVariables(valueToCheck, finalValueToCheckWith, firstCommand, secondCommand, op); };
                    }

                    //The event takes place right as it is read, used for check functions
                    if (playRightAway)
                    {
                        events.Invoke();
                    }

#if UNITY_EDITOR
                    if (!playRightAway && DialogueManager.Instance.debugReadCommandKeywords)
                    {
                        //Debug

                        List<string> debugMessages = new List<string>();
                        debugMessages.Add("=======DISCOVERED A NEW EVENT=======");
                        colorCodeFirst = "<color=green>";
                        debugMessages.Add(colorCodeFirst + "CHECK :: Check keyword detected" + colorCodeLast);
                        debugMessages.Add("\n");
                        debugMessages.Add(colorCodeFirst + "CHECK :: First command start : " + tagArea[firstCommandStartID] + " at " + firstCommandStartID + colorCodeLast);
                        debugMessages.Add(colorCodeFirst + "CHECK :: First command end ID : " + tagArea[firstCommandEndID] + " at " + firstCommandEndID + colorCodeLast);
                        debugMessages.Add(colorCodeFirst + "CHECK :: IfTrue command : " + firstCommand + colorCodeLast);
                        debugMessages.Add("\n");
                        debugMessages.Add(colorCodeFirst + "CHECK :: Second command start : " + tagArea[secondCommandStartID] + " at " + secondCommandStartID + colorCodeLast);
                        debugMessages.Add(colorCodeFirst + "CHECK :: Second command end : " + tagArea[secondCommandEndID] + " at " + secondCommandEndID + colorCodeLast);
                        debugMessages.Add(colorCodeFirst + "CHECK :: Second command : " + secondCommand + colorCodeLast);
                        debugMessages.Add("\n");
                        debugMessages.Add(colorCodeFirst + "CHECK :: String to check with : " + finalValueToCheckWith + colorCodeLast);
                        DialogueManager.Instance.DebugElement(debugMessages.ToArray());
                    }
#endif
                    break;

                default:
                    jump = 1;
                    break;
            }
        }

        return events;
    }
    #endregion
    private float GetFloat(string stringValue, float defaultValue)
    {
        float result = defaultValue;
        float.TryParse(stringValue, out result);
        return result;
    }

    private string GetTime(string cellContent)
    {
        if (cellContent.Split(new char[] { ':' }, System.StringSplitOptions.RemoveEmptyEntries).Length > 2)
        {
            return cellContent;
        }
        //The time isn't readable
        return "00:00:00:15";
    }
}
