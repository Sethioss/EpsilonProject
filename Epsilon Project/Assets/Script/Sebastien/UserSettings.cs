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

    [HideInInspector]
    public string languagePrefix;

    public Sprite profilePicture;

    [Header("If activated, the settings save file will persist after runtime mode in the editor")]
    public bool persistentSaveSettings = false;

#if UNITY_EDITOR
    private OptionMenu sceneOptionMenu;
    private XMLManager xmlManager;
    private bool autoModeMemory;
    private bool inactivePeriodsMemory;
    private int inactivePeriodStartMemory;
    private int inactivePeriodEndMemory;
    private Language languageMemory;
    private Sprite profilePictureMemory;
#endif

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(instance);
#if UNITY_EDITOR
            ResetElements();
#endif
        }
        else
        {
            Destroy(this.gameObject);
        }

        LoadUserSettings();
        SaveSystem.SaveSettings(this);
    }

    private void LoadUserSettings()
    {
        SettingsData data = SaveSystem.LoadSettings();

        if (data != null)
        {
            if (data.autoMode != 0)
            {
                autoMode = true;
            }
            else
            {
                autoMode = false;
            }

            if (data.inactivePeriods != 0)
            {
                inactivePeriods = true;
            }
            else
            {
                inactivePeriods = false;
            }

            inactivePeriodStartHour = data.inactivePeriodStart;
            inactivePeriodEndHour = data.inactivePeriodEnd;
            language = (UserSettings.Language)data.language;
        }

    }

#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        if (!persistentSaveSettings)
        {
            SaveSystem.EraseSettingsData();
        }
    }
    private void Update()
    {
        if (languageMemory != language)
        {
            ResetElements();
            languageMemory = language;
            xmlManager.SwitchLanguage();
        }

        else if (inactivePeriodStartMemory != inactivePeriodStartHour)
        {
            ResetElements();
        }

        else if (inactivePeriodEndMemory != inactivePeriodEndHour)
        {
            ResetElements();
        }

        else if (autoModeMemory != autoMode)
        {
            ResetElements();
        }

        else if (inactivePeriodsMemory != inactivePeriods)
        {
            ResetElements();
        }

        else if (profilePictureMemory != profilePicture)
        {
            ResetElements();
        }
    }

    private void ResetElements()
    {
        autoModeMemory = autoMode;
        inactivePeriodsMemory = inactivePeriods;
        inactivePeriodStartMemory = inactivePeriodStartHour;
        inactivePeriodEndMemory = inactivePeriodEndHour;
        profilePictureMemory = profilePicture;
        languageMemory = language;

        try
        {
            xmlManager = GameObject.FindObjectOfType<XMLManager>();
        }
        catch { }

        try
        {
            sceneOptionMenu = GameObject.FindObjectOfType<OptionMenu>();
            sceneOptionMenu.Init();
        }
        catch { }
    }
#endif
}
