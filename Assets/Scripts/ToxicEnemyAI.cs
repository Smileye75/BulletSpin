// ------------------------------------------------------------
// ToxicEnemyAI.cs
// Handles toxic enemy movement, attack, health, upgrades, and death logic.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls toxic enemy movement, projectile attacks, health, upgrades, and death.
/// </summary>
public class ToxicEnemyAI : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth;           // Maximum health
    private int currentHealth;                        // Current health
    private int toxicEnemyScoreValue = 1000;          // Score value for toxic enemy
    [SerializeField] private int damage;              // Damage dealt to player

    [SerializeField] private Animator animator;       // Animator for enemy

    [Header("Death Settings")]
    [SerializeField] private GameObject disableLight; // Light to disable on death
    [SerializeField] private GameObject toxicPoolPrefab; // Prefab for toxic pool

    [Header("Movement")]
    [SerializeField] private float chaseDistance = 10f; // Distance to start chasing
    [SerializeField] private float moveSpeed = 3f;      // Movement speed

    [Header("References")]
    private Transform player;                           // Player to chase/attack
    [SerializeField] private Transform firePoint;       // Projectile spawn point
    [SerializeField] private GameObject projectilePrefab; // Projectile prefab

    [Header("Attack Settings")]
    [SerializeField] private float fireRate = 2f;       // Time between attacks
    [SerializeField] private float rotationSpeed = 5f;  // Rotation speed

    private float fireCooldown;                         // Attack cooldown timer

    /// <summary>
    /// Initializes health and auto-assigns player reference.
    /// </summary>
    private void Awake()
    {
        currentHealth = maxHealth;
        // Auto-assign player transform if not set
        if (player == null)
        {
            PlayerHealth foundPlayer = FindObjectOfType<PlayerHealth>();
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
        }
    }

    /// <summary>
    /// Handles movement, chasing, and attacking logic each frame.
    /// </summary>
    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > chaseDistance)
        {
            // Chase player
            animator.SetBool("ChasingPlayer", true);
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0f;
            transform.position += direction * moveSpeed * Time.deltaTime;
            RotateTowardPlayer();
        }
        else
        {
            // Attack when within chase distance
            animator.SetBool("ChasingPlayer", false);
            RotateTowardPlayer();
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                animator.SetTrigger("Spit Toxic");
                FireProjectile();
                fireCooldown = fireRate;
            }
        }
    }

    /// <summary>
    /// Rotates enemy to face the player smoothly.
    /// </summary>
    private void RotateTowardPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        if (direction.magnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    /// <summary>
    /// Fires a projectile at the player using ballistic calculation.
    /// </summary>
    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null || player == null) return;

        Vector3 target = player.position;
        Vector3 start = firePoint.position;
        float gravity = Physics.gravity.y;
        float horizontalDistance = Vector3.Distance(new Vector3(start.x, 0, start.z), new Vector3(target.x, 0, target.z));
        float verticalDistance = target.y - start.y;
        float angle = 45f * Mathf.Deg2Rad; // You can adjust this angle for different arcs

        // Calculate initial velocity needed to hit target at angle
        float v = Mathf.Sqrt((horizontalDistance * -gravity) / (Mathf.Sin(2 * angle)));
        Vector3 dir = (target - start);
        dir.y = 0;
        dir.Normalize();

        // Calculate velocity vector
        Vector3 velocity = dir * v * Mathf.Cos(angle);
        velocity.y = v * Mathf.Sin(angle);

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = velocity;
        }
    }

    /// <summary>
    /// Sets the player reference for targeting.
    /// </summary>
    public void SetPlayer(Transform p)
    {
        player = p;
    }

    /// <summary>
    /// Handles collision with player and applies damage.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            Debug.Log($"ToxicEnemyAI triggered with player and is dealing {damage} damage.");
            // Reverse direction: from enemy to player
            Vector3 knockbackSource = transform.position - (other.transform.position - transform.position);
            playerHealth.TakeDamage(damage, knockbackSource);
        }
    }

    /// <summary>
    /// Applies damage to the enemy and triggers death if health reaches zero.
    /// </summary>
    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        GetComponent<FlashDamage>()?.TriggerMaterialChange();
        if (currentHealth <= 0)
            Die();
    }

    /// <summary>
    /// Upgrades enemy health and speed.
    /// </summary>
    public void UpgradeState(int healthBonus, float speedBonus)
    {
        maxHealth += healthBonus;
        currentHealth = maxHealth; // Reset health to max after upgrade
        moveSpeed += speedBonus;
    }

    /// <summary>
    /// Handles enemy death: disables light, spawns toxic pool, awards score, and destroys object.
    /// </summary>
    private void Die()
    {
        animator.SetTrigger("Die");
        // Disable specified GameObject if assigned
        if (disableLight != null)
        {
            disableLight.SetActive(false);
        }

        if (toxicPoolPrefab != null)
        {
            Instantiate(toxicPoolPrefab, transform.position, Quaternion.identity);
        }
        // Stop chasing the player
        player = null;
        FindObjectOfType<GameManager>().AddScore(toxicEnemyScoreValue);
        Destroy(gameObject, 1f); // Or play death animation, etc.
    }
}
