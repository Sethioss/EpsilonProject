using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
public class ReadAndWriteStorage : MonoBehaviour
{
    private void Awake()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
    }

    IEnumerator Start()
    {
        WWW www = new WWW("http://gyanendushekhar.com/wp-content/uploads/2017/07/SampleImage.png");
        while (!www.isDone)
            yield return null;
        Debug.Log(www.texture.name);
        GameObject rawImage = GameObject.Find("RawImage");
        rawImage.GetComponent<RawImage>().texture = www.texture;
    }

    private void Test()
    {
        //AndroidJavaObject SDPath = new AndroidJavaObject("android.os.Environment");
        //AndroidJavaObject SDPath1 = SDPath.Call < AndroidJavaObject("getExternalStorageDirectory", null);
        //string path_str = SDPath1.Call<string>("getAbsolutePath");
    }
}
