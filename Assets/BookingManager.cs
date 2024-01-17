using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Agora.Rtc;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AgoraIO.Media;
using Agora_RTC_Plugin.API_Example;
using Unity.VisualScripting;


public class BookingManager : MonoBehaviour
{
    public Text bookingPageEmailText;
    public Text bookingPagePasswordText;
    public InputField dateInput; // Input field for year/month/day
    public Dictionary<int, string> consultantNames = new Dictionary<int, string>();
    // Dropdown menu to display available slots (initially empty)
    public Button BackButton;
    public Button ForwardButton;
    [SerializeField] Dropdown slotDropdown;
    [SerializeField] Button bookButton;
    public Button ShowDetailsButton;
    public GameObject notificationPanel;
    public Text notificationText;
    public GameObject failureNotificationPanel;
    public Text failureNotificationText;
    public Button Thanhtoanbutton;
    public GameObject paymentPanel;
    public InputField paymentLinkText;
    public Button videoCallButton;

    // List to store fetched slot data
    private List<SlotData> slots;
    private int playerId;
    private string accessToken;
    private long bookedSlotId;
    private string bookedDate;
    private string bookedTime;
    private string loggedInUsername;
    private NetworkingManager networkingManagerInstance; // Assuming you have an instance

    public GameObject detailPanel;
    public InputField moneyInputField;
    public InputField usernameInputField;
    public InputField consultantNameInputField;
    public InputField dateInputField;
    public InputField timeInputField;

    public static string appId;
    public static string channelName;
    public static string token;
    public GameObject AppInputConfig;
    public GameObject CaseScrollerView;
    public GameObject CasePanel;
    public GameObject EventSystem;

    private string _playSceneName = "";


    private string[] _baseSceneNameList = {
        "VideoCanvas"
    };

    // API URLs
    string GetSlotsUrl = @"https://majormapperapi.azurewebsites.net/api/Slot/GetSlotActive";
    string BookingUrl = @"https://majormapperapi.azurewebsites.net/api/Booking";

