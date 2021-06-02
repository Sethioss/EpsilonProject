using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchZoom : MonoBehaviour
{
    //public float perspectiveZoomSpeed = .5f;
    //public float orthoZoomSpeed = .5f;
    public Camera cam;
    protected Plane myPlane;
    public bool rotate;
    //Camera cam;

    private void Awake()
    {
        if(cam == null)
        {
            cam = Camera.main;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount >= 1)
        {
            myPlane.SetNormalAndPosition(transform.up, transform.position);
        }

        var delta1 = Vector3.zero;
            var delta2 = Vector3.zero;

        if(Input.touchCount >= 1)
        {
            delta1 = PlanePositionDelta(Input.GetTouch(0));
            if(Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                cam.transform.Translate(delta1, Space.World);
            }
        }

        if(Input.touchCount >= 2)
        {
            var pos1 = PlanePosition(Input.GetTouch(0).position);
            var pos2 = PlanePosition(Input.GetTouch(1).position);
            var pos1b = PlanePosition(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
            var pos2b = PlanePosition(Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);

            var zoom = Vector3.Distance(pos1, pos2) / Vector3.Distance(pos1b, pos2b);

            if (zoom == 0 || zoom > 10)
                return;

            //cam.transform.position = Vector3.LerpUnclamped(pos1, cam.transform.position, 1 / zoom);

            if (rotate && pos2b != pos2)
            {
                cam.transform.RotateAround(pos1, myPlane.normal, Vector3.SignedAngle(pos2 - pos1, pos2b - pos1b, myPlane.normal));
            }
        }

        

        //    if(Input.touchCount == 2)
        //    {
        //        Touch touchZero = Input.GetTouch(0);
        //        Touch touchOne = Input.GetTouch(1);

        //        Vector2 touchZeroPrevPos = touchZero.position = touchZero.deltaPosition;
        //        Vector2 touchOnePrevPos = touchOne.position = touchOne.deltaPosition;

        //        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        //        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        //        float deltaMagnitudediff = prevTouchDeltaMag - touchDeltaMag;

        //        if (cam.orthographic)
        //        {
        //            cam.orthographicSize += deltaMagnitudediff * orthoZoomSpeed;

        //            cam.orthographicSize = Mathf.Max(
        //                cam.orthographicSize,
        //                0.1f);
        //        }
        //        else
        //        {
        //            cam.fieldOfView += deltaMagnitudediff * perspectiveZoomSpeed;
        //            cam.fieldOfView = Mathf.Clamp(
        //                cam.fieldOfView,
        //                .1f,
        //                179.9f);
        //        }
        //    }
    }

    protected Vector3 PlanePositionDelta(Touch touch)
    {
        if (touch.phase != TouchPhase.Moved)
            return Vector3.zero;

        var rayBefore = cam.ScreenPointToRay(touch.position - touch.deltaPosition);
        var RayNow = cam.ScreenPointToRay(touch.position);
        if(myPlane.Raycast(rayBefore, out var enterBefore) && myPlane.Raycast(RayNow, out var enterNow))
        {
            return rayBefore.GetPoint(enterBefore) - RayNow.GetPoint(enterNow);
        }
        return Vector3.zero;
        
    }

    protected Vector3 PlanePosition(Vector2 screenPos)
    {
        var rayNow = cam.ScreenPointToRay(screenPos);
        if(myPlane.Raycast(rayNow, out var enterNow))
        {
            return rayNow.GetPoint(enterNow);
        }
        return Vector3.zero;
    }
}
