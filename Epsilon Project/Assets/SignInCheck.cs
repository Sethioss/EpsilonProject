using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SignInCheck : MonoBehaviour
{
    public TMP_InputField inputFieldUsername;
    public TMP_InputField inputFieldPassword;
    public TMP_InputField inputFieldPhone;
    public Button LogIn;
    public bool[] check;
    void Update()
    {
        
        if (inputFieldUsername.text.Length > 2)
        {
            check[0]=true;
        }
        else
        {
            check[0] = false;
        }
        if (inputFieldPassword.text.Length > 6)
        {
            check[1] = true;
        }
        else
        {
            check[1] = false;
        }
        if (inputFieldPhone.text.Length == 10)
        {
            check[2] = true;
        }
        else
        {
            check[2] = false;
        }
        if (check[0] == true & check[1] == true & check[2] == true)
        {
            LogIn.interactable = true;
        }
        else
        {
            LogIn.interactable = false;
        }
    }
}
