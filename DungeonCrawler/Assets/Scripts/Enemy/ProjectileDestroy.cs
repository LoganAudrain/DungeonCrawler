using System.Collections;
using UnityEngine;


public class ProjectileDestroy : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 10f;
    [SerializeField] private int damageAmount = 0;
    [SerializeField] public ParticleSystem hitEffect;

    private void Start()
    {
       
        StartCoroutine(WaitAndDestroy(destroyDelay));

    }

    IEnumerator WaitAndDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(hitEffect, new Vector3(transform.position.x, transform.position.y, -1f), Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DoDamage(collision.gameObject, damageAmount);
            Destroy(gameObject);
        }

        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DoDamage(other.gameObject, damageAmount);
            Destroy(gameObject);
        }
        
        
    }

    private void DoDamage(GameObject obj, int damage)
    {
        IDamageable damageable = obj.GetComponent<IDamageable>();
        damageable?.TakeDamage(damage);
    }
}