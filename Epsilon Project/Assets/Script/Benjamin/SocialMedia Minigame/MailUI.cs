using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MailUI : MonoBehaviour
{
    private MinigameManager miniGame;

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
    private XMLManager xmlManager;
    // Start is called before the first frame update
    void Start()
    {
        miniGame = GameObject.FindObjectOfType<MinigameManager>();
        xmlManager = XMLManager.Instance;
        SwitchInbox();
        zoomedHUD.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(xmlManager.languageSwitchTrigger)
        {
            zoomedSender.text = myMailSenders[currentMail].text;
            zoomedContent.text = myMails[currentMail].text;
            xmlManager.languageSwitchTrigger = false;
        }
    }
    public void Click()
    {
        WwiseSoundManager.instance.Click.Post(gameObject);
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
        //if(spamOpen == true)
        //{
        //    switchButtonText.text = "Main inbox";
        //}
        //else
        //{
        //    switchButtonText.text = "Spam";

        //}

    }

    public void OpenMail(int mailID)
    {
        zoomedHUD.SetActive(true);
        WwiseSoundManager.instance.Click.Post(gameObject);
        currentMail = mailID;
        if (spamOpen == false)
        {
            zoomedSender.text = myMailSenders[mailID].text;
            zoomedContent.text = myMails[mailID].text;
            //if()
        }
        else
        {
            zoomedSender.text = myMailSenders[mailID].text;
            zoomedContent.text = spamContent[mailID];
        }
    }

    public void CloseMail()
    {
        WwiseSoundManager.instance.Click.Post(gameObject);
        zoomedHUD.SetActive(false);
        currentMail = -1;
    }

    public void ShareMail()
    {
        if(currentMail == mailToShare )
        {
            WwiseSoundManager.instance.Click.Post(gameObject);
            if ((isMailToShareSpam && spamOpen == true)||(isMailToShareSpam == false && spamOpen == false)){ 
            Debug.Log("Yup this was the correct one");
                miniGame.winAction.Invoke();
            }
            else
            {
                WwiseSoundManager.instance.errorSound.Post(gameObject);
                Debug.Log("Noooo ! This is not correct");
                miniGame.loseAction.Invoke();
            }
        }
        else
        {
            WwiseSoundManager.instance.errorSound.Post(gameObject);
            Debug.Log("Noooo ! This is not correct");
            miniGame.loseAction.Invoke();
        }
    }
}
