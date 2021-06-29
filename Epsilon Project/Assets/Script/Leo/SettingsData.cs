using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public static SettingsData Instance { set; get; }
    public int unlockedAutoMode = 0;
    public int autoModeMessageSent = 0;
    public int autoMode = 0;
    public int inactivePeriods = 0;
    public int inactivePeriodStart, inactivePeriodEnd;
    public int language;

    public SettingsData(UserSettings settings)
    {
        if (settings.autoMode)
        {
            autoMode = 1;
        }

        if (settings.inactivePeriods)
        {
            inactivePeriods = 1;
        }

        if (settings.hasUnlockedAutoMode)
        {
            unlockedAutoMode = 1;
        }

        if (settings.sentAutoModeWindow)
        {
            autoModeMessageSent = 1;
        }

        inactivePeriodStart = settings.inactivePeriodStartHour;
        inactivePeriodEnd = settings.inactivePeriodEndHour;
        language = (int)settings.language;
    }
}
