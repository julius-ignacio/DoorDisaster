using UnityEngine;
using TMPro;
using EasyDoorSystem;

[RequireComponent(typeof(AudioSource))]
public class LockedDoor_Water : MonoBehaviour, IInteractable_Water
{
    [Header("Door Lock Settings")]
    [SerializeField] private string requiredKeyTag = "GarageKey";
    [SerializeField] private bool isLocked = true;
    [SerializeField] private AudioClip lockedSound;
    [SerializeField] private AudioClip unlockSound;
    [SerializeField, Range(0f, 1f)] private float audioVolume = 0.8f;

    [Header("UI Prompt Reference")]
    [SerializeField] private TMP_Text doorMessageText;
    [SerializeField] private float messageDuration = 2f;

    private EasyDoor easyDoor;
    private AudioSource audioSource;
    private InventoryManager_Water inventory;

    private void Awake()
    {
        easyDoor = GetComponent<EasyDoor>() ?? GetComponentInChildren<EasyDoor>() ?? GetComponentInParent<EasyDoor>();
        if (easyDoor == null)
            Debug.LogWarning($"⚠️ No EasyDoor component found on or under {name}");

        audioSource = GetComponent<AudioSource>();
        
        // 🔧 Find the correct inventory on "InventoryManager" GameObject
        GameObject invObj = GameObject.Find("InventoryManager");
        if (invObj != null)
        {
            inventory = invObj.GetComponent<InventoryManager_Water>();
            Debug.Log($"✅ LockedDoor found inventory on: {invObj.name} (Instance: {inventory.GetInstanceID()})");
        }
        else
        {
            inventory = FindFirstObjectByType<InventoryManager_Water>();
            Debug.LogWarning("⚠️ Using FindFirstObjectByType - multiple inventories may exist!");
        }
        
       

        if (doorMessageText != null)
            doorMessageText.text = "";
    }

    public void Interact()
    {
        if (easyDoor == null)
        {
            Debug.LogWarning($"❌ EasyDoor missing on {name}, cannot open door.");
            return;
        }

        if (isLocked)
        {
            if (HasRequiredKey())
            {
                UnlockDoor();
            }
            else
            {
                PlaySound(lockedSound);
                ShowDoorMessage($"🔒 Door is locked. You need the {FormatKeyName(requiredKeyTag)}.");
                Debug.Log($"🚪 {name} is locked. {requiredKeyTag} required.");
                return;
            }
        }

        easyDoor.ToggleDoor();
        Debug.Log($"🚪 {name} toggled {(easyDoor.IsOpen ? "open" : "closed")}");
    }

    private bool HasRequiredKey()
    {
        string cleanTag = requiredKeyTag.Trim();
        bool hasKey = false;

        if (inventory != null)
        {
            hasKey = inventory.HasItem(cleanTag);
            Debug.Log($"🔍 Inventory check for '{cleanTag}': {hasKey}");
        }

       
        Debug.Log($"🔑 [LockedDoor] Final result for {cleanTag} on {name} → {hasKey}");
        return hasKey;
    }

    private void UnlockDoor()
    {
        isLocked = false;
        PlaySound(unlockSound);
        ShowDoorMessage($"🔓 {FormatKeyName(requiredKeyTag)} used. Door unlocked!");
        Debug.Log($"✅ {name} unlocked with {requiredKeyTag}");

        if (inventory != null && inventory.HasItem(requiredKeyTag))
        {
            inventory.UseItem(requiredKeyTag);
            Debug.Log($"🗝️ {requiredKeyTag} consumed from inventory.");
        }
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

    private string FormatKeyName(string keyTag)
    {
        if (string.IsNullOrEmpty(keyTag)) return "Key";
        
        if (keyTag.EndsWith("Key", System.StringComparison.OrdinalIgnoreCase))
            return keyTag.Substring(0, keyTag.Length - 3) + " Key";
        
        return keyTag;
    }

    public string GetPrompt()
    {
        if (isLocked)
            return $"🔒 Locked ({FormatKeyName(requiredKeyTag)} required)";
        
        return "Press [F] to Open Door";
    }

    [ContextMenu("Force Unlock Door")]
    private void ForceUnlock()
    {
        isLocked = false;
        Debug.Log($"✅ {name} force unlocked!");
    }

    [ContextMenu("Lock Door")]
    private void LockDoor()
    {
        isLocked = true;
        Debug.Log($"🔒 {name} locked!");
    }
}