// ------------------------------------------------------------
// BulletShooter.cs
// Handles bullet shooting, aiming, and custom cursor setup.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages bullet shooting, aiming, and custom cursor for the player.
/// </summary>
public class BulletShooter : MonoBehaviour
{
    public static bool isUpgrading = false; // Prevent shooting while upgrading

    [SerializeField] private Texture2D crosshairCursor; // Custom cursor texture
    [SerializeField] private Transform muzzlePoint;      // Bullet spawn point
    [SerializeField] private Transform orbitCenter;      // Orbit center for bullets
    [SerializeField] private Transform ammoSlotPoint;    // Return point for bullets
    [SerializeField] private AmmoSlot ammoSlot;          // Reference to AmmoSlot logic
    [SerializeField] private AudioClip shootSound;       // Sound effect for shooting
    [SerializeField] private AudioSource audioSource;    // Audio source for sounds
    [SerializeField] private AmmoUI ammoUI;              // Reference to AmmoUI logic
    [SerializeField] private ForceReceiver forceReceiver;// Reference to ForceReceiver for knockback

    /// <summary>
    /// Sets the custom cursor on game start.
    /// </summary>
    private void Start()
    {
        if (crosshairCursor != null)
            Cursor.SetCursor(crosshairCursor, Vector2.zero, CursorMode.Auto);
    }

    /// <summary>
    /// Prevents shooting if the current bullet slot is not available.
    /// </summary>
    void Update()
    {
        if (isUpgrading) return;
        int currentIndex = ammoUI.GetCurrentIndex();
        if (!ammoSlot.IsBulletAvailable(currentIndex)) return;
    }

    /// <summary>
    /// Shoots a bullet, applies knockback, and rotates UI to next slot.
    /// </summary>
    public void ShootOnce()
    {
        if (isUpgrading) return;
        int currentIndex = ammoUI.GetCurrentIndex();
        if (!ammoSlot.IsBulletAvailable(currentIndex)) return;

        // Aim at mouse cursor before shooting
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        float rayDistance;
        Vector3 targetPoint = transform.position + transform.forward;
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            targetPoint = ray.GetPoint(rayDistance);
        }
        Vector3 lookDirection = (targetPoint - transform.position).normalized;
        lookDirection.y = 0;
        if (lookDirection.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        if (!ammoSlot.HasAvailableBullet()) return;

        ammoUI.RotateUIToNext();
        int slotIndex;
        var data = ammoSlot.GetNextBullet(out slotIndex);
        if (data == null) return;

        GameObject bullet = Instantiate(data.bulletPrefab, muzzlePoint.position, Quaternion.identity);
        forceReceiver.ApplyKnockback(transform.position + transform.forward);
        if (bullet.TryGetComponent(out LoopingBullets looping))
        {
            looping.Initialize(
                orbitCenter,
                ammoSlotPoint,
                data.numberOfLoops,
                data.verticalOffset,
                data.orbitSpeed,
                data.returnSpeed,
                data.startRadius,
                data.endRadius,
                data.travelDistance,
                data.speed,
                muzzlePoint.forward,
                data.damage
            );
            looping.SetAmmoSlot(ammoSlot, slotIndex);
        }

        if (shootSound && audioSource)
            audioSource.PlayOneShot(shootSound);
    }
}
