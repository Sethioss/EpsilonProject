using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchZoom : MonoBehaviour
{
    public float perspectiveZoomSpeed = .5f;
    public float orthoZoomSpeed = .5f;

    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position = touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position = touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudediff = prevTouchDeltaMag - touchDeltaMag;

            if (cam.orthographic)
            {
                cam.orthographicSize += deltaMagnitudediff * orthoZoomSpeed;

                cam.orthographicSize = Mathf.Max(
                    cam.orthographicSize,
                    0.1f);
            }
            else
            {
                cam.fieldOfView += deltaMagnitudediff * perspectiveZoomSpeed;
                cam.fieldOfView = Mathf.Clamp(
                    cam.fieldOfView,
                    .1f,
                    179.9f);
            }
        }
    }
}
