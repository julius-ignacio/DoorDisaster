using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class FirebaseDatabase : MonoBehaviour
{
    private string databaseURL = "https://doordisaster-7003b-default-rtdb.firebaseio.com/";

    // Save player data (score, position, etc.)
    public IEnumerator SaveData(string idToken, string userId, PlayerData data)
    {
        string jsonData = JsonUtility.ToJson(data);
        string url = $"{databaseURL}users/{userId}.json?auth={idToken}";

        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data saved successfully!");
        }
        else
        {
            Debug.LogError("Error saving data: " + request.error + "\n" + request.downloadHandler.text);
        }
    }

    // Load player data
    public IEnumerator LoadData(string idToken, string userId, System.Action<PlayerData> callback)
    {
        string url = $"{databaseURL}users/{userId}.json?auth={idToken}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data loaded: " + request.downloadHandler.text);
            PlayerData data = JsonUtility.FromJson<PlayerData>(request.downloadHandler.text);
            callback?.Invoke(data);
        }
        else
        {
            Debug.LogError("Error loading data: " + request.error);
        }
    }
}

