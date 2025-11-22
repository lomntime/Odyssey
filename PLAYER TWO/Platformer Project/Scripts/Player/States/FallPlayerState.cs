using UnityEngine;

/// <summary>
/// 下落状态
/// </summary>
public class FallPlayerState : PlayerState
{
    protected override void OnEnter(global::Player entity)
    {
    }

    protected override void OnExit(global::Player entity)
    {
    }

    protected override void OnStep(global::Player entity)
    {
        entity.Gravity();
        entity.SnapToGround();
        entity.FaceDirectionSmooth(entity.LateralVelocity);
        entity.AccelerateToInputDirection();
        entity.Jump();
        entity.Dash();
        entity.StompAttack();

        if (entity.IsGrounded)
        {
            entity.StateManager.Change<IdlePlayerState>();
        }
    }

    protected override void OnContact(global::Player entity, Collider other)
    {
    }
}