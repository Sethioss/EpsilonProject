using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakeIdentity : MonoBehaviour
{
    public string Nickname;

    public Text TextNickname;

    public int day;
    public int month;
    public int year;

    public Text TextDay;
    public Text TextMonth;
    public Text TextYear;


    public void TakeNewidentity()
    {
        Nickname = TextNickname.ToString();
        //day = TextDay;
        //month = TextMonth;
        //year = TextYear;
    }
}
