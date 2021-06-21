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
    //public int[] MinAngleCamera;
    //public int[] MaxAngleCamera;
    //private bool 
    //public void Update()
    //{
    //    for (int i = 0; i < Camera.Length; i++)
    //    {
    //        angleCamera[i] = Camera[i].transform.rotation;
    //        if (angleCamera[i].y > MinAngleCamera[i])
    //        {
    //            Camera[i].gameObject.transform.Rotate(new Vector3(0f, 100f, 0f) * Time.deltaTime);
    //        }
    //        else if (angleCamera[i].y > MinAngleCamera[i])
    //        {
    //            Camera[i].gameObject.transform.Rotate(new Vector3(0f, -100f, 0f) * Time.deltaTime);
    //        }
    //        else
    //        {
    //            Camera[i].gameObject.transform.Rotate(new Vector3(0f, -100f, 0f) * Time.deltaTime);
    //        }
    //    }
    //}
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
