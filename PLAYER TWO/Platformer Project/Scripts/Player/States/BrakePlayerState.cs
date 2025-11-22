using UnityEngine;

/// <summary>
/// 刹车状态
/// </summary>
public class BrakePlayerState : PlayerState
{
    /// <inheritdoc/>
    protected override void OnEnter(Player entity)
    {
    }

    /// <inheritdoc/>                                                           
    protected override void OnExit(Player entity)
    {
    }

    /// <inheritdoc/>
    protected override void OnStep(Player entity)
    {
        var inputDirection = entity.InputManager.MovementCameraDirectionGet();

        if (entity.StatsManager.CurrStats.m_canBackflip &&
            Vector3.Dot(inputDirection, entity.transform.forward) <= 0 &&
            entity.InputManager.JumpDownGet())
        {
            entity.Backflip(entity.StatsManager.CurrStats.m_backflipBackwardForce);
        }
        else
        {
            entity.SnapToGround();
            entity.Jump();
            entity.Fall();
            entity.Decelerate();
            
            if (entity.LateralVelocity.sqrMagnitude <= 0)
            {
                entity.StateManager.Change<IdlePlayerState>();
            }
        }
        
    }

    /// <inheritdoc/>
    protected override void OnContact(Player entity, Collider other)
    {
    }
}