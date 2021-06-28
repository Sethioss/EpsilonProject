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

    private UserSettings userSettings;

    private void Awake()
    {
        currentTime = System.DateTime.UtcNow.ToLocalTime();

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
        //DontDestroyOnLoad(instance);
        userSettings = UserSettings.Instance;
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

    public void SetClockGoal(string input)
    {
        //Parse the data into a readable DateTime object format
        //Converts each string into an int that corresponds (stringToDate[0] = Days ... -> stringToDate[3] = Seconds), then to a DateTime object

        //MinValue initializes day at 1. Accounted for in the "StartWaiting" function
        System.DateTime timeToStore = System.DateTime.MinValue;

        //Debug.LogError("Input : " + input);
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
        timeToStore = timeToStore.AddYears(System.DateTime.UtcNow.Year - 1);
        timeToStore = timeToStore.AddMonths(System.DateTime.UtcNow.Month - 1);

        timeToStore = timeToStore.AddDays(stringToInt[0] - 1);
        timeToStore = timeToStore.AddHours(stringToInt[1]);
        timeToStore = timeToStore.AddMinutes(stringToInt[2]);
        timeToStore = timeToStore.AddSeconds(stringToInt[3]);
        //Debug.LogError("CLOCK :: Setting timeToStore");
        //Debug.LogError(timeToStore.Day + ":" + timeToStore.Hour + ":" + timeToStore.Minute + ":" + timeToStore.Second);

        SetClockGoal(timeToStore);
    }

    private void SetClockGoal(System.DateTime timeToStore)
    {

        System.TimeSpan waitTimeSpan = timeToStore.Subtract(currentTime);
        //Debug.LogError("CurrentTime date : " + currentTime.Date);
        //Debug.LogError("StoredTime date : " + timeToStore.Date);
        //Debug.LogError("waitTimeSpan : " + waitTimeSpan);

        StartWaiting(System.DateTime.MinValue.Add(waitTimeSpan).AddSeconds(1));
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

        System.DateTime inactiveStartHour = new System.DateTime(currentTime.Year, currentTime.Month, currentTime.Day, userSettings.inactivePeriodStartHour, 0, 0);
        System.DateTime inactiveEndHour = new System.DateTime(currentTime.Year, currentTime.Month, currentTime.Day, userSettings.inactivePeriodEndHour, 0, 0);

        if (inactiveStartHour.Hour <= inactiveEndHour.Hour)
        {
            if ((storedTime.Hour >= inactiveStartHour.Hour && storedTime.Hour < inactiveEndHour.Hour) && !userSettings.autoMode && userSettings.inactivePeriods)
            {
                int randomMinute = Random.Range(0, 3);
                int randomSecond = Random.Range(0, 10);
                System.DateTime newTimeToStore = new System.DateTime(currentTime.Year, currentTime.Month, storedTime.Day, inactiveEndHour.Hour, randomMinute, randomSecond);

                storedTime = newTimeToStore;
#if UNITY_EDITOR
                Debug.Log("Available time exceeded : New time = " + storedTime.Day + ":" + storedTime.Hour + ":" + storedTime.Minute + ":" + storedTime.Second);
#endif
            }
        }
        else
        {
            if ((storedTime.Hour >= inactiveStartHour.Hour || storedTime.Hour < inactiveEndHour.Hour) && !userSettings.autoMode && userSettings.inactivePeriods)
            {
                int randomMinute = Random.Range(0, 3);
                int randomSecond = Random.Range(0, 10);
                System.DateTime newTimeToStore = new System.DateTime(currentTime.Year, currentTime.Month, storedTime.Day + 1, inactiveEndHour.Hour, randomMinute, randomSecond);

                storedTime = newTimeToStore;
#if UNITY_EDITOR
                Debug.Log("Available time exceeded : New time = " + storedTime.Day + ":" + storedTime.Hour + ":" + storedTime.Minute + ":" + storedTime.Second);
#endif
            }
        }
        timeToReach = storedTime;

        Debug.Log("You will have to wait until " + timeToReach.ToString("dd:HH:mm:ss"));
        currentlyWaiting = true;
        SaveSystem.SaveTimeToReach(this);

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

    public void StopClock()
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

    public void StartClock(int input)
    {
        //Parses the data into a readable DateTime object format

        //MinValue initializes day at 1. Accounted for in the "StartWaiting" function
        System.DateTime timeToStore = System.DateTime.MinValue;

        int[] intList = GetIntToArray(input);

        //Int list to DateTime object conversion
        timeToStore = timeToStore.AddDays(intList[0]);
        timeToStore = timeToStore.AddHours(intList[1]);
        timeToStore = timeToStore.AddMinutes(intList[2]);
        timeToStore = timeToStore.AddSeconds(intList[3]);
        //Debug.Log("Time stored : " + (timeToStore.Day - 1) + ":" + timeToStore.Hour + ":" + timeToStore.Minute + ":" + timeToStore.Second);

        StartWaiting(timeToStore);
    }

    private int[] GetIntToArray(int input)
    {
        //If 0 is the first digit, it will not be included in the .ToString(), so we check if it's the case to know whether we should add it or not
        int firstDigit = input / 10000000;
        string stringInput = "";
        if (firstDigit == 0)
        {
            stringInput = firstDigit.ToString() + input.ToString();
        }
        else
        {
            stringInput = input.ToString();
        }

        //[0] = Days, [1] = Hours, [2] = Minutes, [3] = Seconds
        List<int> intList = new List<int>();
        for (int i = 0; i < stringInput.Length; i += 2)
        {
            intList.Add(int.Parse(stringInput.Substring(i, 2)));
        }

        return intList.ToArray();
    }
}
