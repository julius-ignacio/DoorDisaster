using UnityEngine;

public class FuseBoxTrigger : MonoBehaviour
{
    public SubtitleManager2 subtitleManager;
    private bool hasTriggered = false;

    void Start()
    {
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
        Debug.Log($"FuseBoxTrigger: Something entered! Tag: {other.tag}, Name: {other.name}");

        // ✅ Only trigger if door has been opened with towel AND subtitles finished
        if (other.CompareTag("Player") && !hasTriggered && HotDoorHandle.DoorOpenedWithTowel)
        {
            Debug.Log("FuseBoxTrigger: Player detected! Showing subtitle...");
            hasTriggered = true;

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
        else if (other.CompareTag("Player") && !HotDoorHandle.DoorOpenedWithTowel)
        {
            Debug.Log("FuseBoxTrigger: Player entered but door subtitles not finished yet");
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