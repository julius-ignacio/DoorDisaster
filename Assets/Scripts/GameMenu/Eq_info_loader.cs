using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

[System.Serializable]
public class EarthquakeInfo
{
    public string time;
    public string magnitude;
    public string location;
}

public class Eq_info_loader : MonoBehaviour
{
    [Header("UI References")]
    public Transform dateTimeColumn;
    public Transform magnitudeColumn;
    public Transform locationColumn;
    public GameObject textPrefab; // TextMeshProUGUI prefab

    // Remote JSON URL
    private string remoteUrl = "https://eqdatajson.tiiny.site/earthquake_data.json";

    void Start()
    {
        StartCoroutine(LoadEarthquakeData());
    }

    IEnumerator LoadEarthquakeData()
    {
        UnityWebRequest request = UnityWebRequest.Get(remoteUrl);
        yield return request.SendWebRequest();
        

        if (request.result != UnityWebRequest.Result.Success)
        {
            
            Debug.LogWarning($"❌ Failed to load remote JSON: {request.error}");
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log($"Raw JSON: {json}");

            EarthquakeInfo[] quakes;
            Debug.Log($"Raw JSON: {json}");


if (json.TrimStart().StartsWith("["))
{
    quakes = JsonHelper.FromJson<EarthquakeInfo>(json);
}
else
            {
    Debug.Log($"Raw JSON: {json}");

    // Parse single object
    EarthquakeInfo single = JsonUtility.FromJson<EarthquakeInfo>(json);
    quakes = new EarthquakeInfo[] { single };
}

            DisplayTop10(quakes);
            Debug.Log($"✅ Successfully fetched {quakes.Length} earthquake entries from web");
        }
    }

    void DisplayTop10(EarthquakeInfo[] quakes)
    {
        int limit = Mathf.Min(10, quakes.Length);
        for (int i = 0; i < limit; i++)
        {
            CreateText(dateTimeColumn, quakes[i].time);
            CreateText(magnitudeColumn, quakes[i].magnitude);
            CreateText(locationColumn, quakes[i].location);
        }
    }

    void CreateText(Transform parent, string text)
    {
        var go = Instantiate(textPrefab, parent);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
    }
}

// JSON Helper
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrapped = "{\"Items\":" + json + "}";  // 👈 wrap it
        Wrapper<T> w = JsonUtility.FromJson<Wrapper<T>>(wrapped);
        return w.Items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}

