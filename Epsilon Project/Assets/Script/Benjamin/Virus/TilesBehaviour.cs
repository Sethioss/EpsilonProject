using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TilesBehaviour : MonoBehaviour
{
    public bool isInfected = false;
    public bool holdsVirus, isGoal;
    public bool isBlocked = true;
    public bool isHacked;
    public Renderer cubeRend;
    public Material orangeMat, yellowMat, redMat, blueMat, greenMat, greyMat;
    public GameObject virus;
    public Animator virusAnim;

    void Update()
    {
        if (holdsVirus) { 
            isInfected = true;
            isBlocked = false;
            cubeRend.material = orangeMat;
            virus.SetActive(true);
        }
        if (isInfected && holdsVirus == false) { 
            cubeRend.material = redMat;
            virus.SetActive(false);
        }

        if (isBlocked)
            cubeRend.material = greyMat;

        if (!isBlocked && !isInfected)
            cubeRend.material = greenMat;

        if (isGoal)
            cubeRend.material = yellowMat;
        if (isHacked)
            cubeRend.material = blueMat;

       if(isGoal && isInfected)
        {
            VirusBehaviour.reachedGoal = true;
        }
    }

    void OnMouseDown()
    {
        if (isHacked)
        {
            isHacked=false;
            cubeRend.material = greenMat;
        }
    }

}
