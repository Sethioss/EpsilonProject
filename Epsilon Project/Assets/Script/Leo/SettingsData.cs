using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public static SettingsData Instance { set; get; }
    public int autoMode;
    public int inactivePeriods;
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

        inactivePeriodStart = settings.inactivePeriodStartHour;
        inactivePeriodEnd = settings.inactivePeriodEndHour;
        language = (int)settings.language;
    }
}
