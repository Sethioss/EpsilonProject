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

    private Dialogue tempDialogue;
    private List<DialogueElement> specialMessageElementBuffer;

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

        tempDialogue = new Dialogue();
        specialMessageElementBuffer = new List<DialogueElement>();

        tempDialogue.fileName = data.name;
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
                int jumpIndex = dialogueStart + 1;

                //PROCESSING FIRST REPLY AND ELEMENT EVENT
                UnityAction replyEvents = SetEvents(currentRow[5]);
                UnityAction elementEvents = SetEvents(currentRow[6]);

                for (int h = 0; h < 3; h++)
                {
                    currentRow[h] = ProcessMessage(currentRow[h]);
                }

                Reply reply = new Reply(currentRow[1], currentRow[2], 0, GetTime(currentRow[4]), replyEvents);

                dialogueElement = new DialogueElement(currentRow[0], reply, tempDialogue.elements.Count, GetTime(currentRow[3]), elementEvents);

                //Iterate through the rows in a nested loop until it finds a new dialogue element
                for (int j = jumpIndex; j < rows.Length - 1; j++)
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
                        jumpIndex++;
                    }
                    else
                    {
                        break;
                    }
                }

                jump = jumpIndex - dialogueStart;
                dialogueElement.messageType = DialogueElement.MessageType.NORMAL;
                tempDialogue.AddDialogueElement(dialogueElement);
                foreach (DialogueElement element in specialMessageElementBuffer)
                {
                    element.index = tempDialogue.elements.Count;
                    tempDialogue.AddDialogueElement(element);
                }
                specialMessageElementBuffer = new List<DialogueElement>();
            }
        }
        tempDialogue.endDialogueAction = tempDialogueEvents;
        return tempDialogue;
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

                #region USERNAME
                case "USERNAME":
                    return DialogueManager.Instance.username;
                #endregion

                #region DMODEL
                case "DMODEL":
                    return SystemInfo.deviceModel;
                #endregion

                #region GPS Infos
                /*case "LAT":
                    return GPS.Instance.latitude.ToString();

                case "LONG":
                    return GPS.Instance.longitude.ToString();*/
                #endregion

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
                #region BRANCH
                case "BRANCH":
                    string fileName = tagArea[i + 1].ToString();

                    events += delegate { DialogueManager.Instance.Branch(fileName); };
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
                #endregion

                #region SET
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
                #endregion

                #region LINK
                case "LINK":

                    string sceneToChangeTo = "";
                    string parameter = "";
                    sceneToChangeTo = tagArea[i + 1];

                    //Find if there's a message to go with the link
                    try
                    {
                        //Get invite message
                        parameter = tagArea[i + 2];
                        int messageStartId = i + 2;
                        char firstLetterOfMessage = parameter[0];
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
                            parameter = tempMessage.Trim('\'');

                        }
                    }
                    catch
                    {

                    }

                    events += delegate { DialogueManager.Instance.SpecialMessage(sceneToChangeTo, (int)DialogueElement.MessageType.LINK); };

                    if (playRightAway)
                    {
                        events.Invoke();
                    }
                    else
                    {
                        //Creation d'un nouvel élément
                        CreateLinkElement(specialMessageElementBuffer, sceneToChangeTo, parameter);
                    }

                    jump = 3;
#if UNITY_EDITOR
                    if (!playRightAway && DialogueManager.Instance.debugReadCommandKeywords)
                    {
                        //Debug
                        List<string> debugMessages = new List<string>();
                        debugMessages.Add("=======DISCOVERED A NEW EVENT=======");
                        colorCodeFirst = "<color=blue>";
                        debugMessages.Add(colorCodeFirst + "LINK :: Link keyword detected " + colorCodeLast);
                        debugMessages.Add(colorCodeFirst + "LINK :: Link leads to scene : " + sceneToChangeTo + colorCodeLast);
                        DialogueManager.Instance.DebugElement(debugMessages.ToArray());
                    }
