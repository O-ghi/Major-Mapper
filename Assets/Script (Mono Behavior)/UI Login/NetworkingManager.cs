using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static NetworkingManager;

public class NetworkingManager : MonoBehaviour
{
    public static NetworkingManager instance;
    public static Account player;
    public delegate void LoginSuccessEventHandler(string email, string password);
    public event LoginSuccessEventHandler OnLoginSuccess;
    private string baseURL = @"https://majormapperapi.azurewebsites.net/api/";


    //Reg
    public InputField UsernameInputField;
    public InputField EmailInputField;
    public InputField PasswordInputField;
    public InputField ConfirmPasswordInputField;
    public InputField SexInputField;
    public InputField AddressInputField;
    public InputField DateofBirthInputField;
    public InputField PhoneInputField;
    public InputField inputUsernameField;


    //Login
    public InputField LoginEmailInputField;
    public InputField LoginPasswordInputField;

    void Awake()
    {
        Debug.Log("NetworkingManager Awake");
        if (instance == null)
        {
            instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        GameObject.DontDestroyOnLoad(gameObject);
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

    public void OnLogInButton(/*string email, string password*/)
    {
        StartCoroutine(LogIn(/*email, password*/));
    }

    public void OnFakeData()
    {
        StartCoroutine(FakeTest());

    }
    public IEnumerator FakeTest()
    {
        var uwr = new UnityWebRequest(baseURL + "Test", "POST");
        string jsonString = "{\"playerId\":" + player.id + ",\"statusGame\":true,\"statusPayment\":true}";

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonString);

        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            Debug.Log("Success: " + uwr.downloadHandler.text);
        }

        TestQuestion testQuestion1 = new TestQuestion
        {
            testId = 1,
            questionId = 1,
            gameData = "string1",
            status = true
        };

        TestQuestion testQuestion2 = new TestQuestion
        {
            testId = 2,
            questionId = 2,
            gameData = "string2",
            status = false
        };

        TestQuestion testQuestion3 = new TestQuestion
        {
            testId = 3,
            questionId = 3,
            gameData = "string3",
            status = true
        };

        TestQuestion testQuestion4 = new TestQuestion
        {
            testId = 4,
            questionId = 4,
            gameData = "string4",
            status = false
        };

        TestQuestion testQuestion5 = new TestQuestion
        {
            testId = 5,
            questionId = 5,
            gameData = "string5",
            status = true
        };

        // Sử dụng các đối tượng TestQuestion
        string jsonString1 = JsonUtility.ToJson(testQuestion1);
        StartCoroutine(FakeTestQuestion(jsonString1));
        string jsonString2 = JsonUtility.ToJson(testQuestion2);
        StartCoroutine(FakeTestQuestion(jsonString2));
        string jsonString3 = JsonUtility.ToJson(testQuestion3);
        StartCoroutine(FakeTestQuestion(jsonString3));
        string jsonString4 = JsonUtility.ToJson(testQuestion4);
        StartCoroutine(FakeTestQuestion(jsonString4));
        string jsonString5 = JsonUtility.ToJson(testQuestion5);
        StartCoroutine(FakeTestQuestion(jsonString5));
    }
    public IEnumerator FakeTestQuestion(string jsonString)
    {

        var uwr = new UnityWebRequest(baseURL + "TestQuestion", "POST");

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonString);

        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            Debug.Log("Success: " + uwr.downloadHandler.text);
        }

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

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            Debug.Log("Success: " + uwr.downloadHandler.text);
        }
    }

    public IEnumerator LogIn(/*string email, string password*/)
    {
        string email = LoginEmailInputField.text;
        string password = LoginPasswordInputField.text;

        // Check email syntax
        if (!IsValidEmail(email))
        {
            // Show a panel or display a message indicating login failure
            Debug.Log("Login failed. Invalid email syntax.");
            // You may want to display an error panel or a message to the player here.
            yield break; // Exit the LogIn method if email syntax is invalid.
        }

        LoginObject loginObject = new LoginObject();
        loginObject.email = LoginEmailInputField.text;
        loginObject.password = LoginPasswordInputField.text;

        string jsonData = JsonUtility.ToJson(loginObject);

        string jsonRequestBody = "{\"email\":\"" + LoginEmailInputField.text + "\",\"password\":\"" + LoginPasswordInputField.text + "\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest uwr = new UnityWebRequest(baseURL + "Account/Login", "POST");
        uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);

        uwr.SetRequestHeader("Content-Type", "application/json");

        // Explicitly set a download handler
        uwr.downloadHandler = new DownloadHandlerBuffer();

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.error != null)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            Debug.Log("Success: " + uwr.downloadHandler.text);

            foreach (var response in uwr.GetResponseHeaders())
            {
                Debug.Log(response.Key.ToString() + " | " + response.Value.ToString());
            }
            if (!string.IsNullOrEmpty(uwr.downloadHandler.text))
            {
                player = JsonUtility.FromJson<Account>(uwr.downloadHandler.text);
                if (player != null)
                {
                    PlayerPrefs.SetInt("playerId", player.id); // Replace "id" with your actual property name
                    PlayerPrefs.SetString("accessToken", player.accessToken);
                    UnityEngine.SceneManagement.SceneManager.LoadScene("UI Booking");

                    // Call OnLoginSuccess event
                    OnLoginSuccess?.Invoke(player.email, player.password);
                }
                else
                {
                    Debug.LogError("Failed to parse user object from server response.");
                }
            }
            else
            {
                Debug.LogError("Empty response received from the server.");
            }
        }
    }
    private bool IsValidEmail(string email)
    {
        // You can use a simple regular expression for basic email validation
        string emailPattern = @"^[a-zA-Z0-9_.+-]+@gmail\.com$";
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(emailPattern);
        return regex.IsMatch(email);
    }


    [Serializable]
    public class LoginObject
    {
        public string email;
        public string password;
    }
    [System.Serializable]
    public class TestQuestion
    {
        public int testId;
        public int questionId;
        public string gameData;
        public bool status;
    }
}