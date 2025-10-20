using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StopDropRoll : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public CharacterController controller;
    public Movements2 movementsScript;
    public Image fireOverlay;
    public TMP_Text centerPromptText;
    public SubtitleManager2 subtitleManager;
    public GameObject healthBar;
    public GameObject oxygenBar;

    [Header("Roll Settings")]
    public float rollStrength = 120f;
    public float rollSpeed = 360f;
    public int rollsRequired = 3;

    [Header("Drop Settings")]
    public float dropAmount = 1.5f;
    public float dropSpeed = 3f;
    public float warningDuration = 2f;

    [Header("Swipe Settings")]
    public float minSwipeDistance = 100f;
    public float circleTolerance = 0.6f;
    public float swipeSensitivity = 0.5f;

    // ✅ Added event callback
    public System.Action OnSDRComplete;

    private bool isOnFire = false;
    private int rollsCompleted = 0;
    private float rollAngle = 0;
    private Vector3 originalCameraPos;
    private Vector3 targetCameraPos;
    private Quaternion originalCameraRotation;
    private bool isDropping = false;
    private bool hasDropped = false;

    // Swipe tracking
    private Vector2 swipeStartPos;
    private Vector2 lastSwipePos;
    private float swipeStartTime;
    private bool isCurrentlyTracking = false;
    private Vector2[] swipePositions = new Vector2[60];
    private int positionCount = 0;

    void Start()
    {
        if (playerCamera != null)
        {
            originalCameraPos = playerCamera.transform.localPosition;
            originalCameraRotation = playerCamera.transform.localRotation;
        }
    }

    void Update()
    {
        if (!isOnFire) return;

        // Handle camera dropping
        if (isDropping && !hasDropped)
        {
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                targetCameraPos,
                dropSpeed * Time.deltaTime
            );

            if (Vector3.Distance(playerCamera.transform.localPosition, targetCameraPos) < 0.1f)
            {
                hasDropped = true;
                isDropping = false;

                if (centerPromptText != null)
                    centerPromptText.text = $"Swipe in a CIRCLE to ROLL! ({rollsRequired} times)";
            }
        }

        // Only allow rolling after dropping
        if (!hasDropped) return;

        // Handle input - works for both mouse and touch
        HandleSwipeInput();

        // Smooth camera roll animation
        if (rollAngle > 0)
        {
            float rotateAmount = Mathf.Min(rollAngle, rollSpeed * Time.deltaTime);
            playerCamera.transform.Rotate(Vector3.forward, rotateAmount);
            rollAngle -= rotateAmount;
        }
    }

    private void HandleSwipeInput()
    {
        // Touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                StartSwipeTracking(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                TrackSwipePosition(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                EndSwipeTracking(touch.position);
            }
        }
        // Mouse input (for PC testing)
        else if (Input.GetMouseButtonDown(0))
        {
            StartSwipeTracking(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            TrackSwipePosition(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndSwipeTracking(Input.mousePosition);
        }
    }

    private void StartSwipeTracking(Vector2 startPos)
    {
        swipeStartPos = startPos;
        lastSwipePos = startPos;
        swipeStartTime = Time.time;
        isCurrentlyTracking = true;
        positionCount = 0;
    }

    private void TrackSwipePosition(Vector2 currentPos)
    {
        if (!isCurrentlyTracking) return;

        // Store position for analysis
        if (positionCount < swipePositions.Length)
        {
            swipePositions[positionCount] = currentPos;
            positionCount++;
        }

        // Rotate camera in real-time while swiping
        Vector2 swipeDelta = currentPos - lastSwipePos;
        float rotationAmount = swipeDelta.magnitude * swipeSensitivity;
        playerCamera.transform.Rotate(Vector3.forward, rotationAmount);

        lastSwipePos = currentPos;
    }

    private void EndSwipeTracking(Vector2 endPos)
    {
        if (!isCurrentlyTracking) return;

        isCurrentlyTracking = false;

        // Analyze if this was a circular motion
        if (IsCircularMotion(endPos))
        {
            CompleteRoll();
        }

        positionCount = 0;
    }

    private bool IsCircularMotion(Vector2 endPos)
    {
        float totalDistance = Vector2.Distance(swipeStartPos, endPos);
        if (totalDistance < minSwipeDistance)
            return false;

        Vector2 center = (swipeStartPos + endPos) / 2f;
        float averageRadius = 0f;
        float radiusVariance = 0f;

        for (int i = 0; i < positionCount; i++)
        {
            float distance = Vector2.Distance(swipePositions[i], center);
            averageRadius += distance;
        }
        averageRadius /= positionCount;

        for (int i = 0; i < positionCount; i++)
        {
            float distance = Vector2.Distance(swipePositions[i], center);
            radiusVariance += Mathf.Abs(distance - averageRadius);
        }
        radiusVariance /= positionCount;

        float circleScore = 1f - (radiusVariance / averageRadius);
        Debug.Log($"Circle Score: {circleScore}, Distance: {totalDistance}");
        return circleScore >= circleTolerance;
    }

    private void CompleteRoll()
    {
        rollsCompleted++;
        rollAngle += rollStrength;

        if (centerPromptText != null)
        {
            int remaining = rollsRequired - rollsCompleted;
            if (remaining > 0)
                centerPromptText.text = $"Good! {remaining} more swipes!";
            else
                centerPromptText.text = "Perfect!";
        }

        Debug.Log($"Roll {rollsCompleted}/{rollsRequired} completed!");

        if (rollsCompleted >= rollsRequired)
        {
            EndStopDropRoll();
        }
    }

    public void TriggerOnFire()
    {
        if (isOnFire) return;

        isOnFire = true;
        rollsCompleted = 0;
        rollAngle = 0;
        isDropping = false;
        hasDropped = false;

        originalCameraPos = playerCamera.transform.localPosition;
        originalCameraRotation = playerCamera.transform.localRotation;
        targetCameraPos = originalCameraPos - Vector3.up * dropAmount;

        if (controller != null) controller.enabled = false;
        if (movementsScript != null) movementsScript.enabled = false;

        if (subtitleManager != null) subtitleManager.HideObjective();
        if (healthBar != null) healthBar.SetActive(false);
        if (oxygenBar != null) oxygenBar.SetActive(false);

        if (fireOverlay != null) fireOverlay.gameObject.SetActive(true);

        if (centerPromptText != null)
        {
            centerPromptText.text = "YOU'RE ON FIRE!\nSTOP! DROP! Get ready to ROLL!";
            centerPromptText.enabled = true;
        }

        Invoke("StartDropping", warningDuration);
        Debug.Log("Stop, Drop, and Roll sequence started!");
    }

    private void StartDropping()
    {
        isDropping = true;
        Debug.Log("Starting drop animation");
    }

    private void EndStopDropRoll()
    {
        isOnFire = false;
        isDropping = false;
        hasDropped = false;

        if (playerCamera != null)
        {
            playerCamera.transform.localPosition = originalCameraPos;
            playerCamera.transform.localRotation = originalCameraRotation;
            rollAngle = 0f;
        }

        if (controller != null) controller.enabled = true;
        if (movementsScript != null) movementsScript.enabled = true;

        if (healthBar != null) healthBar.SetActive(true);
        if (oxygenBar != null) oxygenBar.SetActive(true);

        if (fireOverlay != null) fireOverlay.gameObject.SetActive(false);

        if (centerPromptText != null)
        {
            centerPromptText.text = "Fire extinguished! You're safe!";
            Invoke("HideCenterText", 2f);
        }

        Debug.Log("Stop, Drop, and Roll complete!");

        // ✅ Notify SDRTrigger that SDR is done
        OnSDRComplete?.Invoke();
    }

    private void HideCenterText()
    {
        if (centerPromptText != null)
            centerPromptText.enabled = false;
    }
}
