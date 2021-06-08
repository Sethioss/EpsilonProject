using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TilesBehaviour : MonoBehaviour
{
    public bool isInfected = false;
    public bool holdsVirus, isGoal;
    public bool isBlocked = true;
    public string[] textArray;
    public bool useRandomText;
    public bool isHacked;
    public Renderer cubeRend;
    public Material orangeMat, yellowMat, redMat, blueMat, greenMat, greyMat;
    public GameObject virus;
    public GameObject shield;
    public Animator virusAnim;
    public Animation cubeAnim;
    public GameObject hackUI;
    public string hackString;
    public TMP_Text hackText;
    public TMP_InputField hackField;
    public TMP_Text hackFieldText;
    bool UIActive;


    void Start()
    {
        hackUI.SetActive(false);
        if (isBlocked)
        {
            cubeAnim.Stop();
        }
    }

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
            //virus.SetActive(false);
        }

        if (isBlocked)
            cubeRend.material = greyMat;

        if (!isBlocked && !isInfected)
            cubeRend.material = greenMat;

        if (isGoal)
            cubeRend.material = yellowMat;
        if (isHacked) { 
            cubeRend.material = blueMat;
            shield.SetActive(true);
        }
        if (!isHacked)
        {
            shield.SetActive(false);
        }

        if (isGoal && isInfected)
        {
            VirusBehaviour.reachedGoal = true;
        }

        if(hackText.text == hackField.text && UIActive == true)
        {
            Debug.Log("Deactivating");
            hackUI.SetActive(false);
            hackField.Select();
            hackField.text = "";
            isHacked = false;
            cubeRend.material = greenMat;
            UIActive = false;
        }
    }

    void OnMouseDown()
    {
        if (isHacked && UIActive == false)
        {
            hackUI.SetActive(true);
            if (useRandomText) { 
            int length = Random.Range(4, 6);
            GenerateString( 6);
            }
            else
            {
                PickRandomString();
            }
            hackText.text = hackString;
            UIActive = true;
            
        }
    }
    
    void PickRandomString()
    {
        int stringChosen = Random.Range(0, textArray.Length);
        hackString = textArray[stringChosen];
    }

    void GenerateString( int stringLength)
    {
        for(int i = 0; i < stringLength; i++)
        {
            Debug.Log("Generating new letter");
            int letter = Random.Range(0,25);
            switch (letter)
            {
                case 0:
                {
                        string newString = "a";
                    hackString = hackString + newString;
                        break;
                }
                case 1:
                {
                        string newString = "b";
                        hackString = hackString + newString;
                        break;
                }
                case 2:
                {
                        string newString = "c";
                        hackString = hackString + newString;
                        break;
                }
                case 3:
                {
                        string newString = "d";
                        hackString = hackString + newString;
                        break;
                }
                case 4:
                {
                        string newString = "e";
                        hackString = hackString + newString;
                        break;
                }
                case 5:
                {
                        string newString = "f";
                        hackString = hackString + newString;
                        break;
                }
                case 6:
                {
                        string newString = "g";
                        hackString = hackString + newString;
                        break;
                }
                case 7:
                {
                        string newString = "h";
                        hackString = hackString + newString;
                        break;
                }
                case 8:
                {
                        string newString = "i";
                        hackString = hackString + newString;
                        break;
                }
                case 9:
                {
                        string newString = "j";
                        hackString = hackString + newString;
                        break;
                }
                case 10:
                {
                        string newString = "k";
                        hackString = hackString + newString;
                        break;
                }
                case 11:
                {
                        string newString = "l";
                        hackString = hackString + newString;
                        break;
                }
                case 12:
                {
                        string newString = "m";
                        hackString = hackString + newString;
                        break;
                }
                case 13:
                {
                        string newString = "n";
                        hackString = hackString + newString;
                        break;
                }
                case 14:
                {
                        string newString = "o";
                        hackString = hackString + newString;
                        break;
                }
                case 15:
                {
                        string newString = "p";
                        hackString = hackString + newString;
                        break;
                }
                case 16:
                {
                        string newString = "q";
                        hackString = hackString + newString;
                        break;
                }
                case 17:
                {
                        string newString = "r";
                        hackString = hackString + newString;
                        break;
                }
                case 18:
                {
                        string newString = "s";
                        hackString = hackString + newString;
                        break;
                }
                case 19:
                {
                        string newString = "t";
                        hackString = hackString + newString;
                        break;
                }
                case 20:
                {
                        string newString = "u";
                        hackString = hackString + newString;
                        break;
                }
                case 21:
                {
                        string newString = "v";
                        hackString = hackString + newString;
                        break;
                }
                case 22:
                {
                        string newString = "w";
                        hackString = hackString + newString;
                        break;
                }
                case 23:
                {
                        string newString = "x";
                        hackString = hackString + newString;
                        break;
                }
                case 24:
                {
                        string newString = "y";
                        hackString = hackString + newString;
                        break;
                }
                case 25:
                {
                        string newString = "z";
                        hackString = hackString + newString;
                        break;
                }

            }
            
        }
    }

}
