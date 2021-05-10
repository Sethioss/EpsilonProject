using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public NotificationManager notificationScript;
    public System.DateTime storedTime;
    public System.DateTime currentTime;
    public System.DateTime timeToReach;
    bool currentlyWaiting = false;
    public float timeValue = 60;
    public Text timerText;

    private void Start()
    {
        
    }
    
    void Update()
    {
        currentTime = System.DateTime.UtcNow.ToLocalTime();
        if (currentTime >= timeToReach && currentlyWaiting == true)
        {
            Debug.Log("The wait is over");
            currentlyWaiting = false;
        }
        //Calcule le temps en temps réél quand l'appli est ouverte
        //if(timeValue > 0) { 
        //timeValue -= Time.deltaTime;
        //}
        //else
        //{
        //    timeValue = 0;
        //    Debug.Log("The wait is finally over");
        //}

        //DisplayTime(timeValue);

    }

    public void StartWaiting(float secondsToWait)
    {
        storedTime = System.DateTime.UtcNow.ToLocalTime();
        Debug.Log("The wait started at " + storedTime.ToString("HH:mm:ss"));
        timeToReach = storedTime.AddSeconds(secondsToWait);
        Debug.Log("You will have to wait until " + timeToReach.ToString("HH:mm:ss"));
        currentlyWaiting = true;
        notificationScript.SendNotification(secondsToWait);
        //timeToReach = storedTime.AddDays(values[0]);
        //timeToReach = timeToReach.AddHours(values[1]);
        //timeToReach = timeToReach.AddMinutes(values[2]);
        //timeToReach = timeToReach.AddSeconds(values[3]);
    }

    //Permets d'afficher le timer pour du debugging eventuel
    void DisplayTime(float timeToDisplay)
    {
        if(timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}",minutes,seconds);
    }

    public void ParseStringToTime(string input)
    {
        //Store the time to add it to timeToReach (Initialized at 0)
        System.DateTime timeToStore = System.DateTime.MinValue;

        Debug.Log(timeToStore.Day + ":" + timeToStore.Hour + ":" + timeToStore.Minute + ":" + timeToStore.Second);


        //Parse the data into a readable DateTime object format
        //Turn each string into an int that corresponds (stringToDate[0] = Days ... -> stringToDate[3] = Seconds)
        string[] splitInput = input.Split(new char[] { ':' }, System.StringSplitOptions.RemoveEmptyEntries);
     
        int[] stringToDate = new int[splitInput.Length];
        
        for(int i = 0; i < splitInput.Length; i++)
        {
            int convertedString = int.Parse(splitInput[i]);

            stringToDate[i] = convertedString;
        }

        timeToStore = timeToStore.AddDays(stringToDate[0] - 1);
        timeToStore = timeToStore.AddHours(stringToDate[1]);
        timeToStore = timeToStore.AddMinutes(stringToDate[2]);
        timeToStore = timeToStore.AddSeconds(stringToDate[3]);

        timeToReach = timeToStore;

        //Debug.Log("Int array : " + stringToDate[0] + ":" + stringToDate[1] + ":" + stringToDate[2] + ":" + stringToDate[3]);
        Debug.Log("Time stored : " + timeToStore.Day + ":" + timeToStore.Hour + ":" + timeToStore.Minute + ":" + timeToStore.Second);
    }
}
