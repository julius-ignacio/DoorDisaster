using UnityEngine;

namespace EasyDoorSystem
{
    /// <summary>
    /// Makes a door locked. Player needs a key to unlock it.
    /// Attach this to the same GameObject as EasyDoor.
    /// </summary>
    [RequireComponent(typeof(EasyDoor))]
    public class LockedDoor : MonoBehaviour, IInteractable_Water
    {
        // ========== SETTINGS ==========
        
        [Header("Lock Settings")]
        [SerializeField] private bool isLocked = true;
        [SerializeField] private string requiredKeyID = "OfficeKey";

        [Header("UI Prompts")]
        [SerializeField] private string lockedPrompt = "Press [F] to open door (LOCKED - Need {KEY})";
        [SerializeField] private string unlockedPrompt = "Press [F] to open door";

        [Header("Audio (Optional)")]
        [SerializeField] private AudioClip lockedSound;

        // ========== WHAT OTHER SCRIPTS CAN READ ==========
        
        public bool IsLocked => isLocked;
        public string RequiredKeyID => requiredKeyID;

        // ========== PRIVATE VARIABLES ==========
        
        private EasyDoor door;
        private AudioSource audioSource;

        // ========== SETUP ==========
        
        void Awake()
        {
            door = GetComponent<EasyDoor>();
            audioSource = GetComponent<AudioSource>();
        }

        // ========== INTERFACE IMPLEMENTATION ==========

        /// <summary>
        /// Returns the prompt text to show the player
        /// </summary>
        public string GetPrompt()
        {
            if (isLocked)
            {
                // Show which key is needed
                return lockedPrompt.Replace("{KEY}", requiredKeyID);
            }
            return unlockedPrompt;
        }

        /// <summary>
        /// Called when player presses interact key (F)
        /// </summary>
        public void Interact()
        {
            TryToggleDoor();
        }

        // ========== TRY TO OPEN DOOR ==========
        
        public bool TryOpenDoor()
        {
            if (isLocked)
            {
                if (KeyInventory.Instance != null && KeyInventory.Instance.HasKey(requiredKeyID))
                {
                    UnlockDoor();
                    door.OpenDoor();
                    return true;
                }
                else
                {
                    OnLockedAttempt();
                    return false;
                }
            }

            door.OpenDoor();
            return true;
        }

        public bool TryToggleDoor()
        {
            if (isLocked)
            {
                if (KeyInventory.Instance != null && KeyInventory.Instance.HasKey(requiredKeyID))
                {
                    UnlockDoor();
                    door.ToggleDoor();
                    return true;
                }
                else
                {
                    OnLockedAttempt();
                    return false;
                }
            }

            door.ToggleDoor();
            return true;
        }

        // ========== UNLOCK DOOR ==========
        
        public void UnlockDoor()
        {
            if (!isLocked) return;

            isLocked = false;
            Debug.Log($"🔓 Door unlocked: {gameObject.name}");
        }

        public void LockDoor()
        {
            if (isLocked) return;

            isLocked = true;
            Debug.Log($"🔒 Door locked: {gameObject.name}");
        }

        // ========== WHEN PLAYER TRIES LOCKED DOOR ==========
        
        private void OnLockedAttempt()
        {
            if (lockedSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(lockedSound);
            }

            Debug.Log($"❌ Door is locked! Need key: {requiredKeyID}");
        }

        // ========== SHOW IN EDITOR ==========
        
        private void OnDrawGizmos()
        {
            if (isLocked)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
            }
        }
    }
}
