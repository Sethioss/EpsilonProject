using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
        FileStream stream = new FileStream(path, FileMode.Create);
        TakeIdentityData identitydata = new TakeIdentityData(identity);
        formatter.Serialize(stream, identitydata);
        stream.Close();

    }
    public static void SaveDialogue()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Dialogue.fun";
        FileStream stream = File.Open(path, FileMode.Create);
        DialogueData dialoguedata = new DialogueData();
        formatter.Serialize(stream, dialoguedata);
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
            FileStream stream = new FileStream(path, FileMode.Open);
            TakeIdentityData identitydata = formatter.Deserialize(stream) as TakeIdentityData;
            return identitydata;
        }
        else
        {
            Debug.LogError(" Save file not found in " + path);
            return null;
        }

    }
    public static DialogueData LoadDialogue()
    {
        string path = Application.persistentDataPath + "/Dialogue.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
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
    #endregion
}

