using UnityEngine;
using TMPro;


public class Profile : MonoBehaviour
{
    public TMP_Text playerNameText, AgeText, GradeText, EmailText;
    public  GameObject profileui;
    void Start()
    {
        profileui.SetActive(false);
        var data = DataManager.Instance.playerData;
        playerNameText.text = data.playerName;
        AgeText.text = data.age.ToString();
        GradeText.text = data.gradeLevel.ToString();
        EmailText.text = data.email;
    }
}
