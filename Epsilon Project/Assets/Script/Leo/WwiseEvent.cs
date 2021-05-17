using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseEvent : MonoBehaviour
{
    public AK.Wwise.Event wwiseEvent;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            wwiseEvent.Post(gameObject);
        }
    }
}
