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
        // Add loot logic here if needed
    }
}
