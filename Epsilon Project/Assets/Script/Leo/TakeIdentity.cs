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
        Password = TextPassword.text;
        Debug.Log("c'est : " + Nickname);
        Debug.Log("c'est : " + Password);
    }
}
