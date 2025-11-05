using UnityEngine;

public class SavableObject : MonoBehaviour
{
    [Header("Unique ID per object in scene")]
    public string objectID;

    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;

    private void OnValidate()
    {
        // Auto-generate unique ID if missing
        if (string.IsNullOrEmpty(objectID))
        {
            objectID = System.Guid.NewGuid().ToString();
        }
    }
}
