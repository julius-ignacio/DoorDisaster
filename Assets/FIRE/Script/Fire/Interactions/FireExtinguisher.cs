using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class FireExtinguisher : MonoBehaviour
{
    [Header("Pickup Settings")]
    public GameObject worldExtinguisher;
    public Transform extinguisherHolder;
    public GameObject heldExtinguisherPrefab;
    public GameObject pickupButton;
    public GameObject sprayButton;

    [Header("Dependencies")]
    public TowelPickup towelPickup;

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
    private bool allFiresExtinguished = false;
    private ParticleSystem sprayParticleSystem;
    private bool sprayButtonDestroyed = false;

    private Dictionary<SpreadFire, Coroutine> firesBeingExtinguished = new Dictionary<SpreadFire, Coroutine>();

    // ✅ Static flags for persistence
    public static bool HasPickedUpExtinguisher { get; private set; } = false;
    public static bool HasCompletedQuizzes { get; private set; } = false;
    public static bool AllFiresOut { get; private set; } = false;

    void Start()
    {
        // ✅ Check if player already has extinguisher from previous session
        if (HasPickedUpExtinguisher)
        {
            Debug.Log("🔥 Restoring fire extinguisher from previous session");
            RestoreExtinguisher();
        }
        else
        {
            if (pickupButton != null)
                pickupButton.SetActive(false);
        }

        if (sprayButton != null)
        {
            // ✅ If fires are already out, don't show spray button
            if (AllFiresOut)
            {
                sprayButton.SetActive(false);
            }
            else
            {
                EventTrigger trigger = sprayButton.GetComponent<EventTrigger>();
                if (trigger == null)
                    trigger = sprayButton.AddComponent<EventTrigger>();

                trigger.triggers.Clear();

                EventTrigger.Entry pointerDown = new EventTrigger.Entry();
                pointerDown.eventID = EventTriggerType.PointerDown;
                pointerDown.callback.AddListener((data) => { OnSprayButtonPress(); });
                trigger.triggers.Add(pointerDown);

                EventTrigger.Entry pointerUp = new EventTrigger.Entry();
                pointerUp.eventID = EventTriggerType.PointerUp;
                pointerUp.callback.AddListener((data) => { OnSprayButtonRelease(); });
                trigger.triggers.Add(pointerUp);
            }
        }
    }

    // ✅ Restore extinguisher without showing quizzes again
    void RestoreExtinguisher()
    {
        hasExtinguisher = true;
        canSpray = HasCompletedQuizzes;
        allFiresExtinguished = AllFiresOut;

        Debug.Log($"🔥 RestoreExtinguisher called: HasPickedUp={HasPickedUpExtinguisher}, Quizzes={HasCompletedQuizzes}, FiresOut={AllFiresOut}");

        // Hide world extinguisher
        if (worldExtinguisher != null)
        {
            worldExtinguisher.SetActive(false);
            Debug.Log("🔥 World extinguisher hidden");
        }
        else
        {
            Debug.LogWarning("🔥 worldExtinguisher reference is null!");
        }

        // Spawn held extinguisher
        if (!allFiresExtinguished) // Only show if fires aren't out
        {
            if (extinguisherHolder == null)
            {
                Debug.LogError("🔥 extinguisherHolder is NULL! Cannot restore held extinguisher!");
                // Try to find it
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    extinguisherHolder = player.transform.Find("ExtinguisherHolder");
                    if (extinguisherHolder != null)
                    {
                        Debug.Log("🔥 Found extinguisherHolder via player!");
                    }
                }
            }

            if (heldExtinguisherPrefab == null)
            {
                Debug.LogError("🔥 heldExtinguisherPrefab is NULL! Cannot restore held extinguisher!");
            }

            if (extinguisherHolder != null && heldExtinguisherPrefab != null)
            {
                heldInstance = Instantiate(heldExtinguisherPrefab, extinguisherHolder);
                heldInstance.transform.localPosition = heldPosition;
                heldInstance.transform.localRotation = Quaternion.Euler(heldRotation);
                heldInstance.transform.localScale = Vector3.one * heldScale;

                foreach (Collider col in heldInstance.GetComponentsInChildren<Collider>())
                    col.enabled = false;

                sprayParticleSystem = heldInstance.GetComponentInChildren<ParticleSystem>();
                if (sprayParticleSystem != null)
                {
                    sprayParticleSystem.Stop();
                    Debug.Log("🔥 Held extinguisher spawned and particle system found!");
                }
                else
                {
                    Debug.LogWarning("🔥 No ParticleSystem found on held extinguisher!");
                }
            }
            else
            {
                Debug.LogError("🔥 Cannot spawn held extinguisher - missing references!");
            }
        }
        else
        {
            Debug.Log("🔥 Fires already out, not spawning held extinguisher");
        }

        // Show spray button if quizzes done and fires not out
        if (sprayButton != null && HasCompletedQuizzes && !AllFiresOut)
        {
            sprayButton.SetActive(true);
            Debug.Log("🔥 Spray button restored and activated!");
        }
        else
        {
            Debug.Log($"🔥 Spray button NOT shown: sprayButton={sprayButton != null}, Quizzes={HasCompletedQuizzes}, FiresOut={AllFiresOut}");
        }

        // Hide pickup button
        if (pickupButton != null)
        {
            pickupButton.SetActive(false);
            Debug.Log("🔥 Pickup button hidden");
        }

        Debug.Log($"🔥 Extinguisher restored: canSpray={canSpray}, allFiresOut={allFiresExtinguished}");
    }

    // ✅ Reset static flags on new game/restart
    public static void ResetExtinguisherProgress()
    {
        HasPickedUpExtinguisher = false;
        HasCompletedQuizzes = false;
        AllFiresOut = false;
        Debug.Log("🔥 Fire extinguisher progress reset");
    }

    // ✅ Add this method here for loading from save data
    public static void RestoreExtinguisherState(bool pickedUp, bool quizzesDone, bool firesOut)
    {
        HasPickedUpExtinguisher = pickedUp;
        HasCompletedQuizzes = quizzesDone;
        AllFiresOut = firesOut;
        Debug.Log($"🔥 Restored extinguisher state: picked={pickedUp}, quizzes={quizzesDone}, firesOut={firesOut}");
    }

    void OnTriggerStay(Collider other)
    {
        if (!hasExtinguisher && other.CompareTag("Player"))
        {
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

    public void OnPickupButtonClicked()
    {
        if (!hasExtinguisher)
        {
            PickupAndStartQuizzes(GameObject.FindGameObjectWithTag("Player"));
            if (pickupButton != null)
                pickupButton.SetActive(false);
        }
    }

    void Update()
    {
        // Ensure spray button stays hidden if fires are done
        if (allFiresExtinguished && sprayButton != null && sprayButton.activeSelf)
        {
            sprayButton.SetActive(false);
        }

        // Don't allow spraying if all fires are extinguished
        if (!canSpray || allFiresExtinguished) return;

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

        if (canSpray && (Input.GetMouseButton(0) || Input.touchCount > 0))
        {
            if (isSpraying)
            {
                // Continue spraying
            }
        }
    }

    public void OnSprayButtonPress()
    {
        // Don't allow spraying if all fires are done
        if (allFiresExtinguished) return;

        isSpraying = true;
        Debug.Log("Spray button pressed - spraying started!");
    }

    public void OnSprayButtonRelease()
    {
        isSpraying = false;
        Debug.Log("Spray button released - spraying stopped!");
    }

    public void OnSprayButtonHold()
    {
        OnSprayButtonPress();
    }

    void PickupAndStartQuizzes(GameObject player)
    {
        hasExtinguisher = true;
        HasPickedUpExtinguisher = true; // ✅ Set static flag

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
            sprayButton.SetActive(false);

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

        for (int i = 0; i < quizIDs.Length; i++)
        {
            QuizQuestion2 quiz = QuizDatabase2.GetQuiz(quizIDs[i]);
            if (quiz != null)
            {
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
        HasCompletedQuizzes = true; // ✅ Set static flag

        // Only show spray button if fires aren't already extinguished
        if (sprayButton != null && !allFiresExtinguished)
            sprayButton.SetActive(true);

        if (subtitleManager != null)
        {
            subtitleManager.ShowCustomMessage(
                "Good! Now use the fire extinguisher to put out the fires. Press and hold the Spray button!",
                4f,
                null
            );
        }
    }

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

        if (!anyFireActive && canSpray && !allFiresExtinguished)
        {
            allFiresExtinguished = true;
            AllFiresOut = true; // ✅ Set static flag
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

        // AGGRESSIVELY hide and disable spray button
        if (sprayButton != null && !sprayButtonDestroyed)
        {
            sprayButton.SetActive(false);

            // Disable all components to prevent re-activation
            Button btn = sprayButton.GetComponent<Button>();
            if (btn != null) btn.interactable = false;

            EventTrigger trigger = sprayButton.GetComponent<EventTrigger>();
            if (trigger != null) trigger.enabled = false;

            // Move it far away as extra safety
            RectTransform rect = sprayButton.GetComponent<RectTransform>();
            if (rect != null) rect.anchoredPosition = new Vector2(10000, 10000);

            sprayButtonDestroyed = true;

            Debug.Log("Spray button permanently disabled!");
        }

        if (subtitleManager != null)
        {
            subtitleManager.ShowCustomMessage(
                "Yes! The fire is out. Now I can save Mr. Kitty!",
                3f,
                () => subtitleManager.ShowObjective("Rescue Mr. Kitty in the bedroom")
            );
        }
    }

    // ✅ Public getter for SequenceManager to check status
    public bool HasExtinguisherEquipped()
    {
        return hasExtinguisher;
    }
}