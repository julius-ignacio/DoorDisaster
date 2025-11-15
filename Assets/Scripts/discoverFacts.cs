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

    public GameManager gameManager;




    void Start()
    {
        if (DecipherText != null)
            DecipherText.gameObject.SetActive(false);
        else
            Debug.LogWarning("DecipherText is not assigned in the Inspector.");

        if (DecipherSlider != null)
            DecipherSlider.gameObject.SetActive(false);
        else
            Debug.LogWarning("DecipherSlider is not assigned in the Inspector.");

        if (ReadBtn != null)
            ReadBtn.SetActive(false);
        else

        if (facts != null && facts.Length > 0)
        {
            foreach (GameObject fact in facts)
            {
                if (fact != null)
                    fact.SetActive(false);
                else
                    Debug.LogWarning("One of the facts in the array is missing.");
            }

        }
    }

    public void DecipherInitiated()
    {
        // ✅ Reset slider before starting
        DecipherSlider.value = 0f;
        fill.color = new Color32(255, 255, 255, 255); // reset color to white or default
        DecipherText.text = "Deciphering...";

        // ✅ Hide/disable UI states
        ReadBtn.SetActive(false);

        //trigger plane
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


       public void ReadFacts_water()
    {
        Debug.Log($"Player has discovered fact {factIndex}: {facts[factIndex].name}. Trigger {factIndex}");

        facts[factIndex].SetActive(true);
        Trigger[factIndex].SetActive(false);
        ReadBtn.SetActive(false);


        DataManager.Instance.factsDiscovered++;
        AudioManager.Instance.PlaySFX(8);
    }
}
