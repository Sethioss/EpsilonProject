using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class CharacterBehaviour : MonoBehaviour
{
    public GameObject[] AllCameraTop;
    //public GameObject[] AllDestination;
    public GameObject[] AllSpwan;
    //public GameObject FinalDestination;
    public GameObject[] destinations;
    public float[] stopsTime;
    public int[] teleportDestinations;
    [SerializeField] int step = 0;
    bool doneWaiting = true;
    public NavMeshAgent agent;
    public GameObject winScreen, loseScreen;
    public int NumberGoal = 0;
    private CameraManager cameraManager;
    public GameObject textBox;
    public TMP_Text textBoxText;
    public string[] stopTexts;
    bool textHasToChange = true;
    public bool isTutorial;
    int dialogueStep = 0;
    public XMLTagList myTagList;
    // Start is called before the first frame update
    void Start()
    {
        ChangeDestination();
        NumberGoal = 0;
        //agent.SetDestination(AllDestination[0].transform.position);
        if (isTutorial == false) { 
        AllCameraTop[0].SetActive(true);
        AllCameraTop[1].SetActive(false);
        AllCameraTop[2].SetActive(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    void ChangeDestination()
    {
        Debug.Log("Changing Destination");
        StartCoroutine(WaitAround(stopsTime[step]));
    }

    public IEnumerator WaitAround(float waitTime)
    {
        if (textHasToChange == true)
        {
            if (isTutorial == false)
            {
                int dialogueNumber = Random.Range(0, stopTexts.Length);
                myTagList.tagList[0].tagName = stopTexts[dialogueNumber];
                XMLManager.Instance.GetSceneXMLTags();
                XMLManager.Instance.SwitchLanguage();
            }
            if (isTutorial)
            {
                if (dialogueStep > 0)
                {
                    myTagList.tagList[0].tagName = stopTexts[dialogueStep - 1];
                    XMLManager.Instance.GetSceneXMLTags();
                    XMLManager.Instance.SwitchLanguage();
                }
                dialogueStep++;
            }
            textHasToChange = false;
            textBox.SetActive(true);
        }
        yield return new WaitForSeconds(waitTime);
        if (teleportDestinations[step] != 0)
        {
            switch (teleportDestinations[step])
            {
                case 1:
                    //cameraManager.Glitch();
                    AllCameraTop[0].SetActive(false);
                    AllCameraTop[1].SetActive(true);
                    this.gameObject.SetActive(false);
                    this.transform.position = AllSpwan[0].transform.position;
                    this.gameObject.SetActive(true);
                    break;
                case 2:
                    AllCameraTop[1].SetActive(false);
                    AllCameraTop[2].SetActive(true);
                    this.gameObject.SetActive(false);
                    this.transform.position = AllSpwan[1].transform.position;
                    this.gameObject.SetActive(true);
                    break;
            }
        }
        textBox.SetActive(false);
        textHasToChange = true;
        agent.SetDestination(destinations[step].transform.position);
        step++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == destinations[step - 1])
        {
            Debug.Log("Reached destination");
            if (step == destinations.Length)
            {
                MinigameManager.Instance.winAction.Invoke();
            }
            else
            {
                Debug.Log("Looking for next Destination");
                ChangeDestination();
            }
        }
        if (other.CompareTag("Camera"))
        {
            Debug.Log("You were spotted");
            MinigameManager.Instance.loseAction.Invoke();
        }
        //if (other.CompareTag("Goal"))
        //{
        //    NumberGoal++;
        //    switch (NumberGoal)
        //    {
        //        case 1:
        //            //cameraManager.Glitch();
        //            AllCameraTop[0].SetActive(false);
        //            AllCameraTop[1].SetActive(true);
        //            this.gameObject.SetActive(false);
        //            this.transform.position = AllSpwan[0].transform.position;
        //            this.gameObject.SetActive(true);
        //            agent.SetDestination(AllDestination[1].transform.position);
        //            break;
        //        case 2:
        //            AllCameraTop[1].SetActive(false);
        //            AllCameraTop[2].SetActive(true);
        //            this.gameObject.SetActive(false);
        //            this.transform.position = AllSpwan[1].transform.position;
        //            this.gameObject.SetActive(true);
        //            agent.SetDestination(FinalDestination.transform.position);
        //            break;
        //    }
        //}
        //if (other.CompareTag("FinalGoal"))
        //{
        //    MinigameManager.Instance.winAction.Invoke();
        //}

    }
}
