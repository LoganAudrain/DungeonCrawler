using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] int MaxHealth;

    public int m_currentHealth;

    public delegate void OnHealthChanged(int current, int max);
    public event OnHealthChanged HealthChanged;

    void Start()
    {
        m_currentHealth = MaxHealth;
        HealthChanged?.Invoke(m_currentHealth, MaxHealth);
    }

    public void TakeDamage(int dmg)
    {
        m_currentHealth = Mathf.Max(m_currentHealth - dmg, 0);
        HealthChanged?.Invoke(m_currentHealth, MaxHealth);

        if (m_currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        m_currentHealth = Mathf.Min(m_currentHealth + amount, MaxHealth);
        HealthChanged?.Invoke(m_currentHealth, MaxHealth);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
