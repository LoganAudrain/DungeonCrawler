using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [SerializeField] int MaxHealth;

    private int m_currentHealth;

    public delegate void OnHealthChanged(int current, int max);
    public event OnHealthChanged HealthChanged;

    void Start()
    {
        m_currentHealth = MaxHealth;
        HealthChanged?.Invoke(m_currentHealth, MaxHealth);
    }

    public void TakeDamage(int dmg)
    {
        m_currentHealth -= dmg;

        Animator animator = GetComponent<Animator>();
        if (animator != null)
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
}
