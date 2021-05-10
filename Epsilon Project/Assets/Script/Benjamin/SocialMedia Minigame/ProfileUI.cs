using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    public GameObject zoomedHUD;

    public Image zoomedPicture;
    public Image[] pictures;

    public Sprite[] postedSprites;
    public Sprite[] sharedSprites;
    public Sprite[] likedSprites;


    enum typeOfPages { posted, shared, liked };
    typeOfPages pageOpen;

    void Start()
    {
        zoomedHUD.SetActive(false);
        SwitchPage(1);
    }

    void Update()
    {

    }

    public void SwitchPage(int pageID)
    {
        for (int i = 0; i < pictures.Length; i++)
        {
            switch (pageID)
            {
                case 1:
                    if (i < postedSprites.Length)
                    {
                        pictures[i].sprite = postedSprites[i];
                        pageOpen = typeOfPages.posted;
                    }
                    else
                    {
                        pictures[i].sprite = null;
                    }
                    break;
                case 2:
                    if (i<sharedSprites.Length)
                    {
                        pictures[i].sprite = sharedSprites[i];
                        pageOpen = typeOfPages.shared;
                    }
                    else
                    {
                        pictures[i].sprite = null;
                    }
                    break;
                case 3:
                    if (i < likedSprites.Length)
                    {
                        pictures[i].sprite = likedSprites[i];
                        pageOpen = typeOfPages.liked;
                    }
                    else
                    {
                        pictures[i].sprite = null;
                    }
                    break;
            }
        }
    }

    public void ClickOnPicture(int picID)
    {
        
        switch (pageOpen)
        {
            case typeOfPages.posted:
                if (picID < postedSprites.Length) { 
                zoomedPicture.sprite = postedSprites[picID];
                zoomedHUD.SetActive(true);
                }
                break;
            case typeOfPages.shared:
                if(picID < sharedSprites.Length) { 
                zoomedPicture.sprite = sharedSprites[picID];
                zoomedHUD.SetActive(true);
                }
                break;
            case typeOfPages.liked:
                if(picID < likedSprites.Length) { 
                zoomedPicture.sprite = likedSprites[picID];
                zoomedHUD.SetActive(true);
                }
                break;
        }
    }

    public void CloseHUD()
    {
        zoomedHUD.SetActive(false);
    }
}
