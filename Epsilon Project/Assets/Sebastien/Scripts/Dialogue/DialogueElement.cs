using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class CustomEvent : UnityEvent<int> { }

[System.Serializable]
public class Reply
{
    public int index;
    public float reactionTime;
    public string reply;
    public string reaction;  

    public UnityAction replyEvent = null;

    public Reply(string reply, string reaction, int index, float reactionTime, UnityAction actions)
    {
        this.reply = reply;
        this.reaction = reaction;
        this.index = index;
        this.reactionTime = reactionTime;
        this.replyEvent = actions;
    }
}

[System.Serializable]
public class DialogueElement
{
    public int index;
    public float initiationTime;
    public string message;
    public List<Reply> replies = new List<Reply>();
    public UnityAction elementAction;

    public DialogueElement(string message, Reply reply, int index, float initiationTime, UnityAction elementAction)
    {
        if (message != "")
        {
            this.SetMessage(message);
        }

        if (reply.reply != "")
        {
            this.AddReply(reply);
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
