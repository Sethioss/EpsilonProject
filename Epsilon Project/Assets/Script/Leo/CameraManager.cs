using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private float longestDistance = 0.0f;

    private const float defaultCamSize = 7.5f;

    [Range(defaultCamSize / 2, 90.0f)]
    public float maxCamSize = 10.0f;

    public Transform Player;

    private Vector3 cam;
    private Vector3 center;
    [Range(0.01f, 1.0f)]
    public float SmoothFator = 0.5f;

    //default : 45.0f

    [Range(0.0f, 360.0f)]
    public float CameraAngle = 0f;

    [Range(0.01f, 1.0f)]
    public float SizeSmoothFator = 0.5f;

    private Camera camera;

    void Awake()
    {
        // cam = transform.position - Targets[0].position;
        //camera = transform.GetChild(0).GetComponent<Camera>();

        transform.position = Vector3.zero;
    }

    

    private void Update()
    {
        UpdateCameraSize();
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        center = Vector3.zero;

        Transform[] far = new Transform[2];

        foreach (Transform _t in Player)
        {
            foreach (Transform _t2 in Player)
            {
                if (Vector3.Distance(_t.position, _t2.position) >= longestDistance)
                {
                    far[0] = _t;
                    far[1] = _t2;
                }

                //center = _t.position + _t2.position;
                //center /= 2;
            }

            //center += _t.position;
        }

        center += Player.position;
        //center /= (Targets.Length + 8);

        Vector3 newPos = center;

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFator);

        transform.GetChild(0).transform.localEulerAngles = new Vector3(33.0f, CameraAngle, 0.0f);
    }

    private void UpdateCameraSize()
    {
        longestDistance = 0.0f;

        foreach (Transform _t in Player)
        {
            if (longestDistance <= Vector3.Distance(_t.position, center))
            {
                longestDistance = Vector3.Distance(_t.position, center);
            }
        }

        if (longestDistance >= defaultCamSize)
        {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, (defaultCamSize + longestDistance) / 2.0f - 2.0f, SizeSmoothFator);
        }

        else
        {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, defaultCamSize, SizeSmoothFator);
        }

        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, defaultCamSize, maxCamSize);
    }
}
