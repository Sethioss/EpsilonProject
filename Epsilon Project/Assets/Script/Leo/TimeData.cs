using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimeData : MonoBehaviour
{
    public static TimeData Instance { set; get; }
    public int sec;
    public int min;
    public int hour;
    public int day;

    public TimeData(TimeManager time)
    {
        sec = time.second;
        min = time.minute;
        hour = time.hour;
        day = time.day;
    }
}
