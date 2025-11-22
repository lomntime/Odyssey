using UnityEngine;

/// <summary>
/// 下蹲状态
/// </summary>
public class CrouchPlayerState : PlayerState
{
    protected override void OnEnter(Player entity)
    {
        entity.ResizeCollider(entity.StatsManager.CurrStats.m_crouchHeight);
    }

    protected override void OnExit(Player entity)
    {
        entity.ResizeCollider(entity.OriginHeight);
    }

    protected override void OnStep(Player entity)
    {
        entity.Gravity();
        entity.SnapToGround();
        entity.Fall();
        entity.Decelerate(entity.StatsManager.CurrStats.m_crouchHeight);

        var inputDirection = entity.InputManager.MovementCameraDirectionGet();
        if (entity.InputManager.CrouchAndCrawGet() || !entity.CanStandUp())
        {
            if (inputDirection.sqrMagnitude > 0 && !entity.IsHolding)
            {
                if (entity.LateralVelocity.sqrMagnitude == 0)
                {
                    entity.StateManager.Change<CrawlingPlayerState>();
                }
            }
            else if (entity.InputManager.JumpDownGet())
            {
                entity.Backflip(entity.StatsManager.CurrStats.m_backflipBackwardForce);
            }
        }
        else
        {
            entity.StateManager.Change<IdlePlayerState>();
        }
    }

    protected override void OnContact(Player entity, Collider other)
    {
    }
}