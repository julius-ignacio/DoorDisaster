// First Script: Put this on objects you can interact with
using TMPro;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string interactionText = "INTERACT"; // Text to show (OPEN, PICK UP, etc.)
    public KeyCode interactionKey = KeyCode.E;   // Key to press for interaction

    [Header("Interaction Type")]
    public InteractionType type = InteractionType.Door;

    public enum InteractionType
    {
        Door,
        Pickup,
        Faucet
    }

    // This method gets called when the player interacts with this object
    public virtual void Interact()
    {
        switch (type)
        {
            case InteractionType.Door:
                OpenDoor();
                break;
            case InteractionType.Pickup:
                PickupItem();
                break;
            case InteractionType.Faucet:
                UseFaucet();
                break;
        }
    }

    void OpenDoor()
    {
        Debug.Log("Door opened!");
        // Add your door opening logic here (like animation or moving the door)
    }

    void PickupItem()
    {
        Debug.Log("Item picked up: " + gameObject.name);
        gameObject.SetActive(false); // Hide the item when picked up
        // Add to inventory logic here if you have one
    }

    void UseFaucet()
    {
        Debug.Log("Faucet turned on!");
        // Add your faucet logic here (like water particle effects)
    }
}

