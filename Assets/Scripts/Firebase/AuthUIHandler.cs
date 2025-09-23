using UnityEngine;
using TMPro; // if you're using TextMeshPro
using UnityEngine.UI;

public class AuthUIHandler : MonoBehaviour
{
    public FirebaseAuth firebaseAuth;

    public TMP_InputField emailInput,login_emailInput;
    public TMP_InputField nameInput;
    public TMP_InputField passwordInput, login_passwordInput;
    public TMP_InputField ageInput;
    public TMP_InputField gradeLevelInput;
    public TMP_Text feedbackText;

public void OnRegisterButton()
{
    string email = emailInput.text;
    string password = passwordInput.text;

    if (string.IsNullOrEmpty(email) || !email.Contains("@"))
    {
        feedbackText.text = "Please enter a valid email.";
        return;
    }
    if (string.IsNullOrEmpty(password) || password.Length < 6)
    {
        feedbackText.text = "Password must be at least 6 characters.";
        return;
    }

    int age, grade;
    if (!int.TryParse(ageInput.text, out age) || !int.TryParse(gradeLevelInput.text, out grade))
    {
        feedbackText.text = "Age and Grade must be numbers.";
        return;
    }

    StartCoroutine(firebaseAuth.RegisterUser(email, password, nameInput.text, age, grade, (success, idToken, localId) =>
    {
        if (success)
        {
            feedbackText.text = "Register successful!";
            Debug.Log("Token: " + idToken);
            Debug.Log("UserID: " + localId);
        }
        else
        {
            feedbackText.text = "Register failed!";
        }
    }));
}



    public void OnLoginButton()
    {
        StartCoroutine(firebaseAuth.LoginUser(login_emailInput.text, login_passwordInput.text, (success, idToken, localId) =>
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
