// ------------------------------------------------------------
// GameManager.cs
// Manages game state, upgrades, score, and enemy registration.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game manager for upgrades, score, enemy management, and timers.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Upgrade Trigger")]
    [SerializeField] private GameObject upgradeTriggerPrefab; // UI or object for upgrade event

    [Header("Enemy Management")]
    [SerializeField] private EnemySpawner[] enemySpawners; // Array of spawners
    [SerializeField] private List<GameObject> activeEnemies = new List<GameObject>(); // List of active enemies

    [SerializeField] private int upgradeHealthBonus; // Health bonus for upgrades
    [SerializeField] private float upgradeSpeedBonus; // Speed bonus for upgrades
    [SerializeField] private int upgradeMaxEnemies;   // Max enemies increase for upgrades

    [Header("Score")]
    [SerializeField] public TextMeshProUGUI scoreText; // UI text for score
    private int score = 0;                             // Current score

    public TextMeshProUGUI chestTimer;                 // UI text for upgrade timer

    private float upgradeTimer = 60f;                  // Timer for upgrade event
    private bool upgradeTriggered = false;             // Whether upgrade event is triggered
    private float upgradeTimerDefault = 60f;           // Default timer value

    /// <summary>
    /// Initializes score display on start.
    /// </summary>
    private void Start()
    {
        if (scoreText != null)
            scoreText.text = score.ToString("D6");
    }

    /// <summary>
    /// Handles upgrade timer and triggers upgrade event.
    /// </summary>
    private void Update()
    {
        if (!upgradeTriggered)
        {
            upgradeTimer -= Time.deltaTime;
            if (chestTimer != null)
                chestTimer.text = Mathf.CeilToInt(upgradeTimer).ToString("D2");
            if (upgradeTimer <= 0f)
            {
                upgradeTriggered = true;
                SpawnUpgradeTrigger();
                // Disable all spawners
                foreach (var spawner in enemySpawners)
                {
                    if (spawner != null)
                        spawner.enabled = false;
                }
            }
        }
    }

    /// <summary>
    /// Activates the upgrade trigger object/UI.
    /// </summary>
    public void SpawnUpgradeTrigger()
    {
        upgradeTriggerPrefab.SetActive(true);
    }

    /// <summary>
    /// Registers an enemy to the active list.
    /// </summary>
    public void RegisterEnemy(GameObject enemy)
    {
        if (!activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
    }

    /// <summary>
    /// Unregisters an enemy from the active list.
    /// </summary>
    public void UnregisterEnemy(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
            activeEnemies.Remove(enemy);
    }

    /// <summary>
    /// Upgrades all active enemies with health and speed bonuses.
    /// </summary>
    public void UpgradeAllEnemies(int healthBonus, float speedBonus)
    {
        foreach (var enemyObj in activeEnemies)
        {
            if (enemyObj == null) continue;
            var ai = enemyObj.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.UpgradeState(healthBonus, speedBonus); // Implement this in EnemyAI
            }
            var toxicAi = enemyObj.GetComponent<ToxicEnemyAI>();
            if (toxicAi != null)
            {
                toxicAi.UpgradeState(healthBonus, speedBonus); // Implement this in ToxicEnemyAI
            }
        }
        upgradeTimer = 0;
    }

    /// <summary>
    /// Upgrades the max enemies allowed for all spawners.
    /// </summary>
    public void UpgradeMaxEnemies(int newMax)
    {
        if (enemySpawners != null)
        {
            foreach (var spawner in enemySpawners)
            {
                if (spawner != null)
                    spawner.SetMaxEnemies(newMax);
            }
        }
    }

    /// <summary>
    /// Resets the upgrade timer and state.
    /// </summary>
    public void ResetUpgradeTimer()
    {
        upgradeTimer = upgradeTimerDefault;
        upgradeTriggered = false;
    }

    /// <summary>
    /// Called when upgrade is finished to resume gameplay.
    /// </summary>
    // Call this from UpgradeTrigger.ResumeGame()
    public void OnUpgradeResume()
    {
        UpgradeAllEnemies(upgradeHealthBonus, upgradeSpeedBonus);
        UpgradeMaxEnemies(upgradeMaxEnemies);
        // Re-enable all spawners if needed
        foreach (var spawner in enemySpawners)
        {
            if (spawner != null)
                spawner.enabled = true;
        }
        ResetUpgradeTimer();
        upgradeTriggerPrefab.SetActive(false);
    }

    /// <summary>
    /// Adds score and updates the score display.
    /// </summary>
    public void AddScore(int amount)
    {
        score += amount;
        if (scoreText != null)
            scoreText.text = score.ToString("D6");
    }

    /// <summary>
    /// Gets the current score.
    /// </summary>
    public int GetCurrentScore()
    {
        return score;
    }

}
