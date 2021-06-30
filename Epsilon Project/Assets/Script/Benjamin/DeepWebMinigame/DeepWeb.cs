using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Net.Cache;
using TMPro;
public class DeepWeb : MonoBehaviour
{
    
    public TMP_Text UrlContainer;
    public TMP_Text[] UrlText;
    public GameObject[] UrlUI;

    void Awake() 
    {
        UrlContainer.text = ("http//mirror-11d269e3.onion/");
    }
    public void OpenURL(int UrlNumber)
    {
        //Mettre le son WWise Suivant : Click
        switch (UrlNumber)
        {
            case 0:
                UrlContainer.text = UrlText[0].text;
                UrlUI[0].SetActive(true);
                break;
            case 1:
                UrlContainer.text = UrlText[1].text;
                UrlUI[1].SetActive(true);
                break;
            case 2:
                UrlContainer.text = UrlText[2].text;
                UrlUI[2].SetActive(true);
                break;
            case 3:
                UrlContainer.text = UrlText[3].text;
                UrlUI[3].SetActive(true);
                break;
            case 4:
                UrlContainer.text = UrlText[4].text;
                UrlUI[4].SetActive(true);
                break;
        }
        

    }
    
    public void CloseURL(GameObject pageToClose)
    {
        //Mettre le son WWise Suivant : Click
        UrlContainer.text = ("http//mirror-11d269e3.onion/");
        pageToClose.SetActive(false);
    }

   
}
