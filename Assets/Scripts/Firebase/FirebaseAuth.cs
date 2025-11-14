using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using UnityEngine.SocialPlatforms.Impl;

public class FirebaseAuth : MonoBehaviour
{
    public static FirebaseAuth Instance;

    [Header("Firebase Config")]
    private string apiKey = "AIzaSyBB5GZXI2FlYMbfg_JH-FJU60Mj5zSVk5E";

    public static string UserIdToken;
    public static string UserLocalId;
    public PlayerData playerData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("✅ FirebaseAuth singleton initialized");
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

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

    // NEW: proper serializable payload for password reset
    [System.Serializable]
    public class PasswordResetRequest
    {
        public string requestType; // must be "PASSWORD_RESET"
        public string email;       // the user's email
    }

    // Add these classes to your FirebaseAuth.cs file (after the other serializable classes)

    [System.Serializable]
    public class ChangePasswordRequest
    {
        public string idToken;
        public string password;        // The new password
        public bool returnSecureToken; // Set to true to get a new token
    }

    [System.Serializable]
    public class ChangePasswordResponse
    {
        public string idToken;
        public string refreshToken;
        public string expiresIn;
    }

    // Add this method to your FirebaseAuth class
    public IEnumerator ChangePassword(string idToken, string newPassword, System.Action<bool, string> callback)
    {
        string changePasswordUrl = "https://identitytoolkit.googleapis.com/v1/accounts:update?key=" + apiKey;

        ChangePasswordRequest requestData = new ChangePasswordRequest
        {
            idToken = idToken,
            password = newPassword,
            returnSecureToken = true
        };

        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log("ChangePassword payload: " + jsonData);

        using (UnityWebRequest request = new UnityWebRequest(changePasswordUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Password changed successfully!");

                // Update the stored token with the new one
                ChangePasswordResponse response = JsonUtility.FromJson<ChangePasswordResponse>(request.downloadHandler.text);
                UserIdToken = response.idToken;

                callback?.Invoke(true, "Password changed successfully!");
            }
            else
            {
                string errorMsg = request.downloadHandler.text;
                Debug.LogError($"❌ Failed to change password: {errorMsg}");

                // Map common errors to friendly messages
                string friendlyError = MapChangePasswordError(errorMsg);
                callback?.Invoke(false, friendlyError);
            }
        }
    }

    private string MapChangePasswordError(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return "Request failed. Please try again.";

        if (raw.Contains("INVALID_ID_TOKEN")) return "Session expired. Please login again.";
        if (raw.Contains("WEAK_PASSWORD")) return "Password is too weak. Please use a stronger password.";
        if (raw.Contains("TOKEN_EXPIRED")) return "Session expired. Please login again.";
        if (raw.Contains("USER_NOT_FOUND")) return "User account not found.";

        return "Failed to change password. Please try again.";
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

        LookupAccountRequest requestData = new LookupAccountRequest { idToken = idToken };
        string jsonData = JsonUtility.ToJson(requestData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var info = JsonUtility.FromJson<AccountInfoResponse>(request.downloadHandler.text);
            bool verified = info.users != null && info.users.Length > 0 && info.users[0].emailVerified;
            callback(verified);
        }
        else
        {
            Debug.LogError("Error checking verification: " + request.downloadHandler.text);
            callback(false);
        }
    }

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
            DataManager.Instance.InitPlayerData();
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

            DataManager.Instance.playerData.playerId = UserLocalId;
            DataManager.Instance.playerData.playerName = name;
            DataManager.Instance.playerData.email = email;
            DataManager.Instance.playerData.age = age;
            DataManager.Instance.playerData.gradeLevel = gradeLevel;

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

        VerifyEmailRequest payload = new VerifyEmailRequest
        {
            requestType = "VERIFY_EMAIL",
            idToken = idToken
        };

        string jsonData = JsonUtility.ToJson(payload);
        Debug.Log("SendEmailVerification payload: " + jsonData);

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
            returnSecureToken = true
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

    public IEnumerator SendPasswordResetEmail(string email, Action<bool, string> callback)
    {
        string resetUrl = "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=" + apiKey;

        // FIX: use a serializable payload so JsonUtility includes the fields
        PasswordResetRequest payload = new PasswordResetRequest
        {
            requestType = "PASSWORD_RESET",
            email = email
        };

        string jsonData = JsonUtility.ToJson(payload);
        Debug.Log("SendPasswordReset payload: " + jsonData);

        using (UnityWebRequest request = new UnityWebRequest(resetUrl, "POST"))
        {
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
                string raw = request.downloadHandler.text;
                Debug.LogError("❌ Password reset failed: " + request.error + "\n" + raw);

                // Optional: friendlier messages for common Firebase errors
                string friendly = MapPasswordResetError(raw);
                callback(false, friendly);
            }
        }
    }

    private string MapPasswordResetError(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return "Request failed. Please try again.";
        // Cheap string checks to avoid JSON model; expand as needed
        if (raw.Contains("EMAIL_NOT_FOUND")) return "No account found with that email.";
        if (raw.Contains("INVALID_EMAIL")) return "Email address is invalid.";
        if (raw.Contains("MISSING_EMAIL")) return "Please enter your email.";
        if (raw.Contains("TOO_MANY_ATTEMPTS_TRY_LATER")) return "Too many attempts. Try again later.";
        if (raw.Contains("MISSING_REQ_TYPE")) return "Internal error forming request (missing requestType). Please try again.";
        return "Failed to send reset email. Please check the email and try again.";
    }
}