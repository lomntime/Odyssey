using UnityEngine;

/// <summary>
/// 玩家移动状态
/// </summary>
public class WalkPlayerState : PlayerState
{
    #region override of EntityState

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
        entity.Jump();
        entity.SnapToGround();
        entity.Gravity();
        entity.Fall();
        entity.AccelerateToInputDirection();
        
        var direction = entity.InputManager.MovementCameraDirectionGet();

        if (direction.sqrMagnitude > 0)
        {
            var dot = Vector3.Dot(direction, entity.LateralVelocity);

            if (dot > entity.StatsManager.CurrStats.m_brakeThreshold)
            {
                entity.Accelerate(direction);
                entity.FaceDirectionSmooth(direction);
            }
        }
        else
        {
            entity.Friction();
            if (entity.LateralVelocity.sqrMagnitude <= 0)
            {
                entity.StateManager.Change<IdlePlayerState>();
            }
        }

        if (entity.InputManager.CrouchAndCrawGet())
        {
            entity.StateManager.Change<CrouchPlayerState>();
        }
    }

    /// <inheritdoc/>
    protected override void OnContact(Player entity, Collider collider)
    {
    }

    #endregion
}