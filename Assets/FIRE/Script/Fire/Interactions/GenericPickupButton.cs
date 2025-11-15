using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenericPickupButton : MonoBehaviour
{
    public static GenericPickupButton Instance;

    [Header("UI References")]
    public Button pickupButton;
    public TextMeshProUGUI buttonText; // Optional: show what you're picking up

    private IPickupable currentPickupable;

    // 🟦 Track visibility before pausing
    private bool wasVisibleBeforePause = false;
    private IPickupable pausedPickupable = null; // Remember which object was active
    public GameManager gameManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (pickupButton != null)
        {
            pickupButton.gameObject.SetActive(false);
            pickupButton.onClick.AddListener(OnPickupButtonPressed);
        }
    }

    // Called by pickup objects when player is in range
    public void ShowPickupPrompt(IPickupable pickupable, string promptText = "Pick Up")
    {
        // Don't show if game is paused
        if (gameManager.isPaused)
            return;

        currentPickupable = pickupable;
        if (pickupButton != null)
        {
            pickupButton.gameObject.SetActive(true);
            if (buttonText != null)
                buttonText.text = promptText;
        }
    }

    // Called by pickup objects when player leaves range
    public void HidePickupPrompt()
    {
        currentPickupable = null;
        if (pickupButton != null)
            pickupButton.gameObject.SetActive(false);
    }

    private void OnPickupButtonPressed()
    {
        if (currentPickupable != null)
        {
            currentPickupable.OnPickup();
            HidePickupPrompt();
        }
    }

    // 🟧 Handle pause/resume visibility
    public void OnPause()
    {
        if (pickupButton != null)
        {
            wasVisibleBeforePause = pickupButton.gameObject.activeSelf;

            // Remember which pickupable was active
            if (wasVisibleBeforePause)
            {
                pausedPickupable = currentPickupable;
            }

            pickupButton.gameObject.SetActive(false);
        }
    }

    public void OnResume()
    {
        if (pickupButton != null && wasVisibleBeforePause)
        {
            // Restore the button with the same pickupable
            if (pausedPickupable != null)
            {
                currentPickupable = pausedPickupable;
                pickupButton.gameObject.SetActive(true);
            }

            // Reset the paused state
            pausedPickupable = null;
            wasVisibleBeforePause = false;
        }
    }
}

// Interface for all pickupable objects
public interface IPickupable
{
    void OnPickup();
}