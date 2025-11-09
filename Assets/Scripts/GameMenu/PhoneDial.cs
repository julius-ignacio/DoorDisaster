using System;
using System.Collections.Generic;
using UnityEngine;


public class PhoneDial : MonoBehaviour
{
    private List<String> Numbers = new List<string>()
    {
        "911",
        "(02) 8911-5061",
 "(02) 8790-2300143",
 "(02) 8651-7800",
 "(02) 8426-0219",
 "(02) 8981-7000",
    };

    public void CallNumber(int index)
    {
   
             string telURI = "tel:" + Numbers[index];
            Application.OpenURL(telURI);
        }
    }
