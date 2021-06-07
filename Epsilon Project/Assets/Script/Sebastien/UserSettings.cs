using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSettings : MonoBehaviour
{
    private static UserSettings instance;
    public static UserSettings Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    [Tooltip("Waits autoModeWaitingTime between dialogues")]
    public bool autoMode = false;
    [Header("Only set automode time to 00:00:00:00 for testing purposes, not for builds!")]
    public string autoModeWaitingTime = "00:00:00:01";
    [Tooltip("No messages will be sent in a certain period of time")]

    [Header("Deactivate auto mode for inactive periods to be active")]
    public bool inactivePeriods = true;
    [Header("0 = 00AM")]
    [Range(0, 23)]
    public int inactivePeriodStartHour = 1;
    [Range(0, 23)]
    public int inactivePeriodEndHour = 7;
    public enum Language { Français = 0, English = 1 };
    public Language language;

    public string languagePrefix;

    public Sprite profilePicture;
}
