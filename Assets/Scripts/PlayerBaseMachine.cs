using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseMachine : State
{
    protected PlayerStateMachine stateMachine;
    protected float attackTimer = 0f;

    public PlayerBaseMachine(PlayerStateMachine state)
    {
        this.stateMachine = state;
    }

    protected void Move(Vector3 move, float deltaTime)
    {
        Vector3 finalMove = move + stateMachine.forceReceiver.movement;
        stateMachine.characterController.Move(finalMove * deltaTime);
    }

    protected void FaceMovementDirection(Vector3 movement)
    {
        if (movement == Vector3.zero) return;

        stateMachine.transform.rotation = Quaternion.Lerp(
            stateMachine.transform.rotation,
            Quaternion.LookRotation(movement),
            stateMachine.rotationSpeed * Time.deltaTime
        );
    }

    protected void UpdateAttackCooldown(float deltaTime)
    {
        if (attackTimer > 0)
            attackTimer -= deltaTime;
    }

    protected Vector3 CalculateMovement()
    {
        return new Vector3(
            stateMachine.inputReader.movementValue.x,
            0f,
            stateMachine.inputReader.movementValue.y
        );
    }
}
