using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PuzzleManager_Water : MonoBehaviour
{
    [Header("Puzzle UI")]
    public GameObject puzzleUI; // Drag your puzzle Canvas here
    public TMP_Text riddleText;
    public TMP_Text hintText;
    public TMP_Text feedbackText; // Shows correct/wrong messages

    [Header("Puzzle Logic")]
    private string[] correctOrder = { "Sky", "Roof", "Wall", "Floor" };
    private List<string> playerOrder = new List<string>();
    public bool puzzleSolved = false;

    private LockedDoor_Water linkedDoor;
    private bool isActive = false;

    private void Start()
    {
        if (feedbackText != null)
            feedbackText.text = "";
    }

    public void ActivatePuzzle()
    {
        if (puzzleSolved) return;

        isActive = true;
        if (puzzleUI != null)
            puzzleUI.SetActive(true);

        if (riddleText != null)
        {
            riddleText.text =
                "The Rain’s Journey \n" +
                "\"The rain fell from the sky,\n" +
                "Kissed the roof,\n" +
                "Slid down the walls,\n" +
                "And filled the floor below.\n" +
                "Trace the rain’s path.\"";
        }

        if (hintText != null)
            hintText.text = "Hint: Think how rain naturally moves.";

        if (feedbackText != null)
        {
            feedbackText.text = "";
            feedbackText.color = Color.white;
        }

        // Freeze player movement
        var player = FindObjectOfType<PlayerController_Water>();
        if (player != null)
            player.enabled = false;


        playerOrder.Clear(); // Reset choices
        Debug.Log("Puzzle activated!");
    }

    // Called when one of the four buttons is clicked
    public void OnButtonPressed(string buttonName)
    {
        if (puzzleSolved) return;
        if (playerOrder.Count >= correctOrder.Length)
        {
            feedbackText.text = "You've already selected 4. Press Confirm.";
            feedbackText.color = Color.yellow;
            return;
        }

        playerOrder.Add(buttonName);

        if (feedbackText != null)
        {
            feedbackText.text = $"Selected: {string.Join(", ", playerOrder)}";
            feedbackText.color = Color.white;
        }
    }

    // Called by the Confirm button
    public void OnConfirmPressed()
    {
        if (puzzleSolved) return;

        if (playerOrder.Count < correctOrder.Length)
        {
            feedbackText.text = "Select all 4 before confirming!";
            feedbackText.color = Color.yellow;
            return;
        }

        // Check the sequence
        for (int i = 0; i < correctOrder.Length; i++)
        {
            if (playerOrder[i] != correctOrder[i])
            {
                feedbackText.text = "Wrong order! Try again.";
                feedbackText.color = Color.red;
                playerOrder.Clear();
                return;
            }
        }

        // If correct
        CompletePuzzle();
    }

    public void CompletePuzzle()
    {
        if (puzzleSolved) return;

        puzzleSolved = true;

        if (feedbackText != null)
        {
            feedbackText.text = "Correct! The door is now unlocked.";
            feedbackText.color = Color.green;
        }

        if (linkedDoor != null)
            linkedDoor.PuzzleSolved_UnlockDoor();

        Debug.Log("Puzzle completed — door unlocked!");

        Invoke(nameof(ClosePuzzle), 1.5f);
    }

    public void ClosePuzzle()
    {
        if (puzzleUI != null)
            puzzleUI.SetActive(false);

        // Unfreeze player
        var player = FindObjectOfType<PlayerController_Water>();
        if (player != null)
            player.enabled = true;

    }

    public void SetLinkedDoor(LockedDoor_Water door)
    {
        linkedDoor = door;
    }
}
