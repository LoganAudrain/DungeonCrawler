using System;
using System.Collections;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] WeaponData weapon;
    [SerializeField] CharacterStats characterStats;

    private CharacterStats m_characterStats;
    private bool m_isAttacking = false;

    public WeaponData CurrentWeapon => weapon;

    public void Start()
    {
        m_characterStats = GetComponent<CharacterStats>();

        if (m_characterStats == null)
        {
            throw new MissingComponentException("CharacterStats component is required but was not found.");
        }
    }

    public void UseWeapon(Vector2 aimDir)
    {
        if (weapon == null) return;

        switch (weapon.type)
        {
            case WeaponType.Melee:
                {
                    UseMeleeWeapon(aimDir);
                    break;
                }
            case WeaponType.Ranged:
                {
                    UseRangedWeapon(aimDir);
                    break;
                }
        }
    }

    public void ChangeWeapon(WeaponData _weapon)
    {
        weapon = _weapon;
    }

    private void UseMeleeWeapon(Vector2 aimDir)
    {
        if (weapon.prefab == null) return;
        if (m_isAttacking) return;

        Transform trans = GetComponent<Transform>();

        GameObject weaponInstance = Instantiate(
            weapon.prefab,
            trans.position,
            trans.rotation,
            trans
        );

        WeaponCollisionObserver observer = weaponInstance.AddComponent<WeaponCollisionObserver>();
        observer.OnHit += HandleMeleeHit;

        int direction = (aimDir.x >= 0) ? -1 : 1;
        Vector3 spawnTimeRot = new(weapon.spawnTimeRotationX, weapon.spawnTimeRotationY, weapon.spawnTimeRotationZ);

        if (direction < 0)
        {
            Vector3 inversed = spawnTimeRot;
            inversed.z = -inversed.z;

            weaponInstance.transform.localRotation = Quaternion.Euler(inversed);
        }
        else
        {
            weaponInstance.transform.localRotation = Quaternion.Euler(spawnTimeRot);
        }


        float duration = Mathf.Max(weapon.swingTime, weapon.thrustTime);
        StartCoroutine(SwingWeapon(weaponInstance.transform, aimDir, duration));
    }

    private void UseRangedWeapon(Vector2 aimDir)
    {

    }

    private void HandleMeleeHit(Collider2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        damageable?.TakeDamage(weapon.damage);

        //apply knockback
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        CharacterStats stats = collision.gameObject.GetComponent<CharacterStats>();

        if (rb != null && stats != null)
        {
            Vector2 knockbackDir = collision.gameObject.transform.position - transform.position;
            knockbackDir.Normalize();

            ApplyKnockback(rb, stats, knockbackDir);
        }
    }
    private void ApplyKnockback(Rigidbody2D rb, CharacterStats stats, Vector2 knockbackDir)
    {
        float force = Mathf.Max(m_characterStats.GetStrength - stats.GetConstitution, 1f);
        force *= weapon.knockbackMultiplier;

        StartCoroutine(Knockback(rb, knockbackDir, force, 0.1f));
    }

    private IEnumerator Knockback(Rigidbody2D rb, Vector2 dir, float force, float duration)
    {
        rb.linearVelocity = dir.normalized * force;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector2.zero;

    }

    private IEnumerator SwingWeapon(Transform weaponTransform, Vector2 aimDir, float duration)
    {
        StartCoroutine(AttackCooldown(weapon.attackRate));

        Vector3 startPos = weaponTransform.localPosition;
        Quaternion startRot = weaponTransform.localRotation;

        float elapsed = 0f;
        float swingEndingZAxisAngle = (aimDir.x >= 0) ? -weapon.swingAngle : weapon.swingAngle;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // rotatation (the weapon swing)
            if (elapsed < weapon.swingTime)
            {
                float angle = Mathf.Lerp(startRot.eulerAngles.z, swingEndingZAxisAngle, t);
                weaponTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
            }

            // thrust (the weapon foward movement)
            float thrustT = Mathf.Clamp01(elapsed / (weapon.thrustTime / 2f));
            float thrustOffset;

            if (thrustT < 1f)
            {
                thrustOffset = Mathf.Lerp(0f, weapon.thrustDistance, thrustT);
            }
            else if (thrustT >= 1f)
            {
                float backT = (elapsed - weapon.thrustTime / 2f) / (weapon.thrustTime / 2f);
                thrustOffset = Mathf.Lerp(weapon.thrustDistance, 0f, backT);
            }
            else
            {
                thrustOffset = 0f;
            }


            weaponTransform.localPosition = startPos + weaponTransform.up * thrustOffset;


            yield return null;
        }

        Destroy(weaponTransform.gameObject);
    }

    private IEnumerator AttackCooldown(float cooldown)
    {
        m_isAttacking = true;

        while (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;

            yield return null;
        }

        m_isAttacking = false;
    }
}

public class WeaponCollisionObserver : MonoBehaviour
{
    public event Action<Collider2D> OnHit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnHit?.Invoke(other);
    }
}
