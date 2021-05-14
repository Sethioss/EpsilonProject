using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

public class CameraPhone : MonoBehaviour
{
    private bool camAvaible;
    private WebCamTexture webCamTexture;
    private Texture defaultTexture;
    public RawImage photo;
    public RawImage background;
    public AspectRatioFitter fit;

    private void Awake()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
    }
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

    public void TakePhoto()
    {
        webCamTexture.Play();
        Texture2D PhotoTaken = new Texture2D(webCamTexture.width, webCamTexture.height);
        PhotoTaken.SetPixels(webCamTexture.GetPixels());
        PhotoTaken.Apply();
        photo.texture = PhotoTaken;
        int orient = webCamTexture.videoRotationAngle;
        photo.rectTransform.localEulerAngles = new Vector3(0, 0, -orient);
    }

}
