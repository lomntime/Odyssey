using UnityEngine;

/// <summary>
/// 冲刺状态
/// </summary>
public class DashPlayerState : PlayerState
{
    protected override void OnEnter(Player entity)
    {
        entity.VerticalVelocity = Vector3.zero;
        entity.LateralVelocity = entity.transform.forward * entity.StatsManager.CurrStats.m_dashForce;
        entity.m_playerEvents.EventOnDashStarted?.Invoke();
    }

    protected override void OnExit(Player entity)
    {
        entity.LateralVelocity =
            Vector3.ClampMagnitude(entity.LateralVelocity, entity.StatsManager.CurrStats.m_topSpeed);
        
        entity.m_playerEvents.EventOnDashEnded?.Invoke();
    }

    protected override void OnStep(Player entity)
    {
        entity.Jump();

        if (TimeSinceEnter > entity.StatsManager.CurrStats.m_dashDuration)
        {
            if (entity.IsGrounded)
            {
                entity.StateManager.Change<WalkPlayerState>();
            }
            else
            {
                entity.StateManager.Change<FallPlayerState>();
            }
        }
    }

    protected override void OnContact(Player entity, Collider other)
    {
    }
}