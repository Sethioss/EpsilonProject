using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CamMove[] cams;

    void Update()
    {
        for (int i = 0;i < cams.Length; i++)
        {
            cams[i].Update();
        }
    }

    [System.Serializable]
    public struct CamMove
    {
        public Transform c;
        public float min;
        public float max;
        public int speed;
        public bool sens;
        public Vector3 angleC;

        public float wait;
        private float timer;
        public bool waitTimer;
        public void Update()
        {
            
            angleC = c.eulerAngles;

            if (angleC.y < min)
            {
                timer += Time.deltaTime;
                if (timer >= wait)
                {
                    
                    timer = timer - wait;
                    waitTimer = true;
                    sens = false;
                }
                else
                {
                    c.gameObject.transform.Rotate(new Vector3(0f, 0f, 0f) * Time.deltaTime);
                    waitTimer = false;
                }
            }
            else if (angleC.y > max)
            {
                timer += Time.deltaTime;
                if (timer >= wait)
                {

                    timer = timer - wait;
                    waitTimer = true;
                    sens = true;
                }
                else
                {
                    c.gameObject.transform.Rotate(new Vector3(0f, 0f, 0f) * Time.deltaTime);
                    waitTimer = false;
                }
            }
            
            if (sens == true & waitTimer == true)
            {
                c.transform.Rotate(new Vector3(0f, -speed, 0f) * Time.deltaTime);
            }
            else if (sens == false & waitTimer == true)
            {
                c.transform.Rotate(new Vector3(0f, speed, 0f) * Time.deltaTime);
            }
        }

        
        

    }
}
