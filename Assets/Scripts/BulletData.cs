// ------------------------------------------------------------
// BulletData.cs
// ScriptableObject for bullet configuration and stats.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores configuration and stats for a bullet type.
/// Used to define prefab, visuals, movement, and damage.
/// </summary>
[CreateAssetMenu(menuName = "BulletData")]
public class BulletData : ScriptableObject
{
    [Header("General")]
    public GameObject bulletPrefab; // Prefab for the bullet
    public GameObject imagePrefab;  // UI image prefab for display

    [Header("Stats")]
    public float speed = 20f; // Bullet speed
    public int damage = 1;    // Bullet damage

    [Header("Orbit Settings")]
    public int numberOfLoops = 3;      // Number of orbits before returning
    public float verticalOffset = 1f;  // Height above orbit center
    public float orbitSpeed = 180f;    // Orbit speed in degrees/sec
    public float returnSpeed = 10f;    // Speed when returning to player
    public float startRadius = 3f;     // Initial orbit radius
    public float endRadius = 1f;       // Final orbit radius

    [Header("Travel Settings")]
    public float travelDistance = 2f;  // Distance to travel before orbiting
}
