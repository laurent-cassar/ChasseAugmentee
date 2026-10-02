using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginUI : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField loginInput;      // username ou email
    [SerializeField] private TMP_InputField passwordInput;

    [Header("Buttons")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button goToRegisterButton;

    [Header("Feedback")]
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TextMeshProUGUI errorText;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        goToRegisterButton.onClick.AddListener(OnGoToRegisterClicked);

        errorPanel.SetActive(false);
    }

    private async void OnLoginClicked()
    {
        string login = loginInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            ShowError("ALL FIELDS ARE REQUIRED");
            return;
        }

        loginButton.interactable = false;

        var response = await ApiClient.Instance.LoginAsync(login, password);

        loginButton.interactable = true;

        if (response.success)
        {
            AuthManager.Instance.SaveSession(response.token, response.username);
            Debug.Log("Login réussi → " + response.username);
        }
        else
        {
            ShowError("LOGIN FAILED\nWrong username or password.");
        }
    }

    private void OnGoToRegisterClicked()
    {
        SceneLoader.Load("Register");
    }

    private void ShowError(string message)
    {
        errorText.text = message;
        errorPanel.SetActive(true);
    }
}