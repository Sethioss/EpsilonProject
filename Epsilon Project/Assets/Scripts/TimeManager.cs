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
    //public float timeValue = 60;
    //public Text timerText;

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
        //Calcule le temps en temps r��l quand l'appli est ouverte
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
        Debug.Log("The wait started at" + storedTime.ToString("HH:mm::ss"));
        timeToReach = storedTime.AddSeconds(secondsToWait);
        Debug.Log("You will have to wait until" + timeToReach.ToString("HH:mm:ss"));
        currentlyWaiting = true;
        notificationScript.SendNotification(secondsToWait);
        //timeToReach = storedTime.AddDays(values[0]);
        //timeToReach = timeToReach.AddHours(values[1]);
        //timeToReach = timeToReach.AddMinutes(values[2]);
        //timeToReach = timeToReach.AddSeconds(values[3]);
    }

    //Permets d'afficher le timer pour du debugging eventuel
    //void DisplayTime(float timeToDisplay)
    //{
    //    if(timeToDisplay < 0)
    //    {
    //        timeToDisplay = 0;
    //    }
    //    float minutes = Mathf.FloorToInt(timeToDisplay / 60);
    //    float seconds = Mathf.FloorToInt(timeToDisplay % 60);

    //    timerText.text = string.Format("{0:00}:{1:00}",minutes,seconds);
    //}
}