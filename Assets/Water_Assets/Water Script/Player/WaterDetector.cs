using UnityEngine;

public class WaterDetector : MonoBehaviour
{
    public PlayerOxygen_Water playerOxygen;
    public Transform playerCamera;   // Drag your Main Camera here
    public Transform waterSurface;   // Drag the water plane here

    private void Update()
    {
        if (playerOxygen == null || playerCamera == null || waterSurface == null)
            return;

        // Compare Y positions
        bool underwaterNow = playerCamera.position.y < waterSurface.position.y;

        if (underwaterNow != playerOxygen.isUnderwater)
        {
            playerOxygen.isUnderwater = underwaterNow;

            if (underwaterNow)
                Debug.Log("📸 Camera went underwater");
            else
                Debug.Log("📸 Camera surfaced");
        }
    }
}
