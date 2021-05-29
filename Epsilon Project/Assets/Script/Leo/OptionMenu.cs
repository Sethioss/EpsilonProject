using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public Text sliderText;
    public Slider sliderMinTime;
    public Slider sliderMaxTime;

    private void Update()
    {
        sliderText.text = ("Envoi de message entre "+sliderMinTime.value+"h à "+sliderMaxTime.value+"h .");
    }
}
