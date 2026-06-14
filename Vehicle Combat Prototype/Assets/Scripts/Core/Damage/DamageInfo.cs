using UnityEngine;

public readonly struct DamageInfo
{
    public float Amount { get; }
    public TeamId AttackerTeamId { get; }
    public GameObject Attacker { get; }
    public Vector3 HitPoint { get; }

    public DamageInfo(float amount, TeamId attackerTeamId, GameObject attacker, Vector3 hitPoint)
    {
        Amount = amount;
        AttackerTeamId = attackerTeamId;
        Attacker = attacker;
        HitPoint = hitPoint;
    }
}
