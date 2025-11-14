using UnityEngine;

public class FuseBoxTrigger : MonoBehaviour
{
    public SubtitleManager2 subtitleManager;
    private bool hasTriggered = false;
    private bool playerInside = false;

    void Start()
    {
        hasTriggered = false; // ✅ Reset on scene reload
        playerInside = false;

        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("FuseBoxTrigger: No Collider attached!");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning("FuseBoxTrigger: Collider is not marked as Trigger!");
        }
        Debug.Log("FuseBoxTrigger: Ready and waiting for player");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"FuseBoxTrigger: Player entered! Tag: {other.tag}, Name: {other.name}");
            playerInside = true;
            TryTriggerSubtitle();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerInside && !hasTriggered)
        {
            TryTriggerSubtitle();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("FuseBoxTrigger: Player exited");
            playerInside = false;
        }
    }

    void TryTriggerSubtitle()
    {
        // Only trigger if door has been opened with towel AND hasn't triggered yet AND breaker puzzle not complete
        if (!hasTriggered && HotDoorHandle.DoorOpenedWithTowel && !BreakerPuzzle.BreakerPuzzleComplete)
        {
            Debug.Log("FuseBoxTrigger: Conditions met! Showing subtitle...");
            hasTriggered = true;
            playerInside = false; // Prevent multiple triggers

            FuseBoxInteraction.FuseBoxObjectiveActive = true;
            Debug.Log("FuseBoxTrigger: Fuse box outline activated!");

            if (subtitleManager == null)
            {
                Debug.LogError("FuseBoxTrigger: SubtitleManager is not assigned!");
                return;
            }

            subtitleManager.ShowCustomMessage(
                "Wait, with all this smoke, I should turn off the electricity first. Where's the breaker?",
                4f,
                () => {
                    subtitleManager.ShowObjective("Find and turn off the main breaker");
                }
            );
        }
        else if (!HotDoorHandle.DoorOpenedWithTowel)
        {
            Debug.Log("FuseBoxTrigger: Player inside but waiting for door subtitles to finish...");
        }
    }

    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(1, 1, 0, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;
            if (col is BoxCollider box)
                Gizmos.DrawCube(box.center, box.size);
            else if (col is SphereCollider sphere)
                Gizmos.DrawSphere(sphere.center, sphere.radius);
        }
    }
}