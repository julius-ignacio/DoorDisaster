using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PlayerInteractor_Water : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InventoryManager_Water inventoryManager;
    [SerializeField] private HeartSysWater heartSystem;
    [SerializeField] private FlashlightController_Water flashlightController;
    [SerializeField] private ObjectiveItemTracker_Water itemTracker;
    [SerializeField] private ObjectiveManager_Water objectiveManager;

    [Header("Interaction Settings")]
    [SerializeField] private float interactDistance = 3.5f;
    [SerializeField] private float collectDistance = 3.5f;
    [SerializeField] private LayerMask interactMask = ~0;

    [Header("UI Prompt")]
    [SerializeField] private GameObject interactPromptUI;
    [SerializeField] private Text interactPromptText;

    [Header("Keybinds")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private KeyCode collectKey = KeyCode.E;

    [Header("Mobile / UI Buttons")]
    [SerializeField] private Button interactButton;
    [SerializeField] private Button collectButton;

    private GameObject highlightedObject;
    private IInteractable_Water currentInteractable;

    private bool hasBag = false;
    private bool hasFlashlight = false;

    // Player Keys
    public bool HasOfficeKey { get; private set; }
    public bool HasBasementKey { get; private set; }
    public bool HasBedroomKey { get; private set; }
    public bool HasGarageKey { get; private set; }
    public bool HasStudyKey { get; private set; }
    public bool HasBalconyKey { get; private set; }

    private void Awake()
    {
        playerCamera ??= GetComponentInChildren<Camera>() ?? Camera.main;
        heartSystem ??= FindObjectOfType<HeartSysWater>();
        flashlightController ??= FindObjectOfType<FlashlightController_Water>();
        objectiveManager ??= FindObjectOfType<ObjectiveManager_Water>();
        itemTracker ??= FindObjectOfType<ObjectiveItemTracker_Water>();

        if (inventoryManager == null)
        {
            GameObject invObj = GameObject.Find("InventoryManager");
            if (invObj != null)
                inventoryManager = invObj.GetComponent<InventoryManager_Water>();
        }
    }

    private void Start()
    {
        if (interactButton != null)
        {
            interactButton.onClick.RemoveAllListeners();
            interactButton.onClick.AddListener(() => HandleInteract());
            interactButton.gameObject.SetActive(false);
        }

        if (collectButton != null)
        {
            collectButton.onClick.RemoveAllListeners();
            collectButton.onClick.AddListener(() => HandleCollect());
            collectButton.gameObject.SetActive(false);
        }

        HidePrompt();
    }

    private void Update()
    {
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * Mathf.Max(interactDistance, collectDistance), Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Max(interactDistance, collectDistance), interactMask, QueryTriggerInteraction.Collide))
        {
            string objTag = hit.collider.tag;
            if (string.IsNullOrEmpty(objTag)) return;

            var interactable = hit.collider.GetComponentInParent<IInteractable_Water>();

            // 🔐 Locked Doors
            if (interactable is LockedDoor_Water lockedDoor && hit.distance <= interactDistance)
            {
                currentInteractable = lockedDoor;
                highlightedObject = null;
                ShowPrompt(lockedDoor.GetPrompt());
                SetButtonVisibility(true, false);
                if (Input.GetKeyDown(interactKey)) HandleInteract();
                return;
            }

            // 🧩 Normal interactables
            if (interactable != null && hit.distance <= interactDistance)
            {
                currentInteractable = interactable;
                highlightedObject = null;
                ShowPrompt(interactable.GetPrompt());
                SetButtonVisibility(true, false);
                if (Input.GetKeyDown(interactKey)) HandleInteract();
                return;
            }

            // 📻 Radio
            if (objTag.Equals("Radio", System.StringComparison.OrdinalIgnoreCase) && hit.distance <= interactDistance)
            {
                highlightedObject = hit.collider.gameObject;
                currentInteractable = null;
                ShowPrompt("Press [E] to play the Radio");
                SetButtonVisibility(true, false);
                if (Input.GetKeyDown(interactKey) || Input.GetKeyDown(collectKey))
                    PlayRadio(highlightedObject);
                return;
            }

            // 🍫 Collectables (Snack, Bag, Keys)
            if (IsCollectableObject(objTag) && hit.distance <= collectDistance)
            {
                highlightedObject = hit.collider.gameObject;
                currentInteractable = null;

                string displayName = objTag;
                ShowPrompt($"Press [E] to pick up {displayName}");
                SetButtonVisibility(false, true);

                if (Input.GetKeyDown(collectKey))
                    HandleCollect();
                return;
            }

            ClearFocus();
        }
        else
        {
            ClearFocus();
        }
    }

    private void ClearFocus()
    {
        currentInteractable = null;
        highlightedObject = null;
        HidePrompt();
        SetButtonVisibility(false, false);
    }

    private void SetButtonVisibility(bool interact, bool collect)
    {
        if (interactButton) interactButton.gameObject.SetActive(interact);
        if (collectButton) collectButton.gameObject.SetActive(collect);
    }

    private void HandleInteract()
    {
        if (currentInteractable != null)
            currentInteractable.Interact();
        else if (highlightedObject != null && highlightedObject.CompareTag("Radio"))
            PlayRadio(highlightedObject);
    }

    private void HandleCollect()
    {
        if (highlightedObject != null)
            TryCollect(highlightedObject, highlightedObject.tag);
    }

    private void ShowPrompt(string text)
    {
        if (interactPromptUI && interactPromptText)
        {
            interactPromptUI.SetActive(true);
            interactPromptText.text = text;
        }
    }

    private void HidePrompt()
    {
        if (interactPromptUI)
            interactPromptUI.SetActive(false);
    }

    private bool IsCollectableObject(string tag)
    {
        string lower = tag.ToLower();
        return lower == "snack" || lower == "bag" || lower == "flashlight" || lower.EndsWith("key") || lower == "collect";
    }

    // 🧺 Updated pickup system
    private void TryCollect(GameObject obj, string tag)
    {
        if (obj == null) return;
        string lowerTag = tag.ToLower();

        if (lowerTag == "bag")
        {
            hasBag = true;
            obj.SetActive(false);
            itemTracker?.RegisterPickup("Bag");
            objectiveManager?.CompleteMainObjective("Go to the Bedroom to collect the Bag, Flashlight, and Basement Key");
            Debug.Log("🎒 Bag collected! You can now pick up other items.");
            return;
        }

        if (!hasBag)
        {
            Debug.Log("❌ You need to collect the bag first!");
            return;
        }

        if (lowerTag == "snack")
        {
            CollectSnack(obj);
            return;
        }

        if (lowerTag == "flashlight")
        {
            CollectFlashlight(obj);
            return;
        }

        CollectGeneric(obj, tag);
    }

    // 🔦 Flashlight collection
    private void CollectFlashlight(GameObject obj)
    {
        hasFlashlight = true;
        flashlightController?.PickUpFlashlight();
        inventoryManager?.AddItem("Flashlight");
        itemTracker?.RegisterPickup("Flashlight");
        Debug.Log("🔦 Flashlight collected!");
        Destroy(obj);
    }

    // 🍫 Manual snack pickup
    private void CollectSnack(GameObject obj)
    {
        inventoryManager?.AddItem("Snack");
        itemTracker?.RegisterPickup("Snack");
        Debug.Log("<color=magenta>🍫 Snack picked up and added to inventory!</color>");
        Destroy(obj);
    }

    // ✅ FIXED: Sequential key system with automatic objective updates
    private void CollectGeneric(GameObject obj, string tag)
    {
        string lower = tag.ToLower();

        if (lower.EndsWith("key"))
        {
            inventoryManager?.AddItem(tag);
            itemTracker?.RegisterPickup(tag);

            // ✅ Handle sequential key objectives
            HandleKeyObjective(tag, obj.name);
            
            Debug.Log($"🗝️ Key collected: {tag}");
        }
        else
        {
            inventoryManager?.AddItem(tag);
            itemTracker?.RegisterPickup(tag);
            Debug.Log($"📦 Collected: {tag}");
        }

        Destroy(obj);
    }

    // 🔑 Sequential key objective handler
    private void HandleKeyObjective(string tag, string objectName)
    {
        string tagLower = tag.ToLower();
        string nameLower = objectName.ToLower();

        // Office Key (first)
        if (tagLower.Contains("office") || nameLower.Contains("office"))
        {
            HasOfficeKey = true;
            objectiveManager?.CompleteMainObjective("Find the Office Room Key in the Basement");
            objectiveManager?.AddMainObjective("Find the Study Room Key");
            Debug.Log("🔑 Office Key collected! Next: Study Room Key");
        }
        // Study Key (second)
        else if (tagLower.Contains("study") || nameLower.Contains("study"))
        {
            HasStudyKey = true;
            objectiveManager?.CompleteMainObjective("Find the Study Room Key");
            objectiveManager?.AddMainObjective("Find the Parents Bedroom Key");
            Debug.Log("🔑 Study Key collected! Next: Parents Bedroom Key");
        }
        // Parents Bedroom Key (third)
        else if (tagLower.Contains("parent") || tagLower.Contains("bedroom") || nameLower.Contains("parent") || nameLower.Contains("bedroom"))
        {
            HasBedroomKey = true;
            objectiveManager?.CompleteMainObjective("Find the Parents Bedroom Key");
            objectiveManager?.AddMainObjective("Find the Garage Key");
            Debug.Log("🔑 Parents Bedroom Key collected! Next: Garage Key");
        }
        // Garage Key (fourth)
        else if (tagLower.Contains("garage") || nameLower.Contains("garage"))
        {
            HasGarageKey = true;
            objectiveManager?.CompleteMainObjective("Find the Garage Key");
            objectiveManager?.AddMainObjective("Find the Balcony Key");
            Debug.Log("🔑 Garage Key collected! Next: Balcony Key");
        }
        // Balcony Key (fifth/final)
        else if (tagLower.Contains("balcony") || nameLower.Contains("balcony"))
        {
            HasBalconyKey = true;
            objectiveManager?.CompleteMainObjective("Find the Balcony Key");
            objectiveManager?.AddMainObjective("Escape through the Balcony");
            Debug.Log("🔑 Balcony Key collected! Final objective: Escape!");
        }
        // Basement Key (collected early in bedroom)
        else if (tagLower.Contains("basement") || nameLower.Contains("basement"))
        {
            HasBasementKey = true;
            Debug.Log("🔑 Basement Key collected!");
        }
    }

    private void PlayRadio(GameObject radioObj)
    {
        if (radioObj == null) return;
        AudioSource src = radioObj.GetComponent<AudioSource>();
        if (src == null) return;

        if (!src.isPlaying)
        {
            src.Play();
            objectiveManager?.CompleteMainObjective("Play the Radio");
        }
    }
}