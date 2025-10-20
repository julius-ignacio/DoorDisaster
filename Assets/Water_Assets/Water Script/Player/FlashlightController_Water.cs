using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlashlightController_Water : MonoBehaviour
{
    [Header("References")]
    public Light flashlightLight;
    public TextMeshProUGUI flashlightText;

    [Header("Mobile UI Buttons")]
    public Button toggleButton;   // ON/OFF button (ToggleFlashlightBtn)
    public Button pickupButton;   // Pick up button

    [Header("Settings")]
    public Transform player;
    public float interactDistance = 3f;
    public float lookAtAngle = 45f;

    [HideInInspector] public bool hasFlashlight = false;

    private bool isOn = false;
    private bool isNearby = false;

    void Start()
    {
        // Safety checks
        if (toggleButton == null)
            Debug.LogError("❌ Toggle Button not assigned in Inspector!");

        if (pickupButton == null)
            Debug.LogError("❌ Pickup Button not assigned in Inspector!");

        if (flashlightLight) flashlightLight.enabled = false;
        if (flashlightText) flashlightText.gameObject.SetActive(false);

        // Hide buttons at start
        if (toggleButton) toggleButton.gameObject.SetActive(false);
        if (pickupButton) pickupButton.gameObject.SetActive(false);

        // Add listeners (fresh reset first)
        if (toggleButton)
        {
            toggleButton.onClick.RemoveAllListeners();
            toggleButton.onClick.AddListener(() => ToggleFlashlight());
        }

        if (pickupButton)
        {
            pickupButton.onClick.RemoveAllListeners();
            pickupButton.onClick.AddListener(() => PickUpFlashlight());
        }
    }

    void Update()
    {
        if (player == null) return;

        if (!hasFlashlight)
        {
            if (isNearby && Input.GetKeyDown(KeyCode.E))
                PickUpFlashlight();
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
            ToggleFlashlight();
    }

    public void ShowPickupUI(bool show)
    {
        if (hasFlashlight) return;

        if (pickupButton) pickupButton.gameObject.SetActive(show);
        if (flashlightText) flashlightText.gameObject.SetActive(show);

        flashlightText.text = show ? "Tap or Press E to Pick Up Flashlight" : "";
    }

    private void ToggleFlashlight()
    {
        if (!hasFlashlight) return;

        isOn = !isOn;
        flashlightLight.enabled = isOn;

        if (toggleButton)
        {
            var txt = toggleButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
                txt.text = isOn ? "Turn OFF" : "Turn ON";
        }

        if (flashlightText)
            flashlightText.text = isOn ? "Flashlight ON" : "Flashlight OFF";

        Debug.Log($"💡 Flashlight toggled: {(isOn ? "ON" : "OFF")}");
    }

    public void PickUpFlashlight()
    {
        if (hasFlashlight) return;

        hasFlashlight = true;
        isNearby = false;

        if (pickupButton) pickupButton.gameObject.SetActive(false);
        if (toggleButton) toggleButton.gameObject.SetActive(true);
        if (flashlightText) flashlightText.gameObject.SetActive(true);
        flashlightText.text = "Tap or Press F to turn ON flashlight";

        Debug.Log("🔦 Flashlight picked up!");
    }
}
