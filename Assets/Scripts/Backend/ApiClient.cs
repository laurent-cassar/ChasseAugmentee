using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class RegisterRequest
{
    public string username;
    public string email;
    public string password;
}

[Serializable]
public class LoginRequest
{
    public string login;      // username ou email
    public string password;
}

[Serializable]
public class AuthResponse
{
    public bool success;
    public string message;
    public string token;
    public string username;
}

public class ApiClient : MonoBehaviour
{
    public static ApiClient Instance { get; private set; }

    [Header("Backend")]
    [SerializeField] private string baseUrl = "http://localhost:3000/api";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async Task<AuthResponse> RegisterAsync(string username, string email, string password)
    {
        var requestData = new RegisterRequest
        {
            username = username,
            email = email,
            password = password
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] body = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}/register", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                return new AuthResponse
                {
                    success = false,
                    message = "NETWORK ERROR\nCould not reach the server."
                };
            }

            try
            {
                return JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
            }
            catch
            {
                return new AuthResponse
                {
                    success = false,
                    message = "INVALID SERVER RESPONSE"
                };
            }
        }
    }

    public async Task<AuthResponse> LoginAsync(string login, string password)
    {
        var requestData = new LoginRequest
        {
            login = login,
            password = password
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] body = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}/login", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                return new AuthResponse
                {
                    success = false,
                    message = "NETWORK ERROR\nCould not reach the server."
                };
            }

            try
            {
                return JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
            }
            catch
            {
                return new AuthResponse
                {
                    success = false,
                    message = "INVALID SERVER RESPONSE"
                };
            }
        }
    }
}