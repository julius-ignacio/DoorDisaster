using UnityEngine;
using TMPro;

public class InventoryManager_Water : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int totalItems = 9;
    private int itemsCollected = 0;

    [Header("Battery Storage")]
    public int batteryCount = 0; // how many batteries player currently has

    [Header("UI")]
    public TextMeshProUGUI inventoryText;

    void Start()
    {
        UpdateUI();
    }

    public void AddItem()
    {
        itemsCollected++;
        if (itemsCollected > totalItems)
            itemsCollected = totalItems;

        UpdateUI();
    }

    // ✅ Add a battery to inventory
    public void AddBattery()
    {
        batteryCount++;
        UpdateUI();
        Debug.Log($"<color=green>🔋 Battery added! Total: {batteryCount}</color>");
    }

    // ✅ Use one battery for recharging flashlight
    public bool UseBattery()
    {
        if (batteryCount > 0)
        {
            batteryCount--;
            UpdateUI();
            return true;
        }

        Debug.Log("<color=red>❌ No batteries left in inventory!</color>");
        return false;
    }

    private void UpdateUI()
    {
        if (inventoryText != null)
            inventoryText.text = $"Items: {itemsCollected}/{totalItems} | Batteries: {batteryCount}";
    }
}
