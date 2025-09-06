using UnityEngine;

public class CharacterStats : MonoBehaviour, IDamageable
{
    [Header("Name")]
    public string characterName = "John Doe";

    [Header("Core Stats")]
    [SerializeField] int MaxHealth;
    [SerializeField] int MaxMana;

    [Header("Attributes")]
    [SerializeField] int Strength = 1;
    [SerializeField] int Constitution = 1 ;
    [SerializeField] int Dexterity = 1;
    [SerializeField] int Intelligence = 1;

    [Header("Regeneration (must be multiple of 5)")]
   
    [SerializeField] int ManaRegenRate = 5;

    private int m_currentHealth;
    private int m_currentMana;

    private float m_regenTimer = 0f;
    [SerializeField] private float m_regenInterval = 1f; // Regenerate every 1 second


    // Public getters
    public int GetMaxHealth => (MaxHealth - 10)+ (Constitution * 10);
    public int GetCurrentHealth => m_currentHealth;
    public int GetMaxMana => (MaxMana - 10) + (Intelligence * 10);
    public int GetCurrentMana => m_currentMana; 

    public int GetStrength => Strength;
    public int GetConstitution => Constitution;
    public int GetDexterity => Dexterity;
    public int GetIntelligence => Intelligence;

    public void SetPlayerCharacterStats(PlayerArchetypeDefaults data)
    {
        MaxHealth = data.Health;
        MaxMana = data.Mana;

        Strength = data.Strength;
        Dexterity = data.Dexterity;
        Constitution = data.Constitution;
        Intelligence = data.Intelligence;
    }

    public void IncreaseStrength() { Strength++; RecalculateStats(); }
    public void IncreaseConstitution()
    {
        int oldMax = GetMaxHealth;
        Constitution++;
        RecalculateStats();
        int newMax = GetMaxHealth;
        int diff = newMax - oldMax;
        m_currentHealth += diff;
        if (m_currentHealth > newMax)
            m_currentHealth = newMax;
    }
    public void IncreaseDexterity() { Dexterity++; RecalculateStats(); }
    public void IncreaseIntelligence()
    {
        int oldMax = GetMaxMana;
        Intelligence++;
        RecalculateStats();
        int newMax = GetMaxMana;
        int diff = newMax - oldMax;
        m_currentMana += diff;
        if (m_currentMana > newMax)
            m_currentMana = newMax;
    }

    public void DecreaseStrength() { if (Strength > 1) { Strength--; RecalculateStats(); } }
    public void DecreaseConstitution()
    {
        if (Constitution > 1)
        {
            int oldMax = GetMaxHealth;
            float percent = (float)m_currentHealth / oldMax;
            Constitution--;
            RecalculateStats();
            int newMax = GetMaxHealth;
            m_currentHealth = Mathf.FloorToInt(percent * newMax);
            if (m_currentHealth > newMax)
                m_currentHealth = newMax;
            // Snap to nearest lower multiple of 5
            m_currentHealth = (m_currentHealth / 5) * 5;
        }
    }
    public void DecreaseDexterity() { if (Dexterity > 1) { Dexterity--; RecalculateStats(); } }
    public void DecreaseIntelligence()
    {
        if (Intelligence > 1)
        {
            int oldMax = GetMaxMana;
            float percent = (float)m_currentMana / oldMax;
            Intelligence--;
            RecalculateStats();
            int newMax = GetMaxMana;
            m_currentMana = Mathf.FloorToInt(percent * newMax);
            if (m_currentMana > newMax)
                m_currentMana = newMax;
            // Snap to nearest lower multiple of 5
            m_currentMana = (m_currentMana / 5) * 5;
        }
    }

    void Start()
    {
        RecalculateStats();
        m_currentHealth = GetMaxHealth;
        m_currentMana = GetMaxMana;
    }

    void Update()
    {
        Regenerate();
    }

    private void Regenerate()
    {
        m_regenTimer += Time.deltaTime;
        if (m_regenTimer >= m_regenInterval)
        {
            m_regenTimer = 0f;

            // Mana regen (power of 5)
            if (m_currentMana < GetMaxMana)
            {
                m_currentMana += ManaRegenRate;
                if (m_currentMana > GetMaxMana)
                    m_currentMana = GetMaxMana;
            }
        }
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
        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.AwardExperienceToPlayer();
        }
        else
        {
            Debug.Log("CharacterStats: Non-enemy character died.");
        }

        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = false;

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
