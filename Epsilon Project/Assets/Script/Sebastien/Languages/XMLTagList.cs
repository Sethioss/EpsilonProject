using UnityEngine;

public class XMLTagList : MonoBehaviour
{
    public string collectionName;
    public XMLTextTag[] tagList;

    private void Start()
    {
        XMLManager.Instance.GetSceneXMLTags();
        //Debug.Log("Fetching XMLTags from the " + collectionName + " list present in the scene");
        XMLManager.Instance.SwitchLanguage();
    }
}
