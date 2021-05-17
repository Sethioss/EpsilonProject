using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpreadVirus : MonoBehaviour
{
    public int virusCoordX, virusCoordY;
    int tracker = 0;
    public GameObject[] allTiles;
    GameObject[,] tiles = new GameObject[3,3];
    public GameObject testTile;

    // Start is called before the first frame update
    void Start()
    {
        //tiles[0][0] = testTile;
        for(int i=0; i < 3; i++)
        {
            for(int j =0; j < 3; j++)
            {
                tiles[i, j] = allTiles[i + j];
            }
        }

        for (int i = 0; i < 3; i++)
        {
            Debug.Log("I started " + (i+1) + " times");
            for (int j = 0; j < 3; j++)
            {
                
                Debug.Log("J started " + (j+1) + " times");
                tiles[i, j] = allTiles[tracker];
                tracker++;
                //tiles[j,i].GetComponent<TilesBehaviour>().InfectTile();
                StartCoroutine(StartTagging(tiles[i, j], tracker));

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

}