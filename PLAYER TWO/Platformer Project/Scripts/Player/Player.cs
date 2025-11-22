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
    /// 匍匐爬行
    /// </summary>
    /// <param name="direction"></param>
    public virtual void CrawlingAccelerate(Vector3 direction) => Accelerate(direction, m_statsManager.CurrStats.m_crawlingTurningSpeed, m_statsManager.CurrStats.m_crawlingAcceleration, m_statsManager.CurrStats.m_crawlingTopSpeed);

    /// <summary>
    /// 在空翻动作中平移滑动玩家（后空翻参数）
    /// </summary>
    public virtual void BackflipAcceleration()
    {
        var direction = m_inputManager.MovementCameraDirectionGet();
        Accelerate(direction, StatsManager.CurrStats.m_backflipTurningDrag, StatsManager.CurrStats.m_backflipAirAcceleration, StatsManager.CurrStats.m_crawlingTurningSpeed);
    }
    
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
    /// 设置跳跃次数
    /// </summary>
    /// <param name="amount"></param>
    public virtual void SetJumps(int amount) => m_jumpCounter = amount;
    
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

    /// <summary>
    /// 玩家收到伤害
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="origin"></param>
    public override void ApplyDamage(int amount, Vector3 origin)
    {
        if (!Health.isEmpty && !Health.recovering)
        {
            m_health.Damage(amount);
            var damageDir = origin - transform.position; 
            damageDir.y = 0;
            damageDir = damageDir.normalized;
            FaceDirection(damageDir);

            LateralVelocity = -transform.forward * m_statsManager.CurrStats.m_hurtBackwardsForce;

            if (!IsOnWater)
            {
                VerticalVelocity = Vector3.up * m_statsManager.CurrStats.m_hurtUpwardForce;
                m_stateManager.Change<HurtPlayerState>();
            }
            
            m_playerEvents.EventOnHurt?.Invoke();
            
            // if (Health.isEmpty)
            // {
            //     Throw();
            //     playerEvents.OnDie?.Invoke();
            // }
        }
    }

    /// <summary>
    /// 是否能够站起
    /// </summary>
    public virtual bool CanStandUp() => !SphereCast(Vector3.up, OriginHeight);

    /// <summary>
    /// 后空翻
    /// </summary>
    /// <param name="force"></param>
    public virtual void Backflip(float force)
    {
        if (StatsManager.CurrStats.m_canBackflip && !IsHolding)
        {
            VerticalVelocity = Vector3.up * StatsManager.CurrStats.m_backflipJumpHeight;
            LateralVelocity = -transform.forward * force;
            StateManager.Change<BackflipPlayerState>();
            m_playerEvents.EventOnBackflip?.Invoke();
        }
    }

    /// <summary>
    /// 重置空中冲刺计数
    /// </summary>
    public virtual void ResetAirDash() => m_airDashCounter = 0;
    
    /// <summary>
    /// 冲刺
    /// </summary>
    public virtual void Dash()
    {
        var canAirDash = StatsManager.CurrStats.m_canAirDash && !IsGrounded &&
                         AirDashCounter < StatsManager.CurrStats.m_allowedAirDashes;

        var canGroundDash = StatsManager.CurrStats.m_canGroundDash && IsGrounded &&
                            Time.time - LastDashTime > StatsManager.CurrStats.m_groundDashCoolDown;

        if (InputManager.DashDownGet() && (canAirDash || canGroundDash))
        {
            if (!IsGrounded) m_airDashCounter++;
            m_lastDashTime = Time.time;
            StateManager.Change<DashPlayerState>() ;
        }
    }

    /// <summary>
    /// 下砸攻击
    /// </summary>
    public virtual void StompAttack()
    {
        if (!IsGrounded && !IsHolding && StatsManager.CurrStats.m_canStompAttack && InputManager.StompDownGet())
        {
            StateManager.Change<StompPlayerState>();
        }
    }
    
    #endregion    
    
    #region Override of Entity

    protected override void Awake()
    {
        base.Awake();
        InitializeInputManager();
        InitializeStatsManager();
        InitializeHealth();
        InitializeTag();
        
        
        EntityEvents.EventOnGroundEnter.AddListener(() =>
        {
            ResetJumps();
            ResetAirDash();
        });
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

    /// <summary>
    /// 初始化玩家生命值
    /// </summary>
    protected virtual void InitializeHealth() => m_health = GetComponent<Health>();

    /// <summary>
    /// 初始化玩家Tag
    /// </summary>
    protected virtual void InitializeTag() => m_gameTags = global::GameTags.Player;

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
    /// 玩家生命值
    /// </summary>
    public Health Health => m_health;
    
    /// <summary>
    /// 玩家Tag
    /// </summary>
    public string GameTags => m_gameTags;
    
    /// <summary>
    /// 跳跃计数
    /// </summary>
    public int JumpCounter => m_jumpCounter;
    
    /// <summary>
    /// 空中冲刺计数
    /// </summary>
    public int AirDashCounter => m_airDashCounter;
    
    /// <summary>
    /// 上一次冲刺时间
    /// </summary>
    public float LastDashTime => m_lastDashTime;
    
    /// <summary>
    /// 是否处于水中
    /// </summary>
    public bool IsOnWater => m_isOnWater;
    
    /// <summary>
    /// 是否持有物品
    /// </summary>
    public bool IsHolding { get; protected set; }

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
    /// 玩家生命值
    /// </summary>
    protected Health m_health;
    
    /// <summary>
    /// 玩家Tag
    /// </summary>
    protected string m_gameTags;
    
    /// <summary>
    /// 跳跃计数
    /// </summary>
    protected int m_jumpCounter = 0;
    
    /// <summary>
    /// 空中冲刺计数
    /// </summary>
    protected int m_airDashCounter = 0;

    /// <summary>
    /// 上一次冲刺时间
    /// </summary>
    protected float m_lastDashTime;

    /// <summary>
    /// 是否处于水中
    /// </summary>
    protected bool m_isOnWater;

    #endregion
}