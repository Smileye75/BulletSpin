using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles player movement, attack, and jump logic while in the move state.
/// </summary>
public class PlayerMoveState : PlayerBaseMachine
{
    private readonly int animationSpeed = Animator.StringToHash("Speed");
    private const float animTransitionSpeed = 0.1f;

    private BulletShooter shooter;
    private AmmoSlot ammoSlot;
    private Coroutine shootingCoroutine;

    public PlayerMoveState(PlayerStateMachine state) : base(state) { }

    public override void Enter()
    {
        shooter = stateMachine.GetComponent<BulletShooter>();
        ammoSlot = stateMachine.GetComponent<AmmoSlot>();
        stateMachine.inputReader.isShooting += OnShoot;
    }

    public override void Tick(float deltaTime)
    {
        Vector3 movement = CalculateMovement();
        Move(movement * stateMachine.movementSpeed, deltaTime);

        float blend = movement == Vector3.zero ? 0f : 1f;
        stateMachine.animator.SetFloat(animationSpeed, blend, animTransitionSpeed, deltaTime);

        if (movement != Vector3.zero)
        {
            FaceMovementDirection(movement);
        }
    }

    public override void Exit()
    {
        stateMachine.inputReader.isShooting -= OnShoot;

        if (shootingCoroutine != null)
        {
            stateMachine.StopCoroutine(shootingCoroutine);
        }

        stateMachine.animator.SetBool("IsShooting", false);
    }

    private void OnShoot()
    {
        if (!ammoSlot.HasAvailableBullet())
            return;

        stateMachine.animator.SetBool("IsShooting", true);
        shooter.ShootOnce();

        if (shootingCoroutine != null)
        {
            stateMachine.StopCoroutine(shootingCoroutine);
        }

        shootingCoroutine = stateMachine.StartCoroutine(ResetShootingFlag());
    }

    private IEnumerator ResetShootingFlag()
    {
        yield return new WaitForSeconds(0.1f); // Match this to your shoot animation timing
        stateMachine.animator.SetBool("IsShooting", false);
    }
}
