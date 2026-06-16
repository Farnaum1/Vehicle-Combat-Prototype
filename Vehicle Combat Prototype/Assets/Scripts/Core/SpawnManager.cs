using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Serializable]
    public class VehicleSpawnInfo
    {
        [Header("Vehicle")]
        public Vehicle vehiclePrefab;

        [Header("Team")]
        public TeamId teamId = TeamId.None;

        [Header("Transform")]
        public Vector3 position;
        public Vector3 rotationEuler;
    }

    [Header("Spawn Settings")]
    [SerializeField] private List<VehicleSpawnInfo> vehiclesToSpawn = new();

    [Header("Runtime")]
    [SerializeField] private List<Vehicle> spawnedVehicles = new();

    private void Start()
    {
        SpawnAllVehicles();
    }

    private void SpawnAllVehicles()
    {
        spawnedVehicles.Clear();

        foreach (VehicleSpawnInfo spawnInfo in vehiclesToSpawn)
        {
            SpawnVehicle(spawnInfo);
        }

        Debug.Log($"SpawnManager spawned {spawnedVehicles.Count} vehicles.");
    }

    private void SpawnVehicle(VehicleSpawnInfo spawnInfo)
    {
        if (spawnInfo == null)
        {
            Debug.LogWarning("Spawn info is null.", this);
            return;
        }

        if (spawnInfo.vehiclePrefab == null)
        {
            Debug.LogWarning("Vehicle prefab is missing in SpawnManager.", this);
            return;
        }

        Quaternion rotation = Quaternion.Euler(spawnInfo.rotationEuler);

        Vehicle vehicleInstance = Instantiate(spawnInfo.vehiclePrefab, spawnInfo.position, rotation);

        vehicleInstance.SetTeam(spawnInfo.teamId);

        spawnedVehicles.Add(vehicleInstance);

    }
}
