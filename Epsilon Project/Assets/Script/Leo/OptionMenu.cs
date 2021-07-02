using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class OptionMenu : MonoBehaviour
{
    #region Display Variables (Shown in the game)

    public Image[] profilePictureTransforms;
    public TextMeshProUGUI sliderText;
    public Slider sliderMinTime;
    public Slider sliderMaxTime;
    public Toggle inactivePeriodsToggle;
    public Toggle autoModeToggle;
    public TMP_Dropdown languageDropdown;
    bool audioMuted;

    #endregion

    #region Manager Values

    private int sliderMinTimeValue = 1;
    private int sliderMaxTimeValue = 7;
    private bool inactiveToggleValue = true;
    private bool autoModeToggleValue = true;
    #endregion

    private bool initiating = false;

    private UserSettings userSettings;

    private void Start()
    {
        initiating = true;
        Init();

        initiating = false;
    }

    public void SetLanguage()
    {
        WwiseSoundManager.instance.Click.Post(gameObject);

        userSettings.language = (UserSettings.Language)languageDropdown.value;
        XMLManager.Instance.SwitchLanguage();
    }

    public void Init()
    {
        userSettings = UserSettings.Instance;

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            userSettings.Init();
        }

        foreach (Image image in profilePictureTransforms)
        {
            image.sprite = userSettings.profilePicture;
        }

        sliderMinTimeValue = userSettings.inactivePeriodStartHour;
        sliderMaxTimeValue = userSettings.inactivePeriodEndHour;

        inactiveToggleValue = userSettings.inactivePeriods;
        autoModeToggle.interactable = true;
        autoModeToggleValue = userSettings.autoMode;

        if (!userSettings.autoModeDebug)
        {
            if (userSettings.hasUnlockedAutoMode)
            {
                autoModeToggle.interactable = true;
                autoModeToggleValue = userSettings.autoMode;
            }
            else
            {
                userSettings.autoMode = false;
                autoModeToggle.isOn = false;
                autoModeToggle.interactable = false;
            }
        }


        languageDropdown.value = (int)userSettings.language;

        UpdateAutoMode();
        UpdateMinMaxTime();
    }

    #region Audio
    private void MuteAudio()
    {
        if (!audioMuted)
            SetAudioMute(false);
        else if (audioMuted)
            SetAudioMute(true);
    }
    private void SetAudioMute(bool mute)
    {
        AudioSource[] sources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        for (int index = 0; index < sources.Length; ++index)
        {
            sources[index].mute = mute;
        }
        audioMuted = mute;
    }
    #endregion

    #region InactivityPeriods
    public void SetSliderMinTime()
    {
        sliderMinTimeValue = (int)sliderMinTime.value;
        if (!initiating && !inactivePeriodsToggle.isOn)
        {
            inactivePeriodsToggle.isOn = true;
            autoModeToggle.isOn = false;
        }
        UpdateMinMaxTime();
    }
    public void SetSliderMaxTime()
    {
        sliderMaxTimeValue = (int)sliderMaxTime.value;
        if (!initiating && !inactivePeriodsToggle.isOn)
        {
            inactivePeriodsToggle.isOn = true;
            autoModeToggle.isOn = false;
        }
        UpdateMinMaxTime();
    }
    public void SetMinMaxTimeToToggle()
    {
        WwiseSoundManager.instance.Click.Post(gameObject);
        inactiveToggleValue = inactivePeriodsToggle.isOn;
        UpdateMinMaxTime();
    }
    private void DisableMinMaxTime()
    {
        inactivePeriodsToggle.isOn = false;
        SetMinMaxTimeToToggle();
    }
    #endregion

    #region AutoMode
    public void SetAutoModeToToggle()
    {
        WwiseSoundManager.instance.Click.Post(gameObject);
        autoModeToggleValue = autoModeToggle.isOn;
        UpdateAutoMode();
    } 
    #endregion

    #region Update
    private void UpdateMenu()
    {
        if (autoModeToggle.isOn)
        {
            if (DialogueManager.Instance.timeManager != null)
            {
                if (!DialogueDisplayer.Instance.isWaitingForReply)
                {
                    DialogueManager.Instance.timeManager.StopClock();
                    DialogueManager.Instance.timeManager.StartClock(UserSettings.Instance.autoModeWaitingTime);
                }
            }
        }
        if (autoModeToggle.isOn && inactivePeriodsToggle.isOn)
        {
            inactivePeriodsToggle.isOn = false;
            inactivePeriodsToggle.interactable = false;
            SetMinMaxTimeToToggle();
        }
        else
        {
            if (!inactivePeriodsToggle.interactable)
            {
                inactivePeriodsToggle.interactable = true;
            }
        }

        SaveSystem.SaveSettings(userSettings);
    }
    private void UpdateAutoMode()
    {
        userSettings.autoMode = autoModeToggleValue;
        autoModeToggle.isOn = autoModeToggleValue;

        UpdateAutoModeInManager();
        UpdateMenu();
    }
    private void UpdateMinMaxTime()
    {
        sliderMinTime.value = sliderMinTimeValue;
        sliderMaxTime.value = sliderMaxTimeValue;
        userSettings.inactivePeriods = inactiveToggleValue;
        inactivePeriodsToggle.isOn = inactiveToggleValue;

        XMLManager.Instance.UpdateHour();

        try
        {
            if (!DialogueDisplayer.Instance.isWaitingForReply)
            {
                if (DialogueManager.Instance.displayer.currentBubble == null)
                {
                    DialogueManager.Instance.displayer.CreateMessageBubble();
                }
                DialogueManager.Instance.timeManager.StopClock();
                DialogueManager.Instance.timeManager.StartClock(UserSettings.Instance.autoModeWaitingTime);
            }
        }
        catch
        {

        }

        UpdateMinMaxTimeInManager();
        UpdateMenu();
    }
    private void UpdateMinMaxTimeInManager()
    {
        userSettings.inactivePeriodStartHour = sliderMinTimeValue;
        userSettings.inactivePeriodEndHour = sliderMaxTimeValue;
    }
    private void UpdateAutoModeInManager()
    {
        userSettings.autoMode = autoModeToggleValue;
    }
    #endregion
}
