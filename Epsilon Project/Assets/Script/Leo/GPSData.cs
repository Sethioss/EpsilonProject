using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GPSData : MonoBehaviour
{
    public static GPSData Instance { set; get; }
    public float Latitude;
    public float Longitude;

    public GPSData(GPS gps)
    {
        Latitude = gps.latitude;
        Longitude = gps.longitude;
    }
}