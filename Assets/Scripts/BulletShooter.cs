using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShooter : MonoBehaviour
{
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private Transform orbitCenter;
    [SerializeField] private Transform ammoSlotPoint;
    [SerializeField] private AmmoSlot ammoSlot;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AmmoUI ammoUI;
    [SerializeField] private ForceReceiver forceReceiver;

    public void ShootOnce()
    {
        // Push player back a little when shooting
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
