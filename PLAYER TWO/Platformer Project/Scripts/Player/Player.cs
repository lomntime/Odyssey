using UnityEngine;

/// <summary>
/// 玩家实体
/// </summary>
public class Player : Entity<Player>
{
    #region 外部接口

    /// <summary>
    /// 在指定方向上平滑移动玩家实体
    /// </summary>
    /// <param name="direction"></param>
    public virtual void Accelerate(Vector3 direction)
    {
        var turningDrag = m_statsManager.CurrStats.m_turningDrag;
        var acceleration = m_statsManager.CurrStats.m_acceleration;
        var finalAcceleration = acceleration;
        var topSpeed =  m_statsManager.CurrStats.m_topSpeed;
        
        Accelerate(direction, finalAcceleration, turningDrag, topSpeed);
    }

    /// <summary>
    /// 玩家角色平滑朝向移动方向
    /// </summary>
    /// <param name="direction"></param>
    public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirection(direction, m_statsManager.CurrStats.m_rotationSpeed);
    
    /// <summary>
    /// 平滑减速，使用减速度
    /// </summary>
    public virtual void Decelerate() => Decelerate(m_statsManager.CurrStats.m_deceleration);
    
    /// <summary>
    /// 平滑减速，使用摩擦力
    /// </summary>
    public virtual void Friction()
    {
        if (IsOnSlopingGround)
        {
            Decelerate(m_statsManager.CurrStats.m_slopeFriction);
        }
        else
        {
            Decelerate(m_statsManager.CurrStats.m_friction);
        }
    }

    /// <summary>
    /// 重力下落
    /// </summary>
    public virtual void Gravity()
    {
        IsGrounded = false;
        if (!IsGrounded && VerticalVelocity.y > -m_statsManager.CurrStats.m_gravityTopSpeed)
        {
            var speed = VerticalVelocity.y;
            var force = VerticalVelocity.y > 0
                ? m_statsManager.CurrStats.m_gravity
                : m_statsManager.CurrStats.m_fallGravity;
            
            speed -= force * GravityMultiplier * Time.deltaTime;
            
            speed = Mathf.Max(speed, -m_statsManager.CurrStats.m_gravityTopSpeed);
            
            VerticalVelocity = new Vector3(0f,  speed, 0f);
        }
    }
    
    #endregion    
    
    #region Override of Entity

    protected override void Awake()
    {
        base.Awake();
        InitializeInputManager();
        InitializeStatsManager();
    }

    #endregion

    #region 内部函数

    /// <summary>
    /// 初始化输入管理器
    /// </summary>
    protected virtual void InitializeInputManager() => m_inputManager = GetComponent<PlayerInputManager>();
    
    /// <summary>
    /// 初始化玩家实体数据管理器
    /// </summary>
    protected virtual void InitializeStatsManager() => m_statsManager = GetComponent<PlayerStatsManager>();

    #endregion

    #region 属性

    /// <summary>
    /// 输入管理器
    /// </summary>
    public PlayerInputManager InputManager => m_inputManager;
    
    /// <summary>
    /// 玩家实体属性数据管理器
    /// </summary>
    public PlayerStatsManager StatsManager => m_statsManager;

    #endregion

    #region 字段

    /// <summary>
    /// 输入管理器
    /// </summary>
    protected PlayerInputManager m_inputManager;
    
    /// <summary>
    /// 玩家实体属性数据管理器
    /// </summary>
    protected PlayerStatsManager m_statsManager;

    #endregion
}