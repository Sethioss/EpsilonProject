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

    private int sliderMinTimeValue = 1;
    private int sliderMaxTimeValue = 7;
    private bool inactiveToggleValue = true;
    private bool autoModeToggleValue = true;

    private void Start()
    {
        Init();
    }

    public void HandleInputData(int val)
    {
        if(val == (int)DialogueManager.Language.FR)
        {
            DialogueManager.Instance.language = DialogueManager.Language.FR;
        }
        else if (val == (int)DialogueManager.Language.EN)
        {
            DialogueManager.Instance.language = DialogueManager.Language.EN;
        }
    }

    public void Init()
    {
        foreach (Image image in profilePictureTransforms)
        {
            image.sprite = DialogueManager.Instance.profilePicture;
        }

        sliderMinTimeValue = DialogueManager.Instance.inactivePeriodStartHour;
        sliderMaxTimeValue = DialogueManager.Instance.inactivePeriodEndHour;

        inactiveToggleValue = DialogueManager.Instance.inactivePeriods;
        autoModeToggleValue = DialogueManager.Instance.autoMode;

        UpdateAutoMode();
        UpdateMinMaxTime();
    }

    private void UpdateMenu()
    {
        if(autoModeToggle.isOn && inactivePeriodsToggle.isOn)
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
        DialogueManager.Instance.autoMode = autoModeToggleValue;
        autoModeToggle.isOn = autoModeToggleValue;

        UpdateAutoModeInManager();
        UpdateMenu();
    }

    private void UpdateMinMaxTime()
    {
        sliderMinTime.value = sliderMinTimeValue;
        sliderMaxTime.value = sliderMaxTimeValue;
        DialogueManager.Instance.inactivePeriods = inactiveToggleValue;
        inactivePeriodsToggle.isOn = inactiveToggleValue;

        UpdateMinMaxTimeInManager();
        UpdateMinMaxTimeSliderText();
        UpdateMenu();
    }

    private void UpdateMinMaxTimeInManager()
    {
        DialogueManager.Instance.inactivePeriodStartHour = sliderMinTimeValue;
        DialogueManager.Instance.inactivePeriodEndHour = sliderMaxTimeValue;
    }

    private void UpdateMinMaxTimeSliderText()
    {
        sliderText.text = "I don't want to receive messages between \n" + GetPmAm(sliderMinTimeValue) + " and " + GetPmAm(sliderMaxTimeValue);
    }

    private void UpdateAutoModeInManager()
    {
        DialogueManager.Instance.autoMode = autoModeToggleValue;
    }

    private string GetPmAm(int value)
    {
        string hour;
        hour = value + ".00AM";

        if(value > 12)
        {
            hour = (value % 12) + ".00PM";
        }

        return hour;
    }
}
