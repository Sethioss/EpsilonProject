using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MailUI : MonoBehaviour
{
    public Text switchButtonText;
    public GameObject zoomedHUD;
    public TMP_Text zoomedSender;
    public TMP_Text zoomedContent;

    public int correctMail;
    public bool isCorrectMailSpam;
    bool onCorrectMail;

    public TMP_Text[] myMails;
    public TMP_Text[] myMailSenders;

    public string[] mailContent;
    public string[] mailSenders;

    public string[] spamContent;
    public string[] spamSenders;
    bool spamOpen = true;
    public int mailToShare;
    public bool isMailToShareSpam;
    int currentMail;

    public SwitchScene switchScene;
    // Start is called before the first frame update
    void Start()
    {
        SwitchInbox();
        zoomedHUD.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SwitchInbox()
    {
        
            for (int i = 0; i < myMails.Length; i++)
        {
            if (spamOpen == true)
            {
                if (i < mailContent.Length)
                {
                    myMails[i].text = mailContent[i];
                    myMailSenders[i].text = "From : " + mailSenders[i];
                }
                else
                {
                    myMails[i].text = null;
                    myMailSenders[i].text = null;
                }
            }
            else
            {
                if (i < spamContent.Length)
                {
                    myMails[i].text = spamContent[i];
                    myMailSenders[i].text = "From : " + spamSenders[i];
                }
                else
                {
                    myMails[i].text = "";
                    myMailSenders[i].text = "";
                }
            }
        }
        spamOpen = !spamOpen;
        if(spamOpen == true)
        {
            switchButtonText.text = "Main inbox";
        }
        else
        {
            switchButtonText.text = "Spam";
        }

    }

    public void OpenMail(int mailID)
    {
        zoomedHUD.SetActive(true);
        currentMail = mailID;
        if (spamOpen == false)
        {
            zoomedSender.text = mailSenders[mailID];
            zoomedContent.text = mailContent[mailID];
            //if()
        }
        else
        {
            zoomedSender.text = spamSenders[mailID];
            zoomedContent.text = spamContent[mailID];
        }
    }

    public void CloseMail()
    {
        zoomedHUD.SetActive(false);
        currentMail = -1;
    }

    public void ShareMail()
    {
        if(currentMail == mailToShare )
        {
            if((isMailToShareSpam && spamOpen == true)||(isMailToShareSpam == false && spamOpen == false)){ 
            Debug.Log("Yup this was the correct one");
                switchScene.SwitchGame();
            }
            else
            {
                Debug.Log("Noooo ! This is not correct");
            }
        }
        else
        {
            Debug.Log("Noooo ! This is not correct");
        }
    }
}
