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
        WwiseSoundManager.instance.Click.Post(gameObject);
        //Mettre le son WWise Suivant : Click
    }

    public void LoadNextScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }

    

    public void SetNextDIalogue(TextAsset data)
    {
        DialogueManager.Instance.currentDialogueFile = data;
    }

    public void ReactivateCamera(GameObject cameraHacked)
    {
        StartCoroutine(CameraFlicker(cameraHacked));
        cameraHacked.transform.position = cameraHacked.GetComponent<CameraData>().originalPosition;
        cameraHacked.transform.rotation= cameraHacked.GetComponent<CameraData>().originalRotation;

    }

    IEnumerator CameraFlicker(GameObject cameraHacked)
    {
        //Mettre le son WWise Suivant : Flicker
        WwiseSoundManager.instance.Flicker.Post(gameObject);
        yield return new WaitForSeconds(.5f);
        cameraHacked.SetActive(true);
    }

}
