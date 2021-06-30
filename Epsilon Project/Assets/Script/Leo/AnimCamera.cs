using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimCamera : MonoBehaviour
{
    public Animator[] animCamera;
    private bool Desactive = true;

    public void ChangeAnim()
    {
        if (Desactive == true)
        {
            for (int i = 0; i < animCamera.Length; i++)
            {
                animCamera[i].SetBool("ReactiveCam", false);
            }
            Desactive = false;
        }
        else
        {
            for (int i = 0; i < animCamera.Length; i++)
            {
                animCamera[i].SetBool("ReactiveCam", true);
            }
            Desactive = true;
        }
    }
   
}
