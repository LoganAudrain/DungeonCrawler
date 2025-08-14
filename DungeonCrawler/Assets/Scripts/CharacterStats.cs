using UnityEngine;

public class CharacterStats : MonoBehaviour, IDamageable
{
    [SerializeField] int MaxHealth;

    private int m_currentHealth;


    void Start()
    {
        m_currentHealth = MaxHealth;
    }

    public void TakeDamage(int dmg)
    {
        m_currentHealth -= dmg;

        Animator animator = GetComponent<Animator>();
        if(animator != null)
        {
            animator.SetTrigger("Damage");
        }

        if (m_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelfDestroy(float timer)
    {
        Destroy(gameObject, timer);
    }
}
