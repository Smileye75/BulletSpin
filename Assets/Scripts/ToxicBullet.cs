// ------------------------------------------------------------
// ToxicBullet.cs
// Handles toxic bullet collision, damage, and toxic pool spawning.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns a toxic pool and applies damage to the player on collision.
/// </summary>
public class ToxicBullet : MonoBehaviour
{
    [SerializeField] private GameObject toxicPoolPrefab; // Prefab for toxic pool
    [SerializeField] private float destroyDelay = 0.1f;  // Delay before destroying bullet
    [SerializeField] private int damage = 2;             // Damage dealt to player

    /// <summary>
    /// Handles collision: spawns toxic pool, applies damage, and destroys bullet.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.gameObject.GetComponent<PlayerHealth>();
        if (toxicPoolPrefab != null)
        {
            Instantiate(toxicPoolPrefab, transform.position, Quaternion.identity);
        }

        if (player != null)
        {
            player.TakeDamage(damage, transform.position);
        }

        Destroy(gameObject, destroyDelay * Time.deltaTime);
    }
}
