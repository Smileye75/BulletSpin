
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSlot : MonoBehaviour
{
    [SerializeField] private int maxSlots = 6;
    [SerializeField] private BulletData defaultBullet;

    [Header("DEBUG")]
    [SerializeField] private List<BulletData> debugBulletSlots = new();
    [SerializeField] private List<bool> debugBulletUsage = new();

    private List<BulletData> bulletSlots = new();
    private List<bool> isBulletInUse = new();
    private int nextBulletIndex = 0;

    private void Awake()
    {
        bulletSlots.Clear();
        isBulletInUse.Clear();

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
    /// </summary>
    public void ReturnBullet(int index)
    {
        if (index >= 0 && index < isBulletInUse.Count)
        {
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
    private void UpdateDebugLists()
    {
        debugBulletSlots = new List<BulletData>(bulletSlots);
        debugBulletUsage = new List<bool>(isBulletInUse);
    }

    public List<BulletData> GetAllBullets() => new List<BulletData>(bulletSlots);

}
