using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GameGuide : MonoBehaviour, IPointerClickHandler
{
    public GameObject Header, Body, Footer, Bg, IntroPanel;

    private bool isVisible = false; // tracks if tips are currently shown

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isVisible)
        {
            // Show tips
            Header.GetComponent<TextMeshProUGUI>().text = "Game Tips:";

            var bodyText = Body.GetComponent<TextMeshProUGUI>();
            bodyText.fontSize = 35;
            bodyText.text =
                "Running will accelerate the increase of panic meter.\n" +
                "You can take cover underneath empty tables.\n" +
                "Feel interval between earthquakes.";

            Bg.SetActive(true); // show background if you want
        }
        else
        {
            IntroPanel.SetActive(false); // hide intro panel
           

           // add here setting joysticks to enable!!!!
        }

        isVisible = !isVisible; // flip state
    }
}
