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
    public bool isLeaveMessage = false;

    public UnityAction replyEvent = null;

    public Reply(string reply, string reaction, int index, string reactionTime, UnityAction actions, bool isLeaveMessage = false)
    {
        this.replyText = reply;
        this.reaction = reaction;
        this.index = index;
        this.reactionTime = reactionTime;
        this.replyEvent = actions;
        this.isLeaveMessage = isLeaveMessage;
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
    public bool minigameInvite = false;
    public bool leaveConversationMessage = false;
    public List<Reply> replies = new List<Reply>();
    public UnityAction elementAction;

    public DialogueElement(string message, Reply reply, int index, string initiationTime, UnityAction elementAction, bool isInvite = false)
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
        this.minigameInvite = isInvite;
    }

    public DialogueElement(string message, Reply reply, string initiationTime, UnityAction elementAction, bool isInvite = false)
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
        this.minigameInvite = isInvite;
    }

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