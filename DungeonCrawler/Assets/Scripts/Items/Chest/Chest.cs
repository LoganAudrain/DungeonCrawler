using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{

    private Animator animator;
    private bool isOpen = false;
    private bool playerInRange = false;

    [Header("Chest Settings")]
    [SerializeField] private GameObject mimicPrefab;
    [Range(0, 1)]
    [SerializeField] private float mimicChance = 0.2f; // 20% chance to be a mimic
    [Range(0, 1)]
    [SerializeField] private float emptyChance = 0.2f; // 20% chance to be empty

    [Header("Loot Table")]
    [SerializeField] private LootTable lootTable;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float dropRadius = 1.0f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerInRange && !isOpen)
        {
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                OpenChest();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void OpenChest()
    {
        isOpen = true;

        float roll = Random.value;
        if (roll < mimicChance)
        {
            if (mimicPrefab != null)
            {
                Instantiate(mimicPrefab, transform.position, Quaternion.identity);
                Debug.Log("It's a mimic!");
            }
            Destroy(gameObject);
            return;
        }
        else if (roll < mimicChance + emptyChance)
        {
            animator.SetBool("isEmpty", true); // Play empty animation
            Debug.Log("Chest is empty!");
            return;
        }

        animator.SetBool("isOpen", true); // Play normal open animation
        Debug.Log("Chest opened and gave loot!");

      
    }
    void DropLoot()
    {
        if (lootTable == null || lootTable.lootEntries == null || lootTable.lootEntries.Length == 0)
            return;

        int dropCount = Random.Range(3, 6); // 3 to 5 items
        Vector2[] directions = {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right,
            new Vector2(1,1).normalized, new Vector2(-1,1).normalized,
            new Vector2(1,-1).normalized, new Vector2(-1,-1).normalized
        };

        System.Collections.Generic.List<int> availableDirs = new System.Collections.Generic.List<int>();
        for (int i = 0; i < directions.Length; i++) availableDirs.Add(i);

        for (int i = 0; i < dropCount && availableDirs.Count > 0; i++)
        {
            // Pick a random loot entry based on drop chance
            var entry = lootTable.GetRandomEntry();
            if (entry == null || entry.itemPrefab == null) continue;

            // Pick a random available direction
            int dirIdx = availableDirs[Random.Range(0, availableDirs.Count)];
            availableDirs.Remove(dirIdx);
            Vector2 dropPos = (Vector2)transform.position + directions[dirIdx] * dropRadius;

            // Check for obstacles
            Collider2D hit = Physics2D.OverlapCircle(dropPos, 0.2f, obstacleMask);
            if (hit == null)
            {
                Instantiate(entry.itemPrefab, dropPos, Quaternion.identity);
            }
        }
    }
}
