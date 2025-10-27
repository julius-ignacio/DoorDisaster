using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame_SaveData : MonoBehaviour
{
    public FirebaseDatabase firebaseDatabase; // Drag your FirebaseDatabase GameObject here
    public GameObject SaveBtn;


    void Start()
    {
        SaveBtn.SetActive(false);
    }




    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveBtn.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveBtn.SetActive(false);
        }
    }


    public void Save()
    {
        SceneManager.LoadScene("Temple");

            AudioManager.Instance.StopAll();
AudioManager.Instance.StopLoop();

        if (DataManager.Instance == null)
        {
            Debug.LogError("❌ DataManager is missing in scene!");
            return;
        }

        if (firebaseDatabase == null)
        {
            Debug.LogError("❌ FirebaseDatabase is not assigned!");
            return;
        }

        // Update structured data from global values
        DataManager.Instance.SaveTrialData(DataManager.Instance.currentTrial);

        // Push to Firebase
        StartCoroutine(firebaseDatabase.SaveData(
            FirebaseAuth.UserIdToken,
            FirebaseAuth.UserLocalId,
            DataManager.Instance.playerData
        ));

        //SceneManager.LoadScene("Temple");

        Debug.Log("✅ Stage data saved for Trial " + DataManager.Instance.currentTrial + " Stage ");
    }
}
