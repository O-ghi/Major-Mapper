using UnityEngine;
using UnityEngine.UI;

public class DetailManager : MonoBehaviour
{
    public InputField slotIdInputField;
    public InputField usernameInputField;
    public InputField consultantNameInputField;
    public InputField dateInputField;
    public InputField timeInputField;

    public Button comeBackButton;

    // Reference to BookingManager (adjust based on your project structure)
    public BookingManager bookingManager;

    void Start()
    {
        comeBackButton.onClick.AddListener(OnComeBackButtonClick);
    }

    public void SetBookingDetails(long slotId, string username, string consultantName, string date, string time)
    {
        slotIdInputField.text = slotId.ToString();
        usernameInputField.text = username;
        consultantNameInputField.text = consultantName;
        dateInputField.text = date;
        timeInputField.text = time;
    }

    public void OnComeBackButtonClick()
    {
        bookingManager.gameObject.SetActive(true);
        this.gameObject.SetActive(false); 
    }
}