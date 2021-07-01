using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MinigameProgressionUnit
{
    [Header("The minigame's scene name (The one that will be sent via link)")]
    public string stringID;
    [HideInInspector]
    public int id;
    public bool minigameFinished;

    public MinigameProgressionUnit() { }
    public MinigameProgressionUnit(string stringID, bool isFinished)
    {
        this.stringID = stringID;
        this.minigameFinished = isFinished;
    }
}

public class GameManager : MonoBehaviour
{
    [Header("The ID of the main chatting app scene")]
    public int gameSceneId;
    [HideInInspector]
    public string gameSceneName;

    public int currentMinigameID;
    public List<MinigameProgressionUnit> minigameProgressionList;

    [Header("Keeps the save files between two launchs of the game")]
    public bool persistentSave = false;

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        if (!GameManager.instance.persistentSave)
        {
            GameManager.Instance.EraseSave();
        }
    }
#endif
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
        LoadMinigameProgression();
        //LoadCheckpoint();
        SetMinigamesID();
    }

    private void LoadCheckpoint()
    {
        Debug.LogWarning("Loading CheckpointData");
        CheckpointData data = SaveSystem.LoadCheckpoint();

        if (data != null)
        {
            Instance.minigameProgressionList = new List<MinigameProgressionUnit>();

            for (int i = 0; i < data.minigameName.Count; i++)
            {
                Instance.minigameProgressionList.Add(new MinigameProgressionUnit(data.minigameName[i], data.minigameFinished[i]));
            }

            DialogueManager.Instance.wentBackHome = data.wentBackHome;
            DialogueManager.Instance.wentToBridge = data.wentToBridge;
        }
    }

    private void LoadMinigameProgression()
    {
        Debug.LogWarning("Loading minigameProgressionData");
        MinigameProgressionData data = SaveSystem.LoadMinigameProgressionData();

        if (data != null)
        {
            Instance.minigameProgressionList = new List<MinigameProgressionUnit>();

            for(int i = 0; i < data.minigameName.Count; i++)
            {
                Instance.minigameProgressionList.Add(new MinigameProgressionUnit(data.minigameName[i], data.minigameFinished[i]));
            }
        }
    }

    public void LoadChekpoint()
    {
        Debug.LogWarning("Loading checkpointData");
        CheckpointData data = SaveSystem.LoadCheckpoint();

        if (data != null)
        {
            Instance.minigameProgressionList = new List<MinigameProgressionUnit>();

            for (int i = 0; i < data.minigameName.Count; i++)
            {
                Instance.minigameProgressionList.Add(new MinigameProgressionUnit(data.minigameName[i], data.minigameFinished[i]));
            }

            DialogueManager.Instance.wentBackHome = data.wentBackHome;
            DialogueManager.Instance.wentToBridge = data.wentToBridge;
        }

        SaveSystem.SaveMinigameProgression(Instance.minigameProgressionList);
    }

    public void ResetMinigameProgressionValues()
    {
        Instance.minigameProgressionList = minigameProgressionList;
    }

    private void SetMinigamesID()
    {
        int i = 0;

        foreach (MinigameProgressionUnit unit in Instance.minigameProgressionList)
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
        SaveSystem.EraseMinigameProgressionData();
        SaveSystem.EraseCheckpointData();

        DialogueManager.Instance.dialogueFileToLoad = DialogueManager.Instance.GetElementFileFromName("FR-Intro1");
    }

    public void SetDialogue(TextAsset textToSet)
    {
        Debug.LogError(DialogueManager.Instance.GetElementFileFromName(DialogueManager.Instance.GetLocalisedDialogue(textToSet)).name);
        DialogueManager.Instance.dialogueFileToLoad = DialogueManager.Instance.GetElementFileFromName(DialogueManager.Instance.GetLocalisedDialogue(textToSet));
    }

    public void SetDialogue(string dialogueFileName)
    {
        Debug.LogError("Tables\\" + DialogueManager.Instance.GetLocalisedDialogue(dialogueFileName));
        DialogueManager.Instance.dialogueFileToLoad = (TextAsset)Resources.Load("Tables\\" + DialogueManager.Instance.GetLocalisedDialogue(dialogueFileName));
    }

    public void GoToChatScene()
    {

        SetMinigamesID();
        Debug.LogWarning(Instance.currentMinigameID);
        //Debug.LogWarning("Going to chat scene!");

        if (Instance.currentMinigameID != -1)
        {
            Instance.minigameProgressionList[Instance.currentMinigameID].minigameFinished = true;
        }

        Instance.currentMinigameID = -1;
        SaveSystem.SaveMinigameProgression(Instance.minigameProgressionList);

        SceneManager.LoadScene(gameSceneId);
    }

    public void FindCurrentMinigameBySceneName()
    {
        for (int i = 0; i < Instance.minigameProgressionList.Count; i++)
        {
            if (Instance.minigameProgressionList[i].stringID == SceneManager.GetActiveScene().name)
            {
                Instance.currentMinigameID = Instance.minigameProgressionList[i].id;

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
