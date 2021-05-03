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
        
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(destination.transform.position);
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
            winScreen.SetActive(true);
        }
    }
}
