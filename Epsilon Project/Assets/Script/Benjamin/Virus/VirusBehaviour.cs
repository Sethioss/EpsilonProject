using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusBehaviour : MonoBehaviour
{
    public int numberOfColumns, numberOfRows;
    public int hackFrequency = 3;
    int tracker = 0;
    public static bool reachedGoal;
    bool canHack = true;
    public GameObject[] allTiles;
    GameObject[,] tiles = new GameObject[20, 20];

    void Start()
    {
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
                    StartCoroutine(StartSpreading(i, j, 2f));
                }
            }
        }
    }

    void Update()
    {
        if (reachedGoal)
            Debug.Log("Goal Reached, congrats");
        if (canHack == true && reachedGoal == false)
            StartCoroutine(HackingProcess());
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
                            StartCoroutine(StartSpreading(coordY, coordX - 1, 2f));
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
                        GameObject tileRight = tiles[coordY, coordX + 1];
                        TilesBehaviour tileRightBhv = tileRight.GetComponent<TilesBehaviour>();
                        if (tileRightBhv.isInfected == false && tileRightBhv.isBlocked == false && tileRightBhv.isHacked == false)
                        {
                            currentTileBhv.holdsVirus = false;
                            tileRightBhv.holdsVirus = true;
                            StartCoroutine(StartSpreading(coordY, coordX + 1, 2f));
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
                            StartCoroutine(StartSpreading(coordY - 1, coordX, 2f));
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
                            StartCoroutine(StartSpreading(coordY + 1, coordX, 2f));
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
            canHack = true;
        }
        else
        {
            HackRandom();
        }
    }

}
