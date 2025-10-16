using UnityEngine;

public class ResetDataManager : MonoBehaviour
{
    public int currentTrial;
    public int currentStage;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DataManager.Instance.quizScore = 0;
            DataManager.Instance.factsDiscovered = 0;
            DataManager.Instance.totalQuestionsAnswered = 0;

            if (DataManager.Instance.playerData != null) // ✅ FIXED HERE
            {
                DataManager.Instance.playerData.totalQuestionsAnswered = 0; // ✅ FIXED
                DataManager.Instance.playerData.overallTotalScore = 0;       // ✅ FIXED
            }

            Debug.Log("✅ Player data reset successfully!");
        }
    }
}
