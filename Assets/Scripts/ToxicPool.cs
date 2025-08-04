// ------------------------------------------------------------
// ToxicPool.cs
// Handles damage-over-time logic for toxic pools.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies damage-over-time to the player while inside the toxic pool.
/// </summary>
public class ToxicPool : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 1f; // Damage applied per second
    [SerializeField] private float damageInterval = 1f;  // Interval between damage ticks

    private Dictionary<GameObject, float> lastDamageTime = new Dictionary<GameObject, float>(); // Tracks last damage time per object

    /// <summary>
    /// Applies damage to the player at set intervals while inside the pool.
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                float time = Time.time;
                if (!lastDamageTime.ContainsKey(other.gameObject) || time - lastDamageTime[other.gameObject] >= damageInterval)
                {
                    // Ignore knockback for toxic pool damage
                    health.TakeDamage(damagePerSecond, transform.position, ignoreKnockback: true);
                    lastDamageTime[other.gameObject] = time;
                }
            }
        }
    }

    /// <summary>
    /// Removes player from damage tracking when they exit the pool.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (lastDamageTime.ContainsKey(other.gameObject))
            lastDamageTime.Remove(other.gameObject);
    }

    /// <summary>
    /// Destroys the toxic pool after 5 seconds.
    /// </summary>
    void Start()
    {
        Destroy(gameObject, 5f);
    }
}