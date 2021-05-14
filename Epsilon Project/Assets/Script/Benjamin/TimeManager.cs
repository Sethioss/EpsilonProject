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
    public bool currentlyWaiting = false;
    public float timeValue = 60;
    public Text timerText;

    private static TimeManager instance;
    public static TimeManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        currentTime = System.DateTime.UtcNow.ToLocalTime();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }

        if (this != instance)
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(instance);
    }

    void Update()
    {
        currentTime = System.DateTime.UtcNow.ToLocalTime();
        if (currentTime >= timeToReach && currentlyWaiting == true)
        {
            Debug.Log("The wait is over");
            //currentlyWaiting = false;
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

    private void StartWaiting(System.DateTime timeToWait)
    {
        //Debug.Log(timeToWait.Day + " " + timeToWait.Hour + " " + timeToWait.Minute + " " + timeToWait.Second);
        storedTime = System.DateTime.UtcNow.ToLocalTime();

        Debug.Log("The wait started at " + storedTime.ToString("dd:HH:mm:ss"));

        storedTime = storedTime.AddDays(timeToWait.Day - 1);
        storedTime = storedTime.AddHours(timeToWait.Hour);
        storedTime = storedTime.AddMinutes(timeToWait.Minute);
        storedTime = storedTime.AddSeconds(timeToWait.Second);

        System.DateTime latestMessageSendingHour = new System.DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 1, 0, 0);
        System.DateTime earliestMessageSendingHour = new System.DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 7, 0, 0);

        if ((storedTime.Hour >= latestMessageSendingHour.Hour && storedTime.Hour < earliestMessageSendingHour.Hour) && !DialogueManager.Instance.autoMode)
        {
            int randomMinute = Random.Range(0, 3);
            int randomSecond = Random.Range(0, 10);
            System.DateTime newTimeToStore = new System.DateTime(currentTime.Year, currentTime.Month, storedTime.Day, earliestMessageSendingHour.Hour, randomMinute, randomSecond);

            storedTime = newTimeToStore;
#if UNITY_EDITOR
            Debug.Log("Available time exceeded : New time = " + storedTime.Day + ":" + storedTime.Hour + ":" + storedTime.Minute + ":" + storedTime.Second);
#endif
        }

        timeToReach = storedTime;

        Debug.Log("You will have to wait until " + timeToReach.ToString("dd:HH:mm:ss"));
        currentlyWaiting = true;

        //notificationScript.SendNotification(secondsToWait);
    }

    //Permets d'afficher le timer pour du debugging eventuel
    private void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ResetClock()
    {
        currentlyWaiting = false;
    }

    public void StartClock(string input)
    {
        //Parse the data into a readable DateTime object format
        //Converts each string into an int that corresponds (stringToDate[0] = Days ... -> stringToDate[3] = Seconds), then to a DateTime object

        //MinValue initializes day at 1. Accounted for in the "StartWaiting" function
        System.DateTime timeToStore = System.DateTime.MinValue;

        string[] splitInput = input.Split(new char[] { ':' }, System.StringSplitOptions.RemoveEmptyEntries);
        int[] stringToInt = new int[splitInput.Length];

        //String array to int array conversion
        for (int i = 0; i < splitInput.Length; i++)
        {
            int convertedString;
            convertedString = int.Parse(splitInput[i]);
            stringToInt[i] = convertedString;
        }

        //Int array to DateTime object conversion
        timeToStore = timeToStore.AddDays(stringToInt[0]);
        timeToStore = timeToStore.AddHours(stringToInt[1]);
        timeToStore = timeToStore.AddMinutes(stringToInt[2]);
        timeToStore = timeToStore.AddSeconds(stringToInt[3]);
        //Debug.Log("Time stored : " + (timeToStore.Day - 1) + ":" + timeToStore.Hour + ":" + timeToStore.Minute + ":" + timeToStore.Second);

        StartWaiting(timeToStore);
    }

    
    #region Save Time To Reach
    public int second;
    public int minute;
    public int hour;
    public int day;
    public void LoadTimeToReachData()
    {
        TimeToReachData data = SaveSystem.LoadTimeToReach();

        second = data.sec;
        minute = data.min;
        hour = data.hour;
        day = data.day;
    }
    public void SaveTimeToReachData()
    {
        second = timeToReach.Second;
        minute = timeToReach.Minute;
        hour = timeToReach.Hour;
        day = timeToReach.Day;
        SaveSystem.SaveTimeToReach(this);


    }
    #endregion

}
