using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public AttackData attackData;
    public Transform firePoint; // For ranged attacks

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float detectionRange = 10f;

    [Header("Stats")]
    public int maxHp = 100;
    private int currentHp;

    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    private Transform target;
    private float attackTimer;


    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHp = maxHp;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;
        else
            Debug.LogWarning("EnemyAI2D could not find Player with tag 'Player'.");
    }

    void Update()
    {
        if (!target || !attackData) return;

        float distance = Vector2.Distance(transform.position, target.position);

        // Make firePoint face the player
        if (firePoint && target)
        {
            Vector2 dir = (target.position - firePoint.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            firePoint.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (distance <= detectionRange)
        {
            if (attackData.attackType == AttackType.Melee)
                HandleMelee(distance);
            else if (attackData.attackType == AttackType.Ranged)
                HandleRanged(distance);
        }

        attackTimer -= Time.deltaTime;
    }
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        // Play death animation, disable enemy, etc.
        Destroy(gameObject);
    }

    void HandleMelee(float distance)
    {
        if (distance > attackData.attackRangeMelee)
            MoveTowardsTarget();
        else
            Attack();
    }

    void HandleRanged(float distance)
    {
        if (distance > attackData.attackRangeRanged)
            MoveTowardsTarget();
        else
            Attack();
    }

    void MoveTowardsTarget()
    {
        Vector2 dir = (target.position - transform.position).normalized;

        Vector3 movement = new Vector3(dir.x, dir.y, 0f);
        transform.position += moveSpeed * Time.deltaTime * movement;

        // Animation speed
        animator.SetFloat("speed", movement.magnitude);

        // Sprite flipping
        if (movement.x > 0) spriteRenderer.flipX = false;
        else if (movement.x < 0) spriteRenderer.flipX = true;
    }

    void Attack()
    {
        // Stop movement animation when attacking
        animator.SetFloat("speed", 0f);

        if (attackTimer > 0) return;

        if (attackData.attackType == AttackType.Melee)
        {
            Debug.Log("Melee attack for " + attackData.meleeDamage + " damage!");
            // Call player damage function here
        }
        else if (attackData.attackType == AttackType.Ranged)
        {
            if (attackData.projectilePrefab && firePoint)
            {
                GameObject proj = Instantiate(attackData.projectilePrefab, firePoint.position, firePoint.rotation);
                Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
                if (rb) rb.linearVelocity = (target.position - firePoint.position).normalized * attackData.projectileSpeed;
            }
        }

        attackTimer = attackData.attackCooldown;
    }
}
