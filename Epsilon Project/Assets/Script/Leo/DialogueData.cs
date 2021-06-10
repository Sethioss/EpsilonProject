using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueData : MonoBehaviour
{
    public static DialogueData Instance { set; get; }
    public string[] PreviousText;
    public int ElementText;
    public int[] Choice;

    public int Initialisation;
    public int WaitSent;

    public DialogueData(DialogueDisplayer dialogueDisplayer,DialogueManager dialogueManager)
    {
        int i;
        int j;

        ElementText = dialogueDisplayer.currentDialogueElementId;

        for (i = 0; i < dialogueManager.dialogueList.Count; i++)
        {
            PreviousText[i] = dialogueManager.dialogueList[i].FileName;

            for (j = 0; j < dialogueManager.dialogueList[i].elements.Count; j++)
            {
                int index = i + j;
                Choice[index] = dialogueManager.dialogueList[i].elements[j].ChosenReplyIndex;
            }
        }

        if(dialogueDisplayer.isInitialisation == true)
        {
            Initialisation = 1;
        }
        else
        {
            Initialisation = 0;
        }

        if (dialogueDisplayer.isWaitingForReply == true)
        {
            WaitSent = 1;
        }
        else
        {
            WaitSent = 0;
        }
        
    }
}