    void Awake()
    {
        // Get player ID from PlayerPrefs
        playerId = PlayerPrefs.GetInt("playerId");
        accessToken = PlayerPrefs.GetString("accessToken");
        Debug.Log("Player ID: " + playerId);
        Debug.Log("accessToken: " + accessToken);

        // Handle dropdown selection change
        slotDropdown.onValueChanged.AddListener(SlotDropdown_ValueChanged);
        bookButton.onClick.AddListener(BookButton_Click);

        // Bind book button click event
        GameObject detailsPanel = GameObject.Find("DetailsPanel");
        FetchSlotData(string.Empty);

        consultantNames.Add(6, "Truong Minh Ngoc");
        consultantNames.Add(5, "Nguyen Thi Kieu Trinh");
        slotDropdown.ClearOptions(); // Clear options initially
        BackButton.onClick.AddListener(OnBackButtonClick);
        ForwardButton.onClick.AddListener(OnForwardButtonClick);
        // Bind Thanhtoanbutton click event
        Thanhtoanbutton.onClick.AddListener(ThanhtoanButton_Click);

        // Monitor input field changes
        dateInput.onValueChanged.AddListener(OnDateChanged);
        // Add event listener for "Enter" key press
        dateInput.onEndEdit.AddListener(OnDateInputEndEdit);
        networkingManagerInstance = FindObjectOfType<NetworkingManager>();
        if (networkingManagerInstance == null)
        {
            Debug.LogError("NetworkingManager instance not found.");
            return;
        }

        // Handle the event
        networkingManagerInstance.OnLoginSuccess += OnLoginSuccessHandler;

        videoCallButton.onClick.AddListener(OnVideoCallButtonClick);
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




    void OnDateChanged(string newDate)
    {
        if (!IsValidDate(newDate))
        {
            return;
        }

        FetchSlotData(newDate);
    }

    async void FetchSlotData(string date)
    {
        string apiUrl = $"https://majormapperapi.azurewebsites.net/api/Slot/GetSlotActive?date={date}";

        try
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                Debug.Log("Request Headers: " + string.Join(", ", client.DefaultRequestHeaders.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}")));
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    try
                    {
                        json = "{\"slots\":" + json + "}";
                        Debug.Log("Raw JSON: " + json);

                        // Deserialize the entire list of slots
                        List<SlotData> allSlots = JsonUtility.FromJson<RootObject>(json).slots;

                        // Filter the slots for the specific date
                        slots = allSlots.Where(slot => slot.startDateTime.StartsWith(date)).ToList();

                        // Populate the dropdown with the filtered slots
                        PopulateDropdown(slots);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Error deserializing JSON: " + ex.Message);
                    }
                }
                else
                {
                    Debug.LogError("Error getting slots: " + response.StatusCode);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error fetching slot data: " + ex.Message);
            // Handle other potential exceptions
        }
    }


    void PopulateDropdown(List<SlotData> slots)
    {
        slotDropdown.options.Clear();
        List<SlotData> slotData = new List<SlotData>();

        foreach (var slot in slots)
        {
            var exits = false;
            for (var j = 0; j < slotData.Count; j++)
            {
                if (slotData[j].startDateTime == slot.startDateTime)
                {
                    exits = true;
                    break;
                }
            }
            if (!exits)
            {
                slotData.Add(slot); 
            }
        }
        foreach(var slot in slotData)
        {
            string time = slot.startDateTime.Substring(11, 8);

            Dropdown.OptionData option = new Dropdown.OptionData($"{time} giờ");
            slotDropdown.options.Add(option);
        }
        slotDropdown.RefreshShownValue();
    }


    public bool IsValidDate(string date)
    {
        DateTime parsedDate;
        return DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
    }

    public void OnBackButtonClick()
    {
        string currentDate = dateInput.text;
        DateTime parsedDate;
        if (DateTime.TryParseExact(currentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        {
            DateTime previousDate = parsedDate.AddDays(-1);
            string previousDateString = previousDate.ToString("yyyy-MM-dd");
            dateInput.text = previousDateString;
            FetchSlotData(previousDateString);
        }
    }

    public void OnForwardButtonClick()
    {
        string currentDate = dateInput.text;
        DateTime parsedDate;
        if (DateTime.TryParseExact(currentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        {
            DateTime nextDate = parsedDate.AddDays(1);
            string nextDateString = nextDate.ToString("yyyy-MM-dd");
            dateInput.text = nextDateString;
            FetchSlotData(nextDateString);
        }
    }

    void OnDateInputEndEdit(string newDate)
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            if (IsValidDate(newDate))
            {
                FetchSlotData(newDate);
            }
            else
            {
                Debug.LogError("Invalid date format. Please enter a valid date in the format yyyy-MM-dd.");
            }
        }
    }



    private void OnValidate()
    {
        slotDropdown.ClearOptions();
    }

    public void BookButton_Click()
    {
        if (slotDropdown.value == -1 || !IsValidDate(dateInput.text))
        {
            Debug.LogError("Please select a slot and enter a valid date before booking.");

            // Show failure notification
            ShowFailureNotification();

            return;
        }

        StartCoroutine(BookSlot());
    }

    public IEnumerator BookSlot()
    {
        if (slotDropdown.value == -1 || !IsValidDate(dateInput.text))
        {
            Debug.LogError("Please select a slot and enter a valid date before booking.");

            // Show failure notification
            ShowFailureNotification();

            yield break; // Make sure to exit the coroutine on failure
        }

        int selectedIndex = slotDropdown.value;

        if (selectedIndex < 0 || selectedIndex >= slots.Count)
        {
            Debug.LogError("Invalid slot selection.");
            ShowFailureNotification(); // Show failure notification for invalid slot selection
            yield break;
        }

        SlotData selectedSlot = slots[selectedIndex];
        long selectedSlotId = selectedSlot.id;

        Debug.Log($"Player ID: {playerId}, Selected Slot ID: {selectedSlotId}");

        // Build BookingData object
        BookingData bookingData = new BookingData();
        bookingData.playerId = playerId;
        bookingData.slotId = selectedSlotId;

        // Convert data to JSON payload
        string jsonData = JsonUtility.ToJson(bookingData);
        Debug.Log($"Request Payload: {jsonData}");

        // Create a request object
        using (var request = new UnityWebRequest(BookingUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);
            // Send the request
            yield return request.SendWebRequest();

            // Check for errors
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Booking successful!");

                // Deserialize the response to get the booking ID
                var bookingIdJson = JsonUtility.FromJson<BookingIdData>(request.downloadHandler.text);
                bookedSlotId = bookingIdJson.id;

                // Show success notification
                ShowNotification();

                // Store booking details
                bookedDate = dateInput.text;
                bookedTime = selectedSlot.startDateTime.Substring(11, 8);

                // Display details in the DetailPanel
                SetDetailPanelData();
            }
            else
            {
                Debug.LogError("Booking failed: " + request.error);

                // Show failure notification
                ShowFailureNotification();
            }
        }
    }

    void SetDetailPanelData()
    {
        // Set input fields in the DetailPanel
        moneyInputField.text = "100,000 VND";
        dateInputField.text = bookedDate;
        timeInputField.text = bookedTime;
    }

    void ShowNotification()
    {
        notificationPanel.SetActive(true);
    }

    void ShowFailureNotification()
    {
        failureNotificationPanel.SetActive(true);
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
        paymentData.bookingId = (int)bookedSlotId;

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

                // Display the payment link in the paymentPanel
                paymentLinkText.text = paymentUrl;

                // Open the URL in the system browser
                OpenURLCoroutine(paymentUrl);
            }
            else
            {
                Debug.LogError("Error creating payment URL: " + request.error);
            }
        }
    }

