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
        public float yMin;
        public float yMax;
        public int speed;
        public float wait;
        public bool sens;

        private float timer;
        private bool waitTimer;
        private float offSetY;

        public void Update()
        {
            float SpeedTime;
            SpeedTime = speed * Time.deltaTime;

            if (!sens && offSetY < yMin)
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
                    SpeedTime = 0;
                    waitTimer = false;
                }
            }
            else if (sens && offSetY > yMax)
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
                    SpeedTime = 0;
                    waitTimer = false;
                }
            }

            if (sens == true && waitTimer == true)
            {
                SpeedTime *= 1;
            }
            else if (sens == false && waitTimer == true)
            {
                SpeedTime *= -1;
            }

            c.transform.Rotate(new Vector3(0, SpeedTime, 0));
            offSetY += SpeedTime;
        }
    }
}
