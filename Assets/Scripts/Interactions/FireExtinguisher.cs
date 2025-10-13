using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireExtinguisher : MonoBehaviour
{
    [Header("Pickup Settings")]
    public GameObject worldExtinguisher;
    public Transform extinguisherHolder;
    public GameObject heldExtinguisherPrefab;

    [Header("Held Position")]
    public Vector3 heldPosition = new Vector3(0.5f, -0.3f, 1.5f);
    public Vector3 heldRotation = new Vector3(0, 0, 0);
    public float heldScale = 0.5f;

    [Header("Spray Settings")]
    public float sprayRange = 5f;
<<<<<<< HEAD
    public float extinguishDelay = 2f; // Time before fire goes out
=======
    public float extinguishDelay = 2f;
>>>>>>> 47c3962 (Quiz script changes)

    [Header("References")]
    public FireSafetyQuiz quizManager;
    public SubtitleManager subtitleManager;

    private GameObject heldInstance;
    private bool hasExtinguisher = false;
    private bool canSpray = false;
    private ParticleSystem sprayParticleSystem;

<<<<<<< HEAD
    // Track which fires are being extinguished
=======
>>>>>>> 47c3962 (Quiz script changes)
    private Dictionary<SpreadFire, Coroutine> firesBeingExtinguished = new Dictionary<SpreadFire, Coroutine>();

    void OnTriggerStay(Collider other)
    {
        if (!hasExtinguisher && other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            PickupAndStartQuizzes(other.gameObject);
        }
    }

    void Update()
    {
<<<<<<< HEAD
        // Manual spray control after quizzes
=======
>>>>>>> 47c3962 (Quiz script changes)
        if (canSpray && heldInstance != null)
        {
            if (Input.GetKey(KeyCode.F))
            {
<<<<<<< HEAD
                // Spray is being used
=======
>>>>>>> 47c3962 (Quiz script changes)
                if (sprayParticleSystem != null && !sprayParticleSystem.isPlaying)
                {
                    sprayParticleSystem.Play();
                }

<<<<<<< HEAD
                // Check if spray is hitting any fires
=======
>>>>>>> 47c3962 (Quiz script changes)
                ExtinguishFiresInRange();
            }
            else
            {
<<<<<<< HEAD
                // Stop spray when F is released
=======
>>>>>>> 47c3962 (Quiz script changes)
                if (sprayParticleSystem != null && sprayParticleSystem.isPlaying)
                {
                    sprayParticleSystem.Stop();
                }
            }

<<<<<<< HEAD
            // Check if all fires are out
=======
>>>>>>> 47c3962 (Quiz script changes)
            CheckIfAllFiresOut();
        }
    }

    void PickupAndStartQuizzes(GameObject player)
    {
        hasExtinguisher = true;

<<<<<<< HEAD
        // Hide the world extinguisher
        if (worldExtinguisher != null)
            worldExtinguisher.SetActive(false);

        // Spawn held extinguisher
=======
        if (worldExtinguisher != null)
            worldExtinguisher.SetActive(false);

>>>>>>> 47c3962 (Quiz script changes)
        heldInstance = Instantiate(heldExtinguisherPrefab, extinguisherHolder);
        heldInstance.transform.localPosition = heldPosition;
        heldInstance.transform.localRotation = Quaternion.Euler(heldRotation);
        heldInstance.transform.localScale = Vector3.one * heldScale;

        foreach (Collider col in heldInstance.GetComponentsInChildren<Collider>())
            col.enabled = false;

<<<<<<< HEAD
        // Get particle system reference
=======
>>>>>>> 47c3962 (Quiz script changes)
        sprayParticleSystem = heldInstance.GetComponentInChildren<ParticleSystem>();
        if (sprayParticleSystem != null)
        {
            sprayParticleSystem.Stop();
        }

<<<<<<< HEAD
        // Start quiz sequence
=======
>>>>>>> 47c3962 (Quiz script changes)
        StartCoroutine(ShowQuizzesSequentially());
    }

    IEnumerator ShowQuizzesSequentially()
    {
        string[] quizIDs = {
            "fire_extinguisher_q1",
            "fire_extinguisher_q2",
            "fire_extinguisher_q3",
            "fire_extinguisher_q4"
        };

<<<<<<< HEAD
        foreach (string id in quizIDs)
        {
            QuizQuestion quiz = QuizDatabase.GetQuiz(id);
            if (quiz != null)
            {
=======
        for (int i = 0; i < quizIDs.Length; i++)
        {
            QuizQuestion quiz = QuizDatabase.GetQuiz(quizIDs[i]);
            if (quiz != null)
            {
                // Tell quiz manager if this is the last question
                quizManager.SetLastQuestion(i == quizIDs.Length - 1);

>>>>>>> 47c3962 (Quiz script changes)
                bool quizDone = false;
                quizManager.ShowQuiz(quiz.question, quiz.answers, quiz.correctAnswerIndex, () => quizDone = true);

                yield return new WaitUntil(() => quizDone);
            }
            else
            {
<<<<<<< HEAD
                Debug.LogError("Quiz ID not found: " + id);
            }
        }

        // After all quizzes, allow manual spraying
=======
                Debug.LogError("Quiz ID not found: " + quizIDs[i]);
            }
        }

>>>>>>> 47c3962 (Quiz script changes)
        canSpray = true;

        if (subtitleManager != null)
        {
            subtitleManager.ShowCustomMessage(
                "Good! Now use the fire extinguisher to put out the fires. Press and hold F to spray!",
                4f,
                null
            );
        }
    }

    void ExtinguishFiresInRange()
    {
        if (sprayParticleSystem == null) return;

        Transform sprayTransform = sprayParticleSystem.transform;
<<<<<<< HEAD

        // Find all active fires in the scene
=======
>>>>>>> 47c3962 (Quiz script changes)
        SpreadFire[] allFires = FindObjectsOfType<SpreadFire>();

        foreach (SpreadFire fire in allFires)
        {
            if (fire.IsActive())
            {
<<<<<<< HEAD
                // Check distance
=======
>>>>>>> 47c3962 (Quiz script changes)
                float distance = Vector3.Distance(sprayTransform.position, fire.transform.position);

                if (distance <= sprayRange)
                {
<<<<<<< HEAD
                    // Check if spray is pointing towards fire
                    Vector3 directionToFire = (fire.transform.position - sprayTransform.position).normalized;
                    float angle = Vector3.Angle(sprayTransform.forward, directionToFire);

                    // Extinguish if within cone
                    if (angle < 90f)
                    {
                        // Start extinguishing this fire (if not already being extinguished)
=======
                    Vector3 directionToFire = (fire.transform.position - sprayTransform.position).normalized;
                    float angle = Vector3.Angle(sprayTransform.forward, directionToFire);

                    if (angle < 90f)
                    {
>>>>>>> 47c3962 (Quiz script changes)
                        if (!firesBeingExtinguished.ContainsKey(fire))
                        {
                            Coroutine extinguishCoroutine = StartCoroutine(ExtinguishFireWithDelay(fire));
                            firesBeingExtinguished.Add(fire, extinguishCoroutine);
                            Debug.Log($"Started extinguishing: {fire.gameObject.name}");
                        }
                    }
                }
            }
        }
    }

    IEnumerator ExtinguishFireWithDelay(SpreadFire fire)
    {
<<<<<<< HEAD
        // Wait for the delay
        yield return new WaitForSeconds(extinguishDelay);

        // Extinguish the fire
        if (fire != null && fire.IsActive())
        {
            fire.Extinguish();
            Debug.Log($"🔥 EXTINGUISHED: {fire.gameObject.name}");
        }

        // Remove from tracking dictionary
=======
        yield return new WaitForSeconds(extinguishDelay);

        if (fire != null && fire.IsActive())
        {
            fire.Extinguish();
            Debug.Log($"EXTINGUISHED: {fire.gameObject.name}");
        }

>>>>>>> 47c3962 (Quiz script changes)
        if (firesBeingExtinguished.ContainsKey(fire))
        {
            firesBeingExtinguished.Remove(fire);
        }
    }

    void CheckIfAllFiresOut()
    {
        SpreadFire[] allFires = FindObjectsOfType<SpreadFire>();
        bool anyFireActive = false;

        foreach (SpreadFire fire in allFires)
        {
            if (fire.IsActive())
            {
                anyFireActive = true;
                break;
            }
        }

<<<<<<< HEAD
        // All fires are out!
        if (!anyFireActive && canSpray)
        {
            canSpray = false;
            Debug.Log("✓ All fires extinguished!");
=======
        if (!anyFireActive && canSpray)
        {
            canSpray = false;
            Debug.Log("All fires extinguished!");
>>>>>>> 47c3962 (Quiz script changes)
            StartCoroutine(OnAllFiresExtinguished());
        }
    }

    IEnumerator OnAllFiresExtinguished()
    {
<<<<<<< HEAD
        // Stop spray
=======
>>>>>>> 47c3962 (Quiz script changes)
        if (sprayParticleSystem != null && sprayParticleSystem.isPlaying)
        {
            sprayParticleSystem.Stop();
        }

        yield return new WaitForSeconds(0.5f);

<<<<<<< HEAD
        // Hide extinguisher
        if (heldInstance != null)
            heldInstance.SetActive(false);

        // Show final subtitle
=======
        if (heldInstance != null)
            heldInstance.SetActive(false);

>>>>>>> 47c3962 (Quiz script changes)
        if (subtitleManager != null)
        {
            subtitleManager.ShowCustomMessage(
                "Yes! The fire is out. Now I can save Mr. Kitty!",
                3f,
                () => subtitleManager.ShowObjective("Rescue Mr. Kitty in the bedroom")
            );
        }
    }
}