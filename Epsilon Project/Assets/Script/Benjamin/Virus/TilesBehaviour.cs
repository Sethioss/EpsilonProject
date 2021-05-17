using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TilesBehaviour : MonoBehaviour
{
    public bool isInfected = false;
    public bool holdsVirus;
    Image tileImage;
    // Start is called before the first frame update
    void Start()
    {
        tileImage = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (holdsVirus) { 
            isInfected = true;
            tileImage.color = Color.black;
        }
        if (isInfected && holdsVirus == false)
            tileImage.color = Color.red;
    }

    public void TagTile()
    {
        if (tileImage.color == Color.green)
        {
            tileImage.color = Color.blue;
        }
        else
        tileImage.color = Color.green;
    }

    void GetInfected()
    {
        tileImage.color = Color.red;
    }

    public void DebugYellow()
    {
        tileImage.color = Color.yellow;
    }

}
