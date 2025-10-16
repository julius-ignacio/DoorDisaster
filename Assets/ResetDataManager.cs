using UnityEngine;

public class ResetDataManager : MonoBehaviour
{
    // 🔹 Legacy/global fields (for quick access in scripts)

    public int currentTrial;
    public int currentStage;
public void OnTriggerEnter(Collider other)
    {
             if (other.CompareTag("Player"))
        {
            DataManager.Instance.quizScore = 0;
            DataManager.Instance.factsDiscovered = 0;
            DataManager.Instance.totalQuestionsAnswered = 0;
        }
    }

}
