using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class FireExtinguisher : MonoBehaviour
{
    [Header("Pickup Settings")]
    public GameObject worldExtinguisher;
    public Transform extinguisherHolder;
    public GameObject heldExtinguisherPrefab;
    public GameObject pickupButton; // 👈 assign the pickup UI button here
    public GameObject sprayButton;  // 👈 assign the spray UI button here

    [Header("Dependencies")]
    public TowelPickup towelPickup; // 👈 Assign in Inspector to prevent early pickup

    [Header("Held Position")]
    public Vector3 heldPosition = new Vector3(0.5f, -0.3f, 1.5f);
    public Vector3 heldRotation = new Vector3(0, 0, 0);
    public float heldScale = 0.5f;

    [Header("Spray Settings")]
    public float sprayRange = 5f;
    public float extinguishDelay = 2f;
    private bool isSpraySoundPlaying = false;

    [Header("References")]
    public FireSafetyQuiz quizManager;
    public SubtitleManager2 subtitleManager;

    private GameObject heldInstance;
    private bool hasExtinguisher = false;
    private bool canSpray = false;
    private bool isSpraying = false;
    private ParticleSystem sprayParticleSystem;

    private Dictionary<SpreadFire, Coroutine> firesBeingExtinguished = new Dictionary<SpreadFire, Coroutine>();

    // -------------------------------
    // 🎬  START
    // -------------------------------
    void Start()
    {
        if (pickupButton != null)
            pickupButton.SetActive(false); // Hide extinguisher button at start
    }

    // -------------------------------
    // 🚪  TRIGGER ENTER/EXIT
    // -------------------------------
    void OnTriggerStay(Collider other)
    {
        if (!hasExtinguisher && other.CompareTag("Player"))
        {
            // Don't show button until towel is picked up
            if (towelPickup != null && !towelPickup.HasPickedUpTowel())
                return;

            if (pickupButton != null && !pickupButton.activeSelf)
                pickupButton.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pickupButton != null && pickupButton.activeSelf)
                pickupButton.SetActive(false);
        }
    }

    // -------------------------------
    // 🔘  UI BUTTON PICKUP
    // -------------------------------
    public void OnPickupButtonClicked()
    {
        if (!hasExtinguisher)
        {
            PickupAndStartQuizzes(GameObject.FindGameObjectWithTag("Player"));
            if (pickupButton != null)
                pickupButton.SetActive(false);
        }
    }

    // -------------------------------
    // 🧯  UPDATE LOOP (Spraying)
    // -------------------------------
    void Update()
    {
        // Only allow spraying after quizzes
        if (!canSpray) return;

        if (isSpraying || Input.GetKey(KeyCode.F))
        {
            if (sprayParticleSystem != null && !sprayParticleSystem.isPlaying)
                sprayParticleSystem.Play();

            if (!isSpraySoundPlaying)
            {
                AudioManager.Instance.PlaySFX(33);
                isSpraySoundPlaying = true;
            }

            ExtinguishFiresInRange();
            CheckIfAllFiresOut();
        }
        else
        {
            if (sprayParticleSystem != null && sprayParticleSystem.isPlaying)
                sprayParticleSystem.Stop();

            if (isSpraySoundPlaying)
            {
                AudioManager.Instance.audClip.Stop();
                isSpraySoundPlaying = false;
            }
        }
    }

    // -------------------------------
    // 🖱️  SPRAY BUTTON FUNCTIONS
    // -------------------------------
    public void OnSprayButtonHold()
    {
        isSpraying = true;
    }

    public void OnSprayButtonRelease()
    {
        isSpraying = false;
    }

    // -------------------------------
    // 🔧  PICKUP LOGIC
    // -------------------------------
    void PickupAndStartQuizzes(GameObject player)
    {
        hasExtinguisher = true;

        if (worldExtinguisher != null)
            worldExtinguisher.SetActive(false);

        heldInstance = Instantiate(heldExtinguisherPrefab, extinguisherHolder);
        heldInstance.transform.localPosition = heldPosition;
        heldInstance.transform.localRotation = Quaternion.Euler(heldRotation);
        heldInstance.transform.localScale = Vector3.one * heldScale;

        foreach (Collider col in heldInstance.GetComponentsInChildren<Collider>())
            col.enabled = false;

        sprayParticleSystem = heldInstance.GetComponentInChildren<ParticleSystem>();
        if (sprayParticleSystem != null)
            sprayParticleSystem.Stop();

        if (sprayButton != null)
            sprayButton.SetActive(false); // Hide spray button initially

        StartCoroutine(ShowQuizzesSequentially());
    }

    // -------------------------------
    // 📚  QUIZ SEQUENCE
    // -------------------------------
    IEnumerator ShowQuizzesSequentially()
    {
        string[] quizIDs = {
            "fire_extinguisher_q1",
            "fire_extinguisher_q2",
            "fire_extinguisher_q3",
            "fire_extinguisher_q4"
        };

        for (int i = 0; i < quizIDs.Length; i++)
        {
            QuizQuestion2 quiz = QuizDatabase2.GetQuiz(quizIDs[i]);
            if (quiz != null)
            {
                quizManager.SetLastQuestion(i == quizIDs.Length - 1);

                bool quizDone = false;
                quizManager.ShowQuiz(quiz.question, quiz.answers, quiz.correctAnswerIndex, () => quizDone = true);
                yield return new WaitUntil(() => quizDone);
            }
            else
            {
                Debug.LogError("Quiz ID not found: " + quizIDs[i]);
            }
        }

        canSpray = true;

        if (sprayButton != null)
            sprayButton.SetActive(true); // ✅ show spray button after quizzes

        if (subtitleManager != null)
        {
            subtitleManager.ShowCustomMessage(
                "Good! Now use the fire extinguisher to put out the fires. Press and hold F or the Spray button!",
                4f,
                null
            );
        }
    }

    // -------------------------------
    // 🔥  EXTINGUISH FIRE LOGIC
    // -------------------------------
    void ExtinguishFiresInRange()
    {
        if (sprayParticleSystem == null) return;

        Transform sprayTransform = sprayParticleSystem.transform;
        SpreadFire[] allFires = FindObjectsOfType<SpreadFire>();

        foreach (SpreadFire fire in allFires)
        {
            if (fire.IsActive())
            {
                float distance = Vector3.Distance(sprayTransform.position, fire.transform.position);
                if (distance <= sprayRange)
                {
                    Vector3 directionToFire = (fire.transform.position - sprayTransform.position).normalized;
                    float angle = Vector3.Angle(sprayTransform.forward, directionToFire);

                    if (angle < 90f)
                    {
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
        yield return new WaitForSeconds(extinguishDelay);

        if (fire != null && fire.IsActive())
        {
            fire.Extinguish();
            Debug.Log($"EXTINGUISHED: {fire.gameObject.name}");
        }

        if (firesBeingExtinguished.ContainsKey(fire))
            firesBeingExtinguished.Remove(fire);
    }

    // -------------------------------
    // ✅  CHECK IF ALL FIRES OUT
    // -------------------------------
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

        if (!anyFireActive && canSpray)
        {
            canSpray = false;
            Debug.Log("All fires extinguished!");
            StartCoroutine(OnAllFiresExtinguished());
        }
    }

    IEnumerator OnAllFiresExtinguished()
    {
        if (sprayParticleSystem != null && sprayParticleSystem.isPlaying)
            sprayParticleSystem.Stop();

        yield return new WaitForSeconds(0.5f);

        if (heldInstance != null)
            heldInstance.SetActive(false);

        if (subtitleManager != null)
        {
            subtitleManager.ShowCustomMessage(
                "Yes! The fire is out. Now I can save Mr. Kitty!",
                3f,
                () => subtitleManager.ShowObjective("Rescue Mr. Kitty in the bedroom")
            );
        }

        if (sprayButton != null)
            sprayButton.SetActive(false);
    }
}