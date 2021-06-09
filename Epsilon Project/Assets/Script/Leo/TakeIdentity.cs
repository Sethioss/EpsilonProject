using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TakeIdentity : MonoBehaviour
{
    public string Nickname;
    public string Password;

    public TextMeshProUGUI TextNickname;
    public TextMeshProUGUI TextPassword;


    public void TakeNewidentity()
    {
        Nickname = TextNickname.text;
        Nickname = TextPassword.text;
        Debug.Log("c'est : " + Nickname);
        Debug.Log("c'est : " + Password);
    }
    public void LoadIdentity()
    {
        TakeIdentityData data = SaveSystem.LoadTakeIdentity();

        Nickname = data.Nickname;
        Nickname = data.Nickname;
    }
    public void SaveIdentity()
    {

        SaveSystem.SaveTakeIdentity(this);


    }
}
