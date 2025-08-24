using UnityEngine;

public class panicmetertest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private PanicMeterScript panicMeterScript;

    void OnTriggerEnter(Collider other)
    {
                panicMeterScript.currHealth += 5f; // Increase by 5
    }
}
