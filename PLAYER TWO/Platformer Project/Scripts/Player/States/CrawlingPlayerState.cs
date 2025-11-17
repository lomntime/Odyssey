using UnityEngine;

/// <summary>
/// 爬行状态
/// </summary>
public class CrawlingPlayerState : PlayerState
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
        entity.Jump();
        entity.Fall();

        var direction = entity.InputManager.MovementCameraDirectionGet();
        if (entity.InputManager.CrouchAndCrawGet() || !entity.CanStandUp())
        {
            if (direction.sqrMagnitude > 0)
            {
                entity.CrawlingAccelerate(direction);
                
                entity.FaceDirectionSmooth(entity.LateralVelocity);
            }
            else
            {
                entity.Decelerate(entity.StatsManager.CurrStats.m_crawlingFriction);
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