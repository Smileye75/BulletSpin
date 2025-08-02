using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles gravity, knockback, and external forces for the player.
/// </summary>
public class ForceReceiver : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private CharacterController characterController;

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -20f; // Gravity strength
    private float verticalVelocity = 0f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackSmoothTime = 0.3f;
    [SerializeField] private float knockbackStrength = 5f;

    private Vector3 impact;           // Knockback force
    private Vector3 dampingVelocity;  // For smoothing knockback

    /// <summary>
    /// Combined gravity and knockback movement.
    /// </summary>
    public Vector3 movement => impact + Vector3.up * verticalVelocity;

    private void Update()
    {
        HandleGravity();
        HandleKnockback();
    }

    private void HandleGravity()
    {
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -1f; // Small downward force to stay grounded
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void HandleKnockback()
    {
        impact = Vector3.SmoothDamp(impact, Vector3.zero, ref dampingVelocity, knockbackSmoothTime);
    }

    /// <summary>
    /// Applies knockback away from a source position.
    /// </summary>
    public void ApplyKnockback(Vector3 sourcePosition)
    {
        Vector3 direction = (transform.position - sourcePosition).normalized;
        direction.y = 0f;

        impact += direction * knockbackStrength;
    }

    /// <summary>
    /// Sets gravity dynamically (optional).
    /// </summary>
    public void SetGravity(float newGravity)
    {
        gravity = newGravity;
    }
}
