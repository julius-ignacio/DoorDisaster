using EasyDoorSystem;
using UnityEngine;

[DisallowMultipleComponent]
public class DoorLockedWater : MonoBehaviour
{
    [Header("References")]
    public GameObject door;                  // Door with EasyDoor script
    public Transform waterPlane;             // Real water object
    public GameObject customCollider;        // Your blocker collider under the door

    [Header("Lock Settings")]
    [Range(0f, 1f)]
    public float stuckDepth = 0.5f;          // 0.5 = halfway up the door

    private EasyDoor doorScript;
    private Collider lockCollider;
    private float doorBottom;
    private float doorTop;
    private bool isLocked = false;

    void Start()
    {
        if (door != null)
            doorScript = door.GetComponent<EasyDoor>();

        if (customCollider != null)
        {
            lockCollider = customCollider.GetComponent<Collider>();
            if (lockCollider != null)
            {
                lockCollider.isTrigger = false; // Must block physically
                AdjustColliderCenter(lockCollider);
                lockCollider.enabled = false;   // Start disabled
            }
        }

        // Door vertical bounds
        doorBottom = transform.position.y;
        doorTop = doorBottom + transform.localScale.y;
    }

    void Update()
    {
        if (doorScript == null || waterPlane == null)
            return;

        float waterHeight = waterPlane.position.y;
        float stuckHeight = Mathf.Lerp(doorBottom, doorTop, stuckDepth);

        if (waterHeight >= stuckHeight && !isLocked)
            LockDoor();
        else if (waterHeight < stuckHeight && isLocked)
            UnlockDoor();
    }

    void LockDoor()
    {
        isLocked = true;

        // Physically block door
        if (lockCollider != null)
            lockCollider.enabled = true;

        // Disable EasyDoor script to prevent interaction
        if (doorScript != null)
            doorScript.enabled = false;

        Debug.Log("🚪 Door locked by water pressure!");
    }

    void UnlockDoor()
    {
        isLocked = false;

        // Remove physical block
        if (lockCollider != null)
            lockCollider.enabled = false;

        // Re-enable EasyDoor script
        if (doorScript != null)
            doorScript.enabled = true;

        Debug.Log("💧 Water lowered — door unlocked!");
    }

    public bool CanOpen()
    {
        return !isLocked;
    }

    /// <summary>
    /// Keeps the collider size as-is but repositions it for proper door blocking.
    /// </summary>
    void AdjustColliderCenter(Collider col)
    {
        if (col == null) return;

        if (col is BoxCollider box)
            box.center = new Vector3(box.center.x, transform.localScale.y / 2f, box.center.z + 0.1f);
        else if (col is CapsuleCollider capsule)
            capsule.center = new Vector3(capsule.center.x, transform.localScale.y / 2f, capsule.center.z + 0.1f);
        else
            col.transform.localPosition = new Vector3(col.transform.localPosition.x, transform.localScale.y / 2f, col.transform.localPosition.z + 0.1f);
    }
}
