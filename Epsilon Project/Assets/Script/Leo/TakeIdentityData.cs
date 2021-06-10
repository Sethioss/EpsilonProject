using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TakeIdentityData : MonoBehaviour
{
    public static TakeIdentityData Instance { set; get; }
    public string Nickname;
    public string Password;

    public TakeIdentityData(TakeIdentity takeIdentity)
    {
        Nickname = takeIdentity.Nickname;
        Password = takeIdentity.Password;
    }
}
