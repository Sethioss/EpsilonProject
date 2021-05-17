using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChoosePath : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckPath(bool correctPath)
    {
        if (correctPath)
        {
            Debug.Log("You found the correct path. Congrats !");
            DialogueManager.Instance.currentDialogueFile = (TextAsset)Resources.Load("Tables\\" + 4);
            SceneManager.LoadScene(0);
        }
        else
        {
            Debug.Log("Booh ! That's wrong");
        }
    }
}
