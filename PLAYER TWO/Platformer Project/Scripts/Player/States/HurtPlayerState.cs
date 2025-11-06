using UnityEngine;

/// <summary>
/// 受伤状态
/// </summary>
public class HurtPlayerState : PlayerState
{
    protected override void OnEnter(Player entity)
    {
    }

    protected override void OnExit(Player entity)
    {
    }

    protected override void OnStep(Player entity)
    {
        entity.Gravity();

        if (entity.IsGrounded && entity.VerticalVelocity.y <= 0)
        {
            if (entity.Health.current > 0)
            {
                entity.StateManager.Change<IdlePlayerState>();
            }
            else
            {
                //entity.StateManager.Change<DiePlayerState>();
            }
        }
    }

    protected override void OnContact(Player entity, Collider other)
    {
    }
}