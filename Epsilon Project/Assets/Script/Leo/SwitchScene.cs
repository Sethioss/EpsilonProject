using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    private SceneManager sceneManager;
    public void SwitchSocialMedia()
    {
        SceneManager.LoadScene("SocialMediaMinigame");
    }

    public void SwitchEMails()
    {
        SceneManager.LoadScene("E Mails");
    }

    public void SwitchGame()
    {
        SetNextDIalogue((TextAsset)Resources.Load("Tables\\2"));
        SceneManager.LoadScene("Game");
    }

    public void SetNextDIalogue(TextAsset data)
    {
        DialogueManager.Instance.currentDialogueFile = data;
    }
}
