using UnityEngine;
using UnityEngine.UI;
using EasyDoorSystem;

[DisallowMultipleComponent]
public class PlayerInteractor_Water : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private HeartSysWater heartSystem;

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

    private void Awake()
    {
        playerCamera ??= GetComponentInChildren<Camera>() ?? Camera.main;
        heartSystem ??= FindObjectOfType<HeartSysWater>();
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

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Max(interactDistance, collectDistance), interactMask, QueryTriggerInteraction.Collide))
        {
            string objTag = hit.collider.tag;
            if (string.IsNullOrEmpty(objTag)) return;

            var interactable = hit.collider.GetComponentInParent<IInteractable_Water>();

            // Interactables (doors, objects)
            if (interactable != null && hit.distance <= interactDistance)
            {
                currentInteractable = interactable;
                highlightedObject = null;
                ShowPrompt(interactable.GetPrompt());
                SetButtonVisibility(true, false);
                if (Input.GetKeyDown(interactKey)) HandleInteract();
                return;
            }

            // Radio
            if (objTag.Equals("Radio", System.StringComparison.OrdinalIgnoreCase) && hit.distance <= interactDistance)
            {
                highlightedObject = hit.collider.gameObject;
                ShowPrompt("Press [E] to play the Radio");
                SetButtonVisibility(true, false);
                if (Input.GetKeyDown(interactKey) || Input.GetKeyDown(collectKey))
                    PlayRadio(highlightedObject);
                return;
            }

            // Collectables
            if (IsCollectableObject(objTag) && hit.distance <= collectDistance)
            {
                highlightedObject = hit.collider.gameObject;
                ShowPrompt($"Press [E] to pick up {objTag}");
                SetButtonVisibility(false, true);
                if (Input.GetKeyDown(collectKey)) HandleCollect();
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
        return lower == "snack" || lower == "bag" || lower == "flashlight";
    }

    private void TryCollect(GameObject obj, string tag)
    {
        if (obj == null) return;
        string lowerTag = tag.ToLower();

        // Bag must be collected first
        if (lowerTag == "bag")
        {
            hasBag = true;
            Debug.Log("🎒 Bag collected!");
            Destroy(obj);
            return;
        }

        if (!hasBag)
        {
            Debug.Log("❌ You need the bag first!");
            return;
        }

        // Collect item
        if (lowerTag == "snack")
        {
            Debug.Log("🍫 Snack collected!");
        }
        else if (lowerTag == "flashlight")
        {
            Debug.Log("🔦 Flashlight collected!");
        }

        Destroy(obj);
    }

    private void PlayRadio(GameObject radioObj)
    {
        if (radioObj == null) return;
        AudioSource src = radioObj.GetComponent<AudioSource>();
        if (src != null && !src.isPlaying)
        {
            src.Play();
        }
    }
}