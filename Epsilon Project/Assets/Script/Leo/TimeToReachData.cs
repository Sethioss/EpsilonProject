using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimeToReachData : MonoBehaviour
{
    public static TimeToReachData Instance { set; get; }
    public int sec;
    public int min;
    public int hour;
    public int day;

    public TimeToReachData(TimeManager timeToReach)
    {
        sec = timeToReach.second;
        min = timeToReach.minute;
        hour = timeToReach.hour;
        day = timeToReach.day;
    }
}
