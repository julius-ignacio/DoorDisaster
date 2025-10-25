using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryUI;
    public HeartSys heal;
    public PanicMeterScript panic;
    public GameObject GreenFlashEffect, BlueFlashEffect, YellowFlashEffect;
    public int water = 0;
    public int medkit = 0;
    void Start()
    {
        inventoryUI.SetActive(false);
    }

public void ToggleInventory()
{
    inventoryUI.SetActive(!inventoryUI.activeSelf);
}



    public void UseMedkit()
    {
        if(medkit != 0)
        {
                    medkit--;

        inventoryUI.SetActive(false);
        heal.Heal(1);
                    AudioManager.Instance.PlaySFX(19);

        GreenFlashEffect.SetActive(true);
        StartCoroutine(FlashFade(GreenFlashEffect.GetComponent<CanvasGroup>(), 1));
        }
        
    }


    public void DrinkWater()
    {
        if(water != 0)
        {
                    water--;
        inventoryUI.SetActive(false);
        panic.currHealth -= 20;
                AudioManager.Instance.PlaySFX(18);

        BlueFlashEffect.SetActive(true);
        StartCoroutine(FlashFade(BlueFlashEffect.GetComponent<CanvasGroup>(), 1f));
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
