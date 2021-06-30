using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public static DialogueData Instance { set; get; }
    public List<string> dialogueFileName;
    public List<int> isMinigameFinished;
    public List<int> chosenReplyId;
    public List<int> elementId;
    public List<int> numberOfElementsInDialogue;
    public int currentElement;

    public int initialisation = 0;
    public int waitForReply = 0;
    public int replied = 0;
    public int reacted = 0;
    public int isFinished = 0;

    public int wentBackHome = 0;
    public int wentToBridge = 0;

    public string checkpoint = "Intro-1";

    //public DialogueData() { }
    public DialogueData(List<Dialogue> dialogueToSave)
    {
        DialogueDisplayer displayer = DialogueManager.Instance.displayer;

        Instance = null;
        dialogueFileName = new List<string>();
        isMinigameFinished = new List<int>();
        chosenReplyId = new List<int>();
        elementId = new List<int>();
        numberOfElementsInDialogue = new List<int>();
        currentElement = displayer.currentDialogueElementId;
        checkpoint = DialogueManager.Instance.dialogueCheckpoint;

        //Debug.LogWarning("========= New save call ==========");
        //Fetch all dialogues
        for (int i = 0; i < dialogueToSave.Count; i++)
        {
            //Debug.LogWarning("i = " + i);
            int elementsInDialogue = 0;

            dialogueFileName.Add(dialogueToSave[i].fileName);
            //Debug.LogWarning("Dialogue file name : " + dialogueToSave[i].fileName);

            //Fetch all elements
            for (int j = 0; j < dialogueToSave[i].elements.Count; j++)
            {
                //Debug.LogWarning("Current element : " + j);
                //Debug.LogWarning("Current element message : " + dialogueToSave[i].elements[j].message);
                elementId.Add(dialogueToSave[i].elements[j].index);
                elementsInDialogue++;

                DialogueElement element = dialogueToSave[i].elements[j];

                //Debug.LogWarning("Chosen reply index : " + element.chosenReplyIndex);
                if (element.replies.Count > 0)
                {
                    //Debug.LogWarning("Chosen reply message : " + element.replies[0].replyText);
                    if (element.replies[0].reaction != "")
                    {
                        //Debug.LogWarning("Chosen reply reaction : " + element.replies[0].reaction);
                    }
                }

                chosenReplyId.Add(element.chosenReplyIndex);

                int toAdd = 0;
                if (element.minigameLinkFinished)
                {
                    toAdd = 1;
                }
                isMinigameFinished.Add(toAdd);
            }

            //Debug.LogWarning("This dialogue has " + elementsInDialogue + " elements in it");
            numberOfElementsInDialogue.Add(elementsInDialogue);
        }

        if (displayer.tempIsInitialisation)
        {
            initialisation = 1;
        }
        //Debug.LogError("isInitialisation = " + initialisation);

        if (displayer.tempWaitingForReply)
        {
            waitForReply = 1;
        }
        //Debug.LogError("isWaitingForReply = " + waitForReply);

        if (displayer.tempHasReplied)
        {
            replied = 1;
        }

        if (displayer.tempHasReacted)
        {
            reacted = 1;
        }

        if (displayer.tempIsFinished)
        {
            isFinished = 1;
        }

        wentBackHome = (int)DialogueManager.Instance.wentBackHome;
        wentToBridge = (int)DialogueManager.Instance.wentToBridge;

        Instance = this;
    }
}
