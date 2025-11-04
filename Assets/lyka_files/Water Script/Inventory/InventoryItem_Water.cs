using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class InventoryItem_Water
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public GameObject item3DModel; // optional 3D model for preview
}

public class InventoryUI_Water : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel;
    public TextMeshProUGUI titleText;
    public Image itemIcon;
    public RawImage item3DPreview;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI controlHintsText;
    public Button leftButton;
    public Button rightButton;
    public Button equipButton;
    public Button useButton;

    [Header("Items")]
    public InventoryItem_Water[] items;
    private int currentIndex = 0;

    [Header("3D Preview Camera")]
    public Camera previewCamera;
    private GameObject currentPreviewModel;

    [Header("Mobile / PC Detection")]
    public bool isMobile = false;

    void Start()
    {
        // Auto-detect platform
#if UNITY_ANDROID || UNITY_IOS
        isMobile = true;
#else
        isMobile = false;
#endif

        // Hide panel at start
        inventoryPanel.SetActive(false);

        // Button listeners (for mobile)
        leftButton.onClick.AddListener(PreviousItem);
        rightButton.onClick.AddListener(NextItem);
        equipButton.onClick.AddListener(OnEquip);
        useButton.onClick.AddListener(OnUse);

        UpdateControlHints();
    }

    void Update()
    {
        // Toggle inventory visibility (PC only)
        if (!isMobile && Input.GetKeyDown(KeyCode.Tab))
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);

        if (!inventoryPanel.activeSelf) return;

        // Navigation (PC)
        if (!isMobile)
        {
            if (Input.GetKeyDown(KeyCode.A)) PreviousItem();
            if (Input.GetKeyDown(KeyCode.D)) NextItem();

            // Equip / Use (PC)
            if (Input.GetKeyDown(KeyCode.Space)) OnEquip();
            if (Input.GetKeyDown(KeyCode.F)) OnUse();
        }
    }

    public void ShowItem(int index)
    {
        if (items.Length == 0) return;
        index = Mathf.Clamp(index, 0, items.Length - 1);
        currentIndex = index;

        InventoryItem_Water item = items[index];
        titleText.text = "Inventory";
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;
        itemIcon.sprite = item.icon;

        // Handle 3D Preview
        if (currentPreviewModel != null)
            Destroy(currentPreviewModel);

        if (item.item3DModel != null && previewCamera != null)
        {
            currentPreviewModel = Instantiate(item.item3DModel, previewCamera.transform);
            currentPreviewModel.transform.localPosition = new Vector3(0, 0, 2f);
            currentPreviewModel.transform.localRotation = Quaternion.identity;
        }

        UpdateControlHints();
    }

    public void NextItem()
    {
        if (items.Length == 0) return;
        currentIndex = (currentIndex + 1) % items.Length;
        ShowItem(currentIndex);
    }

    public void PreviousItem()
    {
        if (items.Length == 0) return;
        currentIndex--;
        if (currentIndex < 0) currentIndex = items.Length - 1;
        ShowItem(currentIndex);
    }

    void OnEquip()
    {
        Debug.Log($"Equipped: {items[currentIndex].itemName}");
        // TODO: Add your equip logic here (e.g., flashlight activation, tool equip, etc.)
    }

    void OnUse()
    {
        Debug.Log($"Used: {items[currentIndex].itemName}");
        // TODO: Add your use logic here (e.g., consume battery, heal player, etc.)
    }

    void UpdateControlHints()
    {
        if (isMobile)
        {
            controlHintsText.text = "Tap buttons below to navigate or use items.";
        }
        else
        {
            controlHintsText.text = "Equip Item - (Space)\nUse Item - (F)";
        }
    }
}
