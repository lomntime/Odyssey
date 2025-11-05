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
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    protected override void OnExit(Player entity)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    protected override void OnStep(Player entity)
    {
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
                entity.StateManager.Change<IdelPlayerState>();
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnContact(Player entity, Collider collider)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}