// ------------------------------------------------------------
// PlayerHealth.cs
// Handles player health, damage, healing, death, and UI updates.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages player health, damage feedback, healing, death logic, and UI updates.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;                // Maximum health
    private int currentHealth;                 // Current health

    [Header("UI References")]
    [SerializeField] private Slider healthBar; // Health bar UI

    [SerializeField] private AudioSource audioSource; // Audio source for damage sound
    [SerializeField] private AudioClip damageSound;   // Sound effect for damage
    [SerializeField] private ForceReceiver forceReceiver; // Reference for knockback (unused)
    [SerializeField] private TextMeshProUGUI scoreText;   // Score UI on defeat
    [SerializeField] private GameObject defeatWindow;     // Defeat window UI

    /// <summary>
    /// Initializes health and health bar on start.
    /// </summary>
    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = maxHealth;
        }
        UpdateHealthBar();
    }

    /// <summary>
    /// Applies damage to the player (default direction).
    /// </summary>
    public void TakeDamage(float amount)
    {
        // Default: apply knockback from in front of player
        TakeDamage(amount, transform.position + transform.forward, false);
    }

    /// <summary>
    /// Applies damage to the player, triggers feedback, and handles death.
    /// </summary>
    public void TakeDamage(float amount, Vector3 damageSource, bool ignoreKnockback = false)
    {
        currentHealth = Mathf.RoundToInt(Mathf.Max(currentHealth - amount, 0));
        UpdateHealthBar();
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
        // Knockback removed
        GetComponent<FlashDamage>()?.TriggerMaterialChange();
        if (currentHealth <= 0)
            Die();
    }

    /// <summary>
    /// Heals the player by the specified amount.
    /// </summary>
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthBar();
    }

    /// <summary>
    /// Handles player death: shows defeat window, disables player, updates score UI, and pauses game.
    /// </summary>
    private void Die()
    {
        Debug.Log("Player has died!");
        // Prevent shooting after player death
        BulletShooter.isUpgrading = true;
        if (defeatWindow != null)
        {
            defeatWindow.SetActive(true);
        }

        GameManager gameManager = FindObjectOfType<GameManager>();
        GameObject player = gameObject;
        if (gameManager != null)
        {
            gameManager.GetCurrentScore();
        }

        scoreText.text = gameManager.GetCurrentScore().ToString("D6");

        Time.timeScale = 0f;
    }

    /// <summary>
    /// Updates the health bar UI.
    /// </summary>
    private void UpdateHealthBar()
    {
        if (healthBar == null) return;
        healthBar.value = currentHealth;
    }
}
