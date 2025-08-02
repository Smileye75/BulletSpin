using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main state machine for player logic and transitions.
/// Holds references and settings for movement, combat, and jumping.
/// </summary>
public class PlayerStateMachine : StateMachine
{
    [Header("References")]
    public InputReader inputReader;
    public CharacterController characterController;
    public Animator animator;
    public Transform mainCamera;
    public ForceReceiver forceReceiver;

    [Header("Movement Settings")]
    public float movementSpeed = 5f;
    public float rotationSpeed = 10f;

    private void Start()
    {
        mainCamera = Camera.main.transform;

        SwitchState(new PlayerMoveState(this));
    }
}
