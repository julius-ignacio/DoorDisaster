using UnityEngine;
using TMPro; // if you're using TextMeshPro
using UnityEngine.UI;

public class AuthUIHandler : MonoBehaviour
{
    public FirebaseAuth firebaseAuth;

    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text feedbackText;

    public void OnRegisterButton()
    {
        StartCoroutine(firebaseAuth.RegisterUser(emailInput.text, passwordInput.text, (success, idToken, localId) =>
        {
            if (success)
            {
                feedbackText.text = "✅ Registered successfully!";
                Debug.Log("Token: " + idToken);
                Debug.Log("UserID: " + localId);
            }
            else
            {
                feedbackText.text = "❌ Register failed!";
            }
        }));
    }

    public void OnLoginButton()
    {
        StartCoroutine(firebaseAuth.LoginUser(emailInput.text, passwordInput.text, (success, idToken, localId) =>
        {
            if (success)
            {
                feedbackText.text = "✅ Login successful!";
                Debug.Log("Token: " + idToken);
                Debug.Log("UserID: " + localId);

                // TODO: Move to Game Scene
            }
            else
            {
                feedbackText.text = "❌ Login failed!";
            }
        }));
    }
}
