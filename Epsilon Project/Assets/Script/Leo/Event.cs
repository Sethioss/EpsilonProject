using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event : MonoBehaviour
{
    public int IDdialogue = 0;
    public int IDgame = 0;
    public void PositifIDDia()
    {
        ++IDdialogue;
    }
    public void NegatifIDDia()
    {
        --IDdialogue;
    }
    public void PositifIDGa()
    {
        ++IDgame;
    }
    public void NegatifIDGa()
    {
        --IDgame;
    }
    public void SaveEvent()
    {
        SaveSystem.SaveEvent(this);
    }

    public void LoadEvent()
    {
        EventData data = SaveSystem.LoadEvent();

        IDdialogue = data.IDdialogue;
        IDgame = data.IDgame;
    }
}
