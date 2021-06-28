using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimBanner : MonoBehaviour
{
    public Animator anim;
    private bool Open = false;
    public void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void ChangeAnim()
    {
        if (Open == false)
        {
            anim.SetBool("Open", true);
            Open = true;
        }
        else
        {
            anim.SetBool("Open", false);
            Open = false;
        }
    }

}
