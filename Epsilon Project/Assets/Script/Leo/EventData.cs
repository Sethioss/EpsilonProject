using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventData
{

    public int IDdialogue;
    public int IDgame;
    // etc...

    public EventData (Event even)
    {
        IDdialogue = even.IDdialogue;
        IDgame = even.IDgame;
    }

}
