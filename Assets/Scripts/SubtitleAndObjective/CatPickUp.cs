using UnityEngine;

public class CatPickup : MonoBehaviour
{
    [Header("References")]
    public GameObject cat;
    public SubtitleManager subtitleManager;
    public TowelPickup towelPickup; // Direct reference instead of FindObjectOfType
    public FireSafetyQuiz quizManager; // Add quiz manager reference

    private bool hasPickedUp = false;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            if (Input.GetKey(KeyCode.E))
            {
                // Check if player has towel using direct reference
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
                        () => {
                            // AFTER subtitle ends, show quiz
                            QuizQuestion quiz = QuizDatabase.GetQuiz("pet_hiding");
                            if (quiz != null && quizManager != null)
                            {
                                quizManager.ShowQuiz(quiz.question, quiz.answers, quiz.correctAnswerIndex, () => {
                                    // After quiz completes, show objective
                                    subtitleManager.ShowObjective("Find the nearest exit to escape the fire");
                                });
                            }
                            else
                            {
                                Debug.LogError("Quiz 'pet_hiding' not found or quizManager not assigned!");
                                // Fallback: show objective without quiz
                                subtitleManager.ShowObjective("Find the nearest exit to escape the fire");
                            }
                        }
                    );
                }
                else
                {
                    // Show message that towel is needed first
                    subtitleManager.ShowCustomMessage(
                        "I need to get the wet towel first before I can save the cat!",
                        2f
                    );
                }
            }
        }
    }

    // Method to check if cat was picked up
    public bool HasPickedUpCat()
    {
        return hasPickedUp;
    }
}