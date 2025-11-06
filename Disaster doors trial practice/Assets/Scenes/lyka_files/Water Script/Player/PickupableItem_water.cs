using UnityEngine;

public class PickupableItem_water : MonoBehaviour
{
    [SerializeField] private string itemName = "Key";
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;

    private Transform player;
    private bool isInRange = false;

    private void Start()
    {
        // Find the player - adjust the tag if needed
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogWarning("PickupableItem: No player found with 'Player' tag!");
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Check distance to player
        float distance = Vector3.Distance(transform.position, player.position);
        isInRange = distance <= pickupRange;

        // Pick up when pressing E and in range
        if (isInRange && Input.GetKeyDown(pickupKey))
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        // Show UI message
        if (ItemPickedKey_Water.Instance != null)
        {
            ItemPickedKey_Water.Instance.ShowPickedKey(itemName);
        }

        // Destroy the item
        Destroy(gameObject);
    }

    // Optional: visualize pickup range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}