// ------------------------------------------------------------
// EnemyAI.cs
// Handles enemy movement, health, damage, and death logic.
// ------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Controls enemy movement, health, damage, upgrades, and death behavior.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed; // Movement speed
    private Transform target;             // Target to chase (player)

    [Header("Health")]
    [SerializeField] private int maxHealth; // Maximum health
    private int currentHealth;              // Current health
    private int smallEnemyScoreValue = 100; // Score value for enemy
    [SerializeField] private int damage;    // Damage dealt to player

    [SerializeField] private Animator animator; // Animator for enemy

    [Header("Death Settings")]
    [SerializeField] private GameObject disableLight; // Light to disable on death

    /// <summary>
    /// Initializes enemy health on awake.
    /// </summary>
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Sets the target for the enemy to chase.
    /// </summary>
    public void SetTarget(Transform t)
    {
        target = t;
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
        speed += speedBonus;
    }

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // Audio source for sounds
    [SerializeField] private AudioClip deathSound;    // Sound effect for death

    /// <summary>
    /// Handles enemy death: disables collider, plays animation/sound, awards score, and destroys object.
    /// </summary>
    private void Die()
    {
        Collider col = GetComponent<Collider>();
        Rigidbody rb = GetComponent<Rigidbody>();
        if (col != null && rb != null)
        {
            col.enabled = false;
            rb.isKinematic = true;
        }
        animator.SetTrigger("Die");
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        // Disable specified GameObject if assigned
        if (disableLight != null)
        {
            disableLight.SetActive(false);
        }

        // Stop chasing the player
        target = null;
        FindObjectOfType<GameManager>().AddScore(smallEnemyScoreValue);
        Destroy(gameObject, 1.5f); // Or play death animation, etc.
    }

    /// <summary>
    /// Handles enemy movement and facing the player.
    /// </summary>
    void Update()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Keep movement horizontal

        transform.position += direction * speed * Time.deltaTime;

        // Optional: Face the player
        transform.rotation = Quaternion.LookRotation(direction);
    }

    /// <summary>
    /// Handles collision with the player and applies damage.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage, transform.position);
        }
    }
}
