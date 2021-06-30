using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MinigameProgressionUnit
{
    public string stringID;
    public int id;
    public bool minigameFinished;
}

public class GameManager : MonoBehaviour
{
    [Header("The name of the main chatting app scene")]
    public int gameSceneId;
    [HideInInspector]
    public string gameSceneName;

    [Header("Minigame progression data")]
    public int currentMinigameID;
    public List<MinigameProgressionUnit> minigameProgressionList;

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
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this.gameObject);
        }

        GetGameSceneName();
        SetMinigamesID();
    }

    private void SetMinigamesID()
    {
        int i = 0;

        foreach (MinigameProgressionUnit unit in minigameProgressionList)
        {
            unit.id = i;
            i++;
        }
    }

    public void EraseSave()
    {
        SaveSystem.EraseTakeIdentityData();
        SaveSystem.EraseDialogueData();
        SaveSystem.EraseHackingDialogueData();
        SaveSystem.EraseSettingsData();
        SaveSystem.EraseTimeToReachData();

        DialogueManager.Instance.dialogueFileToLoad = DialogueManager.Instance.GetElementFileFromName("Intro1");
    }

    public void SetDialogue(TextAsset textToSet)
    {
        DialogueManager.Instance.dialogueFileToLoad = textToSet;
    }

    public void SetDialogue(string dialogueFileName)
    {
        DialogueManager.Instance.dialogueFileToLoad = (TextAsset)Resources.Load("Tables\\" + dialogueFileName);
    }

    private void GoToChatScene()
    {
        SceneManager.LoadScene(gameSceneId);

        SetMinigamesID();
        Debug.LogWarning(GameManager.Instance.currentMinigameID);
        Debug.LogWarning("Going to chat scene!");

        if (GameManager.Instance.currentMinigameID != -1)
        {
            GameManager.Instance.minigameProgressionList[GameManager.Instance.currentMinigameID].minigameFinished = true;
        }

        GameManager.Instance.currentMinigameID = -1;
    }

    public void FindCurrentMinigameBySceneName()
    {
        for (int i = 0; i < minigameProgressionList.Count; i++)
        {
            if (minigameProgressionList[i].stringID == SceneManager.GetActiveScene().name)
            {
                GameManager.Instance.currentMinigameID = minigameProgressionList[i].id;
                
                break;
            }
        }
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

    public void SetDialogueAndGoToGame(string fileName)
    {
        SetDialogue(fileName);
        GoToChatScene();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
