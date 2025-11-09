using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class discoverFacts : MonoBehaviour
{
    [Header("Audio & Point sys")]
    public GameObject ReadBtn;
    public GameObject[] Trigger;
    public GameObject[] facts;
    public int factIndex;

    public Objectives objectives;

    [Header("Decipher Slider & Text")]
    public float fillSpeed = 7f;
    private Coroutine fillRoutine;
    public TMP_Text DecipherText;
    public Slider DecipherSlider;
    public Image fill;

    [Header("Selection crosshair pointer")]
    public GameObject pointer;
    public GameManager gameManager;

    void Start()
    {
        DecipherText.gameObject.SetActive(false);
        DecipherSlider.gameObject.SetActive(false);
        ReadBtn.SetActive(false);
        foreach (GameObject fact in facts)
            fact.SetActive(false);
    }

    public void DecipherInitiated()
    {
        // ✅ Reset slider before starting
        DecipherSlider.value = 0f;
        fill.color = new Color32(255, 255, 255, 255); // reset color to white or default
        DecipherText.text = "Deciphering...";

        // ✅ Hide/disable UI states
        pointer.SetActive(false);
        ReadBtn.SetActive(false);
        Trigger[factIndex].SetActive(false);

        DecipherSlider.gameObject.SetActive(true);
        DecipherText.gameObject.SetActive(true);

        // ✅ Stop old coroutine if still running (safety)
        if (fillRoutine != null)
        {
            StopCoroutine(fillRoutine);
            fillRoutine = null;
        }

        // ✅ Start new fill
        fillRoutine = StartCoroutine(FillSlider());
    }

    private IEnumerator FillSlider()
    {
        // Fill gradually
        while (DecipherSlider.value < DecipherSlider.maxValue)
        {
            // Pause if game is paused
            while (gameManager.isPaused)
            {
                yield return null;
            }

            DecipherSlider.value += fillSpeed * Time.unscaledDeltaTime;
            yield return null;
        }

        fillRoutine = null;

        // ✅ Success feedback
        fill.color = new Color32(170, 255, 44, 255);
        DecipherText.text = "Successful!";

        yield return new WaitForSecondsRealtime(1.5f);

        // ✅ Hide decipher UI properly
        DecipherSlider.gameObject.SetActive(false);
        DecipherText.gameObject.SetActive(false);

        // ✅ Continue logic
        ReadFacts(factIndex);
        pointer.SetActive(true);
    }

    void ReadFacts(int index)
    {
        Debug.Log($"Player has discovered fact {index}: {facts[index].name}");

        facts[index].SetActive(true);
        DataManager.Instance.quizScore++;
        DataManager.Instance.factsDiscovered++;
        AudioManager.Instance.PlaySFX(8);

        objectives.UpdateObjectives();
    }
}
