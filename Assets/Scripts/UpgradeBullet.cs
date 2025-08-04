// ------------------------------------------------------------
// UpgradeBullet.cs
// Handles bullet upgrade logic via UI button.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Upgrades the current bullet slot to a new bullet type when the upgrade button is pressed.
/// </summary>
public class UpgradeBullet : MonoBehaviour
{
    [SerializeField] private AmmoSlot ammoSlot;         // Reference to AmmoSlot logic
    [SerializeField] private BulletData upgradedBullet; // Bullet type to upgrade to
    [SerializeField] private AmmoUI ammoUI;             // Needed to get currentIndex
    [SerializeField] private Button upgradeButton;       // UI button for upgrade

    /// <summary>
    /// Registers the upgrade button click event on awake.
    /// </summary>
    private void Awake()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(UpgradeBulletType);
    }

    /// <summary>
    /// Upgrades the bullet in the current slot to the upgraded bullet type.
    /// </summary>
    private void UpgradeBulletType()
    {
        if (ammoSlot == null || ammoUI == null || upgradedBullet == null) return;
        int index = ammoUI.GetCurrentIndex();
        ammoSlot.ReplaceBullet(index, upgradedBullet);
    }
}
