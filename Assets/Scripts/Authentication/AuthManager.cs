using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    public string CurrentUsername { get; private set; }
    public string AuthToken { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(AuthToken);

    private const string TOKEN_KEY = "auth_token";
    private const string USERNAME_KEY = "auth_username";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Restaurer la session si elle existe
        AuthToken = PlayerPrefs.GetString(TOKEN_KEY, "");
        CurrentUsername = PlayerPrefs.GetString(USERNAME_KEY, "");
    }

    public void SaveSession(string token, string username)
    {
        AuthToken = token;
        CurrentUsername = username;
        PlayerPrefs.SetString(TOKEN_KEY, token);
        PlayerPrefs.SetString(USERNAME_KEY, username);
        PlayerPrefs.Save();
    }

    public void ClearSession()
    {
        AuthToken = "";
        CurrentUsername = "";
        PlayerPrefs.DeleteKey(TOKEN_KEY);
        PlayerPrefs.DeleteKey(USERNAME_KEY);
        PlayerPrefs.Save();
    }
}