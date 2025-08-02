using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI References")]
    [SerializeField] private Slider healthBar;

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

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        UpdateHealthBar();
        GetComponent<FlashDamage>()?.TriggerMaterialChange();
        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthBar();
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // TODO: Add death animation, respawn, or end game screen
    }

    private void Update()
    {
        // Press Q to heal, E to take damage (for testing)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Heal(10); // Heal 10
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(10); // Take 10 damage
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null) return;
        healthBar.value = currentHealth;
    }
}
