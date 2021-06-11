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
    public List<bool> miniGameInvite;
    public List<int> numberOfElementsInDialogue;
    public int currentElement;

    public int initialisation;
    public int waitSent;

    public DialogueData(List<Dialogue> dialogueToSave)
    {
        DialogueDisplayer displayer = DialogueManager.Instance.displayer;

        dialogueFileName = new List<string>();
        chosenReplyId = new List<int>();
        elementId = new List<int>();
        miniGameInvite = new List<bool>();
        numberOfElementsInDialogue = new List<int>();
        currentElement = displayer.currentDialogueElementId;

        //Fetch all dialogues
        for(int i = 0; i < dialogueToSave.Count; i++)
        {
            int elementsInDialogue = 0;

            //Debug.LogWarning("Dialogue file name : " + dialogueToSave[i].fileName);
            dialogueFileName.Add(dialogueToSave[i].fileName);

            //Fetch all elements
            for(int j = 0; j < dialogueToSave[i].elements.Count; j++)
            {
                //Debug.LogWarning("Current element : " + j);
                Debug.LogWarning("Current element message : " + dialogueToSave[i].elements[j].message);
                elementsInDialogue++;

                DialogueElement element = dialogueToSave[i].elements[j];
                if(element.replies.Count > 0)
                {
                    Debug.LogWarning("Chosen reply message : " + element.replies[0].replyText);
                    if (element.replies[0].reaction != "")
                    {
                        Debug.LogWarning("Chosen reply reaction : " + element.replies[0].reaction);
                    }
                }

                chosenReplyId.Add(element.chosenReplyIndex);
                elementId.Add(element.index);
                miniGameInvite.Add(element.minigameInvite);
            }

            //Debug.LogWarning("This dialogue has " + elementsInDialogue + " elements in it");
            numberOfElementsInDialogue.Add(elementsInDialogue);
        }

        if (displayer.isInitialisation == true)
        {
            initialisation = 1;
        }
        else
        {
            initialisation = 0;
        }

        if (displayer.isWaitingForReply == true)
        {
            waitSent = 1;
        }
        else
        {
            waitSent = 0;
        }
    }
}
