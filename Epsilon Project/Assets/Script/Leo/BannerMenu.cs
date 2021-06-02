using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerMenu : MonoBehaviour
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
            anim.SetBool("Opning", true);
            Open = true;
        }
        else
        {
            anim.SetBool("Opning", false);
            Open = false;
        }
    }

    public void QuitAplication()
    {
        Application.Quit();
    }

}
