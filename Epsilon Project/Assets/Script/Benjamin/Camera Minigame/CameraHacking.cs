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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeactivateCamera(GameObject target)
    {
        target.SetActive(false);
        ReactivateCamera(target);
    }

    public void LoadNextScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }

    

    public void SetNextDIalogue(TextAsset data)
    {
        DialogueManager.Instance.currentDialogueFile = data;
    }

    public IEnumerator ReactivateCamera(GameObject cameraHacked)
    {
        yield return new WaitForSeconds(3);
        cameraHacked.SetActive(true);
    }

}
