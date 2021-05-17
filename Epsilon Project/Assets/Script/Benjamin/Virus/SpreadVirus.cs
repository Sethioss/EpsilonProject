using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpreadVirus : MonoBehaviour
{
    public int virusCoordX, virusCoordY;
    int tracker = 0;
    public GameObject[] allTiles;
    GameObject[,] tiles = new GameObject[3, 3];
    public GameObject testTile;


    // Start is called before the first frame update
    void Start()
    {
        //tiles[0][0] = testTile;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                tiles[i, j] = allTiles[i + j];
            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {

                tiles[i, j] = allTiles[tracker];
                tracker++;
                //tiles[j,i].GetComponent<TilesBehaviour>().InfectTile();
                StartCoroutine(StartTagging(tiles[i, j], 0));

            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (tiles[i, j].GetComponent<TilesBehaviour>().holdsVirus == true)
                {
                    //Debug.Log("Virus located in " + i + "," + j);
                    StartCoroutine(StartSpreading(i, j));
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator StartTagging(GameObject target, int waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        target.GetComponent<TilesBehaviour>().TagTile();
    }

    public IEnumerator StartSpreading(int coordY, int coordX)
    {
        Debug.Log("StartSpreading");
        yield return new WaitForSeconds(2f);
        CheckSurroundings(coordY, coordX);

    }

    void CheckSurroundings(int coordY, int coordX)
    {
        int direction = Random.Range(1, 5);
        switch (direction)
        {
            case 1:
                if (coordX > 0)
                {
                    if(tiles[coordY, coordX - 1].GetComponent<TilesBehaviour>().isInfected == false) { 
                    Debug.Log("Looking left");
                    tiles[coordY, coordX].GetComponent<TilesBehaviour>().holdsVirus = false;
                    tiles[coordY, coordX - 1].GetComponent<TilesBehaviour>().holdsVirus = true;
                    StartCoroutine(StartSpreading(coordX - 1, coordY));
                    }
                    else
                    {
                        //Debug.Log("Can't look left");
                        CheckSurroundings(coordY, coordX - 1);
                    }
                }
                else
                {
                    //Debug.Log("Can't look left");
                    CheckSurroundings(coordY, coordX - 1);
                }
                break;

            case 2:
                Debug.Log(coordX);
                if (coordX < 2)
                {
                    if (tiles[coordY, coordX + 1].GetComponent<TilesBehaviour>().isInfected == false) { 
                    Debug.Log("Looking Right");
                    tiles[coordY, coordX].GetComponent<TilesBehaviour>().holdsVirus = false;
                    tiles[coordY, coordX + 1].GetComponent<TilesBehaviour>().holdsVirus = true;
                    StartCoroutine(StartSpreading(coordX + 1,coordY));
                    }
                    else
                    {
                        //Debug.Log("Can't look left");
                        CheckSurroundings(coordY, coordX + 1);
                    }
                }
                else
                {
                    //Debug.Log("Can't look left");
                    CheckSurroundings(coordY, coordX +1);
                }
                break;

            case 3:
                if (coordY > 0)
                {
                    Debug.Log("Looking up, current Y coord is" + coordY);
                    if (tiles[coordY - 1, coordX].GetComponent<TilesBehaviour>().isInfected == false) { 
                    Debug.Log("Looking Up");
                    tiles[coordY, coordX].GetComponent<TilesBehaviour>().holdsVirus = false;
                    tiles[coordY - 1, coordX].GetComponent<TilesBehaviour>().holdsVirus = true;
                    StartCoroutine(StartSpreading(coordX, coordY - 1));
                    }
                    else
                    {
                        //Debug.Log("Can't look Up");
                        CheckSurroundings(coordY - 1, coordX);
                    }
                }
                else
                {
                    //Debug.Log("Can't look Up");
                    CheckSurroundings(coordY - 1, coordX);
                }
                break;

            case 4:
                Debug.Log(coordY);
                if (coordY < 2)
                {
                    if (tiles[coordY + 1, coordX].GetComponent<TilesBehaviour>().isInfected == false) { 
                    Debug.Log("Looking Down");
                    tiles[coordY, coordX].GetComponent<TilesBehaviour>().holdsVirus = false;
                    tiles[coordY + 1, coordX].GetComponent<TilesBehaviour>().holdsVirus = true;
                    StartCoroutine(StartSpreading(coordX,coordY + 1));
                    }
                    else
                    {
                        //Debug.Log("Can't look Down");
                        CheckSurroundings(coordY + 1, coordX);
                    }
                }
                else
                {
                    //Debug.Log("Can't look Down");
                    CheckSurroundings(coordY +1, coordX);
                }
                break;

        }
    }
}