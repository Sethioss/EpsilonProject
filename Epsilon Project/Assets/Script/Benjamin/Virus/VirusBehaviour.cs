using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirusBehaviour : MonoBehaviour
{
    public int numberOfColumns, numberOfRows;
    public int hackFrequency = 3;
    int tracker = 0;
    public float timeToFade;
    float initialTimeToFade;
    public Image LoadingBar;
    float currentValue = 100;
    float initialValue;
    public float speed;
    public static bool reachedGoal;
    bool canHack = true;
    public int config;
     GameObject[] allTiles = new GameObject[16];
    public GameObject[] allTiles1;
    public GameObject[] allTiles2;
    public GameObject[] allTiles3;
    public GameObject[] tilesConfig;
    GameObject[,] tiles = new GameObject[20, 20];
    bool gameEnded;

    void Awake()
    {
        reachedGoal = false;
    }
    void Start()
    {
        initialTimeToFade = timeToFade;
        initialValue = currentValue;
        config = Random.Range(1, 4);
        for(int i =0; i < tilesConfig.Length; i++)
        {
            tilesConfig[i].SetActive(false);
        }
        switch (config)
        {
            case 1:
                allTiles = allTiles1;
                tilesConfig[0].SetActive(true);
                break;
            case 2: 
                allTiles = allTiles2;
                tilesConfig[1].SetActive(true);
                break;
            case 3:
                allTiles = allTiles3;
                tilesConfig[2].SetActive(true);
                break;
        }

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
        if (reachedGoal && !gameEnded)
        {
            Debug.Log("Goal Reached, congrats");
            StartCoroutine(EndGame());
        }
        if (canHack == true && reachedGoal == false)
        {
            StartCoroutine(HackingProcess());
        }

        currentValue -= speed * Time.deltaTime;

        LoadingBar.fillAmount = currentValue / 100;

    }

    private void FixedUpdate()
    {
        LoadingBar.fillAmount = currentValue / 100;
        timeToFade -= Time.deltaTime;
        if (timeToFade <= 0)
        {
            MinigameManager.Instance.loseAction.Invoke();
        }
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
                            currentTileBhv.holdsVirus = false;
                            tileLeftBhv.holdsVirus = true;
                            (currentTileBhv.virusAnim).SetBool("canDissolve", true);
                            //Mettre le son WWise Suivant : virus
                            WwiseSoundManager.instance.virus.Post(gameObject);
                            currentValue = initialValue;
                            timeToFade = initialTimeToFade;
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
                            currentTileBhv.holdsVirus = false;
                            tileRightBhv.holdsVirus = true;
                            currentValue = initialValue;
                            timeToFade = initialTimeToFade;
                            (currentTileBhv.virusAnim).SetBool("canDissolve", true);
                            //Mettre le son WWise Suivant : virus
                            WwiseSoundManager.instance.virus.Post(gameObject);
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
                            currentTileBhv.holdsVirus = false;
                            tileUpBhv.holdsVirus = true;
                            currentValue = initialValue;
                            timeToFade = initialTimeToFade;
                            (currentTileBhv.virusAnim).SetBool("canDissolve", true);
                            //Mettre le son WWise Suivant : virus
                            WwiseSoundManager.instance.virus.Post(gameObject);
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
                            currentTileBhv.holdsVirus = false;
                            tileDownBhv.holdsVirus = true;
                            currentValue = initialValue;
                            timeToFade = initialTimeToFade;
                            (currentTileBhv.virusAnim).SetBool("canDissolve", true);
                            //Mettre le son WWise Suivant : virus
                            WwiseSoundManager.instance.virus.Post(gameObject);
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
    public IEnumerator HackingProcess()
    {
        canHack = false;
        yield return new WaitForSeconds(hackFrequency);
        HackRandom();
    }

    void HackRandom()
    {
        int randomColumn = Random.Range(0, numberOfColumns);
        int randomRow = Random.Range(0, numberOfRows);
        if (tiles[randomColumn, randomRow].GetComponent<TilesBehaviour>().isInfected == false && tiles[randomColumn, randomRow].GetComponent<TilesBehaviour>().isBlocked == false && tiles[randomColumn, randomRow].GetComponent<TilesBehaviour>().isHacked == false)
        {
            tiles[randomColumn, randomRow].GetComponent<TilesBehaviour>().isHacked = true;
            //Mettre le son WWise Suivant : shieldAppear
            WwiseSoundManager.instance.shieldAppear.Post(gameObject);
            canHack = true;
        }
        else
        {
            HackRandom();
        }
    }

    //public IEnumerator StartFading()
    //{
    //    currentValue = 100;
    //    yield return new WaitForSeconds(timeToFade);
    //    Debug.Log("You loose");
    //    MinigameManager.Instance.loseAction.Invoke();
    //}

    public IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2f);
        MinigameManager.Instance.winAction.Invoke();
        gameEnded = true;
    }
}
