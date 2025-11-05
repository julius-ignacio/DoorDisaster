using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame_SaveData : MonoBehaviour
{
    public GameObject SaveBtn;

    void Start()
    {
        SaveBtn.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            SaveBtn.SetActive(true);
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            SaveBtn.SetActive(false);
    }

    public void Save()
    {
        // Stop audio
        AudioManager.Instance.StopAll();
        AudioManager.Instance.StopLoop();

        if (DataManager.Instance == null)
        {
            Debug.LogError("❌ DataManager is missing in scene!");
            return;
        }

        if (FirebaseDatabase.Instance == null)
        {
            Debug.LogError("❌ FirebaseDatabase is not assigned!");
            return;
        }

        // Save structured trial data
        DataManager.Instance.SaveTrialData(DataManager.Instance.currentTrial, DataManager.Instance.currentMode);

        // Optional: also persist world locally for this trial/mode
        WorldSaveSystem.SaveWorld(DataManager.Instance.currentTrial, DataManager.Instance.currentMode);

        // Push to Firebase
        StartCoroutine(FirebaseDatabase.Instance.SaveData(
            FirebaseAuth.UserIdToken,
            FirebaseAuth.UserLocalId,
            DataManager.Instance.playerData
        ));

        // Finally transition
        SceneManager.LoadScene("Temple");
        Debug.Log($"✅ Stage data saved for Trial {DataManager.Instance.currentTrial}");
    }
}
























// using System;
// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class EndGame_SaveData : MonoBehaviour
// {
//     public GameObject SaveBtn;


//     void Start()
//     {
//         SaveBtn.SetActive(false);
//     }




//     public void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             SaveBtn.SetActive(true);
//         }
//     }

//     public void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             SaveBtn.SetActive(false);
//         }
//     }


//     public void Save()
//     {
//         SceneManager.LoadScene("Temple");

//         AudioManager.Instance.StopAll();
//         AudioManager.Instance.StopLoop();

//         if (DataManager.Instance == null)
//         {
//             Debug.LogError("❌ DataManager is missing in scene!");
//             return;
//         }

//         if (FirebaseDatabase.Instance == null)
//         {
//             Debug.LogError("❌ FirebaseDatabase is not assigned!");
//             return;
//         }

//         // Update structured data from global values
//         DataManager.Instance.SaveTrialData(
//          DataManager.Instance.currentTrial,
//          DataManager.Instance.currentMode
//      );


//         // Push to Firebase
//         StartCoroutine(FirebaseDatabase.Instance.SaveData(
//             FirebaseAuth.UserIdToken,
//             FirebaseAuth.UserLocalId,
//             DataManager.Instance.playerData
//         ));

//         //SceneManager.LoadScene("Temple");

//         Debug.Log("✅ Stage data saved for Trial " + DataManager.Instance.currentTrial + " Stage ");
//     }
// }
