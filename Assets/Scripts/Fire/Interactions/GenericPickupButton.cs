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
}

// Interface for all pickupable objects
public interface IPickupable
{
    void OnPickup();
}