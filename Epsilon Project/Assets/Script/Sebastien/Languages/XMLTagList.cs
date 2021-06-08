using UnityEngine;

public class XMLTagList : MonoBehaviour
{
    public string collectionName;
    public XMLTextTag[] tagList;

    private void Start()
    {
        XMLManager.Instance.GetSceneXMLTags();
        XMLManager.Instance.SwitchLanguage();
    }
}
