using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Settings : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;
    public TMP_InputField nameInput;
    public TMP_InputField ageInput;
    public TMP_InputField gradeInput;
    public TMP_InputField currentPasswordInput;
    public TMP_InputField newPasswordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_Text feedbackText;
    public Button saveProfileButton;
    public Button changePasswordButton;
    public Button closeButton;
    public GameObject openSettingsButton;
    public FirebaseAuth FirebaseAuth;

    // References found at runtime - no more cross-scene errors!
    private FirebaseDatabase firebaseDatabase;

    private void Awake()
    {
        // Get references at runtime instead of Inspector
        firebaseDatabase = FirebaseDatabase.Instance;
    }

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // Setup close button listener
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSettings);
    }

    public void OpenSettings()
    {
        openSettingsButton.SetActive(false);
        settingsPanel.SetActive(true);
        LoadCurrentData();
        ClearPasswordFields();
        if (feedbackText != null)
            feedbackText.text = "";
    }

    public void CloseSettings()
    {
        openSettingsButton.SetActive(true);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        ClearPasswordFields();
        if (feedbackText != null)
            feedbackText.text = "";
    }

    private void LoadCurrentData()
    {
        if (DataManager.Instance == null || DataManager.Instance.playerData == null)
        {
            Debug.LogWarning("⚠️ DataManager or playerData is null");
            return;
        }

        var data = DataManager.Instance.playerData;

        if (nameInput != null)
            nameInput.text = data.playerName;
        if (ageInput != null)
            ageInput.text = data.age.ToString();
        if (gradeInput != null)
            gradeInput.text = data.gradeLevel.ToString();
    }

    public void OnSaveProfileButton()
    {
        if (nameInput == null || ageInput == null || gradeInput == null)
        {
            Debug.LogError("❌ Input fields not assigned!");
            return;
        }

        string newName = nameInput.text.Trim();
        string ageText = ageInput.text.Trim();
        string gradeText = gradeInput.text.Trim();

        // Validation
        if (string.IsNullOrEmpty(newName))
        {
            if (feedbackText != null)
                feedbackText.text = "❌ Name cannot be empty.";
            return;
        }

        if (!int.TryParse(ageText, out int newAge) || newAge <= 0)
        {
            if (feedbackText != null)
                feedbackText.text = "❌ Please enter a valid age.";
            return;
        }

        if (!int.TryParse(gradeText, out int newGrade) || newGrade <= 0)
        {
            if (feedbackText != null)
                feedbackText.text = "❌ Please enter a valid grade level.";
            return;
        }

        // Disable button to prevent multiple clicks
        if (saveProfileButton != null)
            saveProfileButton.interactable = false;

        if (feedbackText != null)
            feedbackText.text = "Updating profile...";

        // Update local data
        DataManager.Instance.playerData.playerName = newName;
        DataManager.Instance.playerData.age = newAge;
        DataManager.Instance.playerData.gradeLevel = newGrade;

        // Save to Firebase
        string idToken = FirebaseAuth.UserIdToken;
        string localId = FirebaseAuth.UserLocalId;

        if (string.IsNullOrEmpty(idToken) || string.IsNullOrEmpty(localId))
        {
            if (feedbackText != null)
                feedbackText.text = "❌ Not logged in. Please login again.";
            if (saveProfileButton != null)
                saveProfileButton.interactable = true;
            return;
        }

        StartCoroutine(SaveProfileCoroutine(idToken, localId));
    }

    private IEnumerator SaveProfileCoroutine(string idToken, string localId)
    {
        yield return StartCoroutine(FirebaseDatabase.Instance.SaveData(idToken, localId, DataManager.Instance.playerData));

        if (feedbackText != null)
            feedbackText.text = "✅ Profile updated successfully!";

        if (saveProfileButton != null)
            saveProfileButton.interactable = true;

        // Update Profile UI if it exists
        UpdateProfileUI();
    }

    public void OnChangePasswordButton()
    {
        if (currentPasswordInput == null || newPasswordInput == null || confirmPasswordInput == null)
        {
            Debug.LogError("❌ Password input fields not assigned!");
            return;
        }

        string currentPassword = currentPasswordInput.text;
        string newPassword = newPasswordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        // Validation
        if (string.IsNullOrEmpty(currentPassword))
        {
            feedbackText.text = "❌ Please enter your current password.";
            return;
        }
        if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
        {
            feedbackText.text = "❌ New password must be at least 6 characters.";
            return;
        }
        if (newPassword != confirmPassword)
        {
            feedbackText.text = "❌ New passwords do not match.";
            return;
        }
        if (currentPassword == newPassword)
        {
            feedbackText.text = "❌ New password must be different from current password.";
            return;
        }

        changePasswordButton.interactable = false;
        feedbackText.text = "Changing password...";

        // ✅ Ensure FirebaseAuth is alive
        FirebaseAuth auth = FirebaseAuth;
        if (auth == null)
        {
            feedbackText.text = "❌ Error: Authentication system not found.";
            changePasswordButton.interactable = true;
            Debug.LogError("❌ FirebaseAuth.Instance is null in GameMenu!");
            return;
        }

        string email = DataManager.Instance?.playerData?.email;
        if (string.IsNullOrEmpty(email))
        {
            feedbackText.text = "❌ Error: Email not found. Please login again.";
            changePasswordButton.interactable = true;
            Debug.LogError("❌ User email is null or empty!");
            return;
        }

        Debug.Log($"Starting password change process for email: {email}");

        // Re‑login with current password to get fresh token
        StartCoroutine(auth.LoginUser(email, currentPassword, (loginSuccess, idToken, localId) =>
        {
            if (loginSuccess)
            {
                Debug.Log("Login success, starting ChangePassword with token: " + idToken);

                StartCoroutine(auth.ChangePassword(idToken, newPassword, (changeSuccess, message) =>
                {
                    if (changeSuccess)
                    {
                        feedbackText.text = "✅ Password changed successfully!";
                        ClearPasswordFields();
                    }
                    else
                    {
                        feedbackText.text = "❌ Failed to change password: " + message;
                        Debug.LogError("Change password error: " + message);
                    }

                    changePasswordButton.interactable = true;
                }));
            }
            else
            {
                feedbackText.text = "❌ Current password is incorrect.";
                changePasswordButton.interactable = true;
            }
        }));
    }


    private IEnumerator TimeoutCheck(float timeout, System.Func<bool> checkCondition, System.Action onTimeout)
    {
        float elapsed = 0f;
        while (elapsed < timeout)
        {
            if (checkCondition())
                yield break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        onTimeout?.Invoke();
    }

    private void ClearPasswordFields()
    {
        if (currentPasswordInput != null)
            currentPasswordInput.text = "";
        if (newPasswordInput != null)
            newPasswordInput.text = "";
        if (confirmPasswordInput != null)
            confirmPasswordInput.text = "";
    }

    private void UpdateProfileUI()
    {
        // Find and update the Profile component if it exists in the scene
        Profile profile = FindObjectOfType<Profile>();
        if (profile != null)
        {
            var data = DataManager.Instance.playerData;
            if (profile.playerNameText != null)
                profile.playerNameText.text = data.playerName;
            if (profile.AgeText != null)
                profile.AgeText.text = data.age.ToString();
            if (profile.GradeText != null)
                profile.GradeText.text = data.gradeLevel.ToString();
        }
    }
}
