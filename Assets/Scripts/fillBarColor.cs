using UnityEngine;
using UnityEngine.UI;

public class fillBarColor : MonoBehaviour
{
    public Image fillImage; // assign your UI Image
     void Start()
    {
        // Example: Change the color to red at the start
        if (fillImage != null)
        {
            fillImage.color = Color.red;
        }
    }

    // Example function to change color dynamically
    public void SetFillColor(Color newColor)
    {
        if (fillImage != null)
        {
            fillImage.color = newColor;
        }
    }
}
