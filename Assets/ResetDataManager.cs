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
                    foreach (var stage in trial.stages)
                    {
                        if (stage == null) continue;
                        stage.factsDiscovered = 0;
                        stage.questionsAnswered = 0;
                        stage.quizScore = 0;
                        stage.totalScore = 0;
                    }
                }

                Debug.Log("✅ Player data fully reset, including trials and stages!");
            }

        }
    }
}
