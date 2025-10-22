using UnityEngine;

public class ResetDataManager : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (DataManager.Instance.playerData != null)
            {
                var data = DataManager.Instance.playerData;
                data.totalQuestionsAnswered = 0;
                data.overallTotalScore = 0;

                foreach (var trial in data.trials)
                {
                        if (trial == null) continue;
                        trial.factsDiscovered = 0;
                        trial.questionsAnswered = 0;
                        trial.quizScore = 0;
                        trial.totalScore = 0;
                }

                Debug.Log("✅ Player data fully reset, including trials and stages!");
            }

        }
    }
}
