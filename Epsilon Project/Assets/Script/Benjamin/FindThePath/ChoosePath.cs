using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }
        else
        {
            Debug.Log("Booh ! That's wrong");
        }
    }
}
