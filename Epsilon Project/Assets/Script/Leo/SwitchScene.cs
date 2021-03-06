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

    public void SwitchDialogueGame(string resource)
    {
        SetNextDIalogue((TextAsset)Resources.Load(resource));
        SceneManager.LoadScene("Game");
    }
    public void SwitchSceneGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void SetNextDIalogue(TextAsset data)
    {
        DialogueManager.Instance.currentDialogueFile = data;
    }
}
