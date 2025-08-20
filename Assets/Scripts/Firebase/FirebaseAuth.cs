using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class FirebaseAuth : MonoBehaviour
{
    [Header("Firebase Config")]
    private string apiKey = "AIzaSyBB5GZXI2FlYMbfg_JH-FJU60Mj5zSVk5E";

    public static string UserIdToken;
    public static string UserLocalId;

    [System.Serializable]
    public class AuthRequest
    {
        public string email;
        public string password;
        public bool returnSecureToken = true;
    }

    [System.Serializable]
    public class AuthResponse
    {
        public string idToken;
        public string email;
        public string localId;
    }

    // Register
    public IEnumerator RegisterUser(string email, string password, Action<bool, string, string> callback)
    {
        string registerUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + apiKey;

        AuthRequest requestData = new AuthRequest { email = email, password = password };
        string jsonData = JsonUtility.ToJson(requestData);

        UnityWebRequest request = new UnityWebRequest(registerUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
            UserIdToken = authResponse.idToken;
            UserLocalId = authResponse.localId;
            callback(true, authResponse.idToken, authResponse.localId);
        }
        else
        {
            Debug.LogError("Register error: " + request.error + "\n" + request.downloadHandler.text);
            callback(false, null, null);
        }
    }

    // Login
    public IEnumerator LoginUser(string email, string password, Action<bool, string, string> callback)
    {
        string loginUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + apiKey;

        AuthRequest requestData = new AuthRequest { email = email, password = password };
        string jsonData = JsonUtility.ToJson(requestData);

        UnityWebRequest request = new UnityWebRequest(loginUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
            UserIdToken = authResponse.idToken;
            UserLocalId = authResponse.localId;
            callback(true, authResponse.idToken, authResponse.localId);
        }
        else
        {
            Debug.LogError("Login error: " + request.error + "\n" + request.downloadHandler.text);
            callback(false, null, null);
        }
    }
}
