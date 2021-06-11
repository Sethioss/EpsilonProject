using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TakeIdentity : MonoBehaviour
{
    public string Username;
    public string Password;

    public string Day,Month,Year;

    public string Phone;

    public TextMeshProUGUI TextNickname;
    public TextMeshProUGUI TextPassword;

    public TextMeshProUGUI[] TextBirthDate;

    public TextMeshProUGUI TextPhone;
    public void TakeNewidentity()
    {
        Username = TextNickname.text;
        Password = TextPassword.text;

        Day = TextBirthDate[0].text;
        Month = TextBirthDate[1].text;
        Year = TextBirthDate[2].text;
        Phone = TextPhone.text;
        SaveIdentity();
    }
    public void LoadIdentity()
    {
        TakeIdentityData data = SaveSystem.LoadTakeIdentity();

        Username = data.Username;
        Password = data.Password;
    }
    public void SaveIdentity()
    {

        SaveSystem.SaveTakeIdentity(this);


    }

    
}
