using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CheckpointData
{
    public static CheckpointData Instance { get; set; }
    public List<string> minigameName;
    public List<bool> minigameFinished;
    public int wentBackHome;
    public int wentToBridge;

    public CheckpointData(List<MinigameProgressionUnit> mJProgression, DialogueManager dialogueManager)
    {
        minigameName = new List<string>();
        minigameFinished = new List<bool>();
        wentBackHome = 0;
        wentToBridge = 0;

        foreach (MinigameProgressionUnit unit in mJProgression)
        {
            minigameName.Add(unit.stringID);
            minigameFinished.Add(unit.minigameFinished);
        }

        wentBackHome = (int)dialogueManager.wentBackHome;
        wentToBridge = (int)dialogueManager.wentToBridge;
    }
}
