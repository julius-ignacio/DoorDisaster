using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI References")]
    public GameObject inventoryPanel;
    public GameObject backpackButton;

    [Header("Oxygen Slot")]
    public GameObject oxygenSlot;
    public Image oxygenIcon;
    public TextMeshProUGUI oxygenCountText;
    public Button oxygenUseButton;

    [Header("Essential Items Slot")]
    public GameObject essentialItemsSlot;
    public Image essentialItemsIcon;
    public TextMeshProUGUI essentialItemsCountText;
    public TextMeshProUGUI essentialItemsNameText;

    [Header("Player Reference")]
    public Transform player;

    private int oxygenCount = 0;
    private int essentialItemsCount = 0;
    private bool isInventoryOpen = false;
    private bool wasInventoryOpenBeforePause = false;
    private bool isBackpackUnlocked = false; // ✅ NEW: Controls when backpack becomes available

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        // ✅ Hide backpack button at start - only show after 911 call
        if (backpackButton != null)
            backpackButton.SetActive(false);

        if (oxygenUseButton != null)
        {
            oxygenUseButton.onClick.RemoveAllListeners();
            oxygenUseButton.onClick.AddListener(UseOxygenCanister);
        }

        if (oxygenSlot != null)
            oxygenSlot.SetActive(false);

        if (essentialItemsSlot != null)
            essentialItemsSlot.SetActive(false);

        if (oxygenCountText != null)
            oxygenCountText.text = "0";

        if (essentialItemsCountText != null)
            essentialItemsCountText.text = "0";

        if (essentialItemsNameText != null)
            essentialItemsNameText.text = "ESSENTIAL ITEMS";

        // Setup button listener
        if (backpackButton != null)
        {
            Button btn = backpackButton.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(ToggleInventory);
        }
    }

    void Update()
    {
        // ✅ Don't allow toggling inventory when game is paused or backpack not unlocked
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        if (!isBackpackUnlocked)
            return;

        // Toggle inventory with I or Tab key
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        // Close inventory with Escape (if open)
        if (Input.GetKeyDown(KeyCode.Escape) && isInventoryOpen)
        {
            CloseInventory();
        }
    }

    public void ToggleInventory()
    {
        // ✅ Don't toggle if game is paused or backpack not unlocked
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        if (!isBackpackUnlocked)
        {
            Debug.Log("Backpack not unlocked yet!");
            return;
        }

        if (inventoryPanel != null)
        {
            isInventoryOpen = !isInventoryOpen;
            inventoryPanel.SetActive(isInventoryOpen);

            if (isInventoryOpen)
            {
                Time.timeScale = 0f;
                if (GameManager.Instance != null)
                    GameManager.Instance.isPaused = true;
            }
            else
            {
                Time.timeScale = 1f;
                if (GameManager.Instance != null)
                    GameManager.Instance.isPaused = false;
            }
        }
    }

    public void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            isInventoryOpen = false;
            inventoryPanel.SetActive(false);
            Time.timeScale = 1f;

            if (GameManager.Instance != null)
                GameManager.Instance.isPaused = false;
        }
    }

    // ✅ NEW: Call this after 911 call to unlock backpack
    public void UnlockBackpack()
    {
        isBackpackUnlocked = true;

        if (backpackButton != null)
            backpackButton.SetActive(true);

        Debug.Log("Backpack unlocked and button shown!");
    }

    public void AddOxygenCanister()
    {
        oxygenCount++;

        if (oxygenSlot != null)
            oxygenSlot.SetActive(true);

        if (oxygenCountText != null)
            oxygenCountText.text = oxygenCount.ToString();

        Debug.Log($"Oxygen canister added to inventory. Total: {oxygenCount}");
    }

    public void AddEssentialItem()
    {
        essentialItemsCount++;

        if (essentialItemsSlot != null)
            essentialItemsSlot.SetActive(true);

        if (essentialItemsCountText != null)
            essentialItemsCountText.text = essentialItemsCount.ToString();

        Debug.Log($"Essential item added to inventory. Total: {essentialItemsCount}");
    }

    public void UseOxygenCanister()
    {
        if (oxygenCount <= 0 || player == null)
        {
            Debug.Log("No oxygen canisters available!");
            return;
        }

        PlayerOxygen oxygen = player.GetComponent<PlayerOxygen>();
        if (oxygen != null)
        {
            oxygen.RefillOxygen();
            oxygenCount--;

            if (oxygenCountText != null)
                oxygenCountText.text = oxygenCount.ToString();

            if (oxygenCount <= 0 && oxygenSlot != null)
                oxygenSlot.SetActive(false);

            Debug.Log($"Oxygen canister used! Remaining: {oxygenCount}");
        }
    }

    public int GetOxygenCount()
    {
        return oxygenCount;
    }

    public int GetEssentialItemsCount()
    {
        return essentialItemsCount;
    }

    public bool IsBackpackUnlocked()
    {
        return isBackpackUnlocked;
    }

    // ✅ Called by GameManager when pausing
    public void OnPause()
    {
        // ✅ Remember if inventory was open
        wasInventoryOpenBeforePause = isInventoryOpen;

        // ✅ Hide inventory if it was open
        if (isInventoryOpen && inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        // ✅ Hide backpack button
        if (backpackButton != null)
            backpackButton.SetActive(false);

        Debug.Log($"Inventory paused. Was open: {wasInventoryOpenBeforePause}");
    }

    // ✅ Called by GameManager when resuming
    public void OnResume()
    {
        // ✅ Restore inventory if it was open before pause
        if (wasInventoryOpenBeforePause && inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }

        // ✅ Show backpack button again (only if unlocked)
        if (backpackButton != null && isBackpackUnlocked)
            backpackButton.SetActive(true);

        Debug.Log($"Inventory resumed. Restoring open state: {wasInventoryOpenBeforePause}");
    }
}