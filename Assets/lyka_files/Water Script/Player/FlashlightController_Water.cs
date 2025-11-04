using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlashlightController_Water : MonoBehaviour
{
    [Header("References")]
    public Light flashlightLight;
    public TextMeshProUGUI flashlightText;

    [Header("Pickup UI Button")]
    public Button pickupButton;   // Button to pick up flashlight

    [Header("Settings")]
    public Transform player;
    public float interactDistance = 3f;
    public float lookAtAngle = 45f;

    [Header("Breaker Reference")]
    public Breaker1_Water breaker; // ⚡ Link your breaker object here

    [HideInInspector] public bool hasFlashlight = false;

    private bool isOn = false;
    private bool isNearby = false;

    void Start()
    {
        // Safety checks
        if (pickupButton == null)
            Debug.LogError("❌ Pickup Button not assigned in Inspector!");

        if (flashlightLight) flashlightLight.enabled = false;
        if (flashlightText) flashlightText.gameObject.SetActive(false);

        // Hide pickup button at start
        if (pickupButton) pickupButton.gameObject.SetActive(false);

        // Add listener to pickup
        if (pickupButton)
        {
            pickupButton.onClick.RemoveAllListeners();
            pickupButton.onClick.AddListener(() => PickUpFlashlight());
        }

        // Auto-assign breaker if not manually linked
        if (breaker == null)
            breaker = FindObjectOfType<Breaker1_Water>();
    }

    void Update()
    {
        if (player == null) return;

        // 🧭 Handle pickup interaction
        if (!hasFlashlight)
        {
            if (isNearby && Input.GetKeyDown(KeyCode.E))
                PickUpFlashlight();
            return;
        }

        // ⚡ Only react to breaker after flashlight is picked up
        if (breaker != null && hasFlashlight)
        {
            // Breaker OFF → flashlight ON
            if (!breaker.IsPowerOn && !isOn)
            {
                flashlightLight.enabled = true;
                isOn = true;
                if (flashlightText)
                {
                    flashlightText.gameObject.SetActive(true);
                    flashlightText.text = "Flashlight ON (Power is out)";
                }
                Debug.Log("💡 Flashlight automatically ON (breaker off)");
            }
            // Breaker ON → flashlight OFF
            else if (breaker.IsPowerOn && isOn)
            {
                flashlightLight.enabled = false;
                isOn = false;
                if (flashlightText)
                {
                    flashlightText.gameObject.SetActive(true);
                    flashlightText.text = "Power restored. Flashlight OFF";
                }
                Debug.Log("⚡ Flashlight automatically OFF (breaker on)");
            }
        }
    }

    // 🪫 Called by trigger or UI when player near flashlight
    public void ShowPickupUI(bool show)
    {
        if (hasFlashlight) return;

        if (pickupButton) pickupButton.gameObject.SetActive(show);
        if (flashlightText) flashlightText.gameObject.SetActive(show);

        flashlightText.text = show ? "Tap or Press E to Pick Up Flashlight" : "";
    }

    // 🔦 Player picks up flashlight
    public void PickUpFlashlight()
    {
        if (hasFlashlight) return;

        hasFlashlight = true;
        isNearby = false;

        if (pickupButton) pickupButton.gameObject.SetActive(false);

        flashlightText.gameObject.SetActive(true);
        flashlightText.text = "Flashlight acquired!";

        Debug.Log("🔦 Flashlight picked up!");
    }
}
