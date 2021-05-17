using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TilesBehaviour : MonoBehaviour
{
    Image tileImage;
    // Start is called before the first frame update
    void Start()
    {
        tileImage = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
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

}
