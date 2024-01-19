using AgoraIO.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static BookingManager;

public class BookingTest : MonoBehaviour
{
    public Text bookingPageEmailText;
    public Text bookingPagePasswordText;
    public Button informationButton;
    private int playerId;
    private string accessToken;
    private int testId;
    private string loggedInUsername;
    public InputField usernameInputField;
    private NetworkingManager networkingManagerInstance; // Assuming you have an instance

    public GameObject detailPanel;
    public Button Thanhtoanbutton;
    public InputField moneyInputField;

    void Awake()
    {
        playerId = PlayerPrefs.GetInt("playerId");
        accessToken = PlayerPrefs.GetString("accessToken");
        Debug.Log("Player ID: " + playerId);
        Debug.Log("accessToken: " + accessToken);
        networkingManagerInstance = FindObjectOfType<NetworkingManager>();
        if (networkingManagerInstance == null)
        {
            Debug.LogError("NetworkingManager instance not found.");
            return;
        }

        // Handle the event
        networkingManagerInstance.OnLoginSuccess += OnLoginSuccessHandler;
        // Bind book button click event
        GameObject detailsPanel = GameObject.Find("DetailsPanel");
        informationButton.onClick.AddListener(OnInformationButtonClick);
        // Bind Thanhtoanbutton click event
        Thanhtoanbutton.onClick.AddListener(ThanhtoanButton_Click);
    }

    void Start()
    {
        InitializeDetailPanelData();
    }

    private void OnEnable()
    {
        if (networkingManagerInstance != null)
        {
            networkingManagerInstance.OnLoginSuccess += OnLoginSuccessHandler;
        }
    }

    private void OnDisable()
    {
        if (networkingManagerInstance != null)
        {
            networkingManagerInstance.OnLoginSuccess -= OnLoginSuccessHandler;
        }
    }

    public string LoggedInUsername
    {
        get { return loggedInUsername; }
        set
        {
            loggedInUsername = value;
        }
    }

    private async void OnLoginSuccessHandler(string email, string password)
    {
        // Set the user data in the booking panel
        bookingPageEmailText.text = email;
        bookingPagePasswordText.text = password;

        // Fetch the username using the API
        string apiUrl = "https://majormapperapi.azurewebsites.net/api/Account";

        try
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Debug.Log("API Response: " + json);

                    var accountData = JsonUtility.FromJson<AccountData>(json);
                    Debug.Log("Deserialized AccountData: " + JsonUtility.ToJson(accountData));

                    if (accountData.accounts != null && accountData.accounts.Count > 0)
                    {
                        var firstAccount = accountData.accounts[0];

                        // Set the username and playerId using the fetched data
                        LoggedInUsername = firstAccount.name;
                        usernameInputField.text = LoggedInUsername;

                        // Set the playerId for the video call
                        playerId = firstAccount.id;

                        // Store playerId in PlayerPrefs
                        PlayerPrefs.SetInt("playerId", playerId);
                        PlayerPrefs.Save(); // Ensure the PlayerPrefs changes are saved

                        // Update the channelName to reflect the correct playerId
                        PlayerPrefs.SetString("VideoCanvas", playerId.ToString());
                    }
                    else
                    {
                        Debug.LogError("No accounts found in the response.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error fetching account data: " + ex.Message);
        }
    }

    void OnInformationButtonClick()
    {
        // Set detail panel data and toggle visibility
        SetDetailPanelData();
    }

    void InitializeDetailPanelData()
    {
        moneyInputField.text = "20,000 VND";
    }

    void SetDetailPanelData()
    {
        // Set input fields in the DetailPanel
        InitializeDetailPanelData();
    }

    private void ThanhtoanButton_Click()
    {
        StartCoroutine(CreatePaymentUrl());
    }

    private IEnumerator CreatePaymentUrl()
    {
        // Create PaymentData object
        PaymentData paymentData = new PaymentData();
        paymentData.playerId = playerId;
        paymentData.testId = testId;

        // Convert data to JSON payload
        string jsonData = JsonUtility.ToJson(paymentData);
        Debug.Log($"Payment Request Payload: {jsonData}");

        // Create a request object
        using (var request = new UnityWebRequest(@"https://majormapperapi.azurewebsites.net/api/Payment/CreatePaymentUrl", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request
            yield return request.SendWebRequest();

            // Check for errors
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Payment URL created successfully!");

                // Deserialize the response to get the payment URL
                string paymentUrl = request.downloadHandler.text;
                Debug.Log("Payment URL: " + paymentUrl);

                // Open the URL in the system browser
                OpenURLCoroutine(paymentUrl);
            }
            else
            {
                Debug.LogError("Error creating payment URL: " + request.error);
            }
        }
    }

    private void OpenURLCoroutine(string url)
    {
        // Use Application.OpenURL to open the payment link in the system browser
        Application.OpenURL(url);
    }

    [Serializable]
    public class PaymentData
    {
        public int playerId;
        public int bookingId;
        public int testId;
        public string orderId;
        public string transactionId;
        public int amount;
        public string description;
        public string secureHash;
    }

    [Serializable]
    public class RootObject
    {
        public List<SlotData> slots;
    }

    [Serializable]
    public class AccountData
    {
        public List<Account> accounts;
    }

    [Serializable]
    public class Account
    {
        public int id;
        public string name;
        public string email;
    }
}
