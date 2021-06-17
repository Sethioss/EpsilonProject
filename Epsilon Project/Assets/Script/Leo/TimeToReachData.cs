using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimeToReachData
{
    public static TimeToReachData Instance { set; get; }
    public int sec;
    public int min;
    public int hour;
    public int day;

    public TimeToReachData(TimeManager timeManager)
    {
        sec = timeManager.timeToReach.Second;
        min = timeManager.timeToReach.Minute;
        hour = timeManager.timeToReach.Hour;
        day = timeManager.timeToReach.Day;
    }
}
