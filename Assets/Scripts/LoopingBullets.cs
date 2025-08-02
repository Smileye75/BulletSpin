using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingBullets : MonoBehaviour
{
    // -------------------
    // Bullet Phases
    // -------------------
    private enum BulletPhase { TravelForward, Orbiting, Returning }
    private BulletPhase phase = BulletPhase.TravelForward;

    // -------------------
    // Configurable Parameters (set via Initialize)
    // -------------------
    private Transform orbitCenter;   // The player or orbit center
    private Transform returnTarget;  // Where the bullet returns
    private float orbitSpeed;        // Orbit speed (deg/sec)
    private float returnSpeed;       // Return speed
    private float startRadius;       // Initial orbit radius
    private float endRadius;         // Final orbit radius
    private float verticalOffset;    // Height above player
    private int numberOfLoops;       // How many loops
    private float travelDistance;    // Forward travel distance
    private float travelSpeed;       // Forward travel speed
    private Vector3 travelDirection; // Initial direction
    private AmmoSlot ammoSlot;

    private int bulletDamage;



    // -------------------
    // Runtime State
    // -------------------
    private float angle;             // Current orbit angle (deg)
    private float currentRadius;     // Current orbit radius
    private int completedLoops;      // Loops completed
    private float traveled;          // Distance traveled forward
    private float orbitEnterSpeed = 5f; // How quickly the bullet snaps to its orbit radius

    /// <summary>
    /// Initializes the bullet's movement and orbit parameters.
    /// </summary>
public void Initialize(
    Transform orbitCenter,
    Transform returnTarget,
    int loops,
    float verticalOffset,
    float orbitSpeed,
    float returnSpeed,
    float startRadius,
    float endRadius,
    float travelDistance,
    float travelSpeed,
    Vector3 travelDirection,
    int damage)
{
    this.orbitCenter = orbitCenter;
    this.returnTarget = returnTarget;
    this.numberOfLoops = loops;
    this.verticalOffset = verticalOffset;
    this.orbitSpeed = orbitSpeed;
    this.returnSpeed = returnSpeed;
    this.startRadius = startRadius;
    this.endRadius = endRadius;
    this.travelDistance = travelDistance;
    this.travelSpeed = travelSpeed;
    this.travelDirection = travelDirection.normalized;
    this.bulletDamage = damage;

    phase = BulletPhase.TravelForward;
    traveled = 0f;
    angle = 0f;
    completedLoops = 0;
}

    /// <summary>
/// Sets the trail renderer's color if one is attached.
/// </summary>

    private void Update()
    {
        // Safety check: must have valid references
        if (orbitCenter == null || returnTarget == null) return;

        // Handle bullet phase logic
        switch (phase)
        {
            case BulletPhase.TravelForward:
                TravelForward(); // Move forward before orbiting
                break;
            case BulletPhase.Orbiting:
                OrbitAroundPlayer(); // Orbit around the player
                break;
            case BulletPhase.Returning:
                ReturnToPlayer(); // Return to ammo slot
                break;
        }
    }

    /// <summary>
    /// Moves the bullet forward for a set distance before starting to orbit.
    /// </summary>
    private void TravelForward()
    {
        float move = travelSpeed * Time.deltaTime;
        transform.position += travelDirection * move;
        traveled += move;

        if (traveled >= travelDistance)
        {
            // Set up orbit phase
            Vector3 toBullet = transform.position - orbitCenter.position;
            currentRadius = toBullet.magnitude;
            angle = Mathf.Atan2(toBullet.z, toBullet.x) * Mathf.Rad2Deg;
            phase = BulletPhase.Orbiting;
        }
    }

    /// <summary>
    /// Orbits the bullet around the player, shrinking the radius each loop.
    /// </summary>
    private void OrbitAroundPlayer()
    {
        angle += orbitSpeed * Time.deltaTime;
        float radians = angle * Mathf.Deg2Rad;

        // Smooth radius transition per loop
        float t = Mathf.Clamp01((float)completedLoops / numberOfLoops);
        float targetRadius = Mathf.Lerp(startRadius, endRadius, t);
        currentRadius = Mathf.Lerp(currentRadius, targetRadius, Time.deltaTime * orbitEnterSpeed);

        Vector3 offset = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)) * currentRadius;
        transform.position = orbitCenter.position + offset + Vector3.up * verticalOffset;

        // When a full loop completes
        if (angle >= 360f)
        {
            angle -= 360f;
            completedLoops++;

            // After all loops, start returning
            if (completedLoops >= numberOfLoops)
            {
                phase = BulletPhase.Returning;
            }
        }
    }

// Ammo management fields
private int bulletSlotIndex;

/// <summary>
/// Sets the ammo slot and bullet index for return logic.
/// </summary>
public void SetAmmoSlot(AmmoSlot slot, int index)
{
    ammoSlot = slot;
    bulletSlotIndex = index;
}
private void OnTriggerEnter(Collider other)
{
    if (other.TryGetComponent<EnemyAI>(out var enemy) || other.CompareTag("Enemy"))
    {
        enemy.TakeDamage(bulletDamage);
    }

}

    /// <summary>
    /// Returns the bullet to the ammo slot and destroys it when close.
    /// </summary>
    private void ReturnToPlayer()
    {
    transform.position = Vector3.MoveTowards(transform.position, returnTarget.position, returnSpeed * Time.deltaTime);

    if (Vector3.Distance(transform.position, returnTarget.position) < 0.1f)
    {
if (Vector3.Distance(transform.position, returnTarget.position) < 0.1f)
{
    ammoSlot?.ReturnBullet(bulletSlotIndex);
    Destroy(gameObject);
}
    }
    }
}
