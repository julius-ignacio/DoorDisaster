using UnityEngine;

public class triggerSurvey_btn : MonoBehaviour
{
    public SurveyManager surveyManager;

    public void callSurvey()
    {
        // Use the static database
        surveyManager.BeginSurvey(SurveyDatabase.playerFeedback);
    }
}
