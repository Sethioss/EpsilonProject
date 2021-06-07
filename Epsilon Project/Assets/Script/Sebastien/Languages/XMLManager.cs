using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;

public class XMLManager : MonoBehaviour
{
    public TextAsset languageFile;

    private List<Dictionary<string, string>> dictionary = new List<Dictionary<string, string>>();
    private Dictionary<string, string> tempObj;

    private List<XMLTagList> xmlTagLists;
    private List<XMLTextTag> xmlTextTags;

    private XMLTextTag hourTextTag;

    private static XMLManager instance;
    public static XMLManager Instance
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
        else
        {
            Destroy(this.gameObject);
        }

        CreateDictionary();
    }

    public void GetSceneXMLTags()
    {
        xmlTagLists = new List<XMLTagList>();
        xmlTextTags = new List<XMLTextTag>();

        Debug.Log("Finding new XMLTags");
        List<XMLTagList> sceneTagLists = new List<XMLTagList>(GameObject.FindObjectsOfType<XMLTagList>());
        xmlTagLists = sceneTagLists;

        foreach (XMLTagList list in xmlTagLists)
        {
            foreach (XMLTextTag tag in list.tagList)
            {
                xmlTextTags.Add(tag);
            }
        }
    }

    private void CreateDictionary()
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(languageFile.text);

        XmlNodeList languages = doc.GetElementsByTagName("language");

        foreach (XmlNode language in languages)
        {
            XmlNodeList languageValues = language.ChildNodes;
            tempObj = new Dictionary<string, string>();

            foreach (XmlNode languageKey in languageValues)
            {
                tempObj.Add(languageKey.Name, languageKey.InnerText);
            }

            dictionary.Add(tempObj);
        }
    }

    public void SwitchLanguage()
    {
        int languageId = (int)UserSettings.Instance.language;
        dictionary[languageId].TryGetValue("Prefix", out UserSettings.Instance.languagePrefix);

        foreach (XMLTextTag xmlTag in xmlTextTags)
        {
            switch ((int)xmlTag.dynamicID)
            {
                case 0:
                    try
                    {
                        xmlTag.tagTxtObject.text = dictionary[languageId][xmlTag.tagName];
                    }
                    catch
                    {
                        Debug.Log(xmlTag.tagName + " Couldn't find its textMeshPro component. If there are none in the scene, please consider removing it from XMLTagList");
                    }
                    break;

                case 1:
                    try
                    {
                        string initialStr = dictionary[languageId][xmlTag.tagName];
                        xmlTag.tagTxtObject.text = GetHourString(initialStr);
                        hourTextTag = xmlTag;
                    }
                    catch
                    {
                        Debug.Log(xmlTag.tagName + " Couldn't find its textMeshPro component. If there are none in the scene, please consider removing it from XMLTagList");
                    }

                    break;
            }
        }
    }

    public void UpdateHour()
    {
        string initialStr = dictionary[(int)UserSettings.Instance.language][hourTextTag.tagName];
        hourTextTag.tagTxtObject.text = GetHourString(initialStr);
    }

    private string GetHourString(string processedString)
    {
        string inactiveHourStart;
        string inactiveHourEnd;

        inactiveHourStart = GetHour(UserSettings.Instance.inactivePeriodStartHour);
        inactiveHourEnd = GetHour(UserSettings.Instance.inactivePeriodEndHour);

        string newString = string.Format(processedString, inactiveHourStart, inactiveHourEnd);
        return newString;
    }

    private string GetHour(int value)
    {
        string hour = "";

        if ((int)UserSettings.Instance.language == 1)
        {
            hour = value + ".00AM";

            if (value > 12)
            {
                hour = (value % 12) + ".00PM";
            }

            return hour;
        }

        else
        {
            return value + ".00";
        }

    }
}