#endif
                    break;
                #endregion

                #region LEAVE
                case "LEAVE":

                    jump = 1;

                    string nextDialogueToLaunch = "";

                    try
                    {
                        parameter = tagArea[i + 1];
                        int parameterNumber = i + 1;
                        char firstLetterOfMessage = parameter[0];
                        int messageEndId = parameterNumber;

                        if (firstLetterOfMessage == '\'')
                        {
                            string tempMessage = tagArea[parameterNumber];
                            while (tempMessage[tempMessage.Length - 1] != '\'')
                            {
                                tempMessage = tagArea[messageEndId];
                                messageEndId++;
                                tempMessage = tagArea[messageEndId];
                            }

                            tempMessage = "";

                            for (int j = parameterNumber; j <= messageEndId; j++)
                            {
                                if (j == parameterNumber)
                                {
                                    tempMessage += tagArea[j];
                                }
                                else
                                {
                                    tempMessage += " " + tagArea[j];
                                }
                            }

                            parameter = tempMessage.Trim('\'');
                        }
                        else
                        {
                            parameter = "";
                        }
                        nextDialogueToLaunch = parameter;
                        jump = 2;
                    }
                    catch
                    {
                        nextDialogueToLaunch = "";
                    }

                    events += delegate { DialogueManager.Instance.SpecialMessage(null, (int)DialogueElement.MessageType.LEAVE); };

                    if (playRightAway)
                    {
                        events.Invoke();
                    }
                    else
                    {
                        CreateLeaveElement(specialMessageElementBuffer, nextDialogueToLaunch);
                    }

                    break;
                #endregion

                #region INFO
                case "INFO":

                    string infoString = "";
                    //Find if there's a message to go with the link
                    try
                    {
                        //Get invite message
                        infoString = tagArea[i + 1];
                        int messageStartId = i + 1;
                        char firstLetterOfMessage = infoString[0];
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
                            infoString = tempMessage.Trim('\'');

                        }
                    }
                    catch
                    {

                    }

                    events += delegate { DialogueManager.Instance.SpecialMessage(null, (int)DialogueElement.MessageType.INFO); };

                    if (playRightAway)
                    {
                        events.Invoke();
                    }
                    else
                    {
                        //Creation d'un nouvel élément

                        CreateInfoElement(specialMessageElementBuffer, infoString);
                    }

                    jump = 3;

                    break;

                #endregion

                #region SCENE
                case "SCENE":

                    string sceneChange = "";
                    sceneChange = tagArea[i + 1];

                    events += delegate { DialogueManager.Instance.ChangeScene(sceneChange); };
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
                        debugMessages.Add(colorCodeFirst + "SCENE :: Scene leads to scene : " + sceneChange + colorCodeLast);
                        DialogueManager.Instance.DebugElement(debugMessages.ToArray());
                    }

