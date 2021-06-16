using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#region Reply Object
[System.Serializable]
public class Reply
{
    public int index;
    public string reactionTime;
    public string replyText;
    public string reaction;

    public bool reactionSent;

    public UnityAction replyEvent = null;

    public Reply(string reply, string reaction, int index, string reactionTime, UnityAction actions)
    {
        this.replyText = reply;
        this.reaction = reaction;
        this.index = index;
        this.reactionTime = reactionTime;
        this.replyEvent = actions;
    }

    public Reply() { }
}
#endregion

#region Dialogue Element Object
[System.Serializable]
public class DialogueElement
{
    public int chosenReplyIndex = -1;
    public int index;
    public string initiationTime;
    public string message;
    public enum MessageType { NORMAL = 0, LINK = 1, LEAVE = 2};
    public MessageType messageType;

    public List<Reply> replies = new List<Reply>();
    public UnityAction elementAction;

    public DialogueElement(string message, Reply reply, int index, string initiationTime, UnityAction elementAction)
    {
        if (message != "")
        {
            this.SetMessage(message);
        }

        if (reply.replyText != "")
        {
            this.AddReply(reply);
        }

        this.index = index;
        this.initiationTime = initiationTime;
        this.elementAction = elementAction;
    }

    public DialogueElement(string message, Reply reply, string initiationTime, UnityAction elementAction)
    {

        if (message != "")
        {
            this.SetMessage(message);
        }

        if (reply.replyText != "")
        {
            this.AddReply(reply);
        }

        this.initiationTime = initiationTime;
        this.elementAction = elementAction;

    }

    public DialogueElement(string message, int index, string initiationTime, UnityAction elementAction)
    {

        if (message != "")
        {
            this.SetMessage(message);
        }

        this.index = index;
        this.initiationTime = initiationTime;
        this.elementAction = elementAction;

    }

    public DialogueElement() { }

    private void SetMessage(string message)
    {
        this.message = message;
    }

    public void AddReply(Reply reply)
    {
        this.replies.Add(reply);
    }
}
#endregion

#region Dialogue Object
[System.Serializable]
public class Dialogue
{
    public int id;
    public string fileName;
    [SerializeField]
    public List<DialogueElement> elements = new List<DialogueElement>();
    public UnityAction endDialogueAction;

    public void AddDialogueElement(DialogueElement element)
    {
        elements.Add(element);
    }
}
#endregion