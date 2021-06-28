using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionMenu : MonoBehaviour
{
    public Image[] profilePictureTransforms;
    public TextMeshProUGUI sliderText;
    public Slider sliderMinTime;
    public Slider sliderMaxTime;
    public Toggle inactivePeriodsToggle;
    public Toggle autoModeToggle;
    public TMP_Dropdown languageDropdown;
    bool audioMuted;

    private int sliderMinTimeValue = 1;
    private int sliderMaxTimeValue = 7;
    private bool inactiveToggleValue = true;
    private bool autoModeToggleValue = true;

    private UserSettings userSettings;

    private void Start()
    {
        Init();
    }

    public void SetLanguage()
    {
        userSettings.language = (UserSettings.Language)languageDropdown.value;
        XMLManager.Instance.SwitchLanguage();
    }

    public void Init()
    {
        userSettings = UserSettings.Instance;

        foreach (Image image in profilePictureTransforms)
        {
            image.sprite = userSettings.profilePicture;
        }

        sliderMinTimeValue = userSettings.inactivePeriodStartHour;
        sliderMaxTimeValue = userSettings.inactivePeriodEndHour;

        inactiveToggleValue = userSettings.inactivePeriods;
        autoModeToggleValue = userSettings.autoMode;

        languageDropdown.value = (int)userSettings.language;

        UpdateAutoMode();
        UpdateMinMaxTime();
    }

    private void UpdateMenu()
    {
        if (autoModeToggle.isOn)
        {
            if (DialogueManager.Instance.timeManager != null)
            {
                DialogueManager.Instance.timeManager.StopClock();
                DialogueManager.Instance.timeManager.StartClock(UserSettings.Instance.autoModeWaitingTime);
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

    public void SetSliderMinTime()
    {
        sliderMinTimeValue = (int)sliderMinTime.value;
        UpdateMinMaxTime();
    }

    public void SetSliderMaxTime()
    {
        sliderMaxTimeValue = (int)sliderMaxTime.value;
        UpdateMinMaxTime();
    }

    public void SetMinMaxTimeToToggle()
    {
        inactiveToggleValue = inactivePeriodsToggle.isOn;
        UpdateMinMaxTime();
    }

    private void DisableMinMaxTime()
    {
        inactivePeriodsToggle.isOn = false;
        SetMinMaxTimeToToggle();
    }

    public void SetAutoModeToToggle()
    {
        autoModeToggleValue = autoModeToggle.isOn;
        UpdateAutoMode();
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
            if (inactivePeriodsToggle.isOn)
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
}
