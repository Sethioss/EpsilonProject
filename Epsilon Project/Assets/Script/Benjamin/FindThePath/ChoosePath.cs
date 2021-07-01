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
            //Mettre le son WWise Suivant : Click
            WwiseSoundManager.instance.Click.Post(gameObject);
            Debug.Log("You found the correct path. Congrats !");
            MinigameManager.Instance.winAction.Invoke();
        }
        else
        {
            //Mettre le son WWise Suivant : Error
            WwiseSoundManager.instance.errorSound.Post(gameObject);
            Debug.Log("Booh ! That's wrong");
        }
    }
}
