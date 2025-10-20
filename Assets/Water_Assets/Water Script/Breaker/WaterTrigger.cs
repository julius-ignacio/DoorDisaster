using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
    public WaterRising water;  // Drag your "Water" object with WaterRising here in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            water.StartFloodSequence(); // ✅ Correct method name
            Debug.Log("Player entered trigger -> water rising");
        }
    }
}
