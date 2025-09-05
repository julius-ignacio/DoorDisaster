using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Header("Item Properties")]
    public string itemName = "Item";
    public int value = 1; // How much this item is worth (coins, health, ammo, etc.)
    public ItemType itemType = ItemType.Generic;

    [Header("Visual Effects")]
    public bool rotateItem = true;
    public float rotationSpeed = 50f;
    public bool bobUpDown = false;
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;

    [Header("Audio")]
    public AudioClip customPickupSound;

    private Vector3 startPosition;
    private Rigidbody rb;

    public enum ItemType
    {
        Generic,
        Towel,
        Food
        
    }

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();

        // Make sure this object has the "Pickup" tag
        if (!gameObject.CompareTag("Pickup"))
        {
            Debug.LogWarning(itemName + " should have the 'Pickup' tag!");
        }
    }

    void Update()
    {
        // Rotate the item visually
        if (rotateItem)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }

        // Bob up and down only if no Rigidbody, or Rigidbody is kinematic
        if (bobUpDown && (rb == null || rb.isKinematic))
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    // Optional: Handle pickup with collision instead of distance check
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PickupScript playerPickup = other.GetComponent<PickupScript>();
            if (playerPickup != null)
            {
                // Auto pickup on collision
                // playerPickup.PickupItem(gameObject); // Uncomment if you want auto-pickup
            }
        }
    }
}