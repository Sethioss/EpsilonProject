using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UpdateDataText : MonoBehaviour
{
    public Text IDdia;
    public Text IDgame;

    void Update()
    {
        IDdia.text = Event.Instance.IDdialogue.ToString();
        IDgame.text = Event.Instance.IDgame.ToString();
    }
}
