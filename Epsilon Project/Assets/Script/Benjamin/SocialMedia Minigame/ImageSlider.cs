using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSlider : MonoBehaviour
{
    public Sprite[] mySprites;
    public Image profilePicture;
    int currentPicture = 0;
    // Start is called before the first frame update
    void Start()
    {
        profilePicture.sprite = mySprites[currentPicture];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePicture (bool decreasing)
    {
        if (decreasing == true && currentPicture > 0)
        {
            currentPicture--;
            profilePicture.sprite = mySprites[currentPicture];
        }
        else if (decreasing == false && currentPicture < mySprites.Length - 1)
        {
            currentPicture++;
            profilePicture.sprite = mySprites[currentPicture];
        }
    }
}
