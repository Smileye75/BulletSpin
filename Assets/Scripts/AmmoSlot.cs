// ------------------------------------------------------------
// AmmoSlot.cs
// Manages bullet slots, usage, and replacement for the player.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles bullet slot management, usage tracking, and replacement logic.
/// </summary>
public class AmmoSlot : MonoBehaviour
{
    [SerializeField] private int maxSlots = 6; // Maximum number of bullet slots
    [SerializeField] private BulletData defaultBullet; // Default bullet type

    [Header("DEBUG")]
    [SerializeField] private List<BulletData> debugBulletSlots = new(); // Debug view of slots
    [SerializeField] private List<bool> debugBulletUsage = new(); // Debug view of usage

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // Plays return sound
    [SerializeField] private AudioClip returnSound; // Sound when bullet returns

    private List<BulletData> bulletSlots = new(); // Actual bullet slots
    private List<bool> isBulletInUse = new(); // Usage flags
    private int nextBulletIndex = 0; // Next slot to use

    private void Awake()
    {
        bulletSlots.Clear();
        isBulletInUse.Clear();

        // Initialize slots and usage flags
        for (int i = 0; i < maxSlots; i++)
        {
            bulletSlots.Add(defaultBullet);
            isBulletInUse.Add(false);
        }

        UpdateDebugLists();
    }

    /// <summary>
    /// Returns true if there's at least one available bullet.
    /// </summary>
    public bool HasAvailableBullet()
    {
        foreach (bool used in isBulletInUse)
        {
            if (!used) return true;
        }
        return false;
    }

    /// <summary>
    /// Returns the first available bullet and marks it in use.
    /// </summary>
    public BulletData GetNextBullet(out int slotIndex)
    {
        int startingIndex = nextBulletIndex;

        for (int i = 0; i < bulletSlots.Count; i++)
        {
            int index = (startingIndex + i) % bulletSlots.Count;

            if (!isBulletInUse[index])
            {
                isBulletInUse[index] = true;
                slotIndex = index;
                nextBulletIndex = (index + 1) % bulletSlots.Count; // move to next in circular queue
                return bulletSlots[index];
            }
        }

        // No available bullets
        slotIndex = -1;
        return null;
    }

    /// <summary>
    /// Marks the specified index as available (bullet returned).
    /// Plays return sound if assigned.
    /// </summary>
    public void ReturnBullet(int index)
    {
        if (index >= 0 && index < isBulletInUse.Count)
        {
            if (audioSource != null && returnSound != null)
            {
                audioSource.PlayOneShot(returnSound);
            }
            isBulletInUse[index] = false;
            UpdateDebugLists();
        }
    }

    /// <summary>
    /// Returns a list of availability flags (true = available).
    /// </summary>
    public List<bool> GetBulletAvailability()
    {
        List<bool> available = new List<bool>();
        foreach (bool used in isBulletInUse)
            available.Add(!used);
        return available;
    }

    /// <summary>
    /// Replaces the bullet in the specified slot with a new bullet type.
    /// Marks slot as available after replacement.
    /// </summary>
    public void ReplaceBullet(int slotIndex, BulletData newBullet)
    {
        if (slotIndex >= 0 && slotIndex < bulletSlots.Count)
        {
            bulletSlots[slotIndex] = newBullet;
            isBulletInUse[slotIndex] = false; // Mark as available after replacement
            UpdateDebugLists();
        }
    }

    /// <summary>
    /// Updates debug lists for inspector visualization.
    /// </summary>
    private void UpdateDebugLists()
    {
        debugBulletSlots = new List<BulletData>(bulletSlots);
        debugBulletUsage = new List<bool>(isBulletInUse);
    }

    /// <summary>
    /// Returns a copy of all bullet slots.
    /// </summary>
    public List<BulletData> GetAllBullets() => new List<BulletData>(bulletSlots);

    /// <summary>
    /// Returns true if the bullet in the specified slot is available (not in use).
    /// </summary>
    public bool IsBulletAvailable(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < isBulletInUse.Count)
            return !isBulletInUse[slotIndex];
        return false;
    }
}
