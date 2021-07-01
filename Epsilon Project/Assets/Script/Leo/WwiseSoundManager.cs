using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseSoundManager : MonoBehaviour
{
    public static WwiseSoundManager instance;
    public AK.Wwise.Event Click;
    public AK.Wwise.Event errorSound;
    public AK.Wwise.Event Flicker;
    public AK.Wwise.Event GoVoice;
    public AK.Wwise.Event Like;
    public AK.Wwise.Event shieldAppear;
    public AK.Wwise.Event shieldDisappear;
    public AK.Wwise.Event StopVoice;
    public AK.Wwise.Event virus;

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
