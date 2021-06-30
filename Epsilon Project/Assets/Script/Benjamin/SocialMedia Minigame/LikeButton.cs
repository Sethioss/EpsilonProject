using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LikeButton : MonoBehaviour
{
    public bool isLiked;
    public Sprite likedSprite, unlikedSprite;
    public int numberOfLikes;
    public TMP_Text likedText;

    private void Start()
    {
        if (isLiked)
        {
            gameObject.GetComponent<Image>().sprite = likedSprite;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = unlikedSprite;
        }
    }

    private void Update()
    {
        likedText.text = numberOfLikes.ToString();
    }

    public void Like()
    {
        if (isLiked)
        {
            //Mettre le son WWise Suivant : Like
            gameObject.GetComponent<Image>().sprite = unlikedSprite;
            isLiked = false;
            numberOfLikes--;
        }
        else
        {
            //Mettre le son WWise Suivant : Dislike
            gameObject.GetComponent<Image>().sprite = likedSprite;
            isLiked = true;
            numberOfLikes++;
        }
    }
}
