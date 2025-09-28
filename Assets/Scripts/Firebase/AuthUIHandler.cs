using UnityEngine;
using TMPro; // if you're using TextMeshPro
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // for scene loading

public class AuthUIHandler : MonoBehaviour
{
    public FirebaseAuth firebaseAuth;

    public TMP_InputField emailInput, login_emailInput;
    public TMP_InputField nameInput;
    public TMP_InputField passwordInput, login_passwordInput;
    public TMP_InputField ageInput;
    public TMP_InputField gradeLevelInput;
    public TMP_Text feedbackTextlog, feedbackTextReg;

    public void OnRegisterButton()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            feedbackTextReg.text = "Please enter a valid email.";
            return;
        }
        if (string.IsNullOrEmpty(password) || password.Length < 6)
        {
            feedbackTextReg.text = "Password must be at least 6 characters.";
            return;
        }

        int age, grade;
        if (!int.TryParse(ageInput.text, out age) || !int.TryParse(gradeLevelInput.text, out grade))
        {
            feedbackTextReg.text = "Age and Grade must be numbers.";
            return;
        }

        StartCoroutine(firebaseAuth.RegisterUser(email, password, nameInput.text, age, grade, (success, idToken, localId) =>
        {
            if (success)
            {
                feedbackTextReg.text = "Register successful!";
                Debug.Log("Token: " + idToken);
                Debug.Log("UserID: " + localId);
            }
            else
            {
                feedbackTextReg.text = "Register failed!";
            }
        }));
    }



    public void OnLoginButton()
    {
        StartCoroutine(firebaseAuth.LoginUser(login_emailInput.text, login_passwordInput.text, (success, idToken, localId) =>
        {
            if (success)
            {
                feedbackTextlog.text = "✅ Login successful!";
                Debug.Log("Token: " + idToken);
                Debug.Log("UserID: " + localId);

                // TODO: Move to Game Scene


                // Delay and then load MainMenu
                StartCoroutine(LoadMainMenuWithDelay(2f));
            }
            else
            {
                feedbackTextlog.text = "❌ Login failed!";
            }
        }));
    }
    
        private IEnumerator LoadMainMenuWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("GameMenu"); // Change to your MainMenu scene name
    }
}
