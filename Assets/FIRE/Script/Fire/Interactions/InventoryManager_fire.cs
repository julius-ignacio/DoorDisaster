using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryManager_fire : MonoBehaviour
{
    public static InventoryManager_fire Instance;

    [Header("UI References")]
    public GameObject inventoryPanel;
    public GameObject backpackButton;

    [Header("Backpack Model Reference")]
    public GameObject backpack_model;

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
    private bool isBackpackUnlocked = false;

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

        if (backpackButton != null)
        {
            Button btn = backpackButton.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(ToggleInventory);
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

                    if(backpack_model != null)
        {
            if(!backpack_model.activeSelf)
            {
                backpackButton.SetActive(true);
            }
        }

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

        // ✅ NEW: Close inventory by touching/clicking outside the panel
        if (isInventoryOpen && inventoryPanel != null)
        {
            // Check for touch input
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (!IsTouchOverUI(Input.GetTouch(0).position))
                {
                    CloseInventory();
                }
            }
            // Check for mouse click (editor testing)
            else if (Input.GetMouseButtonDown(0))
            {
                if (!IsTouchOverUI(Input.mousePosition))
                {
                    CloseInventory();
                }
            }
        }
    }

    // ✅ NEW: Check if touch/click is over the inventory panel
    private bool IsTouchOverUI(Vector2 position)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = position;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Check if any of the hits are the inventory panel or its children
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == inventoryPanel || result.gameObject.transform.IsChildOf(inventoryPanel.transform))
            {
                return true;
            }
        }

        return false;
    }

    public void ToggleInventory()
    {
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

                // ✅ Notify subtitle manager
                SubtitleManager2 subtitleManager = FindObjectOfType<SubtitleManager2>();
                if (subtitleManager != null)
                    subtitleManager.OnInventoryOpen();
            }
            else
            {
                Time.timeScale = 1f;
                if (GameManager.Instance != null)
                    GameManager.Instance.isPaused = false;

                // ✅ Notify subtitle manager
                SubtitleManager2 subtitleManager = FindObjectOfType<SubtitleManager2>();
                if (subtitleManager != null)
                    subtitleManager.OnInventoryClose();
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

            // ✅ Notify subtitle manager
            SubtitleManager2 subtitleManager = FindObjectOfType<SubtitleManager2>();
            if (subtitleManager != null)
                subtitleManager.OnInventoryClose();
        }
    }

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

    // ✅ NEW: Public method to check if inventory is open
    public bool IsInventoryOpen()
    {
        return isInventoryOpen;
    }

    public void OnPause()
    {
        wasInventoryOpenBeforePause = isInventoryOpen;

        if (isInventoryOpen && inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (backpackButton != null)
            backpackButton.SetActive(false);

        Debug.Log($"Inventory paused. Was open: {wasInventoryOpenBeforePause}");
    }

    public void OnResume()
    {
        if (wasInventoryOpenBeforePause && inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }

        if (backpackButton != null && isBackpackUnlocked)
            backpackButton.SetActive(true);

        Debug.Log($"Inventory resumed. Restoring open state: {wasInventoryOpenBeforePause}");
    }
}