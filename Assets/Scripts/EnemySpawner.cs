// ------------------------------------------------------------
// EnemySpawner.cs
// Spawns enemies at random positions near spawn points at intervals.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns enemies at random positions near specified spawn points at set intervals.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private GameObject enemyPrefab;         // Prefab for spawned enemies
    [SerializeField] private float minimumSpawnTime = 1f;    // Minimum spawn interval
    [SerializeField] private float maximumSpawnTime = 3f;    // Maximum spawn interval

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;        // Array of spawn points
    [SerializeField] private float spawnRadius = 2f;         // Radius around spawn point

    [Header("Target")]
    [SerializeField] private Transform playerTransform;      // Player to target

    [Header("Limits")]
    [SerializeField] private int maxEnemies = 10;            // Maximum active enemies
    [SerializeField] private int maxEnemiesLimit = 20;       // Absolute max allowed

    private float timeUntilSpawn;                            // Time until next spawn
    private List<GameObject> spawnedEnemies = new List<GameObject>(); // List of active enemies

    /// <summary>
    /// Initializes spawn timer on awake.
    /// </summary>
    private void Awake()
    {
        SetTimeUntilSpawn();
    }

    /// <summary>
    /// Handles enemy spawning and cleanup each frame.
    /// </summary>
    private void Update()
    {
        // Remove destroyed enemies from the list
        spawnedEnemies.RemoveAll(e => e == null);
        timeUntilSpawn -= Time.deltaTime;

        if (timeUntilSpawn <= 0 && spawnedEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
            SetTimeUntilSpawn();
        }
    }

    /// <summary>
    /// Sets the time until the next enemy spawn.
    /// </summary>
    private void SetTimeUntilSpawn()
    {
        timeUntilSpawn = Random.Range(minimumSpawnTime, maximumSpawnTime);
    }

    /// <summary>
    /// Increases the max enemies allowed, up to the limit.
    /// </summary>
    public void SetMaxEnemies(int newMax)
    {
        if (maxEnemies > maxEnemiesLimit)
        {
            maxEnemies = maxEnemiesLimit;
        }
        else
        {
            maxEnemies += newMax;
        }
    }

    /// <summary>
    /// Spawns a new enemy at a random position near a spawn point.
    /// </summary>
    private void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0 || playerTransform == null)
        {
            Debug.LogWarning("Spawner missing references.");
            return;
        }
        if (spawnedEnemies.Count >= maxEnemies) return;

        Transform chosenPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector2 circle = Random.insideUnitCircle * spawnRadius;
        Vector3 offset = new Vector3(circle.x, 0f, circle.y);
        Vector3 spawnPosition = chosenPoint.position + offset;

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        spawnedEnemies.Add(enemy);

        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.SetTarget(playerTransform);
        }
    }
}
