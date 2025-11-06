using UnityEngine;

public class SavableFlag : MonoBehaviour
{
    [Header("Unique ID per flag group")]
    public string id;

    [Header("Behaviours to persist (enabled/disabled)")]
    public Behaviour[] behaviours;

    [Header("GameObjects to persist (activeSelf)")]
    public GameObject[] objects;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
            id = System.Guid.NewGuid().ToString();
    }
}