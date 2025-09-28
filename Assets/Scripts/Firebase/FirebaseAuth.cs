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


    // Login
public IEnumerator LoginUser(string email, string password, Action<bool, string, string> callback)
{
    string loginUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + apiKey;

    RegisterRequest requestData = new RegisterRequest { email = email, password = password };
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

        Debug.Log("✅ Login successful!");
        callback(true, authResponse.idToken, authResponse.localId);
    }
    else
    {
        Debug.LogError("❌ Login error: " + request.error + "\n" + request.downloadHandler.text);
        callback(false, null, null);
    }
}

}
