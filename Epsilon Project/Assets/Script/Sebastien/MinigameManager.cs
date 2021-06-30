using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    public UnityEvent leaveGameAction;
    public UnityEvent winAction;
    public UnityEvent loseAction;

    private static MinigameManager instance;
    public static MinigameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private GameManager cachedGameManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Start()
    {
        cachedGameManager = GameManager.Instance;
        cachedGameManager.FindCurrentMinigameBySceneName();
    }

    public void ActivateWinAction()
    {
        winAction.Invoke();
    }
    public void ActivateLoseAction()
    {
        loseAction.Invoke();
    }
    public void ActivateLeaveGameAction()
    {
        leaveGameAction.Invoke();
    }
}
