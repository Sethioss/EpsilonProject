using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ProflleUpdate : MonoBehaviour
{
    public TextMeshProUGUI UsernameText;
    public TextMeshProUGUI BirthDateText;
    public TextMeshProUGUI PhoneNumberText;

    public string Username;
    public string Day, Month, Year;
    public string PhoneNumber;

    
    void Update()
    {
        UsernameText.text = Username;
        BirthDateText.text = Day + "/" +  Month + "/" +  Year;
        PhoneNumberText.text = PhoneNumber;
    }

    public void LoadIdentity()
    {
        TakeIdentityData data = SaveSystem.LoadTakeIdentity();

        Username = data.Username;
        Day = data.Day;
        Month = data.Month;
        Year = data.Year;
        PhoneNumber = data.Phone;
    }
    public void changeProfil()
    {
        NativeGallery.CanOpenSettings();
        //NativeGallery.SaveImageToGallery(maTexture, "GalleryTest", "Mon img {0}.png");
    }
}
