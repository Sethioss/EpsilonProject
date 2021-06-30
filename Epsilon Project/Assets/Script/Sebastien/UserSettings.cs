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

    [Header("True = Acts as if it had already unlocked autoMode")]
    public bool autoModeDebug = false;
    public bool sentAutoModeWindow;
    public bool hasUnlockedAutoMode;
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

    public GameObject popUpObject;

#if UNITY_EDITOR
    private OptionMenu sceneOptionMenu;
    private XMLManager xmlManager;
    private bool messageSentMemory;
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
        }
        else
        {
            Destroy(this.gameObject);
        }

        Init();        
    }

    public void Init()
    {
        LoadUserSettings();

        if (hasUnlockedAutoMode && !sentAutoModeWindow)
        {
            //Create a window to say you unlocked automode
            try
            {
                popUpObject.GetComponent<AnimBanner>().ChangeAnim();
            }
            catch
            {

            }

            sentAutoModeWindow = true;
        }

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

            if (data.unlockedAutoMode != 0)
            {
                hasUnlockedAutoMode = true;
            }
            else
            {
                hasUnlockedAutoMode = false;
            }

            if (data.autoModeMessageSent != 0)
            {
                sentAutoModeWindow = true;
            }
            else
            {
                sentAutoModeWindow = false;
            }

            inactivePeriodStartHour = data.inactivePeriodStart;
            inactivePeriodEndHour = data.inactivePeriodEnd;
            language = (UserSettings.Language)data.language;
        }

    }

#if UNITY_EDITOR
    private void Update()
    {
        if (languageMemory != language)
        {
            ResetElements();
            languageMemory = language;
            xmlManager.SwitchLanguage();
        }

        if (inactivePeriodStartMemory != inactivePeriodStartHour)
        {
            ResetElements();
        }

        if (inactivePeriodEndMemory != inactivePeriodEndHour)
        {
            ResetElements();
        }

        if(messageSentMemory != sentAutoModeWindow)
        {
            ResetElements();
        }

        if (autoModeMemory != autoMode)
        {
            ResetElements();
        }

        if (inactivePeriodsMemory != inactivePeriods)
        {
            ResetElements();
        }

        if (profilePictureMemory != profilePicture)
        {
            ResetElements();
        }
    }

    private void ResetElements()
    {
        autoModeMemory = autoMode;
        messageSentMemory = sentAutoModeWindow;
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
        SaveSystem.SaveSettings(this);
    }
#endif
}
