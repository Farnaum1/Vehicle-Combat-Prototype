using UnityEngine;

public class MachineGun : Weapon
{
    protected override void Fire()
    {
        Ray ray = new Ray(FirePoint.position, FirePoint.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, Data.range))
        {
            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                DamageInfo damageInfo = CreateDamageInfo(hit.point);
                damageable.TakeDamage(damageInfo);
            }
        }

        Debug.DrawRay(FirePoint.position, FirePoint.forward * Data.range, Color.blue, 0.2f);
    }
}
