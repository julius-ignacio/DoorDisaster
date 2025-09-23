using UnityEngine;
using TMPro;
using UnityEngine.Scripting;

[Preserve]
public class SwitchAuth : MonoBehaviour
{
    [Header("Container Objects")]
    public GameObject register, login;

    public TextMeshProUGUI switchertextbtn; // Assign TMP text directly in Inspector

    void Start()
    {
        register.SetActive(false);
    }

    public void OnClick()
    {
        if (register.activeSelf == false) // check if Register is hidden
        {
            switchertextbtn.text = "Already have an account? Login";
            register.SetActive(true);
            login.SetActive(false);

        }
        else
        {
            switchertextbtn.text = "Don't have an account? Register";

          register.SetActive(false);
            login.SetActive(true);
        }
    }
}
