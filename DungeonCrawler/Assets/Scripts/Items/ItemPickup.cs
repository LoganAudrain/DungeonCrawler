using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] ItemStats itemStats;
    public float pickupDelay = 0f;
    private float spawnTime;
    void Start()
    {
        spawnTime = Time.time;
    }
    private bool CanBePickedUp()
    {
        return Time.time >= spawnTime + pickupDelay;

    }
    public void SetPickupDelay(float delay)
    {
        pickupDelay = delay;
        spawnTime = Time.time;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!CanBePickedUp())
            {
                return;
            }


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
