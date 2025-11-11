using UnityEngine;
using TMPro;
using Unity.Android.Gradle.Manifest;
using System.Collections;
using System;

public class Objectives_water : MonoBehaviour
{
    public TMP_Text objectivetext1;
    public TMP_Text objectivetext2;
    public TMP_Text objectivetext3;
    public GameObject[] ItemsToCollect;
   // public GameObject balconyBarrier;
    public bool radioListened = false;
    public bool breakerTurnedOFF = false;



    public GameNotifier gameNotifier;
    private bool AllobjectivesCompleted = false;

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
            enableCollidersAndOutlines(0, 1, 2);
            StartCoroutine(Transition(2f));
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

            StartCoroutine(Transition(2f));
            Objective3();
        }
    }

    void Objective3()
    {
        objectivetext1.text = "Head down to the basement.";
        objectivetext2.text = "Turn off the breaker.";
        objectivetext3.text = " ";


        if (breakerTurnedOFF)
        {
            enableCollidersAndOutlines(3, 0, 0);

            StartCoroutine(Transition(2f));
            Objective4();
        }
    }

    void Objective4()
    {
        objectivetext1.text = "Get the key to open the office room.";
        objectivetext2.text = "";
        objectivetext3.text = "";


        if (ItemsToCollect[3].activeSelf == false)
        {
            enableCollidersAndOutlines(4, 5, 6);

            StartCoroutine(Transition(2f));
            Objective5();
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
            enableCollidersAndOutlines(7, 8, 9);

            StartCoroutine(Transition(2f));
            Objective6();
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
            enableCollidersAndOutlines(10, 11, 0);

            StartCoroutine(Transition(2f));
            Objective7();
        }
    }

    void Objective7()
    {
        objectivetext1.text = "Get the Walkie Talkie.";
        objectivetext2.text = "Get the ket to open the Garage.";
        objectivetext3.text = " ";


        if (ItemsToCollect[10].activeSelf == false &&
            ItemsToCollect[11].activeSelf == false)
        {
            enableCollidersAndOutlines(12, 0, 0);

            StartCoroutine(Transition(2f));
            Objective8();
        }
    }


    void Objective8()
    {
        objectivetext1.text = "Head to the kitchen and get food supplies.";
        objectivetext2.text = " ";
        objectivetext3.text = " ";

        if (ItemsToCollect[12].activeSelf == false)
        {
            enableCollidersAndOutlines(13, 14, 15);

            StartCoroutine(Transition(2f));
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

            StartCoroutine(Transition(2f));
            Objective10();  
        }
    }

        void Objective10()
    {
        objectivetext1.text = "ESCAPE THE HOUSE!";
        objectivetext2.text = " ";
        objectivetext3.text = " ";

    }
    

    void enableCollidersAndOutlines(int index1, int index2, int index3)
    {
        ItemsToCollect[index1].GetComponent<Outline>().enabled = true;
        ItemsToCollect[index1].GetComponent<BoxCollider>().enabled = true;

        ItemsToCollect[index2].GetComponent<Outline>().enabled = true;
        ItemsToCollect[index2].GetComponent<BoxCollider>().enabled = true;

        ItemsToCollect[index3].GetComponent<Outline>().enabled = true;
        ItemsToCollect[index3].GetComponent<BoxCollider>().enabled = true;
    }




    
      private IEnumerator Transition(float duration)
    {
        objectivetext1.color = Color.green;
        objectivetext2.color = Color.green;
        objectivetext3.color = Color.green;
        yield return new WaitForSeconds(duration);
                objectivetext1.color = Color.white;
        objectivetext2.color = Color.white;
        objectivetext3.color = Color.white;
    }
}
