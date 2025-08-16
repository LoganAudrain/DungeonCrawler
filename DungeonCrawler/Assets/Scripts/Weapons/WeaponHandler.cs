using System.Collections;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] WeaponData weapon;
    [SerializeField] CharacterStats characterStats;

    private bool m_isAttacking = false;

    public void UseWeapon(Vector2 aimDir)
    {
        if(weapon == null) return;

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

        int direction = (aimDir.x >= 0) ? -1 : 1;
        Vector3 spawnTimeRot = new(weapon.spawnTimeRotationX, weapon.spawnTimeRotationY, weapon.spawnTimeRotationZ);

        if(direction < 0)
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

    private void OnTriggerEnter2D(Collider2D collision)
    { //because melee weapons are children of the player, the melee weapons collisions go here

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        damageable?.TakeDamage(weapon.damage + (characterStats.GetStrength * 2));
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
