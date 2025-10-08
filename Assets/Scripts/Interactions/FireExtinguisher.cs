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
    public float extinguishDelay = 2f; // Time before fire goes out

    [Header("References")]
    public FireSafetyQuiz quizManager;
    public SubtitleManager subtitleManager;

    private GameObject heldInstance;
    private bool hasExtinguisher = false;
    private bool canSpray = false;
    private ParticleSystem sprayParticleSystem;

    // Track which fires are being extinguished
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
        // Manual spray control after quizzes
        if (canSpray && heldInstance != null)
        {
            if (Input.GetKey(KeyCode.F))
            {
                // Spray is being used
                if (sprayParticleSystem != null && !sprayParticleSystem.isPlaying)
                {
                    sprayParticleSystem.Play();
                }

                // Check if spray is hitting any fires
                ExtinguishFiresInRange();
            }
            else
            {
                // Stop spray when F is released
                if (sprayParticleSystem != null && sprayParticleSystem.isPlaying)
                {
                    sprayParticleSystem.Stop();
                }
            }

            // Check if all fires are out
            CheckIfAllFiresOut();
        }
    }

    void PickupAndStartQuizzes(GameObject player)
    {
        hasExtinguisher = true;

        // Hide the world extinguisher
        if (worldExtinguisher != null)
            worldExtinguisher.SetActive(false);

        // Spawn held extinguisher
        heldInstance = Instantiate(heldExtinguisherPrefab, extinguisherHolder);
        heldInstance.transform.localPosition = heldPosition;
        heldInstance.transform.localRotation = Quaternion.Euler(heldRotation);
        heldInstance.transform.localScale = Vector3.one * heldScale;

        foreach (Collider col in heldInstance.GetComponentsInChildren<Collider>())
            col.enabled = false;

        // Get particle system reference
        sprayParticleSystem = heldInstance.GetComponentInChildren<ParticleSystem>();
        if (sprayParticleSystem != null)
        {
            sprayParticleSystem.Stop();
        }

        // Start quiz sequence
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

        foreach (string id in quizIDs)
        {
            QuizQuestion quiz = QuizDatabase.GetQuiz(id);
            if (quiz != null)
            {
                bool quizDone = false;
                quizManager.ShowQuiz(quiz.question, quiz.answers, quiz.correctAnswerIndex, () => quizDone = true);

                yield return new WaitUntil(() => quizDone);
            }
            else
            {
                Debug.LogError("Quiz ID not found: " + id);
            }
        }

        // After all quizzes, allow manual spraying
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

        // Find all active fires in the scene
        SpreadFire[] allFires = FindObjectsOfType<SpreadFire>();

        foreach (SpreadFire fire in allFires)
        {
            if (fire.IsActive())
            {
                // Check distance
                float distance = Vector3.Distance(sprayTransform.position, fire.transform.position);

                if (distance <= sprayRange)
                {
                    // Check if spray is pointing towards fire
                    Vector3 directionToFire = (fire.transform.position - sprayTransform.position).normalized;
                    float angle = Vector3.Angle(sprayTransform.forward, directionToFire);

                    // Extinguish if within cone
                    if (angle < 90f)
                    {
                        // Start extinguishing this fire (if not already being extinguished)
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
        // Wait for the delay
        yield return new WaitForSeconds(extinguishDelay);

        // Extinguish the fire
        if (fire != null && fire.IsActive())
        {
            fire.Extinguish();
            Debug.Log($"🔥 EXTINGUISHED: {fire.gameObject.name}");
        }

        // Remove from tracking dictionary
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

        // All fires are out!
        if (!anyFireActive && canSpray)
        {
            canSpray = false;
            Debug.Log("✓ All fires extinguished!");
            StartCoroutine(OnAllFiresExtinguished());
        }
    }

    IEnumerator OnAllFiresExtinguished()
    {
        // Stop spray
        if (sprayParticleSystem != null && sprayParticleSystem.isPlaying)
        {
            sprayParticleSystem.Stop();
        }

        yield return new WaitForSeconds(0.5f);

        // Hide extinguisher
        if (heldInstance != null)
            heldInstance.SetActive(false);

        // Show final subtitle
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