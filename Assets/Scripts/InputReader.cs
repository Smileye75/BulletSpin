using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Reads and processes player input, raising events for jump, attack, and movement.
/// </summary>
public class InputReader : MonoBehaviour, Controls.IPlayerActions
{
    [Header("Runtime Input Values")]
    [Tooltip("Current movement input value.")]
    [HideInInspector] public Vector2 movementValue; // Stores movement input (WASD/joystick)

    // Events for input actions
    public event Action isShooting;   

    private Controls controls; // Input system controls asset

    private void Start()
    {
        // Initialize and enable input controls
        controls = new Controls();
        controls.Player.SetCallbacks(this);
        controls.Enable();
    }

    private void OnDestroy()
    {
        // Disable input controls when destroyed
        controls.Disable();
    }

    /// <summary>
    /// Handles movement input.
    /// </summary>
    public void OnMovement(InputAction.CallbackContext context)
    {
        // Store movement input value
        movementValue = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Handles attack input.
    /// </summary>
    public void OnShooting(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        // Raise attack event
        isShooting?.Invoke();
    }

}
