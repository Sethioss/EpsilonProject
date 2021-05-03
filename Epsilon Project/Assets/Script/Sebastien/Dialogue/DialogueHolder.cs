using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHolder : MonoBehaviour
{
    public List<Dialogue> dialogueList = new List<Dialogue>();

    private static DialogueHolder instance;
    public static DialogueHolder Instance
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

    public void AddDialogue(Dialogue dialogue)
    {
        dialogueList.Add(dialogue);
        dialogue.id = dialogueList.Count - 1;
    }
}
