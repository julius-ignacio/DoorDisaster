using UnityEngine;
using TMPro;

public class Almanac : MonoBehaviour
{
    [Header("Normal")]
    public TMP_Text[] quizscore, wronganswers, factsdiscovered;

    [Header("Hard")]
    public TMP_Text[] quizscoreHard, wronganswersHard, factsdiscoveredHard;

    void Update()
    {
        var data = DataManager.Instance.playerData;

        // NORMAL MODE (0)
        for (int i = 0; i < data.Mode[0].trials.Length; i++)
        {
            quizscore[i].text = data.Mode[0].trials[i].quizScore.ToString();
            wronganswers[i].text = data.Mode[0].trials[i].wrongAnswers.ToString();
            factsdiscovered[i].text = data.Mode[0].trials[i].factsDiscovered.ToString();
        }

        // HARD MODE (1)
        for (int i = 0; i < data.Mode[1].trials.Length; i++)
        {
            quizscoreHard[i].text = data.Mode[1].trials[i].quizScore.ToString();
            wronganswersHard[i].text = data.Mode[1].trials[i].wrongAnswers.ToString();
            factsdiscoveredHard[i].text = data.Mode[1].trials[i].factsDiscovered.ToString();
        }
    }
}
