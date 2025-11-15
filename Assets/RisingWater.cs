using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(BoxCollider))]
public class RisingWater : MonoBehaviour
{
    [Header("Rising settings")]
    public float riseIntervalSeconds = 35f;
    public float riseAmountPerStep = 1f;
    public float stopAtWorldY = 99999f;

    private Coroutine riseRoutine;

    [Header("Player/Systems")]
    public OxygenMeterScript oxygenMeter;
    public GameObject Head, Knees;
    public Movements playerMovements;

    [Header("UI - Water Rising Alert")]
    public TMP_Text waterRisingAlertText;
    public CanvasGroup waterRisingAlertGroup;

    public float alertDuration = 0.8f;
    public float fadeInDuration = 0.15f;
    public float fadeOutDuration = 0.25f;

    private Coroutine alertRoutine;

    [Header("UI - Rise Countdown Timer")]
    public TMP_Text riseCountdownText;
    public float riseCountdownTime;
    private Coroutine countdownRoutine;

    [Header("Oxygen")]
    [SerializeField] float oxygenDrainRate = 8f;
    [SerializeField] float oxygenRegenRate = 2f;

    public bool isHeadUnderwater = false;

    public

    void OnEnable()
    {
        HideAlertImmediate();
        ResetCountdownImmediate();
        riseRoutine = StartCoroutine(RiseLoop());
    }

    void OnDisable()
    {
        if (riseRoutine != null) StopCoroutine(riseRoutine);
        if (alertRoutine != null) StopCoroutine(alertRoutine);
        if (countdownRoutine != null) StopCoroutine(countdownRoutine);

        HideAlertImmediate();
        ResetCountdownImmediate();
    }

    void Start()
    {
        if (DataManager.Instance != null)
        {
            int mode = DataManager.Instance.currentMode;
            if (mode == 1) // Hard mode
            {
                riseIntervalSeconds = 35f;
            }

            else
            {
                riseIntervalSeconds = 60f;
            }
        }
    }

    void Update()
    {
        riseCountdownTime = riseIntervalSeconds;

        if (oxygenMeter != null)
        {
            if (isHeadUnderwater)
            {
                oxygenMeter.currHealth -= oxygenDrainRate * Time.deltaTime;
                AudioManager.Instance.PlayLoop(AudioManager.Instance.Clips[37]);
            }
            else
            {
                oxygenMeter.currHealth += oxygenRegenRate * Time.deltaTime;
                AudioManager.Instance.StopLoop();
            }
            oxygenMeter.currHealth = Mathf.Clamp(oxygenMeter.currHealth, 0, oxygenMeter.maxHealth);
        }
    }

    IEnumerator RiseLoop()
    {
        while (transform.position.y < stopAtWorldY)
        {
            // Start countdown every loop
            StartRiseCountdown();

            yield return new WaitForSeconds(riseIntervalSeconds);

            float currentY = transform.position.y;
            float nextY = Mathf.Min(currentY + riseAmountPerStep, stopAtWorldY);

            if (nextY > currentY)
            {
                transform.position = new Vector3(transform.position.x, nextY, transform.position.z);
                ShowAlert("Water Rising!");
                AudioManager.Instance.PlaySFX(37);
            }
            else break;
        }
    }

    // ------------------------------
    // COUNTDOWN TIMER
    // ------------------------------
    void StartRiseCountdown()
    {
        if (countdownRoutine != null)
            StopCoroutine(countdownRoutine);

        countdownRoutine = StartCoroutine(RiseCountdown());
    }

    IEnumerator RiseCountdown()
    {
        float t = riseCountdownTime;

        while (t > 0)
        {
            if (riseCountdownText != null)
                riseCountdownText.text = "Next Rise: " + Mathf.Ceil(t);

            t -= Time.deltaTime;
            yield return null;
        }

        if (riseCountdownText != null)
            riseCountdownText.text = "Next Rise: 0";
    }

    void ResetCountdownImmediate()
    {
        if (riseCountdownText != null)
            riseCountdownText.text = string.Empty;
    }

    // ------------------------------
    // RISING ALERT
    // ------------------------------
    void ShowAlert(string msg)
    {
        if (waterRisingAlertText != null)
            waterRisingAlertText.text = msg;

        if (alertRoutine != null)
            StopCoroutine(alertRoutine);

        alertRoutine = StartCoroutine(AlertSequence());
    }

    IEnumerator AlertSequence()
    {
        yield return FadeCanvasGroup(waterRisingAlertGroup, 0f, 1f, fadeInDuration);
        yield return new WaitForSeconds(alertDuration);
        yield return FadeCanvasGroup(waterRisingAlertGroup, 1f, 0f, fadeOutDuration);
    }

    IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        if (group == null) yield break;

        if (!group.gameObject.activeSelf)
            group.gameObject.SetActive(true);

        float t = 0f;
        group.alpha = from;
        group.blocksRaycasts = to > 0.99f;

        if (duration <= 0f)
        {
            group.alpha = to;
            yield break;
        }

        while (t < duration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        group.alpha = to;

        if (to <= 0.001f)
            group.blocksRaycasts = false;
    }

    void HideAlertImmediate()
    {
        if (waterRisingAlertGroup != null)
        {
            waterRisingAlertGroup.alpha = 0f;
            waterRisingAlertGroup.blocksRaycasts = false;
            if (!waterRisingAlertGroup.gameObject.activeSelf)
                waterRisingAlertGroup.gameObject.SetActive(true);
        }

        if (waterRisingAlertText != null)
            waterRisingAlertText.text = "";
    }

    // ------------------------------
    // TRIGGERS
    // ------------------------------
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Knees"))
        {
            if (playerMovements != null) playerMovements.speed = 5f;
        }

        if (other.CompareTag("Head"))
        {
            isHeadUnderwater = true;
            if (oxygenMeter != null) oxygenMeter.currHealth -= 2;
            if (playerMovements != null) playerMovements.gravity = -3f;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Head"))
        {
            isHeadUnderwater = false;
            if (playerMovements != null)
            {
                playerMovements.gravity = -9.81f;
            }
        }

        if (other.CompareTag("Knees"))
        {
            if (playerMovements != null)
            {
                playerMovements.speed = 3f;

            }
        }
    }
}
