using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] ItemStats itemStats;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null)
            {
                bool wasPickedUp = inventory.AddItem(itemStats);
                if (wasPickedUp)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
