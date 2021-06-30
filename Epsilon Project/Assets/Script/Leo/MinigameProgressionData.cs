using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MinigameProgressionData
{
    public static MinigameProgressionData Instance { get; set; }
    public List<string> minigameName;
    public List<bool> minigameFinished;

    public MinigameProgressionData(List<MinigameProgressionUnit> mJProgression)
    {
        minigameName = new List<string>();
        minigameFinished = new List<bool>();

        foreach(MinigameProgressionUnit unit in mJProgression)
        {
            minigameName.Add(unit.stringID);
            minigameFinished.Add(unit.minigameFinished);
        }
    }
}
