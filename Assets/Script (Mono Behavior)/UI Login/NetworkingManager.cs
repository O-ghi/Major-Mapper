using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkingManager : MonoBehaviour
{
    public static NetworkingManager instance;
    public static LogIn logIn;
    public string baseURL = @"https://majormapper.azurewebsites.net/api/";

    //Reg
    public InputField UsernameInputField;
    public InputField EmailInputField;
    public InputField PasswordInputField;
    public InputField ConfirmPasswordInputField;
    public InputField SexInputField;
    public InputField AddressInputField;
    public InputField DateofBirthInputField;
    public InputField PhoneInputField;

    //Login
    public InputField LoginEmailInputField;
    public InputField LoginPasswordInputField;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnRegButton()
    {
        Register tempReg = new Register()
        {
            name = UsernameInputField.text,
            email = EmailInputField.text,
            password = PasswordInputField.text,
            confirmPassword = ConfirmPasswordInputField.text,
            gender = SexInputField.text,
            role = 3,
            address = AddressInputField.text,
            doB = DateofBirthInputField.text,
            phone = PhoneInputField.text
        };
        StartCoroutine(Register(tempReg));
    }

    public void OnLogInButton()
    {
        StartCoroutine(LogIn());
    }

    public IEnumerator Register(Register register)
    {
        var uwr = new UnityWebRequest(baseURL + "Account", "POST");
        string jsonData = JsonUtility.ToJson(register);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if(uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            Debug.Log("Success: " + uwr.downloadHandler.text);
        }
    }

    public IEnumerator LogIn()
    {
        WWWForm form = new WWWForm();

        form.AddField("grant_type", "password");
        form.AddField("username", LoginEmailInputField.text);
        form.AddField("password", LoginPasswordInputField.text);

        UnityWebRequest uwr = UnityWebRequest.Post(baseURL + "Account/Login", form);

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            Debug.Log("Success: " + uwr.downloadHandler.text);

            logIn = JsonUtility.FromJson<LogIn>(uwr.downloadHandler.text);
        }
    }
}
