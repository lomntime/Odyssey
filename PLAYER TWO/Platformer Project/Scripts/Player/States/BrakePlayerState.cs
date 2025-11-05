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
        entity.Decelerate();

        if (entity.LateralVelocity.sqrMagnitude <= 0)
        {
            entity.StateManager.Change<IdelPlayerState>();
        }
    }

    /// <inheritdoc/>
    protected override void OnContact(Player entity, Collider other)
    {
    }
}