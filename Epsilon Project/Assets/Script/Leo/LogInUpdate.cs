using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class LogInUpdate : MonoBehaviour
{
    
    public TextMeshProUGUI UsernameText;
    public TextMeshProUGUI PasswordText;

    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;

    public GameObject LogIncorrect;
    public GameObject LogCorrect;

    public GameObject CadreIncorrect;
    public GameObject CadreCorrect;

    public GameObject Register;

    private string Username;
    private string Password;

    TakeIdentityData data;
    public void Start()
    {
        Debug.Log("Start Log");
        data = SaveSystem.LoadTakeIdentity();
        Debug.Log(data);

        if (data != null)
        {
            Debug.Log("Log in");
            LoadIdentity();

            UsernameText.text = Username;
            PasswordText.text = Password;

            UsernameInput.interactable = false;
            PasswordInput.interactable = false;

            LogIncorrect.SetActive(false);
            LogCorrect.SetActive(true);

            CadreIncorrect.SetActive(false);
            CadreCorrect.SetActive(true);

            Register.GetComponent<Button>().interactable = false;
        }
        else 
        {
            Debug.Log("Register");

            UsernameInput.interactable = true;
            PasswordInput.interactable = true;

            LogIncorrect.SetActive(true);
            LogCorrect.SetActive(false);

            CadreIncorrect.SetActive(true);
            CadreCorrect.SetActive(false);

            Register.GetComponent<Button>().interactable = true;
        }
    }
    public void LoadIdentity()
    {
        TakeIdentityData data = SaveSystem.LoadTakeIdentity();

        Username = data.Username;
        Password = data.Password;
    }
    
}
