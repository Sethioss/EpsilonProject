using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TakeIdentityData : MonoBehaviour
{
    public static TakeIdentityData Instance { set; get; }
    public string Username;
    public string Password;
    public string Day, Month, Year;
    public string Phone;
    public TakeIdentityData(TakeIdentity takeIdentity)
    {
        Username = takeIdentity.Username;
        Password = takeIdentity.Password;

        Day = takeIdentity.Day;
        Month = takeIdentity.Month;
        Year = takeIdentity.Year;

        Phone = takeIdentity.Phone;
    }
}
