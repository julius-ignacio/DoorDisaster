using System;
using UnityEngine;

public class EndGame_SaveData : MonoBehaviour
{
    public int trialIndex;   // Assign in Inspector or set from GameFlow manager
    public int stageIndex;   // Assign in Inspector or set from GameFlow manager
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

    public void OTriggerExit(Collider other)
    {
               if (other.CompareTag("Player"))
        {
            SaveBtn.SetActive(false);
        }
    }


    public void Save()
    {
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
        DataManager.Instance.SaveStageData(trialIndex, stageIndex);

        // Push to Firebase
        StartCoroutine(firebaseDatabase.SaveData(
            FirebaseAuth.UserIdToken,
            FirebaseAuth.UserLocalId,
            DataManager.Instance.playerData
        ));

        Debug.Log("✅ Stage data saved for Trial " + trialIndex + " Stage " + stageIndex);
    }
}
