using UnityEngine;

public class SpreadFire : MonoBehaviour
{
    private ParticleSystem fireParticles;

    void Awake()
    {
        fireParticles = GetComponent<ParticleSystem>();
        if (fireParticles == null)
        {
            Debug.LogError("No ParticleSystem found on SpreadFire!");
        }
    }

    public void Extinguish()
    {
        if (fireParticles != null && fireParticles.isPlaying)
        {
            fireParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            Debug.Log("Fire extinguished!");
        }
    }

    public bool IsActive()
    {
        return fireParticles != null && fireParticles.isPlaying;
    }
}
