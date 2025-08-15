using UnityEngine;

public class CharacterStats : MonoBehaviour, IDamageable
{

    [Header("Core Stats")]
    [SerializeField] int MaxHealth;
    [SerializeField] int MaxMana;

    [Header("Attributes")]
    [SerializeField] int Strength = 1;
    [SerializeField] int Constitution = 1 ;
    [SerializeField] int Dexterity = 1;
    [SerializeField] int Intelligence = 1;

    private int m_currentHealth;
    private int m_currentMana;

    // Public getters
    public int GetMaxHealth => MaxHealth + Constitution * 2;
    public int GetCurrentHealth => m_currentHealth;
    public int GetMaxMana => MaxMana + Intelligence * 2;
    public int GetCurrentMana => m_currentMana; // Assuming current mana is same as max mana for now

    public int GetStrength => Strength;
    public int GetConstitution => Constitution;
    public int GetDexterity => Dexterity;
    public int GetIntelligence => Intelligence;

    public void IncreaseStrength() { Strength++; RecalculateStats(); }
    public void IncreaseConstitution() { Constitution++; RecalculateStats(); }
    public void IncreaseDexterity() { Dexterity++; RecalculateStats(); }
    public void IncreaseIntelligence() { Intelligence++; RecalculateStats(); }

    public void DecreaseStrength() { if (Strength > 1) { Strength--; RecalculateStats(); } }
    public void DecreaseConstitution() { if (Constitution > 1) { Constitution--; RecalculateStats(); } }
    public void DecreaseDexterity() { if (Dexterity > 1) { Dexterity--; RecalculateStats(); } }
    public void DecreaseIntelligence() { if (Intelligence > 1) { Intelligence--; RecalculateStats(); } }

    void Start()
    {
        RecalculateStats();
        m_currentHealth = MaxHealth;
        m_currentMana = MaxMana;
    }

    private void RecalculateStats()
    {
        // Optionally clamp current health/mana to new max
        if (m_currentHealth > GetMaxHealth)
            m_currentHealth = GetMaxHealth;
        if (m_currentMana > GetMaxMana)
            m_currentMana = GetMaxMana;
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
