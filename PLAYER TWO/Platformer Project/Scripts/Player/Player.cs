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
    /// 依据相机方向平滑移动玩家
    /// </summary>
    public virtual void AccelerateToInputDirection()
    {
        var inputDirection = m_inputManager.MovementCameraDirectionGet();
        Accelerate(inputDirection);
    }

    /// <summary>
    /// 重力下落
    /// </summary>
    public virtual void Gravity()
    {
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

    /// <summary>
    /// 通过Snap使玩家保持贴地
    /// </summary>
    public virtual void SnapToGround() => SnapToGround(StatsManager.CurrStats.m_snapForce);

    /// <summary>
    /// 重置跳跃计数
    /// </summary>
    public virtual void ResetJumps() => m_jumpCounter = 0;
    
    /// <summary>
    /// 下落逻辑
    /// </summary>
    public virtual void Fall()
    {
        if (!IsGrounded)
        {
            m_stateManager.Change<FallPlayerState>();
        }
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    public virtual void Jump()
    {
        var canMultiJump = (m_jumpCounter > 0) && (m_jumpCounter < m_statsManager.CurrStats.m_multiJumps);
        var canCoyoteJump = (m_jumpCounter == 0) && (Time.time < LastGroundTime + m_statsManager.CurrStats.m_coyoteJumpThreshold);
        
        if (IsGrounded || canMultiJump || canCoyoteJump)
        {
            if (m_inputManager.JumpDownGet()) 
            {
                Jump(m_statsManager.CurrStats.m_maxJumpHeight);
            }
        }
        
        if (m_inputManager.JumpUpGet() && (m_jumpCounter > 0) && (VerticalVelocity.y > m_statsManager.CurrStats.m_minJumpHeight))
        {
            VerticalVelocity = Vector3.up * m_statsManager.CurrStats.m_minJumpHeight;
        }
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    /// <param name="height">跳跃高度</param>
    public virtual void Jump(float height)
    {
        m_jumpCounter++;
        VerticalVelocity = Vector3.up * height;
        m_stateManager.Change<FallPlayerState>();
        m_playerEvents.EventOnJump?.Invoke();
    }
    
    #endregion    
    
    #region Override of Entity

    protected override void Awake()
    {
        base.Awake();
        InitializeInputManager();
        InitializeStatsManager();
        
        EntityEvents.EventOnGroundEnter.AddListener(ResetJumps);
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
    
    /// <summary>
    /// 跳跃计数
    /// </summary>
    public int JumpCounter => m_jumpCounter;

    #endregion

    #region 字段
    
    /// <summary>
    /// 玩家实体事件
    /// </summary>
    [Header("玩家实体事件")]
    public PlayerEvents m_playerEvents;

    /// <summary>
    /// 输入管理器
    /// </summary>
    protected PlayerInputManager m_inputManager;
    
    /// <summary>
    /// 玩家实体属性数据管理器
    /// </summary>
    protected PlayerStatsManager m_statsManager;
    
    /// <summary>
    /// 跳跃计数
    /// </summary>
    protected int m_jumpCounter = 0;

    #endregion
}