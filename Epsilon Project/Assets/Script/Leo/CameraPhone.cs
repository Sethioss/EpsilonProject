using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraPhone : MonoBehaviour
{
    private bool camAvaible;
    private WebCamTexture webCamTexture;
    private Texture defaultTexture;

    public RawImage background;
    public AspectRatioFitter fit;

    private void Start()
    {
        defaultTexture = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("No camera detected");
            camAvaible = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                webCamTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        if (background == null)
        {
            Debug.Log("Unable to find back camera");
            return;
        }

        webCamTexture.Play();
        background.texture = webCamTexture;

        camAvaible = true;
    }

    private void Update()
    {
        if (!camAvaible)
        {
            return;
        }

        float ratio = (float)webCamTexture.width / (float)webCamTexture.height;
        fit.aspectRatio = ratio;

        float scaleY = webCamTexture.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = webCamTexture.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, -orient);
    }


}
