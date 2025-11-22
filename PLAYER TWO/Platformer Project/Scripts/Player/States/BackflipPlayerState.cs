using UnityEngine;

/// <summary>
/// 后空翻状态
/// </summary>
public class BackflipPlayerState : PlayerState
{
    protected override void OnEnter(Player entity)
    {
        entity.SetJumps(1);
        
        entity.m_playerEvents.EventOnJump?.Invoke();

        if (entity.StatsManager.CurrStats.m_backflipLockMovement)
        {
            entity.InputManager.LockMovementDirection();
        }
    }

    protected override void OnExit(Player entity)
    {
    }

    protected override void OnStep(Player entity)
    {
        entity.Gravity(entity.StatsManager.CurrStats.m_backflipGravity);
        
        entity.BackflipAcceleration();

        if (entity.IsGrounded)
        {
            entity.LateralVelocity = Vector3.zero;
            entity.StateManager.Change<IdlePlayerState>();
        }
        else if(entity.VerticalVelocity.y < 0) 
        {
        }
    }

    protected override void OnContact(Player entity, Collider other)
    {
    }
}