using UnityEngine;

public interface IDamageable
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    bool IsDestroyed { get; }

    void TakeDamage(DamageInfo damageInfo);
    void Repair(float amount);
}