    public void OpenPaymentLink()
    {
        string paymentUrl = paymentLinkText.text;

        // Open the URL in the default web browser
        Application.OpenURL(paymentUrl);
    }

    private void OpenURLCoroutine(string url)
    {
        // Use Application.OpenURL to open the payment link in the system browser
        Application.OpenURL(url);
    }

    private void OnVideoCallButtonClick()
    {
        // Retrieve playerId from PlayerPrefs
        playerId = PlayerPrefs.GetInt("playerId", 0);

        // Set up video call parameters
        string appId = "32f662b1d5cf4a50bbf47cd0ba9bfcd5";
        string channelName = playerId.ToString();

        // Generate token
        string token = GenerateToken(channelName);

        // Store parameters for retrieval in the VideoCanvas scene
        PlayerPrefs.SetString("VideoCallAppId", appId);
        PlayerPrefs.SetString("VideoCallToken", token);
    }

    public string GenerateToken(string channelName)
    {
        string appId = "32f662b1d5cf4a50bbf47cd0ba9bfcd5";
        string appCertificate = "b1f5ac0e01f04a58a3fb5f6c43b903c4";
        uint uid = 0;
        uint expirationTimeInSeconds = 3600;
        uint currentTimeStamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        uint privilegeExpiredTs = currentTimeStamp + expirationTimeInSeconds;
        RtcTokenBuilder.Role role = RtcTokenBuilder.Role.RolePublisher;

        string token = RtcTokenBuilder.buildTokenWithUID(appId, appCertificate, channelName, uid, role, privilegeExpiredTs);

        return token;
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
    public class BookingIdData
    {
        public long id;
    }


    [Serializable]
    public class SlotData
    {
        public int id;
        public int consultantId;
        public string startDateTime;
        public string endDateTime;
        public string status;
        public string createDateTime;
    }


    public class BookingData
    {
        public int playerId;
        public long slotId;
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


    private void SlotDropdown_ValueChanged(int index)
    {
        //long parsedSlotId;
        //parsedSlotId = slots[index].id;
        //Debug.Log("parsedSlotId " + parsedSlotId);
        Debug.Log("Dropdown Value Changed: " + index);
    }
    public void OnShowDetailsClick()
    {
        ToggleDetailPanel(true);
    }
    void ToggleDetailPanel(bool show)
    {
        detailPanel.SetActive(show);
    }
}


