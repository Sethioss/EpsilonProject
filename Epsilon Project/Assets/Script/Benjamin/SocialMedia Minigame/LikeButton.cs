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
            WwiseSoundManager.instance.Like.Post(gameObject);
            gameObject.GetComponent<Image>().sprite = unlikedSprite;
            isLiked = false;
            numberOfLikes--;
        }
        else
        {
            WwiseSoundManager.instance.Like.Post(gameObject);
            gameObject.GetComponent<Image>().sprite = likedSprite;
            isLiked = true;
            numberOfLikes++;
        }
    }
}
