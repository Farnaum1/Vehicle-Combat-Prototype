using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<VehicleDamagedEventArgs> VehicleDamaged;
    public static event Action<VehicleDestroyedEventArgs> VehicleDestroyed;
    public static event Action<WeaponFiredEventArgs> WeaponFired;

    public static void RaiseVehicleDamaged(VehicleDamagedEventArgs args)
    {
        VehicleDamaged?.Invoke(args);
    }

    public static void RaiseVehicleDestroyed(VehicleDestroyedEventArgs args)
    {
        VehicleDestroyed?.Invoke(args);
    }

    public static void RaiseWeaponFired(WeaponFiredEventArgs args)
    {
        WeaponFired?.Invoke(args);
    }
}

public readonly struct VehicleDamagedEventArgs
{
    public GameObject VehicleObject { get; }
    public TeamId VehicleTeamId { get; }
    public DamageInfo DamageInfo { get; }
    public float CurrentHealth { get; }
    public float MaxHealth { get; }

    public VehicleDamagedEventArgs(GameObject vehicleObject, TeamId vehicleTeamId, 
        DamageInfo damageInfo,
        float currentHealth,
        float maxHealth)
    {
        VehicleObject = vehicleObject;
        VehicleTeamId = vehicleTeamId;
        DamageInfo = damageInfo;
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
    }
}

public readonly struct VehicleDestroyedEventArgs
{
    public GameObject VehicleObject { get; }
    public TeamId VehicleTeamId { get; }
    public TeamId AttackerTeamId { get; }
    public GameObject Attacker { get; }

    public VehicleDestroyedEventArgs(GameObject vehicleObject, TeamId vehicleTeamId,
        TeamId attackerTeamId,
        GameObject attacker)
    {
        VehicleObject = vehicleObject;
        VehicleTeamId = vehicleTeamId;
        AttackerTeamId = attackerTeamId;
        Attacker = attacker;
    }
}

public readonly struct WeaponFiredEventArgs
{
    public GameObject WeaponObject { get; }
    public GameObject OwnerObject { get; }
    public TeamId OwnerTeamId { get; }

    public WeaponFiredEventArgs(GameObject weaponObject, GameObject ownerObject,
        TeamId ownerTeamId)
    {
        WeaponObject = weaponObject;
        OwnerObject = ownerObject;
        OwnerTeamId = ownerTeamId;
    }
}
