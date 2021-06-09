using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class XMLTextTag
{
    public TextMeshProUGUI tagTxtObject;
    public string tagName;
    public enum DynamicIDs { Static = 0, inactivePeriodSet = 1, customString = 2, mj1PostInfos = 3, date = 4, sprite = 5};
    [SerializeField]
    [Header("Static = Fixed translation")]
    public DynamicIDs dynamicID;
    [Header("For name or string based dynamicIDs")]
    public string[] customParameters;
}
