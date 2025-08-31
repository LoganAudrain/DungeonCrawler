using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public AttackData attackData;
    public Transform firePoint; // For ranged attacks

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float detectionRange = 10f;



    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    private Transform target;
    private float attackTimer;
    private bool isAttacking = false;


    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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
            Vector2 toPlayer = target.position - transform.position;
            float minVerticalThreshold = 1f; // Sensitivity for "directly above/below"

            float xOffset, yOffset;

            if (Mathf.Abs(toPlayer.x) < minVerticalThreshold && Mathf.Abs(toPlayer.y) > minVerticalThreshold)
            {
                // Player is directly above or below
                xOffset = 0f;
                yOffset = Mathf.Sign(toPlayer.y) * 0.11f;
            }
            else if (Mathf.Abs(toPlayer.x) > minVerticalThreshold && Mathf.Abs(toPlayer.y) > minVerticalThreshold)
            {
                // Diagonal
                xOffset = Mathf.Sign(toPlayer.x) * 0.11f;
                yOffset = Mathf.Sign(toPlayer.y) * 0.11f;
            }
            else
            {
                // Mostly horizontal
                xOffset = Mathf.Sign(toPlayer.x) * 0.11f;
                yOffset = 0f;
            }

            firePoint.localPosition = new Vector3(xOffset, yOffset, firePoint.localPosition.z);

            // Make firePoint face the player
            Vector2 dir = (target.position - firePoint.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (attackData.flipProjectile)
                angle += 180f;
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

    public void AwardExperienceToPlayer()
    {
        if (attackData == null) return;
        int exp = attackData.Experience;
        if (exp <= 0) { 
            Debug.Log("No experience to award.");
            return;
        }

        
        
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            var menu = gameManager.GetComponent<InventoryMenu>();
            if (menu != null)
            {
               menu.AddExperience(exp);
               Debug.Log("Awarded " + exp + " experience to player.");
            }
            else
            {
              Debug.LogWarning("Player does not have InventoryMenu component to receive experience.");
            }
        }
        else
        {
            Debug.LogWarning("Could not find Player to award experience.");
        }
    }

    void HandleMelee(float distance)
    {
        if (distance > attackData.attackRangeMelee)
        {
            isAttacking = false;
            MoveTowardsTarget();
        }
        else
            Attack();
    }

    void HandleRanged(float distance)
    {
        if (distance > attackData.attackRangeRanged)
        {
            isAttacking = false;
            MoveTowardsTarget();
        }
        else
            Attack();
    }

    void MoveTowardsTarget()
    {
        if (isAttacking) return;

        Vector2 dir = (target.position - transform.position).normalized;

        Vector3 movement = new Vector3(dir.x, dir.y, 0f);
        transform.position += moveSpeed * Time.deltaTime * movement;

        // Animation speed
        animator.SetFloat("speed", movement.magnitude);

        // Sprite flipping
        if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
            if (firePoint != null)
                firePoint.localPosition = new Vector3(Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, firePoint.localPosition.z);
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
            if (firePoint != null)
                firePoint.localPosition = new Vector3(-Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, firePoint.localPosition.z);
        }
    }

    void Attack()
    {
        // Stop movement animation when attacking


        isAttacking = true;

        float targetSpeed = 0f;
        float dampTime = 0.01f;

        animator.SetFloat("speed", targetSpeed, dampTime, Time.deltaTime);

        if (attackTimer > 0) return;

        if (attackData.attackType == AttackType.Melee)
        {
            Debug.Log("Melee attack for " + attackData.meleeDamage + " damage!");
            // Call player damage function here
            var playerStats = target.GetComponent<CharacterStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(attackData.meleeDamage);
            }
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
