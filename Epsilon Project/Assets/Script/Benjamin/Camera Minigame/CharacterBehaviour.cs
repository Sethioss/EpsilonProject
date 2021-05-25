using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBehaviour : MonoBehaviour
{
    public GameObject destination;
    public NavMeshAgent agent;
    public GameObject winScreen, loseScreen;
    // Start is called before the first frame update
    void Start()
    {
        agent.SetDestination(destination.transform.position);
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
            loseScreen.SetActive(true);
        }
        if (other.CompareTag("Goal"))
        {
            MinigameManager.Instance.winAction.Invoke();
        }
    }
}
