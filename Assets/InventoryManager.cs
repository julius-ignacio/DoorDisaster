using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;


public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryUI;
    public HeartSys heal;
    public PanicMeterScript panic;
    public GameObject GreenFlashEffect, BlueFlashEffect, YellowFlashEffect;
    public int water = 0;
    public int medkit = 0;
    public GameNotifier gameNotifier;

    public TextMeshProUGUI medkitCounter, waterCounter;

    void Start()
    {
        inventoryUI.SetActive(false);
    }

    public void ToggleInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
        medkitCounter.text = medkit.ToString();
        waterCounter.text = water.ToString();
    }



    public void UseMedkit()
    {
        if (medkit != 0)
        {
            checkifHealthisFull();
        }

    }


    private void checkifHealthisFull()
    {
        if (heal.currentHearts < 8 && heal.isHelmetUsed == false)
        {
            heal.Heal(1);
            medkit--;
            medkitCounter.text = medkit.ToString();
            inventoryUI.SetActive(false);


            AudioManager.Instance.PlaySFX(19);
            AudioManager.Instance.PlaySFX(8); //points

            GreenFlashEffect.SetActive(true);
            StartCoroutine(FlashFade(GreenFlashEffect.GetComponent<CanvasGroup>(), 1));
        }
    }




    public void DrinkWater()
    {
        if (water != 0)
        {
            water--;
            waterCounter.text = water.ToString();

            inventoryUI.SetActive(false);
            panic.currHealth -= 20;
            AudioManager.Instance.PlaySFX(18);

            BlueFlashEffect.SetActive(true);
            StartCoroutine(FlashFade(BlueFlashEffect.GetComponent<CanvasGroup>(), 1f));
        }
    }


        private void checkifPanicisNotHigh()
    {
        if (panic.currHealth <= 75)
        {
            heal.Heal(1);
            medkit--;
            medkitCounter.text = medkit.ToString();
            inventoryUI.SetActive(false);


            AudioManager.Instance.PlaySFX(19);
            AudioManager.Instance.PlaySFX(8); //points

            GreenFlashEffect.SetActive(true);
            StartCoroutine(FlashFade(GreenFlashEffect.GetComponent<CanvasGroup>(), 1));
        } else
        {
            
        }
    }

    private IEnumerator FlashFade(CanvasGroup flashGroup, float duration)
    {
        flashGroup.gameObject.SetActive(true);
        flashGroup.alpha = 1f;
        yield return new WaitForSeconds(duration);
        flashGroup.alpha = 0f;
        flashGroup.gameObject.SetActive(false);
    }
}
