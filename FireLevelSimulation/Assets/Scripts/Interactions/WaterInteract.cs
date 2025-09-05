using UnityEngine;

public class WaterInteract : MonoBehaviour
{
    public ParticleSystem waterParticle;
    private bool isWaterOn = false;

    public void Interact()
    {
        if (isWaterOn)
        {
            waterParticle.Stop();
            isWaterOn = false;
        }
        else
        {
            waterParticle.Play();
            isWaterOn = true;
        }
    }
}
