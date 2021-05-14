using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimeToStartWritingData : MonoBehaviour
{
    public static TimeToReachData Instance { set; get; }
    public int sec;
    public int min;
    public int hour;
    public int day;

    public TimeToStartWritingData(DialogueDisplayer timeWriting)
    {
        sec = timeWriting.second;
        min = timeWriting.minute;
        hour = timeWriting.hour;
        day = timeWriting.day;
    }
}
