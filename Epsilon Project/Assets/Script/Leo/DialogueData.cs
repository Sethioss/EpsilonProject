using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public static DialogueData Instance { set; get; }
    public List<string> PreviousText;
    public int ElementText;
    public List<int> Choice;

    public int Initialisation;
    public int WaitSent;

    public DialogueData()
    {
        DialogueManager manager = DialogueManager.Instance;
        DialogueDisplayer displayer = DialogueManager.Instance.displayer;

        /*ElementText = displayer.currentDialogueElementId;

        for (i = 0; i < manager.dialogueList.Count; i++)
        {
            PreviousText[i] = manager.dialogueList[i].FileName;

            for (j = 0; j < manager.dialogueList[i].elements.Count; j++)
            {
                int index = i + j;
                Choice[index] = manager.dialogueList[i].elements[j].ChosenReplyIndex;
            }
        }*/

        if (displayer.isInitialisation == true)
        {
            Initialisation = 1;
        }
        else
        {
            Initialisation = 0;
        }

        if (displayer.isWaitingForReply == true)
        {
            WaitSent = 1;
        }
        else
        {
            WaitSent = 0;
        }
    }
}
