using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public static DialogueData Instance { set; get; }
    public List<string> dialogueFileName;
    public List<int> chosenReplies;
    public List<int> elementId;
    public List<bool> miniGameInvite;
    public List<int> numberOfElementsInDialogue;
    public int currentElement;

    public int initialisation;
    public int waitSent;

    public DialogueData()
    {
        DialogueManager manager = DialogueManager.Instance;
        DialogueDisplayer displayer = DialogueManager.Instance.displayer;

        dialogueFileName = new List<string>();
        chosenReplies = new List<int>();
        elementId = new List<int>();
        miniGameInvite = new List<bool>();
        numberOfElementsInDialogue = new List<int>();
        currentElement = displayer.currentDialogueElementId;

        //Fetch all dialogues
        for(int i = 0; i < manager.dialogueList.Count; i++)
        {
            int elementsInDialogue = 0;
            Debug.LogWarning("Dialogue file name : " + manager.dialogueList[i].FileName);
            dialogueFileName.Add(manager.dialogueList[i].FileName);

            //Fetch all elements
            for(int j = 0; j < manager.dialogueList[i].elements.Count; j++)
            {
                Debug.LogWarning("Current element : " + j);
                Debug.LogWarning("Current element message : " + manager.dialogueList[i].elements[j].message);
                elementsInDialogue++;

                DialogueElement element = manager.dialogueList[i].elements[j];
                Debug.LogWarning("Chosen reply index : " + element.chosenReplyIndex);
                if(element.chosenReplyIndex != -1)
                {
                    Debug.LogWarning("Chosen reply message : " + element.replies[element.chosenReplyIndex].replyText);
                    if (element.replies[element.chosenReplyIndex].reaction != "")
                    {
                        Debug.LogWarning("Chosen reply reaction : " + element.replies[element.chosenReplyIndex].reaction);
                    }
                }

                chosenReplies.Add(element.chosenReplyIndex);
                elementId.Add(element.index);
                miniGameInvite.Add(element.minigameInvite);
            }

            Debug.LogWarning("This dialogue has " + elementsInDialogue + " elements in it");
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
