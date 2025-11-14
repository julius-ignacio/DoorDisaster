using UnityEngine;

public class FuseBoxInteraction : MonoBehaviour, IPickupable
{
    [Header("References")]
    public BreakerPuzzle breakerPuzzle;

    [Header("Outline Settings")]
    private Outline outline;

    // ✅ Static flag set by FuseBoxTrigger
    public static bool FuseBoxObjectiveActive { get; set; } = false;

    private bool hasInteracted = false;
    private bool playerInRange = false;

    void Start()
    {
        // ❌ DON'T reset - this flag needs to persist when loading saves
        // BreakerPuzzleComplete = false;  // REMOVED

        // Get outline component
        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false; // Hidden at start

        // Debug checks
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("FuseBoxInteraction: No Collider attached!");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning("FuseBoxInteraction: Collider is not marked as Trigger!");
        }

        if (breakerPuzzle == null)
        {
            Debug.LogError("FuseBoxInteraction: BreakerPuzzle is not assigned!");
        }

        Debug.Log($"FuseBoxInteraction.Start(): BreakerPuzzleComplete={BreakerPuzzle.BreakerPuzzleComplete}");
    }

    void Update()
    {
        // ✅ Show outline only after FuseBoxTrigger has been activated
        if (outline != null && !hasInteracted)
        {
            outline.enabled = FuseBoxObjectiveActive;
        }

        // Check if player is still in range when story completes
        if (playerInRange && !hasInteracted && SubtitleManager2.IntroStoryComplete)
        {
            // Show the prompt if it wasn't shown before (because story just completed)
            if (GenericPickupButton.Instance != null && FuseBoxObjectiveActive)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Turn Off Breaker");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"FuseBoxInteraction: Something entered! Tag: {other.tag}, Name: {other.name}");

        if (other.CompareTag("Player") && !hasInteracted)
        {
            playerInRange = true;

            // Only show prompt if intro story is complete AND objective is active
            if (SubtitleManager2.IntroStoryComplete && FuseBoxObjectiveActive)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Turn Off Breaker");
                Debug.Log("FuseBoxInteraction: Player in range - showing prompt");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            GenericPickupButton.Instance.HidePickupPrompt();
            Debug.Log("FuseBoxInteraction: Player left range");
        }
    }

    public void OnPickup()
    {
        if (!playerInRange || hasInteracted) return;

        // Extra safety check
        if (!SubtitleManager2.IntroStoryComplete || !FuseBoxObjectiveActive)
        {
            Debug.Log("Cannot interact with fuse box yet");
            return;
        }

        hasInteracted = true;
        GenericPickupButton.Instance.HidePickupPrompt();

        // Disable outline
        if (outline != null)
            outline.enabled = false;

        Debug.Log("FuseBoxInteraction: Opening breaker puzzle!");

        // Open the puzzle directly
        if (breakerPuzzle != null)
        {
            breakerPuzzle.ShowPuzzle();
        }
        else
        {
            Debug.LogError("FuseBoxInteraction: BreakerPuzzle reference is missing!");
        }
    }

    /// <summary>
    /// Check if the breaker puzzle has been completed
    /// </summary>
    public bool IsTurnedOff()
    {
        return BreakerPuzzle.BreakerPuzzleComplete || hasInteracted;
    }

    /// <summary>
    /// Restore the turned-off state when loading a save
    /// </summary>
    public void RestoreTurnedOffState()
    {
        hasInteracted = true;

        if (outline != null)
            outline.enabled = false;

        Debug.Log("FuseBoxInteraction: Restored turned-off state from save");
    }

    // Visual debug in Scene view
    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(0, 0, 1, 0.3f); // Blue
            Gizmos.matrix = transform.localToWorldMatrix;
            if (col is BoxCollider box)
                Gizmos.DrawCube(box.center, box.size);
            else if (col is SphereCollider sphere)
                Gizmos.DrawSphere(sphere.center, sphere.radius);
        }
    }
}