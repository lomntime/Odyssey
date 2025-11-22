using UnityEngine;

/// <summary>
/// 旋转状态
/// </summary>
public class SpinPlayerState : PlayerState
{
    protected override void OnEnter(Player entity)
    {
        if (entity.IsGrounded)
        {
            entity.VerticalVelocity = Vector3.up * entity.StatsManager.CurrStats.m_airSpinUpwardForce;
        }
    }

    protected override void OnExit(Player entity)
    {
    }

    protected override void OnStep(Player entity)
    {
        entity.Gravity();
        entity.SnapToGround();
        //entity.AirDive();
        entity.StompAttack();
        entity.AccelerateToInputDirection();

        if (TimeSinceEnter >= entity.StatsManager.CurrStats.m_spinDuration)
        {
            if (entity.IsGrounded)
            {
                entity.StateManager.Change<IdlePlayerState>();
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