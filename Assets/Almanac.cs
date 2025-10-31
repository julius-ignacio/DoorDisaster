using UnityEngine;
using TMPro;


public class Almanac : MonoBehaviour
{
    [Header("Texts")]
    public TMP_Text[] quizscore, wronganswers, factsdiscovered, totalscore;
    void Update()
    {
        int length = DataManager.Instance.playerData.trials.Length;
        var data = DataManager.Instance.playerData;

        for (int i = 0; i < length; i++)
        {
            quizscore[i].text = data.trials[i].quizScore.ToString();
            wronganswers[i].text = data.trials[i].wrongAnswers.ToString();
            factsdiscovered[i].text = data.trials[i].factsDiscovered.ToString();
            totalscore[i].text = data.trials[i].totalScore.ToString();
        }


    }
}
