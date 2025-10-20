using UnityEngine;

[DisallowMultipleComponent]
public class DoorInteractWater : MonoBehaviour, IInteractable_Water
{
    [Header("Door Settings")]
    public Transform doorHinge;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;

    public DoorLockedWater waterLock;

    void Start()
    {
        if (doorHinge == null) doorHinge = transform;
        closedRot = doorHinge.localRotation;
        openRot = Quaternion.Euler(0f, openAngle, 0f) * closedRot;

        if (waterLock == null)
            waterLock = GetComponent<DoorLockedWater>();
    }

    public string GetPrompt()
    {
        if (waterLock != null && !waterLock.CanOpen())
            return "The door is stuck under the water";

        return isOpen ? "Press E to close" : "Press E to open";
    }

    public void Interact()
    {
        if (waterLock != null && !waterLock.CanOpen())
        {
            Debug.Log("Door is stuck — can't open while submerged.");
            return;
        }

        isOpen = !isOpen;
        StopAllCoroutines();
        StartCoroutine(RotateDoor(isOpen ? openRot : closedRot));
    }

    private System.Collections.IEnumerator RotateDoor(Quaternion targetRot)
    {
        while (Quaternion.Angle(doorHinge.localRotation, targetRot) > 0.01f)
        {
            doorHinge.localRotation = Quaternion.Slerp(
                doorHinge.localRotation, targetRot, Time.deltaTime * openSpeed);
            yield return null;
        }
    }
}
