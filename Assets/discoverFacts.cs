using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class discoverFacts : MonoBehaviour
{
    [Header("Audio & Point sys")]
    public AudioManager aud;
    public GameObject ReadBtn;
    public GameObject Trigger;
    public GameObject[] facts;
    public int factIndex;


    [Header("Decipher Slider & Text")]
    public float fillSpeed = 5f; // Speed of fill per second
    private Coroutine fillRoutine;
    public TMP_Text DecipherText;
    public Slider DecipherSlider;
    public Image fill;
    private Coroutine successDelay;



    [Header("Selection crosshair pointer")]
    public GameObject pointer;

    public GameManager gameManager;



    void Start()
    {
        DecipherText.gameObject.SetActive(false);
        DecipherSlider.gameObject.SetActive(false);
        ReadBtn.SetActive(false);
        foreach (GameObject fact in facts) fact.SetActive(false);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) { ReadBtn.SetActive(true); }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) { ReadBtn.SetActive(false); facts[factIndex].SetActive(false); }
    }

    public void DecipherInitiated()
    {
        pointer.SetActive(false);
        DecipherSlider.gameObject.SetActive(true);
        DecipherText.gameObject.SetActive(true);
        ReadBtn.SetActive(false);
        Trigger.SetActive(false);
        if (fillRoutine == null) // prevent multiple coroutines stacking
            fillRoutine = StartCoroutine(FillSlider());
    }

private IEnumerator FillSlider()
{
    while (DecipherSlider.value < DecipherSlider.maxValue)
    {
        // Wait until game is not paused
        while (gameManager.isPaused)
        {
            yield return null; // keep waiting until unpaused
        }

        // Use unscaledDeltaTime so pause/resume works correctly
        DecipherSlider.value += fillSpeed * Time.unscaledDeltaTime;

        yield return null; // next frame
    }

    // Stop when full
    fillRoutine = null;

    // Show success feedback
    fill.color = new Color32(170, 255, 44, 255); // green
    DecipherText.text = "Successful!";

    // Use realtime so pause doesn’t freeze the delay
    yield return new WaitForSecondsRealtime(2f);

    // Now continue
    DecipherSlider.gameObject.SetActive(false);
    DecipherText.gameObject.SetActive(false);
    ReadFacts();
    pointer.SetActive(true);
}







    public void ReadFacts()
    {
        Success(factIndex);
    }

    void Success(int index)
    {
        Debug.Log("Player has discovered a fact!");

        facts[index].SetActive(true);
        DataManager.Instance.playerScore_erudition++;
        aud.PlaySFX(9);

    }
    


}
