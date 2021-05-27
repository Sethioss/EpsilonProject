using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpreadVirus : MonoBehaviour
{
    //public int virusCoordX, virusCoordY;
    //public int numberOfColumns, numberOfRows;
    //public int hackFrequency = 3;
    //public static bool reachedGoal;
    //int tracker = 0;
    //public GameObject[] allTiles;
    //GameObject[,] tiles = new GameObject[20, 20];
    //public GameObject testTile;
    //bool canHack = true;



    //// Start is called before the first frame update
    //void Start()
    //{
    //    //tiles[0][0] = testTile;
    //    for (int i = 0; i < numberOfRows - 1; i++)
    //    {
    //        for (int j = 0; j < numberOfColumns-1; j++)
    //        {
    //            tiles[i, j] = allTiles[i + j];
    //        }
    //    }

    //    for (int i = 0; i < numberOfRows; i++)
    //    {
    //        for (int j = 0; j < numberOfColumns; j++)
    //        {

    //            tiles[i, j] = allTiles[tracker];
    //            tracker++;
    //            //tiles[j,i].GetComponent<TilesBehaviour>().InfectTile();
    //            StartCoroutine(StartTagging(tiles[i, j], 0));

    //        }
    //    }

    //    for (int i = 0; i < numberOfRows; i++)
    //    {
    //        for (int j = 0; j < numberOfColumns; j++)
    //        {
    //            if (tiles[i, j].GetComponent<TilesBehaviour>().holdsVirus == true)
    //            {
    //                //Debug.Log("Virus located in " + i + "," + j);
    //                StartCoroutine(StartSpreading(i, j, 2f));
    //            }
    //        }
    //    }



    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (reachedGoal)
    //        Debug.Log("Goal Reached, congrats");
    //    if (canHack == true && reachedGoal == false)
    //        StartCoroutine(HackingProcess());
    //}

    

    ////public IEnumerator StartTagging(GameObject target, int waitTime)
    ////{
    ////    yield return new WaitForSeconds(waitTime);
    ////    target.GetComponent<TilesBehaviour>().TagTile();
    ////}

    //public IEnumerator HackingProcess()
    //{
    //    canHack = false;
    //    yield return new WaitForSeconds(hackFrequency);
    //    HackRandom();
    //}

    //void HackRandom()
    //{
    //    int randomColumn = Random.Range(0, numberOfColumns);
    //    int randomRow = Random.Range(0, numberOfRows);
    //    if(tiles[randomColumn, randomRow].GetComponent<TilesBehaviour>().isInfected == false && tiles[randomColumn, randomRow].GetComponent<TilesBehaviour>().isBlocked == false && tiles[randomColumn, randomRow].GetComponent<TilesBehaviour>().isHacked == false)
    //    {
    //        tiles[randomColumn, randomRow].GetComponent<TilesBehaviour>().isHacked = true;
    //        canHack = true;
    //    }
    //    else
    //    {
    //        HackRandom();
    //    }
    //}

    //public IEnumerator StartSpreading(int coordY, int coordX, float timeToWait)
    //{
    //    //Debug.Log("StartSpreading");
    //    yield return new WaitForSeconds(timeToWait);
        
    //    if (reachedGoal == false) { 
    //    CheckSurroundings(coordY, coordX);
    //    }

    //}

    //void CheckSurroundings(int coordY, int coordX)
    //{
        
    //    if (reachedGoal == false)
    //    {
    //        Debug.Log("We are at Y " + coordY + " X " + coordX);
    //        GameObject currentTile = tiles[coordX, coordY];
    //        TilesBehaviour currentTileBhv = currentTile.GetComponent<TilesBehaviour>();
    //        int direction = Random.Range(1,5);
    //        switch (direction)
    //        {
                
    //            case 1:
    //                    Debug.Log("Initalizing Research");
    //                if (coordX > 0)
    //                {
    //                    GameObject tileLeft = tiles[coordY, coordX-1];
    //                    TilesBehaviour tileLeftBhv = tileLeft.GetComponent<TilesBehaviour>();
    //                    if (tileLeftBhv.isInfected == false && tileLeftBhv.isBlocked == false && tileLeftBhv.isHacked == false)
    //                    {
    //                        Debug.Log("Looking left");
    //                        currentTileBhv.holdsVirus = false;
    //                        tileLeftBhv.holdsVirus = true;
                            
    //                        StartCoroutine(StartSpreading(coordY, coordX - 1,2f));
    //                    }
    //                    else
    //                    {
    //                        //Debug.Log("Can't look left");
    //                        StartCoroutine(StartSpreading(coordY, coordX, 0.05f));
    //                    }
    //                }
    //                else
    //                {
    //                    //Debug.Log("Can't look left");
    //                    StartCoroutine(StartSpreading(coordY, coordX, 0.05f));
    //                }
    //                break;

    //            case 2:
    //                if (coordX + 1 <= numberOfColumns-1)
    //                {
    //                    GameObject tileRight = tiles[coordY, coordX+1];
    //                    TilesBehaviour tileRightBhv = tileRight.GetComponent<TilesBehaviour>();
    //                    if (tileRightBhv.isInfected == false && tileRightBhv.isBlocked == false && tileRightBhv.isHacked == false)
    //                    {
    //                        Debug.Log("Looking Right");
    //                        currentTileBhv.holdsVirus = false;
    //                        tileRightBhv.holdsVirus = true;
    //                        StartCoroutine(StartSpreading(coordY, coordX + 1, 2f));
    //                    }
    //                    else
    //                    {
    //                        //Debug.Log("Can't look left");
    //                        StartCoroutine(StartSpreading(coordY, coordX, 0.05f));
    //                    }
    //                }
    //                else
    //                {
    //                    //Debug.Log("Can't look left");
    //                    StartCoroutine(StartSpreading(coordY, coordX, 0.05f));
    //                }
    //                break;

    //            case 3:
    //                    Debug.Log("Initalizing Research");
    //                if (coordY > 0)
    //                {
    //                    GameObject tileUp = tiles[coordY - 1, coordX];
    //                    TilesBehaviour tileUpBhv = tileUp.GetComponent<TilesBehaviour>();

    //                    if (tileUpBhv.isInfected == false && tileUpBhv.isBlocked == false && tileUpBhv.isHacked == false)
    //                    {
    //                        currentTileBhv.holdsVirus = false;
    //                        tileUpBhv.holdsVirus = true;
    //                        StartCoroutine(StartSpreading(coordY - 1, coordX,2f));
    //                    }
    //                    else
    //                    {
    //                        //Debug.Log("Can't look Up");
    //                        StartCoroutine(StartSpreading(coordY, coordX, 0.05f));
    //                    }
    //                }
    //                else
    //                {
    //                    //Debug.Log("Can't look Up");
    //                    StartCoroutine(StartSpreading(coordY, coordX, 0.05f));
    //                }
    //                break;

    //            case 4:
    //                //Debug.Log(coordY);Debug.Log("Initalizing Research");
    //                if (coordY < numberOfRows-1)
    //                {Debug.Log("Initalizing Research");
    //                    GameObject tileDown = tiles[coordY + 1, coordX];
    //                    TilesBehaviour tileDownBhv = tileDown.GetComponent<TilesBehaviour>();
    //                    if (tileDownBhv.isInfected == false && tileDownBhv.isBlocked == false && tileDownBhv.isHacked == false)
    //                    {
    //                        Debug.Log("Looking Down");
    //                        currentTileBhv.holdsVirus = false;
    //                        tileDownBhv.holdsVirus = true;
    //                        StartCoroutine(StartSpreading(coordY + 1, coordX,2f));
    //                    }
    //                    else
    //                    {
    //                        //Debug.Log("Can't look Down");
    //                        StartCoroutine(StartSpreading(coordY, coordX, 0.05f));
    //                    }
    //                }
    //                else
    //                {
    //                    //Debug.Log("Can't look Down");
    //                    StartCoroutine(StartSpreading(coordY, coordX, 0.05f));
    //                }
    //                break;

    //        }
    //    }
    //}
}