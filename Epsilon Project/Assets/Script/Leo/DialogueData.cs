using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public static DialogueData Instance { set; get; }
    public List<string> dialogueFileName;
    public List<int> chosenReplyId;
    public List<int> elementId;
    public List<int> numberOfElementsInDialogue;
    public int currentElement;

    public int initialisation = 0;
    public int waitForReply = 0;
    public int replied = 0;

    //public DialogueData() { }
    public DialogueData(List<Dialogue> dialogueToSave)
    {
        DialogueDisplayer displayer = DialogueManager.Instance.displayer;

        dialogueFileName = new List<string>();
        chosenReplyId = new List<int>();
        elementId = new List<int>();
        numberOfElementsInDialogue = new List<int>();
        currentElement = displayer.currentDialogueElementId;

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

    }
}
