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
    [SerializeField] public bool automaticOpen = true;
    [SerializeField] public bool smartDoorOpen = true;
    [Tooltip("The direction the door should open towards (e.g., forward vector)")]
    [SerializeField] private Vector3 doorForward = Vector3.right;
    [Tooltip("Automatically close after specified time (0 = no auto-close)")]
    [SerializeField] private float autoCloseDelay = 5f;

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
    private Quaternion openRotation;
    private Transform playerTransform;
    private Coroutine movementCoroutine;
    private bool playerInRange = false;

    void Awake()
    {
        // AudioSource no longer needed since we're using AudioManager
    }

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

        if (item != null)
            item.SetInteractable(false);

        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        bool playerNearby = distanceToPlayer <= interactionDistance;

        if (automaticOpen)
        {
            // Auto-open when player enters range
            if (playerNearby && !IsOpen && !IsMoving)
                OpenDoor();

            // Auto-close when player leaves range
            if (!playerNearby && IsOpen && !IsMoving)
                CloseDoor();
        }
        else
        {
            // Show interaction prompt only if not auto-opening
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
    }

    public void OpenDoor()
    {
        if (IsOpen || IsMoving) return;
        if (item != null && item.HasBeenPickedUp()) return;

        Vector3 targetPos = movementType != MovementType.Rotation ? openedPosition : transform.localPosition;
        float angle = openAngle;

        // Smart door: open away from player
        if (smartDoorOpen && playerTransform != null)
        {
            Vector3 doorToPlayer = (playerTransform.position - transform.position).normalized;
            float dot = Vector3.Dot(doorToPlayer, transform.TransformDirection(doorForward));
            angle = dot > 0 ? openAngle : -openAngle;
        }

        Quaternion targetOpenRotation = closedRotation * Quaternion.Euler(0, angle, 0);
        Vector3 targetRot = targetOpenRotation.eulerAngles;

        MoveDoor(targetPos, targetRot, true);
        PlaySound(doorOpenSoundIndex);
        OnDoorOpening.Invoke();
    }

    public void CloseDoor()
    {
        if (!IsOpen || IsMoving) return;

        Vector3 targetPos = movementType != MovementType.Rotation ? closedPosition : transform.localPosition;
        Vector3 targetRot = movementType != MovementType.Position ? closedRotation.eulerAngles : transform.localEulerAngles;

        MoveDoor(targetPos, targetRot, false);
        PlaySound(doorCloseSoundIndex);
        OnDoorClosed.Invoke();
    }

    public void ToggleDoor()
    {
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

        float progress = 0;
        float maxSpeed = Mathf.Max(openSpeed, openSpeed); // Can separate rotation/position speeds if needed

        while (progress < 1)
        {
            progress += Time.deltaTime * maxSpeed;

            if (movementType != MovementType.Position)
            {
                transform.localRotation = Quaternion.Slerp(startRot, targetQuaternion, progress);
            }

            if (movementType != MovementType.Rotation)
            {
                transform.localPosition = Vector3.Lerp(startPos, targetPos, progress);
            }

            yield return null;
        }

        // Ensure final positions are exact
        if (movementType != MovementType.Position)
            transform.localRotation = targetQuaternion;

        if (movementType != MovementType.Rotation)
            transform.localPosition = targetPos;

        // Enable or disable item only after fully open
        if (item != null)
            item.SetInteractable(opening);

        IsOpen = opening;
        IsMoving = false;

        // Auto-close logic
        if (autoCloseDelay > 0 && IsOpen)
            Invoke(nameof(CloseDoor), autoCloseDelay);
    }

    private void PlaySound(int soundIndex)
    {
        if (soundIndex >= 0 && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(soundIndex);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 gizmoOffset = new Vector3(0, 0, 0); // Adjust X, Y, Z as needed
        Gizmos.DrawWireSphere(transform.position + gizmoOffset, interactionDistance);
    }
}

// Adapter to make DoorController work with IPickupable system
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