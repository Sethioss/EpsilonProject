using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class VirusTutorial : MonoBehaviour
{
    public int numberOfColumns, numberOfRows;
    public int hackFrequency = 3;
    int tracker = 0;
    public float currentValue;
    public static bool reachedGoal;
    bool canHack = true;
    public int config;
    public GameObject[] allTiles;
    GameObject[,] tiles = new GameObject[20, 20];
    bool gameEnded;
    public int step = 0;
    bool justStepped;
    public string[] tutorialDialogue;
    public TMP_Text dialogueText;
    public GameObject dialogueUI;
    public GameObject endButtons;
    public GameObject closeButton;
    public XMLTagList myTagList;
    
    void Start()
    {
        
        XMLManager.Instance.GetSceneXMLTags();
        XMLManager.Instance.SwitchLanguage();
        Time.timeScale = 0;
        reachedGoal = false;
        for (int i = 0; i < numberOfRows - 1; i++)
        {
            for (int j = 0; j < numberOfColumns - 1; j++)
            {
                tiles[i, j] = allTiles[i + j];
            }
        }

        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                tiles[i, j] = allTiles[tracker];
                tracker++;
            }
        }

        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                if (tiles[i, j].GetComponent<TilesBehaviour>().holdsVirus == true)
                {
                    StartCoroutine(StartSpreading(i, j, 4f));
                }
            }
        }
    }

    void Update()
    {
        myTagList.tagList[0].tagName = tutorialDialogue[step];
        XMLManager.Instance.GetSceneXMLTags();
        XMLManager.Instance.SwitchLanguage();
        if (step==4 && justStepped==true)
        {
            StartCoroutine (DialogueAppear());
            closeButton.SetActive(false);
            endButtons.SetActive(true);
        }
        if(step == 1 && justStepped == true)
        {
            StartCoroutine(DialogueAppear());
        }
        if (step == 2 && justStepped == true)
        {
            StartCoroutine(DialogueAppear());
        }
    }

    public IEnumerator DialogueAppear()
    {
        justStepped = false;
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0;
        dialogueUI.SetActive(true);
        //Mettre le son WWise Suivant : Click
    }

    public IEnumerator StartSpreading(int coordY, int coordX, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        if (reachedGoal == false)
        {
            CheckSurroundings(coordY, coordX);
        }

    }

    void CheckSurroundings(int coordY, int coordX)
    {

        if (reachedGoal == false)
        {
            GameObject currentTile = tiles[coordY, coordX];
            TilesBehaviour currentTileBhv = currentTile.GetComponent<TilesBehaviour>();
            int direction = Random.Range(1, 5);
            switch (direction)
            {
                case 1:
                    if (coordX > 0)
                    {
                        GameObject tileLeft = tiles[coordY, coordX - 1];
                        TilesBehaviour tileLeftBhv = tileLeft.GetComponent<TilesBehaviour>();
                        if (tileLeftBhv.isInfected == false && tileLeftBhv.isBlocked == false && tileLeftBhv.isHacked == false)
                        {
                            step++;
                            justStepped = true;
                            currentTileBhv.holdsVirus = false;
                            tileLeftBhv.holdsVirus = true;
                            (currentTileBhv.virusAnim).SetBool("canDissolve", true);
                            //Mettre le son WWise Suivant : virus
                            StartCoroutine(StartSpreading(coordY, coordX - 1, 4f));
                        }
                        else
                        {
                            StartCoroutine(StartSpreading(coordY, coordX, 0.1f));
                        }
                    }
                    else
                    {
                        StartCoroutine(StartSpreading(coordY, coordX, 0.1f));
                    }
                    break;

                case 2:
                    if (coordX + 1 <= numberOfColumns - 1)
                    {
                        currentTile = tiles[coordY, coordX];
                        currentTileBhv = currentTile.GetComponent<TilesBehaviour>();
                        GameObject tileRight = tiles[coordY, coordX + 1];
                        TilesBehaviour tileRightBhv = tileRight.GetComponent<TilesBehaviour>();
                        if (tileRightBhv.isInfected == false && tileRightBhv.isBlocked == false && tileRightBhv.isHacked == false)
                        {
                            step++;
                            justStepped = true;
                            currentTileBhv.holdsVirus = false;
                            tileRightBhv.holdsVirus = true;
                            (currentTileBhv.virusAnim).SetBool("canDissolve", true);
                            //Mettre le son WWise Suivant : virus
                            StartCoroutine(StartSpreading(coordY, coordX + 1, 4f));
                        }
                        else
                        {
                            StartCoroutine(StartSpreading(coordY, coordX, 0.1f));
                        }
                    }
                    else
                    {
                        StartCoroutine(StartSpreading(coordY, coordX, 0.1f));
                    }
                    break;

                case 3:
                    if (coordY > 0)
                    {
                        GameObject tileUp = tiles[coordY - 1, coordX];
                        TilesBehaviour tileUpBhv = tileUp.GetComponent<TilesBehaviour>();
                        if (tileUpBhv.isInfected == false && tileUpBhv.isBlocked == false && tileUpBhv.isHacked == false)
                        {
                            step++;
                            justStepped = true;
                            currentTileBhv.holdsVirus = false;
                            tileUpBhv.holdsVirus = true;
                            (currentTileBhv.virusAnim).SetBool("canDissolve", true);
                            //Mettre le son WWise Suivant : virus
                            StartCoroutine(StartSpreading(coordY - 1, coordX, 4f));
                        }
                        else
                        {
                            StartCoroutine(StartSpreading(coordY, coordX, 0.1f));
                        }
                    }
                    else
                    {
                        StartCoroutine(StartSpreading(coordY, coordX, 0.1f));
                    }
                    break;

                case 4:
                    if (coordY < numberOfRows - 1)
                    {
                        GameObject tileDown = tiles[coordY + 1, coordX];
                        TilesBehaviour tileDownBhv = tileDown.GetComponent<TilesBehaviour>();
                        if (tileDownBhv.isInfected == false && tileDownBhv.isBlocked == false && tileDownBhv.isHacked == false)
                        {
                            step++;
                            justStepped = true;
                            currentTileBhv.holdsVirus = false;
                            tileDownBhv.holdsVirus = true;
                            (currentTileBhv.virusAnim).SetBool("canDissolve", true);
                            //Mettre le son WWise Suivant : virus
                            StartCoroutine(StartSpreading(coordY + 1, coordX, 4f));
                        }
                        else
                        {
                            StartCoroutine(StartSpreading(coordY, coordX, 0.1f));
                        }
                    }
                    else
                    {
                        StartCoroutine(StartSpreading(coordY, coordX, 0.1f));
                    }
                    break;

            }
        }
    }
        public void CloseTutorial()
        {
        Time.timeScale = 1;
        dialogueUI.SetActive(false);
        //Mettre le son WWise Suivant : Click
    }

    public void ChangeScene(int sceneToLoad)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneToLoad);
        //Mettre le son WWise Suivant : Click
    }
}
