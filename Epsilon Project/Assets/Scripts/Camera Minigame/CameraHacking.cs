using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraHacking : MonoBehaviour
{
    public GameObject hackingMenu;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeactivateCamera(GameObject target)
    {
        target.SetActive(false);
        hackingMenu.SetActive(false);
        Time.timeScale = 1;
        
    }

    public void LoadNextScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
}
