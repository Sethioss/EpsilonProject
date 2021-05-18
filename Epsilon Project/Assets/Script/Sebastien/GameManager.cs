using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string gameSceneName;
    private int gameSceneId;

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

        GetGameSceneId();
    }

    private void SetDialogue(TextAsset textToSet)
    {
        DialogueManager.Instance.currentDialogueFile = textToSet;
    }

    private void GoToChatScene()
    {
        SceneManager.LoadScene(gameSceneId);
    }

    private void GetGameSceneId()
    {
        gameSceneId = SceneManager.GetSceneByName(gameSceneName).buildIndex;

        if (gameSceneId == -1)
        {
            Debug.Log("Couldn't find the scene of name " + gameSceneName + ". Index will be set to 0 by default, but can lead to exceptions.");
            gameSceneId = 0;
        }
    }

    public void SetDialogueAndGoToGame(TextAsset textToSet)
    {
        SetDialogue(textToSet);
        GoToChatScene();
    }
}
