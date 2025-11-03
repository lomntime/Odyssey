using UnityEngine;

public class IdelPlayerState : PlayerState
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
        throw new System.NotImplementedException();
    }

    #endregion
}       