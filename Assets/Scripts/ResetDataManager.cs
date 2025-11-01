using UnityEngine;

public class ResetDataManager : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (DataManager.Instance.playerData != null)
            {
                var data = DataManager.Instance;
                data.totalQuestionsAnswered = 0;
                data.quizScore = 0;
                data.wrongAnswers = 0;
                data.factsDiscovered = 0;
                data.Npcs_saved = 0;

    

                Debug.Log("✅ Player data fully reset, including trials and stages!");
            }

        }
    }
}
