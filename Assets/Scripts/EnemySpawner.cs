using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns enemies at random positions near specified spawn points at set intervals.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float minimumSpawnTime = 1f;
    [SerializeField] private float maximumSpawnTime = 3f;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnRadius = 2f;

    [Header("Target")]
    [SerializeField] private Transform playerTransform;

    private float timeUntilSpawn;

    private void Awake()
    {
        SetTimeUntilSpawn();
    }

    private void Update()
    {
        timeUntilSpawn -= Time.deltaTime;

        if (timeUntilSpawn <= 0)
        {
            SpawnEnemy();
            SetTimeUntilSpawn();
        }
    }

    private void SetTimeUntilSpawn()
    {
        timeUntilSpawn = Random.Range(minimumSpawnTime, maximumSpawnTime);
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0 || playerTransform == null)
        {
            Debug.LogWarning("Spawner missing references.");
            return;
        }

        Transform chosenPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector2 circle = Random.insideUnitCircle * spawnRadius;
        Vector3 offset = new Vector3(circle.x, 0f, circle.y);
        Vector3 spawnPosition = chosenPoint.position + offset;

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.SetTarget(playerTransform);
        }
    }
}
