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
    [SerializeField] private float collectDistance = 2f;
    [SerializeField] private LayerMask interactMask = ~0;

    [Header("UI Prompt")]
    [SerializeField] private GameObject interactPromptUI;
    [SerializeField] private Text interactPromptText;

    [Header("Keybinds")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private KeyCode collectKey = KeyCode.E;
    [SerializeField] private KeyCode healKey = KeyCode.C;

    [Header("Mobile / UI Buttons")]
    [SerializeField] private Button interactButton;
    [SerializeField] private Button collectButton;
    [SerializeField] private Button healButton;

    [Header("Outline Settings")]
    [SerializeField] private Color outlineColor = Color.yellow;
    [SerializeField, Range(0f, 10f)] private float outlineWidth = 5f;

    private GameObject highlightedObject;
    private Outline currentOutline;
    private IInteractable_Water currentInteractable;

    private bool hasBag = false;
    private bool hasFlashlight = false;
    private int bandageCount = 0;

    // 🔑 Player Keys
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
    }

    private void Start()
    {
        // ✅ Hook up UI Buttons (if assigned)
        if (interactButton != null)
            interactButton.onClick.AddListener(() => HandleInteract());

        if (collectButton != null)
            collectButton.onClick.AddListener(() => HandleCollect());

        if (healButton != null)
            healButton.onClick.AddListener(() => HandleHealingInput());
    }

    private void Update()
    {
        HandleInteractionAndHighlight();

        // ✅ Keep keyboard controls too
        if (Input.GetKeyDown(healKey))
            HandleHealingInput();
    }

    private void HandleHealingInput()
    {
        if (bandageCount > 0 && heartSystem != null)
        {
            heartSystem.RefillHeart(1);
            bandageCount--;
            Debug.Log($"<color=green>💖 Used Bandage. Remaining: {bandageCount}</color>");
        }
        else
        {
            Debug.Log("<color=red>❌ No bandages available!</color>");
        }
    }

    private void HandleInteractionAndHighlight()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * Mathf.Max(interactDistance, collectDistance), Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Max(interactDistance, collectDistance), interactMask, QueryTriggerInteraction.Ignore))
        {
            string objTag = hit.collider.tag;
            var interactable = hit.collider.GetComponentInParent<IInteractable_Water>();

            if (interactable != null && hit.distance <= interactDistance)
            {
                currentInteractable = interactable;
                ShowPrompt(interactable.GetPrompt());

                if (Input.GetKeyDown(interactKey))
                    HandleInteract();
            }
            else
            {
                currentInteractable = null;
                HidePrompt();
            }

            if (IsCollectableObject(objTag) && hit.distance <= collectDistance)
            {
                HighlightObject(hit.collider.gameObject);

                if (Input.GetKeyDown(collectKey))
                    HandleCollect();
            }
            else if (interactable == null)
            {
                ClearHighlight();
            }
        }
        else
        {
            currentInteractable = null;
            ClearHighlight();
            HidePrompt();
        }
    }

    // 🔘 Handle UI/Key Interact
    private void HandleInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
            Debug.Log($"🟢 Interacted with: {currentInteractable}");
        }
    }

    // 🔘 Handle UI/Key Collect
    private void HandleCollect()
    {
        if (highlightedObject != null)
        {
            string tag = highlightedObject.tag;
            TryCollect(highlightedObject, tag);
        }
    }

    private void ShowPrompt(string text)
    {
        if (interactPromptUI != null && interactPromptText != null)
        {
            interactPromptUI.SetActive(true);
            interactPromptText.text = text;
        }
    }

    private void HidePrompt()
    {
        if (interactPromptUI != null)
            interactPromptUI.SetActive(false);
    }

    private bool IsCollectableObject(string tag)
    {
        return tag == "Collect" || tag == "bag" || tag == "Bandage" || tag == "Battery" ||
               tag == "Flashlight" || tag.EndsWith("Key");
    }

    private void TryCollect(GameObject obj, string tag)
    {
        if (tag == "bag")
        {
            hasBag = true;
            Debug.Log("<color=yellow>👜 Collected bag — you can now collect other items.</color>");
            obj.SetActive(false);
            ClearHighlight();
            objectiveManager?.CompleteObjective("Get the Bag");
            ActivateNarrationTrigger(obj);
            return;
        }

        if (!hasBag)
        {
            Debug.Log("<color=red>❌ You need the bag first!</color>");
            return;
        }

        Collect(obj, tag);
    }

    private void Collect(GameObject obj, string tag)
    {
        switch (tag)
        {
            case "Flashlight":
                if (!hasFlashlight)
                {
                    hasFlashlight = true;
                    flashlightController?.PickUpFlashlight();
                    DataManager_Water.Instance?.CollectItem(tag);
                    Debug.Log("<color=yellow>🔦 Flashlight collected! You can now use it.</color>");
                    objectiveManager?.CompleteObjective("Get the Flashlight");
                }
                break;

            case "OfficeKey": HasOfficeKey = true; DataManager_Water.Instance?.CollectKey(tag); break;
            case "BasementKey": HasBasementKey = true; DataManager_Water.Instance?.CollectKey(tag); break;
            case "BedroomKey": HasBedroomKey = true; DataManager_Water.Instance?.CollectKey(tag); break;
            case "GarageKey": HasGarageKey = true; DataManager_Water.Instance?.CollectKey(tag); break;
            case "StudyKey": HasStudyKey = true; DataManager_Water.Instance?.CollectKey(tag); break;
            case "BalconyKey": HasBalconyKey = true; DataManager_Water.Instance?.CollectKey(tag); break;

            case "Bandage":
                bandageCount++;
                DataManager_Water.Instance?.CollectItem(tag);
                Debug.Log($"<color=cyan>🩹 Collected Bandage! Total: {bandageCount}</color>");
                break;

            case "Battery":
                inventoryManager?.AddBattery();
                DataManager_Water.Instance?.CollectItem(tag);
                Debug.Log("<color=yellow>🔋 Collected Battery!</color>");
                break;

            default:
                inventoryManager?.AddItem();
                DataManager_Water.Instance?.CollectItem(tag);
                break;
        }

        if (tag == "Collect")
            itemTracker?.RegisterPickup(tag);

        Debug.Log($"✅ Collected: {obj.name} ({tag})");
        obj.SetActive(false);
        ClearHighlight();
        ActivateNarrationTrigger(obj);
    }

    private void ActivateNarrationTrigger(GameObject obj)
    {
        var trigger = obj.GetComponent<Narrate.NarrationTrigger_Water>();
        if (trigger != null)
        {
            trigger.ActivateNarration();
            Debug.Log($"🎭 NarrationTrigger_Water activated from {obj.name}");
        }
        else
        {
            Debug.Log($"⚠️ No NarrationTrigger_Water found on {obj.name}");
        }
    }

    // ✅ Highlight System
    private void HighlightObject(GameObject obj)
    {
        if (highlightedObject == obj) return;

        ClearHighlight();
        highlightedObject = obj;

        currentOutline = obj.GetComponent<Outline>();
        if (currentOutline == null)
        {
            currentOutline = obj.AddComponent<Outline>();
            currentOutline.OutlineMode = Outline.Mode.OutlineAll;
            currentOutline.OutlineColor = outlineColor;
            currentOutline.OutlineWidth = outlineWidth;
        }
        else
        {
            currentOutline.enabled = true;
        }
    }

    private void ClearHighlight()
    {
        if (highlightedObject == null) return;

        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }

        highlightedObject = null;
    }
}
