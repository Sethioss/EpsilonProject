using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakeIdentity : MonoBehaviour
{
    public string Nickname;
    public string Password;

    public Text TextNickname;
    public Text TextPassword;


    public void TakeNewidentity()
    {
        Nickname = TextNickname.text;
        Password = TextPassword.text;
        Debug.Log("c'est : " + Nickname);
        Debug.Log("c'est : " + Password);
    }
}
