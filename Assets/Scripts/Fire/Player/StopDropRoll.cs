using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StopDropRoll : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;        // FPS camera
    public CharacterController controller; // Player CharacterController
    public Movements movementsScript;  // Your Movements component
    public Image fireOverlay;          // Fire overlay UI
    public TMP_Text centerPromptText;  // Centered Stop, Drop, Roll prompt
    public SubtitleManager2 subtitleManager; // Reference to hide objectives
    public GameObject healthBar;       // Health bar to hide
    public GameObject oxygenBar;       // Oxygen bar to hide

    [Header("Roll Settings")]
    public float rollStrength = 120f;   // Degrees per press (120° = 3 presses for full 360°)
    public float rollSpeed = 360f;      // Speed of camera rotation (degrees per second)
    public int pressesRequired = 3;     // Number of SPACE presses to finish

    [Header("Drop Settings")]
    public float dropAmount = 1.5f;
    public float dropSpeed = 3f;
    public float warningDuration = 2f;  

    private bool isOnFire = false;
    private int presses = 0;
    private float rollAngle = 0;
    private Vector3 originalCameraPos;
    private Vector3 targetCameraPos;
    private bool isDropping = false;
    private bool hasDropped = false;

    void Start()
    {
        // Store original camera position
        if (playerCamera != null)
            originalCameraPos = playerCamera.transform.localPosition;
    }

    void Update()
    {
        if (!isOnFire) return;

        // Handle camera dropping animation
        if (isDropping && !hasDropped)
        {
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                targetCameraPos,
                dropSpeed * Time.deltaTime
            );

            // Check if drop is complete
            if (Vector3.Distance(playerCamera.transform.localPosition, targetCameraPos) < 0.1f)
            {
                hasDropped = true;
                isDropping = false;

                // Update text to show rolling instructions
                if (centerPromptText != null)
                    centerPromptText.text = $"Press SPACE repeatedly to ROLL! ({pressesRequired} times)";
            }
        }

        // Only allow rolling after dropping
        if (!hasDropped) return;

        // Detect SPACE presses for rolling
        if (Input.GetKeyDown(KeyCode.Space))
        {
            presses++;
            rollAngle += rollStrength; // Add 120 degrees per press

            // Update prompt with remaining presses
            if (centerPromptText != null)
                centerPromptText.text = $"Keep ROLLING! Press SPACE {pressesRequired - presses} more times!";

            if (presses >= pressesRequired)
            {
                EndStopDropRoll();
            }
        }

        // Smooth camera roll animation
        if (rollAngle > 0)
        {
            float rotateAmount = Mathf.Min(rollAngle, rollSpeed * Time.deltaTime);
            playerCamera.transform.Rotate(Vector3.forward, rotateAmount);
            rollAngle -= rotateAmount;
        }
    }

    // Triggered when player enters fire
    public void TriggerOnFire()
    {
        if (isOnFire) return;

        isOnFire = true;
        presses = 0;
        rollAngle = 0;
        isDropping = false; // Don't start dropping yet
        hasDropped = false;

        // Store original position and calculate drop target
        originalCameraPos = playerCamera.transform.localPosition;
        targetCameraPos = originalCameraPos - Vector3.up * dropAmount;

        // Disable movement
        if (controller != null) controller.enabled = false;
        if (movementsScript != null) movementsScript.enabled = false;

        // Hide UI elements
        if (subtitleManager != null) subtitleManager.HideObjective();
        if (healthBar != null) healthBar.SetActive(false);
        if (oxygenBar != null) oxygenBar.SetActive(false);

        // Show overlay
        if (fireOverlay != null) fireOverlay.gameObject.SetActive(true);

        // Show initial fire warning (player stays standing for 2 seconds)
        if (centerPromptText != null)
        {
            centerPromptText.text = "YOU'RE ON FIRE!\nSTOP! DROP! Get ready to ROLL!";
            centerPromptText.enabled = true;
        }

        // Start dropping after 2 seconds
        Invoke("StartDropping", warningDuration);

        Debug.Log("Stop, Drop, and Roll sequence started!");
    }

    // Called after warning duration to start the drop
    private void StartDropping()
    {
        isDropping = true;
        Debug.Log("Starting drop animation");
    }

    // Ends the rolling sequence
    private void EndStopDropRoll()
    {
        isOnFire = false;
        isDropping = false;
        hasDropped = false;

        // Reset camera position and rotation
        if (playerCamera != null)
        {
            playerCamera.transform.localPosition = originalCameraPos;
            playerCamera.transform.rotation = Quaternion.Euler(
                playerCamera.transform.rotation.eulerAngles.x,
                playerCamera.transform.rotation.eulerAngles.y,
                0f // Reset Z rotation to 0
            );
        }

        // Re-enable movement
        if (controller != null) controller.enabled = true;
        if (movementsScript != null) movementsScript.enabled = true;

        // Show UI elements again
        if (healthBar != null) healthBar.SetActive(true);
        if (oxygenBar != null) oxygenBar.SetActive(true);

        // Hide overlay
        if (fireOverlay != null) fireOverlay.gameObject.SetActive(false);

        // Show completion message briefly
        if (centerPromptText != null)
        {
            centerPromptText.text = "Fire extinguished! You're safe!";
            // Hide after 2 seconds - SDRTrigger will handle quiz/objective
            Invoke("HideCenterText", 2f);
        }

        Debug.Log("Stop, Drop, and Roll complete - fire extinguished!");
    }

    // Just hide the center text
    private void HideCenterText()
    {
        if (centerPromptText != null)
            centerPromptText.enabled = false;
    }
}