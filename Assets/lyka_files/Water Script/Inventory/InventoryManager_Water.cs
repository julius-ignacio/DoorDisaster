using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InventoryManager_Water : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int totalItems = 9;
    private int itemsCollected = 0;

    [Header("Item Storage")]
    public int snackCount = 0;
    public int keyCount = 0;

    private Dictionary<string, int> itemDictionary = new Dictionary<string, int>();

    [Header("UI")]
    public GameObject inventoryUI;
    public TextMeshProUGUI snackCountText;
    public TextMeshProUGUI inventoryText;

    [Header("References")]
    public PlayerOxygen_Water playerOxygen;
    public HeartSysWater heartSys;
    public AudioSource eatSound;
    public PlayerController_Water playerController;

    private bool isInventoryOpen = false;

    private void Start()
    {
        if (inventoryUI != null)
            inventoryUI.SetActive(false);

        if (heartSys == null)
            heartSys = FindObjectOfType<HeartSysWater>();

        if (playerController == null)
            playerController = FindObjectOfType<PlayerController_Water>();

  


        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            ToggleInventory();

        if (Input.GetKeyDown(KeyCode.H))
            UseSnack();

        // 💊 Heal shortcut key (optional)
        if (Input.GetKeyDown(KeyCode.J))
            HealPlayer(2); // Heal by 2 hearts for testing
    }

    public void ToggleInventory()
    {
        if (isInventoryOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    public void OpenInventory()
    {
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(true);
            isInventoryOpen = true;

            if (playerController != null)
                playerController.enabled = false;


            Debug.Log("📂 Inventory opened");
        }
    }

    public void CloseInventory()
    {
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
            isInventoryOpen = false;

            if (playerController != null)
                playerController.enabled = true;


            Debug.Log("📁 Inventory closed");
        }
    }

    public void AddItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogWarning("⚠️ Tried to add an empty or null item name!");
            return;
        }

        Debug.Log($"🔍 AddItem called with: '{itemName}' on GameObject: {gameObject.name}");

        itemsCollected = Mathf.Min(itemsCollected + 1, totalItems);
        string lower = itemName.ToLower();

        switch (lower)
        {
            case "snack":
                snackCount++;
                Debug.Log($"<color=green>🍫 Snack added! Total: {snackCount}</color>");
                break;

            default:
                if (!itemDictionary.ContainsKey(itemName))
                    itemDictionary[itemName] = 0;
                itemDictionary[itemName]++;

                if (itemName.ToLower().Contains("key"))
                    keyCount++;

                Debug.Log($"<color=green>📦 Added: '{itemName}' (x{itemDictionary[itemName]})</color>");
                break;
        }

        UpdateUI();
    }

    public bool HasItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
            return false;

        string lower = itemName.ToLower();

        if (lower == "snack")
            return snackCount > 0;

        foreach (var key in itemDictionary.Keys)
        {
            if (key.ToLower() == lower)
                return itemDictionary[key] > 0;
        }

        return false;
    }

    public bool UseItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
            return false;

        bool used = false;
        string lower = itemName.ToLower();

        switch (lower)
        {
            case "snack":
                if (snackCount > 0)
                {
                    snackCount--;
                    used = true;
                }
                break;

            default:
                string foundKey = null;
                foreach (var key in itemDictionary.Keys)
                {
                    if (key.ToLower() == lower)
                    {
                        foundKey = key;
                        break;
                    }
                }

                if (foundKey != null && itemDictionary[foundKey] > 0)
                {
                    itemDictionary[foundKey]--;
                    used = true;

                    if (foundKey.ToLower().Contains("key"))
                        keyCount = Mathf.Max(0, keyCount - 1);
                }
                break;
        }

        if (used)
        {
            itemsCollected = Mathf.Max(0, itemsCollected - 1);
            UpdateUI();
            Debug.Log($"<color=yellow>✅ Used item: {itemName}</color>");
            return true;
        }

        Debug.Log($"<color=red>❌ No {itemName} available to use!</color>");
        return false;
    }

    // 🍫 Eat a snack to heal
    public void UseSnack()
    {
        if (snackCount <= 0)
        {
            Debug.Log("❌ No snacks left to use!");
            return;
        }

        // 🚫 Prevent overhealing
        if (heartSys != null && heartSys.currentHearts >= heartSys.maxHearts)
        {
            Debug.Log("<color=yellow>❤️ Health already full — no need to eat a snack!</color>");
            return;
        }

        snackCount--;
        itemsCollected = Mathf.Max(0, itemsCollected - 1);
        UpdateUI();

        HealPlayer(1); // Heals 1 heart per snack
        eatSound?.Play();

        Debug.Log("<color=green>🍫 Snack eaten! Player healed by 1 heart!</color>");
    }

    // 💖 General heal function (you can call from anywhere)
    public void HealPlayer(int amount)
    {
        if (heartSys == null)
        {
            Debug.LogWarning("⚠️ HealPlayer failed — no HeartSysWater reference found!");
            return;
        }

        if (amount <= 0)
        {
            Debug.LogWarning("⚠️ Heal amount must be positive!");
            return;
        }

        heartSys.RefillHeart(amount);
        Debug.Log($"<color=lime>💖 Player healed by {amount} hearts!</color>");
    }

    private void UpdateUI()
    {
        if (snackCountText != null)
            snackCountText.text = snackCount.ToString();

        if (inventoryText != null)
        {
            string uiText =
                $"Items: {itemsCollected}/{totalItems}\n" +
                $"🍫 Snacks: {snackCount}\n" +
                $"🗝️ Keys: {keyCount}";

            foreach (var pair in itemDictionary)
            {
                if (pair.Value > 0)
                    uiText += $"\n📦 {pair.Key}: {pair.Value}";
            }

            inventoryText.text = uiText;
        }
    }
}
