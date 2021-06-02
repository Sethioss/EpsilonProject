using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionMenu : MonoBehaviour
{
    public TextMeshProUGUI sliderText;
    public Slider sliderMinTime;
    public Slider sliderMaxTime;
    public Toggle inactivePeriodsToggle;
    public Toggle autoModeToggle;

    private int sliderMinTimeValue = 1;
    private int sliderMaxTimeValue = 7;
    private bool inactiveToggleValue = true;
    private bool autoModeToggleValue = true;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        sliderMinTimeValue = DialogueManager.Instance.inactivePeriodStartHour;
        sliderMaxTimeValue = DialogueManager.Instance.inactivePeriodEndHour;

        inactiveToggleValue = DialogueManager.Instance.inactivePeriods;
        autoModeToggleValue = DialogueManager.Instance.autoMode;

        UpdateAutoMode();
        UpdateMinMaxTime();
    }

    private void UpdateAutoMode()
    {
        DialogueManager.Instance.autoMode = autoModeToggleValue;
        autoModeToggle.isOn = autoModeToggleValue;

        UpdateAutoModeInManager();
    }

    private void UpdateAutoModeInManager()
    {
        DialogueManager.Instance.autoMode = autoModeToggleValue;
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

    public void EnableMinMaxTime()
    {
       inactiveToggleValue = inactivePeriodsToggle.isOn;
       UpdateMinMaxTime();
    }

    public void EnableAutoMode()
    {
        autoModeToggleValue = autoModeToggle.isOn;
        UpdateAutoMode();
    }

    private void UpdateMinMaxTime()
    {
        sliderMinTime.value = sliderMinTimeValue;
        sliderMaxTime.value = sliderMaxTimeValue;

        UpdateMinMaxToggle();
        UpdateMinMaxTimeInManager();
        UpdateMinMaxTimeSliderText();
    }

    private void UpdateMinMaxTimeInManager()
    {
        DialogueManager.Instance.inactivePeriodStartHour = sliderMinTimeValue;
        DialogueManager.Instance.inactivePeriodEndHour = sliderMaxTimeValue;
    }

    private void UpdateMinMaxTimeSliderText()
    {
        sliderText.text = "You're available to receive messages from : \n" + GetPmAm(sliderMinTimeValue) + " to " + GetPmAm(sliderMaxTimeValue);
    }

    private void UpdateMinMaxToggle()
    {
        DialogueManager.Instance.inactivePeriods = inactiveToggleValue;
        inactivePeriodsToggle.isOn = inactiveToggleValue;
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
