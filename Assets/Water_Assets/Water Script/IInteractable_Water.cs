using UnityEngine;

public interface IInteractable_Water
{
    // Text to show when aiming at this object (e.g., "Press E to toggle breaker")
    string GetPrompt();

    // Called when the player presses E while aiming at this object
    void Interact();
}
