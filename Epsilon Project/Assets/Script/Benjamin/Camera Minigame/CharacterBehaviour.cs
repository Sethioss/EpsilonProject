using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBehaviour : MonoBehaviour
{
    public GameObject[] AllCameraTop;
    public GameObject[] AllDestination;
    public GameObject[] AllSpwan;
    public GameObject FinalDestination;
    public NavMeshAgent agent;
    public GameObject winScreen, loseScreen;
    public int NumberGoal = 0;
    private CameraManager cameraManager;
    // Start is called before the first frame update
    void Start()
    {
        NumberGoal = 0;
        agent.SetDestination(AllDestination[0].transform.position);
        AllCameraTop[0].SetActive(true);
        AllCameraTop[1].SetActive(false);
        AllCameraTop[2].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Camera"))
        {
            Debug.Log("You were spotted");
            //loseScreen.SetActive(true);
        }
        if (other.CompareTag("Goal"))
        {
            NumberGoal++;
            switch (NumberGoal)
            {
                case 1:
                    cameraManager.Glitch();
                    AllCameraTop[0].SetActive(false);
                    AllCameraTop[1].SetActive(true);
                    this.gameObject.SetActive(false);
                    this.transform.position = AllSpwan[0].transform.position;
                    this.gameObject.SetActive(true);
                    agent.SetDestination(AllDestination[1].transform.position);
                    break;
                case 2:
                    AllCameraTop[1].SetActive(false);
                    AllCameraTop[2].SetActive(true);
                    this.gameObject.SetActive(false);
                    this.transform.position = AllSpwan[1].transform.position;
                    this.gameObject.SetActive(true);
                    agent.SetDestination(FinalDestination.transform.position);
                    break;
            }
        }
        if (other.CompareTag("FinalGoal"))
        {
            MinigameManager.Instance.winAction.Invoke();
        }
            
    }
}
