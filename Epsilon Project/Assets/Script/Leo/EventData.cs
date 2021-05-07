using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventData
{
    public static EventData Instance { set; get; }
    public int IDdialogue;
    public int IDgame;
    public EventData(Event even)
    {
        IDdialogue = even.IDdialogue;
        IDgame = even.IDgame;
    }
    
}
