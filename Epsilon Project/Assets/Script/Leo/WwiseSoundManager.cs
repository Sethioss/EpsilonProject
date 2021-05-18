using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseSoundManager : MonoBehaviour
{
    private static WwiseSoundManager instance;
    public static WwiseSoundManager Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }

        if (this != instance)
        {
            Destroy(this.gameObject);
        }
    }


}
