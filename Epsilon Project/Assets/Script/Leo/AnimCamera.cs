using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimCamera : MonoBehaviour
{
    public GameObject[] Camera;
    public Animator[] animCamera;
    public Vector3[] posCamera;
    public Quaternion[] angleCamera;
    private bool Desactive = true;
    
    public void Start()
    {
        for (int i = 0; i < Camera.Length; i++)
        {
            posCamera[i] = Camera[i].transform.position;
            angleCamera[i] = Camera[i].transform.rotation;
        }
            
        
    }

    public void ChangeAnim()
    {
        if (Desactive == true)
        {
            for (int i = 0; i < Camera.Length; i++)
            {
                Camera[i].transform.position = posCamera[i];
                Camera[i].transform.rotation = angleCamera[i];
                animCamera[i].SetBool("ReactiveCam", false);
            }
            Desactive = false;
        }
        else
        {
            for (int i = 0; i < Camera.Length; i++)
            {
                Camera[i].transform.position = posCamera[i];
                Camera[i].transform.rotation = angleCamera[i];
                animCamera[i].SetBool("ReactiveCam", true);
            }
            Desactive = true;
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
    }
}
