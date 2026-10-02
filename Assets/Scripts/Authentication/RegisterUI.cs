using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class RegisterUI : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField confirmPasswordInput;

    [Header("Buttons")]
    [SerializeField] private Button createAccountButton;
    [SerializeField] private Button goToLoginButton;

    [Header("Feedback")]
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private GameObject successPanel;
    [SerializeField] private TextMeshProUGUI successText;
    [SerializeField] private Button continueButton;

    private void Start()
    {
        createAccountButton.onClick.AddListener(OnCreateAccountClicked);
        goToLoginButton.onClick.AddListener(OnGoToLoginClicked);
        continueButton.onClick.AddListener(OnContinueClicked);

        errorPanel.SetActive(false);
        successPanel.SetActive(false);
    }

    private async void OnCreateAccountClicked()
    {
        string username = usernameInput.text.Trim();
        string email = emailInput.text.Trim();
        string password = passwordInput.text;
        string confirm = confirmPasswordInput.text;

        // Validation côté client
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirm))
        {
            ShowError("ALL FIELDS ARE REQUIRED");
            return;
        }

        if (!IsValidEmail(email))
        {
            ShowError("INVALID EMAIL FORMAT");
            return;
        }

        if (password.Length < 6)
        {
            ShowError("PASSWORD MUST BE AT LEAST 6 CHARACTERS");
            return;
        }

        if (password != confirm)
        {
            ShowError("PASSWORDS DO NOT MATCH");
            return;
        }

        createAccountButton.interactable = false;

        var response = await ApiClient.Instance.RegisterAsync(username, email, password);

        createAccountButton.interactable = true;

        if (response.success)
        {
            AuthManager.Instance.SaveSession(response.token, response.username);
            ShowSuccess("ACCOUNT CREATED ✓\nWelcome to the Internet.");
        }
        else
        {
            ShowError(response.message.ToUpper());
        }
    }

    private void OnGoToLoginClicked()
    {
        
        Debug.Log("Go to Login (étape suivante)");
        // SceneLoader.Load("Login");
    }

    private void OnContinueClicked()
    {
        
        Debug.Log("Continue → Home (étape suivante)");
        // SceneLoader.Load("Home");
    }

    private void ShowError(string message)
    {
        errorText.text = message;
        errorPanel.SetActive(true);
        successPanel.SetActive(false);
    }

    private void ShowSuccess(string message)
    {
        successText.text = message;
        successPanel.SetActive(true);
        errorPanel.SetActive(false);
    }

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}