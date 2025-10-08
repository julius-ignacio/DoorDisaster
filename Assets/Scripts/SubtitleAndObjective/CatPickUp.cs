using System.Collections;
using UnityEngine;

public class CatPickup : MonoBehaviour
{
    [Header("References")]
    public GameObject cat;
    public SubtitleManager subtitleManager;
    public TowelPickup towelPickup;
    public FireSafetyQuiz quizManager;

    [Header("Teleport Settings")]
    public Transform player;
    public Transform houseASpawnPoint; // Drag empty GameObject in House A

    private bool hasPickedUp = false;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (towelPickup != null && towelPickup.HasPickedUpTowel())
                {
                    hasPickedUp = true;

                    // Hide the cat object
                    cat.SetActive(false);

                    // Hide the objective
                    subtitleManager.HideObjective();

                    // Show cat rescue message FIRST
                    subtitleManager.ShowCustomMessage(
                        "The cat is now safe! Let's get out of here!",
                        3f,
                        () =>
                        {
                            // AFTER subtitle ends, show quiz
                            QuizQuestion quiz = QuizDatabase.GetQuiz("pet_hiding");
                            if (quiz != null && quizManager != null)
                            {
                                quizManager.ShowQuiz(
                                    quiz.question,
                                    quiz.answers,
                                    quiz.correctAnswerIndex,
                                    () =>
                                    {
                                        // After quiz completes → teleport back to House A
                                        StartCoroutine(TeleportBackToHouseA());
                                    }
                                );
                            }
                            else
                            {
                                Debug.LogError("Quiz 'pet_hiding' not found or quizManager not assigned!");
                                // Fallback → teleport back without quiz
                                StartCoroutine(TeleportBackToHouseA());
                            }
                        }
                    );
                }
                else
                {
                    subtitleManager.ShowCustomMessage(
                        "I need to get the wet towel first before I can save the cat!",
                        2f
                    );
                }
            }
        }
    }

    private IEnumerator TeleportBackToHouseA()
    {
        // Optional: fade out screen or play sound
        yield return new WaitForSeconds(1f);

        if (player != null && houseASpawnPoint != null)
        {
            player.position = houseASpawnPoint.position; // Teleport back to House A
        }

        yield return new WaitForSeconds(1f);

        subtitleManager.ShowObjective("Find the nearest exit to escape the fire");
    }

    public bool HasPickedUpCat()
    {
        return hasPickedUp;
    }
}
