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

            // ✅ Deactivate GameObject so save system saves it as inactive
            gameObject.SetActive(false);

            Debug.Log($"Fire extinguished: {gameObject.name}");
        }
    }

    public bool IsActive()
    {
        // ✅ Check both particle system and GameObject active state
        return gameObject.activeSelf && fireParticles != null && fireParticles.isPlaying;
    }
}