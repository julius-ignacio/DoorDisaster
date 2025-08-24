using UnityEngine;
using TMPro;
using UnityEngine.Scripting;

[Preserve]
public class SwitchAuth : MonoBehaviour
{
    public GameObject loginbtn, registerbtn;
    public TextMeshProUGUI switchertextbtn; // Assign TMP text directly in Inspector

    public void OnClick()
    {
        if (registerbtn.activeSelf == false) // check if Register is hidden
        {
            // Show Register UI
            loginbtn.SetActive(false);
            registerbtn.SetActive(true);
            switchertextbtn.text = "Already have an account? Login";
        }
        else
        {
            // Show Login UI
            loginbtn.SetActive(true);
            registerbtn.SetActive(false);
            switchertextbtn.text = "Don't have an account? Register";
        }
    }
}
