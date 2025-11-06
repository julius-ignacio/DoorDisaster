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

    [Header("Verification Panel")]
    public GameObject verifyPanel, switcher;


    [Header("Panel for switching IF successful")]
    public GameObject login, register;

    void Update()
    {
        if (verifyPanel != null && verifyPanel.activeSelf)  // ✅ Checks if it's ACTIVE
        {
            switcher.SetActive(false);
        }
        else
        {
            switcher.SetActive(true);
        }
    }

    void Start()
    {
        switcher.SetActive(true);
        forgotPanel.SetActive(false);   
    }

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
                feedbackTextReg.text = "Register successful! Sending verification email...";
                Debug.Log("Token: " + idToken);
                Debug.Log("UserID: " + localId);

                // Send verification email
                StartCoroutine(firebaseAuth.SendEmailVerification(idToken, (verifySuccess, message) =>
                {
                    if (verifySuccess)
                    {
                        feedbackTextReg.text = "✅ Verification email sent! Please check your inbox.";
                    }
                    else
                    {
                        feedbackTextReg.text = "❌ Failed to send verification email: " + message;
                    }
                }));

                register.SetActive(false);
                login.SetActive(false);
                verifyPanel.SetActive(true);
                SetVerifyEmail(email);
            }

            else
            {
                feedbackTextReg.text = "Register failed!";
            }
        }));
    }



    public void OnLoginButton()
    {
        string email = login_emailInput.text.Trim();
        string password = login_passwordInput.text;

        StartCoroutine(firebaseAuth.LoginUser(email, password, (success, idToken, localId) =>
        {
            if (success)
            {
                Debug.Log("Login returned token: " + idToken);
                // Check if the email is verified
                StartCoroutine(firebaseAuth.CheckEmailVerified(idToken, (isVerified) =>
                {
                    if (isVerified)
                    {
                        feedbackTextlog.text = "✅ Login successful!";
                        Debug.Log("Token: " + idToken);
                        Debug.Log("UserID: " + localId);

                        // Load player data from Firebase
                        StartCoroutine(FindObjectOfType<FirebaseDatabase>().LoadData(idToken, localId, (loadedData) =>
                        {
                            if (loadedData != null)
                            {
                                DataManager.Instance.playerData = loadedData;
                                Debug.Log("✅ Player data loaded into DataManager");
                            }
                            else
                            {
                                Debug.LogWarning("⚠️ No existing player data found, using defaults");
                            }
                        }));

                        // Delay and load main menu
                        StartCoroutine(LoadMainMenuWithDelay(2f));
                    }
                    else
                    {
                        feedbackTextlog.text = "";
                        Debug.LogWarning("User email not verified yet.");

                        login.SetActive(false);
                        verifyPanel.SetActive(true);

                        SetVerifyEmail(email);
                    }

                }));
            }
            else
            {
                feedbackTextlog.text = "❌ Login failed! Check your credentials.";
            }
        }));
    }


    public TMP_Text verifyFeedbackText;
    public TMP_InputField verifyEmailInput;
    public TMP_InputField verifyPasswordInput; // ADD THIS FIELD

    public void OnResendVerificationButton()
    {
        string email = verifyEmailInput.text.Trim();
        string password = verifyPasswordInput.text;
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            verifyFeedbackText.text = "Please enter email and password.";
            return;
        }

        StartCoroutine(firebaseAuth.LoginUser(email, password, (success, idToken, localId) =>
        {
            if (success && !string.IsNullOrEmpty(idToken))
            {
                StartCoroutine(firebaseAuth.SendEmailVerification(idToken, (verifySuccess, message) =>
                {
                    verifyFeedbackText.text = verifySuccess ? "✅ Verification email resent!" : "❌ Failed: " + message;
                }));
            }
            else
            {
                verifyFeedbackText.text = "❌ Couldn't log in with provided credentials.";
            }
        }));
    }


    private void SetVerifyEmail(string email)
    {
        if (verifyEmailInput != null)
            verifyEmailInput.text = email;
    }



    private IEnumerator LoadMainMenuWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("GameMenu"); // Change to your MainMenu scene name
    }


    public void OnVerifyButton()
    {
        string email = verifyEmailInput.text.Trim();
        string password = verifyPasswordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            verifyFeedbackText.text = "Please enter email and password.";
            return;
        }

        // Try logging in again to get a fresh idToken
        StartCoroutine(firebaseAuth.LoginUser(email, password, (success, idToken, localId) =>
        {
            if (success && !string.IsNullOrEmpty(idToken))
            {
                // Check if email is verified now
                StartCoroutine(firebaseAuth.CheckEmailVerified(idToken, (isVerified) =>
                {
                    if (isVerified)
                    {
                        verifyFeedbackText.text = "✅ Email verified successfully!";

                        // Optional: Auto login & go to game
                        StartCoroutine(FindObjectOfType<FirebaseDatabase>().LoadData(idToken, localId, (loadedData) =>
                        {
                            if (loadedData != null)
                            {
                                DataManager.Instance.playerData = loadedData;
                                Debug.Log("✅ Player data loaded after verification");
                            }
                        }));

                        StartCoroutine(LoadMainMenuWithDelay(2f)); // Load your main game/menu scene
                    }
                    else
                    {
                        verifyFeedbackText.text = "❌ Email not verified yet. Please check your inbox.";
                    }
                }));
            }
            else
            {
                verifyFeedbackText.text = "❌ Invalid login credentials.";
            }
        }));
    }




  [Header("Forgot Password UI")]
    public GameObject forgotPanel;              // Panel with email input + send button
    public TMP_InputField resetEmailInput;      // Email field inside the forgot panel
    public TMP_Text resetFeedbackText;          // Feedback text in the forgot panel
    public Button sendResetButton;              // "Send reset link" button

    public void OpenForgotPanel()
    {
        resetFeedbackText.text = "";
        resetEmailInput.text = "";
        if (forgotPanel) forgotPanel.SetActive(true);
        switcher.SetActive(false);
        login.SetActive(false);
    }

    public void CloseForgotPanel()
    {
        if (forgotPanel) forgotPanel.SetActive(false);
        switcher.SetActive(true);
        login.SetActive(true);
    }

    public void OnSendPasswordReset()
    {
        string email = resetEmailInput != null ? resetEmailInput.text.Trim() : "";

        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            if (resetFeedbackText) resetFeedbackText.text = "Please enter a valid email.";
            return;
        }

        if (sendResetButton) sendResetButton.interactable = false;
        if (resetFeedbackText) resetFeedbackText.text = "Sending reset link...";

        StartCoroutine(firebaseAuth.SendPasswordResetEmail(email, (success, message) =>
        {
            if (success)
            {
                if (resetFeedbackText) resetFeedbackText.text = "✅ Password reset email sent! Check your inbox.";
            }
            else
            {
                // message contains raw error JSON from Firebase; show a friendly fallback
                if (resetFeedbackText) resetFeedbackText.text = "❌ Failed to send reset email. Please check the email address and try again.";
                Debug.LogError("Password reset error: " + message);
            }

            if (sendResetButton) sendResetButton.interactable = true;
        }));
    }

}
