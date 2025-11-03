using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using UnityEngine.SocialPlatforms.Impl;

public class FirebaseAuth : MonoBehaviour
{
    [Header("Firebase Config")]
    private string apiKey = "AIzaSyBB5GZXI2FlYMbfg_JH-FJU60Mj5zSVk5E";

    public static string UserIdToken;
    public static string UserLocalId;
    public PlayerData playerData;

    [System.Serializable]
public class AccountInfoResponse
{
    public UserInfo[] users;
}

    [System.Serializable]
    public class UserInfo
    {
        public bool emailVerified;
    }


[System.Serializable]
public class VerifyEmailRequest
{
    public string requestType;
    public string idToken;
}


public IEnumerator CheckEmailVerified(string idToken, Action<bool> callback)
{
    if (string.IsNullOrEmpty(idToken))
    {
        Debug.LogError("❌ CheckEmailVerified called with EMPTY idToken!");
        callback(false);
        yield break;
    }

    string url = "https://identitytoolkit.googleapis.com/v1/accounts:lookup?key=" + apiKey;

    // --- CHANGE STARTS HERE ---
    LookupAccountRequest requestData = new LookupAccountRequest { idToken = idToken };
    string jsonData = JsonUtility.ToJson(requestData);
    // --- CHANGE ENDS HERE ---

    UnityWebRequest request = new UnityWebRequest(url, "POST");
    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");

    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
        var info = JsonUtility.FromJson<AccountInfoResponse>(request.downloadHandler.text);
        bool verified = info.users[0].emailVerified;
        callback(verified);
    }
    else
    {
        Debug.LogError("Error checking verification: " + request.downloadHandler.text);
        callback(false);
    }
}





    // Register
    public IEnumerator RegisterUser(string email, string password, string name, int age, int gradeLevel, Action<bool, string, string> callback)
    {

        if (DataManager.Instance == null)
        {
            Debug.LogError("❌ DataManager is not ready yet!");
            yield break;
        }

        if (DataManager.Instance.playerData == null)
        {
            DataManager.Instance.playerData = new PlayerData();
            DataManager.Instance.InitPlayerData(); // Make InitPlayerData public
        }


        string registerUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + apiKey;


        RegisterRequest requestData = new RegisterRequest { email = email, password = password };
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

            // Initialize player data
            DataManager.Instance.playerData.playerId = UserLocalId;
            DataManager.Instance.playerData.playerName = name;
            DataManager.Instance.playerData.email = email;
            DataManager.Instance.playerData.age = age;
            DataManager.Instance.playerData.gradeLevel = gradeLevel;

            // Save to DB as PlayerData (not separate profile)
            yield return DataManager.Instance.StartCoroutine(
                FindObjectOfType<FirebaseDatabase>().SaveData(UserIdToken, UserLocalId, DataManager.Instance.playerData)
            );

            Debug.Log("✅ Registered and PlayerData saved!");
            callback(true, authResponse.idToken, authResponse.localId);
        }

        else
        {
            Debug.LogError("❌ Register error: " + request.error + "\n" + request.downloadHandler.text);
            callback(false, null, null);
        }
    }


public IEnumerator SendEmailVerification(string idToken, System.Action<bool, string> callback)
{
    string url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}";

    // ✅ Use a real serializable class
    VerifyEmailRequest payload = new VerifyEmailRequest
    {
        requestType = "VERIFY_EMAIL",
        idToken = idToken
    };

    string jsonData = JsonUtility.ToJson(payload);
    Debug.Log("SendEmailVerification payload: " + jsonData); // for debugging

    using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Verification email sent!");
            callback?.Invoke(true, "Verification email sent successfully!");
        }
        else
        {
            string errorMsg = request.downloadHandler.text;
            Debug.LogError($"❌ Failed to send verification email: {errorMsg}");
            callback?.Invoke(false, errorMsg);
        }
    }
}


public IEnumerator LoginUser(string email, string password, Action<bool, string, string> callback)
{
    string loginUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + apiKey;

    RegisterRequest requestData = new RegisterRequest
    {
        email = email,
        password = password,
        returnSecureToken = true // ✅ must be included
    };

    string jsonData = JsonUtility.ToJson(requestData);
    Debug.Log("Login payload: " + jsonData);

    UnityWebRequest request = new UnityWebRequest(loginUrl, "POST");
    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");

    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
        Debug.Log("Login response: " + request.downloadHandler.text);
        AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);

        if (string.IsNullOrEmpty(authResponse.idToken))
        {
            Debug.LogError("❌ Login success but idToken missing!");
            callback(false, null, null);
            yield break;
        }

        UserIdToken = authResponse.idToken;
        UserLocalId = authResponse.localId;

        Debug.Log("✅ Login successful! idToken: " + UserIdToken);
        callback(true, authResponse.idToken, authResponse.localId);
    }
    else
    {
        Debug.LogError("❌ Login error: " + request.error + "\n" + request.downloadHandler.text);
        callback(false, null, null);
    }
}



/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public IEnumerator SendPasswordResetEmail(string email, Action<bool, string> callback)
    {
        string resetUrl = "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=" + apiKey;

        var requestData = new
        {
            requestType = "PASSWORD_RESET",
            email = email
        };

        string jsonData = JsonUtility.ToJson(requestData);
        UnityWebRequest request = new UnityWebRequest(resetUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Password reset email sent to: " + email);
            callback(true, "Password reset email sent!");
        }
        else
        {
            Debug.LogError("❌ Password reset failed: " + request.error + "\n" + request.downloadHandler.text);
            callback(false, request.downloadHandler.text);
        }
    }


}
