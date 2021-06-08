using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PostRenderer : MonoBehaviour
{
    public Material _material;

    void Awake()
    {
        _material = Resources.Load("Textures/TVnoise") as Material;
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // TV noise
        _material.SetFloat("_OffsetNoiseX", Random.Range(0f, 0.6f));
        float offsetNoise = _material.GetFloat("_OffsetNoiseY");
        _material.SetFloat("_OffsetNoiseY", offsetNoise + Random.Range(-0.2f, 0.2f));
        
        // Vertical shift
        float offsetPosY = _material.GetFloat("_OffsetPosY");
        if (offsetPosY > 0.0f)
        {
            _material.SetFloat("_OffsetPosY", offsetPosY - Random.Range(0f, offsetPosY));
        }
        else if (offsetPosY < 0.0f)
        {
            _material.SetFloat("_OffsetPosY", offsetPosY + Random.Range(0f, -offsetPosY));
        }
        else if (Random.Range(0, 40) == 1)
        {
            _material.SetFloat("_OffsetPosY", Random.Range(-4f, 4f));
        }

        Graphics.Blit(source, destination, _material);
    }
}
