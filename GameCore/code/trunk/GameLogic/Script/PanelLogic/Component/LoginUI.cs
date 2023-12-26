using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginUI
{
    public LoginPanel _LoginPanel;

    public Text Email;
    public Text Password;

    public TriggerListener SubmitButton;
    public TriggerListener RegisterButton;

    
    public void Init()
    {
        Email = _LoginPanel.transform.Find("Input/Email/Text").GetOrAddComponent<Text>();
        Password = _LoginPanel.transform.Find("Input/Password/Text").GetOrAddComponent<Text>();

        SubmitButton = _LoginPanel.transform.Find("Button/SubmitButton").GetOrAddComponent<TriggerListener>();
        RegisterButton = _LoginPanel.transform.Find("Button/RegisterButton").GetOrAddComponent<TriggerListener>();

    }
}
