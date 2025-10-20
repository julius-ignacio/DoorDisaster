using UnityEngine;
using TMPro;
using EasyDoorSystem;

[RequireComponent(typeof(EasyDoor))]
[RequireComponent(typeof(AudioSource))]
public class LockedDoor_Water : MonoBehaviour, IInteractable_Water
{
    [Header("Door Lock Settings")]
    [SerializeField] private string requiredKeyTag; // e.g., "GarageKey"
    [SerializeField] private bool isLocked = true;
    [SerializeField] private AudioClip lockedSound;
    [SerializeField] private AudioClip unlockSound;
    [SerializeField, Range(0f, 1f)] private float audioVolume = 0.8f;

    [Header("UI Prompt Reference")]
    [SerializeField] private TMP_Text doorMessageText;
    [SerializeField] private float messageDuration = 2f;

    [Header("Optional Puzzle Reference")]
    [SerializeField] private PuzzleManager_Water puzzleManager; // Drag PuzzleManager here
    [SerializeField] private bool requiresPuzzleAfterKey = false;

    private EasyDoor easyDoor;
    private AudioSource audioSource;
    private PlayerInteractor_Water playerInteractor;
    private bool puzzleActivated = false;

    private void Awake()
    {
        easyDoor = GetComponent<EasyDoor>();
        audioSource = GetComponent<AudioSource>();
        playerInteractor = FindFirstObjectByType<PlayerInteractor_Water>();

        if (doorMessageText != null)
            doorMessageText.text = "";

        if (puzzleManager != null)
            puzzleManager.SetLinkedDoor(this); // auto-link the door to the puzzle
    }

    public void Interact()
    {
        if (!easyDoor) return;

        if (isLocked)
        {
            if (HasRequiredKey())
            {
                if (requiresPuzzleAfterKey && !puzzleActivated)
                {
                    ActivatePuzzle();
                    return;
                }

                UnlockDoor();
            }
            else
            {
                PlaySound(lockedSound);
                ShowDoorMessage($"Door is locked. You need the {requiredKeyTag} to open it.");
                Debug.Log($"Door is locked. You need the {requiredKeyTag} to open it.");
                return;
            }
        }

        easyDoor.ToggleDoor();
    }

    private bool HasRequiredKey()
    {
        if (!playerInteractor) return false;

        return requiredKeyTag switch
        {
            "OfficeKey" => playerInteractor.HasOfficeKey,
            "BasementKey" => playerInteractor.HasBasementKey,
            "BedroomKey" => playerInteractor.HasBedroomKey,
            "GarageKey" => playerInteractor.HasGarageKey,
            "StudyKey" => playerInteractor.HasStudyKey,
            "BalconyKey" => playerInteractor.HasBalconyKey,
            _ => false,
        };
    }

    private void UnlockDoor()
    {
        isLocked = false;
        PlaySound(unlockSound);
        ShowDoorMessage($"{requiredKeyTag} used. Door unlocked!");
        Debug.Log($"{requiredKeyTag} used. Door unlocked!");
    }

    private void ActivatePuzzle()
    {
        if (puzzleManager != null)
        {
            puzzleManager.ActivatePuzzle();
            puzzleActivated = true;
            ShowDoorMessage("Puzzle activated! Solve it to unlock the garage.");
            Debug.Log("Puzzle activated for this door.");
        }
        else
        {
            Debug.LogWarning("requiresPuzzleAfterKey is true, but no PuzzleManager assigned!");
        }
    }

    public void PuzzleSolved_UnlockDoor()
    {
        UnlockDoor();
    }

    private void PlaySound(AudioClip clip)
    {
        if (!clip || !audioSource) return;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.volume = audioVolume;
        audioSource.Play();
    }

    private void ShowDoorMessage(string message)
    {
        if (doorMessageText == null) return;

        doorMessageText.text = message;
        CancelInvoke(nameof(ClearDoorMessage));
        Invoke(nameof(ClearDoorMessage), messageDuration);
    }

    private void ClearDoorMessage()
    {
        if (doorMessageText != null)
            doorMessageText.text = "";
    }

    public string GetPrompt()
    {
        return isLocked ? $"Locked ({requiredKeyTag} required)" : "Open Door";
    }
}
