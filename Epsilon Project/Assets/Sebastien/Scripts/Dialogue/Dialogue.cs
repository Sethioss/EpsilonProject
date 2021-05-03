using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Dialogue
{
    public int id;
    [SerializeField]
    public List<DialogueElement> elements = new List<DialogueElement>();
    public UnityAction endDialogueAction;

    public void AddDialogueElement(DialogueElement element)
    {
        elements.Add(element);
    }
}
