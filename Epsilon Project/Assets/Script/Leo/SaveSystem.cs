using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{

    public static void SaveEvent (Event even)
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
        string path = Application.persistentDataPath + "/event.fun";
        FileStream stream = new FileStream(path, FileMode.Create);
        GPSData gpsdata = new GPSData(gps);
        formatter.Serialize(stream, gpsdata);
        stream.Close();

    }
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
}

