using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public static class SaveSystem
{
    #region Save
    public static void SaveEvent(Event even)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/event.fun";
        FileStream stream = new FileStream(path, FileMode.Create);
        EventData data = new EventData(even);
        formatter.Serialize(stream, data);
        stream.Close();

    }
    public static void SaveGPS(GPS gps)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/GPS.fun";
        FileStream stream = new FileStream(path, FileMode.Create);
        GPSData gpsdata = new GPSData(gps);
        formatter.Serialize(stream, gpsdata);
        stream.Close();

    }
    public static void SaveTimeToReach(TimeManager time)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/TimeReach.fun";
        FileStream stream = new FileStream(path, FileMode.Create);
        TimeToReachData timedata = new TimeToReachData(time);
        formatter.Serialize(stream, timedata);
        stream.Close();

    }
    public static void SaveTimeToStartWriting(DialogueDisplayer timeWriting)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/TimeWriting.fun";
        FileStream stream = new FileStream(path, FileMode.Create);
        TimeToStartWritingData timedata = new TimeToStartWritingData(timeWriting);
        formatter.Serialize(stream, timedata);
        stream.Close();

    }
    public static void SaveTakeIdentity(TakeIdentity identity)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Identity.fun";
        FileStream stream = File.Open(path, FileMode.OpenOrCreate);
        TakeIdentityData identitydata = new TakeIdentityData(identity);
        formatter.Serialize(stream, identitydata);
        stream.Close();

    }

    public static void SaveHackingDialogue(List<Dialogue> dialogueListToSaveTo)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/HackingDialogue.fun";
        FileStream stream = File.Open(path, FileMode.OpenOrCreate);
        DialogueData dialoguedata = new DialogueData(dialogueListToSaveTo);
        formatter.Serialize(stream, dialoguedata);
        stream.Close();

    }

    public static void SaveDialogue(List<Dialogue> dialogueListToSaveTo)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Dialogue.fun";
        FileStream stream = File.Open(path, FileMode.OpenOrCreate);
        DialogueData dialoguedata = new DialogueData(dialogueListToSaveTo);
        formatter.Serialize(stream, dialoguedata);
        stream.Close();

    }

    public static void SaveSettings(UserSettings settings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Settings.fun";
        FileStream stream = File.Open(path, FileMode.OpenOrCreate);
        SettingsData settingsData = new SettingsData(settings);
        formatter.Serialize(stream, settingsData);
        stream.Close();

    }

    public static void SaveMinigameProgression(List<MinigameProgressionUnit> mjProgressionData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/MinigameProgression.fun";
        FileStream stream = File.Open(path, FileMode.OpenOrCreate);
        MinigameProgressionData progressionData = new MinigameProgressionData(mjProgressionData);
        formatter.Serialize(stream, progressionData);
        stream.Close();

    }

    public static void SaveCheckpoint(List<MinigameProgressionUnit> mjProgressionData, DialogueManager manager)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Checkpoint.fun";
        FileStream stream = File.Open(path, FileMode.OpenOrCreate);
        CheckpointData checkpointData = new CheckpointData(mjProgressionData, manager);
        formatter.Serialize(stream, checkpointData);
        stream.Close();
    }

    #endregion
    #region Load
    public static GPSData LoadGPS()
    {
        string path = Application.persistentDataPath + "/GPS.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            GPSData gpsdata = formatter.Deserialize(stream) as GPSData;
            return gpsdata;
        }
        else
        {
            Debug.LogError(" Save file not found in " + path);
            return null;
        }
    }
    public static EventData LoadEvent()
    {
        string path = Application.persistentDataPath + "/event.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            EventData data = formatter.Deserialize(stream) as EventData;
            return data;
        }
        else
        {
            Debug.LogError(" Save file not found in " + path);
            return null;
        }
    }
    public static TimeToReachData LoadTimeToReach()
    {
        string path = Application.persistentDataPath + "/TimeReach.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            TimeToReachData timedata = formatter.Deserialize(stream) as TimeToReachData;
            stream.Close();
            return timedata;
        }
        else
        {
            Debug.LogError(" Save file not found in " + path);
            return null;
        }
    }
    public static TimeToStartWritingData LoadTimeToStartWriting()
    {
        string path = Application.persistentDataPath + "/TimeWriting.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            TimeToStartWritingData timedata = formatter.Deserialize(stream) as TimeToStartWritingData;
            stream.Close();
            return timedata;
        }
        else
        {
            Debug.LogError(" Save file not found in " + path);
            return null;
        }
    }
    public static TakeIdentityData LoadTakeIdentity()
    {
        string path = Application.persistentDataPath + "/Identity.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            TakeIdentityData identitydata = (TakeIdentityData)formatter.Deserialize(stream);
            stream.Close();
            return identitydata;
        }
        else
        {
            Debug.LogError(" Save file not found in " + path);
            return null;
        }

    }

    public static DialogueData LoadHackingDialogue()
    {
        string path = Application.persistentDataPath + "/HackingDialogue.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            DialogueData data = (DialogueData)formatter.Deserialize(stream);
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Load file not found in " + path);
            return null;
        }

    }

    public static DialogueData LoadDialogue()
    {
        string path = Application.persistentDataPath + "/Dialogue.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            DialogueData data = (DialogueData)formatter.Deserialize(stream);
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Load file not found in " + path);
            return null;
        }

    }

    public static SettingsData LoadSettings()
    {
        string path = Application.persistentDataPath + "/Settings.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            SettingsData data = (SettingsData)formatter.Deserialize(stream);
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Load file not found in " + path);
            return null;
        }

    }

    public static MinigameProgressionData LoadMinigameProgressionData()
    {
        string path = Application.persistentDataPath + "/MinigameProgression.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            MinigameProgressionData data = (MinigameProgressionData)formatter.Deserialize(stream);
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Load file not found in " + path);
            return null;
        }

    }

    public static CheckpointData LoadCheckpoint()
    {
        string path = Application.persistentDataPath + "/Checkpoint.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            CheckpointData data = (CheckpointData)formatter.Deserialize(stream);
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Load file not found in " + path);
            return null;
        }
    }
    #endregion
    #region Erase
    public static void EraseSettingsData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Settings.fun";
        File.Delete(path);
        Debug.LogWarning("Files have been successfully deleted at " + path);
    }
    public static void EraseTakeIdentityData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Identity.fun";
        File.Delete(path);
        Debug.LogWarning("Files have been successfully deleted at " + path);
    }
    public static void EraseDialogueData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Dialogue.fun";
        File.Delete(path);
        Debug.LogWarning("Files have been successfully deleted at " + path);
    }

    public static void EraseHackingDialogueData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/HackingDialogue.fun";
        File.Delete(path);
        Debug.LogWarning("Files have been successfully deleted at " + path);
    }

    public static void EraseTimeToReachData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/TimeReach.fun";
        File.Delete(path);
        Debug.LogWarning("Files have been successfully deleted at " + path);
    }

    public static void EraseMinigameProgressionData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/MinigameProgression.fun";
        File.Delete(path);
        Debug.LogWarning("Files have been successfully deleted at " + path);
    }

    public static void EraseCheckpointData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Checkpoint.fun";
        File.Delete(path);
        Debug.LogWarning("Files have been successfully deleted at " + path);
    }

    #endregion
}

