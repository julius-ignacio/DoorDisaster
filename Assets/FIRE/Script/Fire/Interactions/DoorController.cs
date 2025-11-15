using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DoorController : MonoBehaviour
{
    public enum MovementType { Rotation, Position, Both }

    [Header("Door Settings")]
    [SerializeField] private MovementType movementType = MovementType.Rotation;
    [SerializeField] public float openAngle = 90f;
    [SerializeField] public float openSpeed = 2f;
    [SerializeField] public float interactionDistance = 4f;
    [SerializeField] public bool smartDoorOpen = true;
    [Tooltip("The direction the door should open towards (e.g., forward vector)")]
    [SerializeField] private Vector3 doorForward = Vector3.right;

    [Header("Transform Targets")]
    [SerializeField] private Vector3 closedPosition;
    [SerializeField] private Vector3 openedPosition;

    [Header("Audio")]
    [SerializeField] private int doorOpenSoundIndex = -1;
    [SerializeField] private int doorCloseSoundIndex = -1;

    [Header("Item inside Cabinet")]
    public ItemPickup item;

    [Header("Events")]
    public UnityEvent OnDoorOpening;
    public UnityEvent OnDoorClosed;

    public bool IsOpen { get; private set; } = false;
    public bool IsMoving { get; private set; } = false;

    private Quaternion closedRotation;
    private Transform playerTransform;
    private Coroutine movementCoroutine;
    private bool playerInRange = false;
    public GameManager gameManager;

    void Start()
    {
        closedRotation = transform.localRotation;
        if (item != null)
            item.SetInteractable(false);

        playerTransform = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if (playerTransform == null || gameManager == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        bool playerNearby = distanceToPlayer <= interactionDistance;

        if (gameManager.isPaused)
        {
            if (playerInRange)
            {
                GenericPickupButton.Instance.HidePickupPrompt();
                playerInRange = false;
            }
            return;
        }

        if (playerNearby && !playerInRange && !IsMoving)
        {
            playerInRange = true;
            GenericPickupButton.Instance.ShowPickupPrompt(new DoorPickupAdapter(this), "Interact");
        }
        else if (!playerNearby && playerInRange)
        {
            playerInRange = false;
            GenericPickupButton.Instance.HidePickupPrompt();
        }
    }

    public void OpenDoor()
    {
        if (IsOpen || IsMoving) return;
        if (item != null && item.HasBeenPickedUp()) return;

        float angle = openAngle;

        if (smartDoorOpen && playerTransform != null)
        {
            Vector3 doorToPlayer = (playerTransform.position - transform.position).normalized;
            Vector3 doorDir = transform.TransformDirection(doorForward);
            float dot = Vector3.Dot(doorToPlayer, doorDir);

            Debug.Log($"[DoorController] Smart Open: dot={dot}, doorForward={doorForward}, doorDir={doorDir}");

            if (dot > 0.1f)
                angle = openAngle;
            else if (dot < -0.1f)
                angle = -openAngle;
            else
                angle = openAngle; // fallback
        }

        Quaternion targetOpenRotation = closedRotation * Quaternion.Euler(0, angle, 0);
        MoveDoor(transform.localPosition, targetOpenRotation.eulerAngles, true);
        PlaySound(doorOpenSoundIndex);
        OnDoorOpening.Invoke();
    }

    public void CloseDoor()
    {
        if (!IsOpen || IsMoving) return;

        MoveDoor(closedPosition, closedRotation.eulerAngles, false);
        PlaySound(doorCloseSoundIndex);
        OnDoorClosed.Invoke();
    }

    public void ToggleDoor()
    {
        if (IsMoving) return;

        if (IsOpen)
            CloseDoor();
        else
            OpenDoor();
    }

    private void MoveDoor(Vector3 targetPosition, Vector3 targetRotation, bool opening)
    {
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        movementCoroutine = StartCoroutine(AnimateDoor(targetPosition, targetRotation, opening));
    }

    private IEnumerator AnimateDoor(Vector3 targetPos, Vector3 targetRot, bool opening)
    {
        IsMoving = true;
        Quaternion startRot = transform.localRotation;
        Vector3 startPos = transform.localPosition;
        Quaternion targetQuaternion = Quaternion.Euler(targetRot);

        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * openSpeed;

            if (movementType != MovementType.Position)
                transform.localRotation = Quaternion.Slerp(startRot, targetQuaternion, progress);

            if (movementType != MovementType.Rotation)
                transform.localPosition = Vector3.Lerp(startPos, targetPos, progress);

            yield return null;
        }

        if (movementType != MovementType.Position)
            transform.localRotation = targetQuaternion;

        if (movementType != MovementType.Rotation)
            transform.localPosition = targetPos;

        if (item != null)
            item.SetInteractable(opening);

        IsOpen = opening;
        IsMoving = false;
    }

    private void PlaySound(int soundIndex)
    {
        if (soundIndex >= 0 && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(soundIndex);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}

public class DoorPickupAdapter : IPickupable
{
    private DoorController doorController;

    public DoorPickupAdapter(DoorController door)
    {
        doorController = door;
    }

    public void OnPickup()
    {
        doorController.ToggleDoor();
    }
}
