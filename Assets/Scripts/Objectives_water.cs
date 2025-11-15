using UnityEngine;
using TMPro;
using System.Collections;
using System;
using EasyDoorSystem;


public class Objectives_water : MonoBehaviour
{
    public TMP_Text objectivetext1;
    public TMP_Text objectivetext2;
    public TMP_Text objectivetext3;
    public GameObject[] ItemsToCollect;  //keys - 0 3 4 7 11 15
    public GameObject[] Doorlocks;
    // public GameObject balconyBarrier;
    public bool radioListened = false;
    public bool breakerTurnedOFF = false;



    [Header("Lights")]
    public GameObject[] lightsToTurnOff;


    public InventoryManager inventory;



    public GameNotifier gameNotifier;
    private bool AllobjectivesCompleted = false;

    // Add to class:
public bool gaveO2, gaveO4, gaveO5, gaveO6, gaveO7, gaveO8, gaveO9;

    void Start()
    {
        // Disable all outlines & colliders at start (safe checks)
        if (ItemsToCollect != null)
        {
            for (int i = 0; i < ItemsToCollect.Length; i++)
            {
                var item = ItemsToCollect[i];
                if (item == null) continue;

                if (item.TryGetComponent<Outline>(out var outline))
                    outline.enabled = false;

                if (item.TryGetComponent<Collider>(out var col))
                    col.enabled = false;
            }
        }
    }


    void EnableInteractionForItem(GameObject item)
    {
        if (item == null) return;

        if (item.TryGetComponent<Outline>(out var outline))
            outline.enabled = true;

        if (item.TryGetComponent<Collider>(out var col))
            col.enabled = true;
    }


    void Update()
    {
        Objective1();

    }



    void Objective1()
    {
        objectivetext1.text = "Listen to the radio.";
        objectivetext2.text = " ";
        objectivetext3.text = " ";


        if (radioListened)
        {
            EnableInteractionForItem(ItemsToCollect[0]);
            EnableInteractionForItem(ItemsToCollect[1]);
            EnableInteractionForItem(ItemsToCollect[2]);
            Objective2();
        }
    }


    void Objective2()
    {
        objectivetext1.text = "Find the key to the basement.";
        objectivetext2.text = "Get a Backpack.";
        objectivetext3.text = "Get a Flashlight.";


        if (ItemsToCollect[0].activeSelf == false &&
            ItemsToCollect[1].activeSelf == false &&
            ItemsToCollect[2].activeSelf == false)
        {
            Doorlocks[0].GetComponent<EasyDoor>().enabled = true;
            Doorlocks[0].GetComponent<BoxCollider>().enabled = false;

            if (!gaveO2) { inventory.updateImportantItemsCounter(3); gaveO2 = true; }

            Objective3();
        }

        else
        {
            Doorlocks[0].GetComponent<EasyDoor>().enabled = false;
        }
    }

    void Objective3()
    {
        objectivetext1.text = "Head down to the basement.";
        objectivetext2.text = "Turn off the breaker.";
        objectivetext3.text = " ";


        if (breakerTurnedOFF)
        {

            foreach (var light in lightsToTurnOff)
            {
                light.SetActive(false);
            }

            EnableInteractionForItem(ItemsToCollect[3]);

            Objective4();
        }
    }

    void Objective4()
    {
        objectivetext1.text = "Get the key beside the breaker to open the office room.";
        objectivetext2.text = "";
        objectivetext3.text = "";


        if (ItemsToCollect[3].activeSelf == false)
        {
            Doorlocks[1].GetComponent<EasyDoor>().enabled = true;
            Doorlocks[1].GetComponent<BoxCollider>().enabled = false;

           if (!gaveO4) { inventory.updateImportantItemsCounter(1); gaveO4 = true; }


            EnableInteractionForItem(ItemsToCollect[4]);
            EnableInteractionForItem(ItemsToCollect[5]);
            EnableInteractionForItem(ItemsToCollect[6]);

            Objective5();
        }

        else
        {
            Doorlocks[1].GetComponent<EasyDoor>().enabled = false;
        }
    }


