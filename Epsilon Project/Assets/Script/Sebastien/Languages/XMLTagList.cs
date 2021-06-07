using UnityEngine;

public class XMLTagList : MonoBehaviour
{
    public XMLTextTag[] tagList;

    private void Start()
    {
        XMLManager.Instance.GetSceneXMLTags();
        XMLManager.Instance.SwitchLanguage();
    }
}
