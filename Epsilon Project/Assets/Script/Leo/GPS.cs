using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class GPS : MonoBehaviour
{
    public static GPS instance;
    public static GPS Instance
    {
        get
        {
            return instance;
        }
    }

    public float latitude;
    public float longitude;

    private void Awake()
    {

        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(instance);
        }

        if (this != instance)
        {
            Destroy(this.gameObject);
        }

    }

    private void Start()
    {
        DontDestroyOnLoad(instance);
        StartCoroutine(StartLocationService());
    }

    public void LoadGPS()
    {
        GPSData data = SaveSystem.LoadGPS();

        latitude = data.Latitude;
        longitude = data.Longitude;
    }
    public void SaveGPS()
    {

        SaveSystem.SaveGPS(this);


    }

    private IEnumerator StartLocationService()
    {
        

        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("User has not enabled GPS");
            yield break;
        }

        Input.location.Start();
        int maxWait = 20;

        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if(maxWait <= 0)
        {
            Debug.Log("Timed out");
            yield break;
        }

        if(Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determin device location");
            yield break;
        }

        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;

        yield break;
    }
    
}