#endif
                    break;
                #endregion

                #region CHECK
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
                    firstCommandStartID++;

                    //First command end
                    int firstCommandEndID = firstCommandStartID + 1;
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
                        events += delegate { DialogueManager.Instance.CompareStringVariables(valueToCheck, finalValueToCheckWith, firstCommand, secondCommand, op); };
                    }
                    else
                    {
                        finalValueToCheckWith = valueToCheckWith;
                        events += delegate { DialogueManager.Instance.CompareFloatVariables(valueToCheck, finalValueToCheckWith, firstCommand, secondCommand, op); };
                    }

                    try
                    {
                        string[] brokenDownFirstCommand = firstCommand.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                        string[] brokenDownSecondCommand = secondCommand.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                        if (brokenDownFirstCommand[0] == "LINK")
                        {
                            CreateLinkElement(specialMessageElementBuffer, brokenDownFirstCommand[1], brokenDownFirstCommand[2].Trim('\''));
                        }
                        else if (brokenDownFirstCommand[0] == "LEAVE")
                        {
                            CreateLeaveElement(specialMessageElementBuffer, brokenDownFirstCommand[1].Trim('\''));
                        }

                        if (brokenDownSecondCommand[0] == "LINK")
                        {
                            CreateLinkElement(specialMessageElementBuffer, brokenDownSecondCommand[1], brokenDownSecondCommand[2].Trim('\''));
                        }
                        else if (brokenDownSecondCommand[0] == "LEAVE")
                        {
                            CreateLeaveElement(specialMessageElementBuffer, brokenDownSecondCommand[1].Trim('\''));
                        }
                    }
                    catch
                    {
                        return events;
                    }

                    jump = secondCommandEndID - i;

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
                        debugMessages.Add(colorCodeFirst + "CHECK :: First command : " + firstCommand + colorCodeLast);
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
                #endregion

                #region END
                case "END":
                    jump = 1;

                    events += delegate { DialogueManager.Instance.SpecialMessage(null, (int)DialogueElement.MessageType.END); };

                    if (playRightAway)
                    {
                        events.Invoke();
                    }
                    else
                    {
                        //Creation d'un nouvel élément

                        CreateEndElement(specialMessageElementBuffer, XMLManager.Instance.GetTagValue("endMessage"));
                    }

                    break;
                #endregion

                #region CHECKPOINT
                case "CHECKPOINT":
                    jump = 1;
                    events += delegate { DialogueManager.Instance.SetCheckpoint(); };

                    break;
                #endregion

                #region GAMEOVER
                case "GAMEOVER":
                    jump = 1;

                    string gameOverString = "";
                    //Find if there's a message to go with the link
                    try
                    {
                        //Get invite message
                        gameOverString = tagArea[i + 1];
                        int messageStartId = i + 1;
                        char firstLetterOfMessage = gameOverString[0];
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
                            gameOverString = tempMessage.Trim('\'');

                        }
                    }
                    catch
                    {

                    }

                    events += delegate { DialogueManager.Instance.SpecialMessage(null, (int)DialogueElement.MessageType.GAMEOVER); };

                    if (playRightAway)
                    {
                        events.Invoke();
                    }
                    else
                    {
                        //Creation d'un nouvel élément

                        CreateGameOverElement(specialMessageElementBuffer, gameOverString);
                    }

                    break;
                #endregion

                default:
                    jump = 1;
                    break;
            }
        }

        return events;
    }
    #endregion

    #region Special Element Creation
    private void CreateGameOverElement(List<DialogueElement> elementBuffer, string gameOverString)
    {
        DialogueElement newElement = new DialogueElement(gameOverString, tempDialogue.elements.Count, "00:00:00:01", null);

        //Info element
        newElement.messageType = DialogueElement.MessageType.INFO;
        elementBuffer.Add(newElement);

        //Game Over element
        newElement = new DialogueElement("GAME OVER", tempDialogue.elements.Count, "00:00:00:01", null);
        newElement.messageType = DialogueElement.MessageType.GAMEOVER;

        UnityAction leaveActions = null;
        leaveActions += delegate { DialogueManager.Instance.GoToCheckpoint(); };

        //Checkpoint reload reply
        Reply leaveReply = new Reply(XMLManager.Instance.GetTagValue("gameOverReply1"), null, 0, "00:00:00:01", leaveActions);
        newElement.AddReply(leaveReply);

        //Back to menu reply
        leaveActions = null;
        leaveActions += delegate { DialogueManager.Instance.AskForConfirmation(); };

        //Checkpoint reload reply
        leaveReply = new Reply(XMLManager.Instance.GetTagValue("gameOverReply2"), null, 1, "00:00:00:01", leaveActions);
        newElement.AddReply(leaveReply);

        elementBuffer.Add(newElement);
    }
    private void CreateEndElement(List<DialogueElement> elementBuffer, string infoString)
    {
        DialogueElement newElement = new DialogueElement(infoString, tempDialogue.elements.Count, "00:00:00:01", null);

        newElement.messageType = DialogueElement.MessageType.END;

        UnityAction leaveActions = null;
        leaveActions += delegate { DialogueManager.Instance.AskForConfirmation(); };

        Reply leaveReply = new Reply(XMLManager.Instance.GetTagValue("endReply"), null, 0, "00:00:00:05", leaveActions);
        newElement.AddReply(leaveReply);
        newElement.messageType = DialogueElement.MessageType.END;

        elementBuffer.Add(newElement);
    }
    private void CreateInfoElement(List<DialogueElement> elementBuffer, string infoString)
    {
        DialogueElement newElement = new DialogueElement(infoString, tempDialogue.elements.Count, "00:00:00:01", null);

        newElement.messageType = DialogueElement.MessageType.INFO;

        elementBuffer.Add(newElement);
    }
    private void CreateLeaveElement(List<DialogueElement> elementBuffer, string branchToGoTo, string leaveMessage = null)
    {

        DialogueElement newElement = new DialogueElement(XMLManager.Instance.GetTagValue("leaveMessage"), tempDialogue.elements.Count, "00:00:00:05", null);

        UnityAction leaveActions = null;

        if (branchToGoTo != "")
        {
            leaveActions += delegate { GameManager.Instance.SetDialogueAndGoToChatScene(UserSettings.Instance.languagePrefix + "-" + branchToGoTo); };
        }
        else
        {
            Debug.LogError("LEAVE :: No dialogue to go to has been set. If this is not intended, please verify the dialogue file");
        }


        Reply leaveReply = new Reply(XMLManager.Instance.GetTagValue("leaveReply1"), null, 0, "00:00:00:05", leaveActions);
        newElement.AddReply(leaveReply);
        newElement.messageType = DialogueElement.MessageType.LEAVE;

        elementBuffer.Add(newElement);
    }
    private void CreateLinkElement(List<DialogueElement> elementBuffer, string sceneToChangeTo, string inviteMessage)
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
        changeSceneAction += delegate { DialogueManager.Instance.ChangeScene(sceneToChangeTo); };

        DialogueElement newElement = new DialogueElement(message, tempDialogue.elements.Count,
            "00:00:00:07", changeSceneAction);

        newElement.messageType = DialogueElement.MessageType.LINK;

        foreach (MinigameProgressionUnit unit in GameManager.Instance.minigameProgressionList)
        {
            if (unit.stringID == sceneToChangeTo)
            {
                //Debug.LogWarning("There's a match!");
                if (unit.minigameFinished)
                {
                    newElement.minigameLinkFinished = true;
                }
                else
                {
                    newElement.minigameLinkFinished = false;
                }
                break;
            }
        }

        elementBuffer.Add(newElement);
    }

    #endregion

    #region Misc
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
    private string GetTime(string cellContent)
    {
        if (cellContent.Split(new char[] { ':' }, System.StringSplitOptions.RemoveEmptyEntries).Length > 2)
        {
            return cellContent;
        }
        //The time isn't readable
        return "00:00:00:07";
    }
    #endregion
}
