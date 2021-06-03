using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject CameraGlitch = null;

    public PostRenderer postRenderer = null;
    public void Glitch()
    {
        StartCoroutine(WaitRenderer());
    }
    private IEnumerator WaitRenderer()
    {
        postRenderer.enabled = false;
        
        yield return new WaitForSeconds(0.25f);
        
        postRenderer.enabled = true;
        //Sfx?.RandomSFXbug();
        yield return new WaitForSeconds(0.5f);
        postRenderer.enabled = false;
        //Sfx?.MusicChaine();
        
    }
    private IEnumerator WaitGlitch()
    {
        postRenderer.enabled = true;
        //Sfx?.RandomSFXbug();
        yield return new WaitForSeconds(0.5f);
        postRenderer.enabled = false;
        //Sfx?.MusicChaine();
    }
}