    void Objective5()
    {
        objectivetext1.text = "Get the flare gun.";
        objectivetext2.text = "Get the document.";
        objectivetext3.text = "Get the key to open the Study Room";


        if (ItemsToCollect[4].activeSelf == false &&
            ItemsToCollect[5].activeSelf == false &&
            ItemsToCollect[6].activeSelf == false)
        {
            Doorlocks[2].GetComponent<EasyDoor>().enabled = true;
            Doorlocks[2].GetComponent<BoxCollider>().enabled = false;

           if (!gaveO5) { inventory.updateImportantItemsCounter(3); gaveO5 = true; }


            EnableInteractionForItem(ItemsToCollect[7]);
            EnableInteractionForItem(ItemsToCollect[8]);
            EnableInteractionForItem(ItemsToCollect[9]);


            Objective6();
        }
        else
        {
            Doorlocks[2].GetComponent<EasyDoor>().enabled = false;
        }
    }


    void Objective6()
    {
        objectivetext1.text = "Get the Medkit.";
        objectivetext2.text = "Get water.";
        objectivetext3.text = "Get the key to open the Parent's Bedroom.";


        if (ItemsToCollect[7].activeSelf == false &&
            ItemsToCollect[8].activeSelf == false &&
            ItemsToCollect[9].activeSelf == false)
        {
            Doorlocks[3].GetComponent<EasyDoor>().enabled = true;
            Doorlocks[3].GetComponent<BoxCollider>().enabled = false;

            if (!gaveO6) { inventory.updateImportantItemsCounter(3); gaveO6 = true; }



            EnableInteractionForItem(ItemsToCollect[10]);
            EnableInteractionForItem(ItemsToCollect[11]);


            Objective7();
        }
        else
        {
            Doorlocks[3].GetComponent<EasyDoor>().enabled = false;
        }
    }

    void Objective7()
    {
        objectivetext1.text = "Get the Walkie Talkie.";
        objectivetext2.text = "Get the key to open the Garage.";
        objectivetext3.text = " ";


        if (ItemsToCollect[10].activeSelf == false &&
            ItemsToCollect[11].activeSelf == false)
        {
            Doorlocks[4].GetComponent<EasyDoor>().enabled = true;
            Doorlocks[4].GetComponent<BoxCollider>().enabled = false;

       if (!gaveO7) { inventory.updateImportantItemsCounter(2); gaveO7 = true; }


            EnableInteractionForItem(ItemsToCollect[12]);

            Objective8();
        }
        else
        {
            Doorlocks[4].GetComponent<EasyDoor>().enabled = false;
        }
    }


    void Objective8()
    {
        objectivetext1.text = "Head to the kitchen and get food supplies.";
        objectivetext2.text = " ";
        objectivetext3.text = " ";

        if (ItemsToCollect[12].activeSelf == false)
        {
            EnableInteractionForItem(ItemsToCollect[13]);
            EnableInteractionForItem(ItemsToCollect[14]);
            EnableInteractionForItem(ItemsToCollect[15]);

      if (!gaveO8) { inventory.updateImportantItemsCounter(1); gaveO8 = true; }


            Objective9();
        }
    }

    void Objective9()
    {
        objectivetext1.text = "Get the rope in the basement.";
        objectivetext2.text = "Get a duct tape.";
        objectivetext3.text = "Get the key to open the Balcony.";

        if (ItemsToCollect[13].activeSelf == false &&
            ItemsToCollect[14].activeSelf == false &&
            ItemsToCollect[15].activeSelf == false)
        {

            Doorlocks[5].GetComponent<BoxCollider>().enabled = true;

           if (!gaveO9) { inventory.updateImportantItemsCounter(3); gaveO9 = true; }


            Objective10();
        }
        else
        {
            Doorlocks[5].GetComponent<BoxCollider>().enabled = false;
        }
    }

    void Objective10()
    {
        objectivetext1.text = "Open the Balcony door and escape.";
        objectivetext2.text = " ";
        objectivetext3.text = " ";

    }


    public void ApplySavedState(bool radio, bool breaker)
{
    radioListened = radio;
    breakerTurnedOFF = breaker;

    // Reconstruct scene side-effects:
    // If radio already listened, enable basement key/backpack/flashlight interactions
    if (radioListened)
    {
        EnableInteractionForItem(ItemsToCollect[0]);
        EnableInteractionForItem(ItemsToCollect[1]);
        EnableInteractionForItem(ItemsToCollect[2]);
    }

    // If breaker turned off, ensure lights are off and basement progression items enabled
    if (breakerTurnedOFF)
    {
        foreach (var light in lightsToTurnOff)
        {
            if (light != null) light.SetActive(false);
        }
        EnableInteractionForItem(ItemsToCollect[3]);
    }

    // Force an immediate objective text refresh so UI matches restored state.
    Objective1();
}
}
