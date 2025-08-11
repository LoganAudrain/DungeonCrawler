using UnityEngine;

public class ProjectileDestroy : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 10f; 

    private void Start()
    {
        Destroy(gameObject, destroyDelay); // Destroy after ?? seconds
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}