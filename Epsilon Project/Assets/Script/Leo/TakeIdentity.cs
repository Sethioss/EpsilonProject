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
        Nickname = TextNickname.ToString();
        Password = TextPassword.ToString(); 
    }
}
