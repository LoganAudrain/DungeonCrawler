using UnityEngine;

public enum AttackType { Melee, Ranged }

[CreateAssetMenu(fileName = "NewAttackData", menuName = "Enemy/Attack Data")]
public class AttackData : ScriptableObject
{
    public AttackType attackType;

    [Header("General")]
    public float attackRangeMelee = 2f;
    public float attackRangeRanged = 10f;

    public float attackCooldown = 1f;

    [Header("Melee")]
    public int meleeDamage = 10;

    [Header("Ranged")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
}
