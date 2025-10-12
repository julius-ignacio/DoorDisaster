using UnityEngine;
using TMPro;
using System.IO;
using System.Collections.Generic;

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
    public GameObject textPrefab; // A TextMeshProUGUI prefab for each row

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "earthquake_data.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            EarthquakeInfo[] quakes = JsonHelper.FromJson<EarthquakeInfo>(json);
            DisplayTop10(quakes);
        }
        else
        {
            Debug.LogWarning("No earthquake_data.json found!");
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

// Utility for array JSON
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrapped = "{\"Items\":" + json + "}";
        Wrapper<T> w = JsonUtility.FromJson<Wrapper<T>>(wrapped);
        return w.Items;
    }

    [System.Serializable]
    private class Wrapper<T> { public T[] Items; }
}
