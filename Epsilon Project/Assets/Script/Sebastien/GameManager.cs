using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("The name of the main chatting app scene")]
    public int gameSceneId;
    [HideInInspector]
    public string gameSceneName;

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

        GetGameSceneName();
    }

    public void SetDialogue(TextAsset textToSet)
    {
        DialogueManager.Instance.currentDialogueFile = textToSet;
    }

    private void GoToChatScene()
    {
        SceneManager.LoadScene(gameSceneId);
    }

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void GetGameSceneName()
    {
        gameSceneName = SceneManager.GetSceneByBuildIndex(gameSceneId).name;

        if (gameSceneId == -1)
        {
            Debug.Log("Couldn't find the scene of name " + gameSceneName + ". Index will be set to Game by default, but can lead to exceptions.");
            gameSceneName = "Game";
        }
    }

    public void SetDialogueAndGoToGame(TextAsset textToSet)
    {
        SetDialogue(textToSet);
        GoToChatScene();
    }
}
