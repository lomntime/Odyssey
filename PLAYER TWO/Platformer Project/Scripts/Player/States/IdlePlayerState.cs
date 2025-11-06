using UnityEngine;

/// <summary>
/// 玩家空闲状态
/// </summary>
public class IdlePlayerState : PlayerState
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
        
        var direction = entity.InputManager.MovementDirectionGet();

        // 存在方向和水平速度时切换到walk状态
        if (direction.sqrMagnitude > 0  || entity.LateralVelocity.sqrMagnitude > 0)
        {
            entity.StateManager.Change<WalkPlayerState>();
        }
    }

    /// <inheritdoc/>
    protected override void OnContact(Player entity, Collider collider)
    {
    }

    #endregion
}       