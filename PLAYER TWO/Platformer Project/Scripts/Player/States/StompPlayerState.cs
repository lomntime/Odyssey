using UnityEngine;

/// <summary>
/// 重击下砸状态
/// </summary>
public class StompPlayerState : PlayerState
{
    protected override void OnEnter(Player entity)
    {
        m_landed = m_falling = false;
        m_airTimer = m_groundTimer = 0f;
        entity.Velocity = Vector3.zero;
        entity.m_playerEvents.EventOnStompStarted?.Invoke();
    }

    protected override void OnExit(Player entity)
    {
        entity.m_playerEvents.EventOnStompEnding?.Invoke();
    }

    protected override void OnStep(Player entity)
    {
        if (!m_falling)
        {
            m_airTimer += Time.deltaTime;

            if (m_airTimer >= entity.StatsManager.CurrStats.m_stompAirTime)
            {
                m_falling = true;
                entity.m_playerEvents.EventOnStompFalling?.Invoke();
            }
        }
        else
        {
            entity.VerticalVelocity += Vector3.down * entity.StatsManager.CurrStats.m_slopeDownwardForce;
        }

        if (entity.IsGrounded)
        {
            if (!m_landed)
            {
                m_landed = true;
                entity.m_playerEvents.EventOnStompLanding?.Invoke();
            }

            if (m_groundTimer >= entity.StatsManager.CurrStats.m_stompGroundTime)
            {
                entity.VerticalVelocity = Vector3.up * entity.StatsManager.CurrStats.m_stompGroundTime;
                entity.StateManager.Change<FallPlayerState>();
            }
            else
            {
                m_groundTimer += Time.deltaTime;
            }
        }
    }

    protected override void OnContact(Player entity, Collider other)
    {
    }
    
    protected float m_airTimer;
    
    protected float m_groundTimer;

    protected bool m_falling;

    protected bool m_landed;
}