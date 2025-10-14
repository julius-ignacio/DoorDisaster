using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PhivolcsFetcher : MonoBehaviour
{
    [System.Serializable]
    public class EarthquakeData
    {
        public string magnitude;
        public string location;
        public string time;
        public string depth;
    }

    [System.Serializable]
    public class PhivolcsData
    {
        public EarthquakeData latest_earthquake;
    }

    public string url = "https://your-json-endpoint-or-localserver/phivolcs_latest.json";

    IEnumerator Start()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Failed to fetch data: {request.error}");
                // fallback to offline JSON
                LoadOfflineData();
            }
            else
            {
                string json = request.downloadHandler.text;
                EarthquakeData[] quakes = JsonHelper.FromJson<EarthquakeData>(json);
                if (quakes.Length > 0)
                {
                    Debug.Log($"Latest EQ: M{quakes[0].magnitude} - {quakes[0].location}");
                }
                else
                {
                    Debug.LogWarning("No earthquake data found.");
                }

            }
        }
    }

    void LoadOfflineData()
    {
        TextAsset offlineJson = Resources.Load<TextAsset>("phivolcs_latest");
        PhivolcsData data = JsonUtility.FromJson<PhivolcsData>(offlineJson.text);
        Debug.Log($"[Offline] Latest EQ: M{data.latest_earthquake.magnitude} - {data.latest_earthquake.location}");
    }
}
