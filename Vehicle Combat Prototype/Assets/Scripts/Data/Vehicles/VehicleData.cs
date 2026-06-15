using UnityEngine;

[CreateAssetMenu(fileName = "NewVehicleData", menuName = "CombatVehicle/Vehicle Data")]
public class VehicleData : ScriptableObject
{
    [Header("General")]
    public string vehicleName;
    public GameObject vehiclePrefab;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 10f;
    public float rotationSpeed = 180f;

    [Header("Combat")]
    public WeaponData defaultWeapon; 
}
