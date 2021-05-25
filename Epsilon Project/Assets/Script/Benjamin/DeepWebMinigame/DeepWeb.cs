using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeepWeb : MonoBehaviour
{
    public GameObject sendMailUI;
    public Text inputText;
    public string correctAdress;
    

    public void OpenURL(GameObject pageToOpen)
    {
        pageToOpen.SetActive(true);
    }

    public void CloseURL(GameObject pageToClose)
    {
        pageToClose.SetActive(false);
    }

    public void SendMail()
    {
        sendMailUI.SetActive(true);
    }

    public void CloseMail()
    {
        sendMailUI.SetActive(false);
    }

    public void ForwardMail()
    {
        if(inputText.text == correctAdress)
        {
            Debug.Log("You sent it to the correct person, congrats !");
            MinigameManager.Instance.winAction.Invoke();
            
        }
        else
        {
            Debug.Log("This was not the correct adress you dummy");
        }
    }
}
