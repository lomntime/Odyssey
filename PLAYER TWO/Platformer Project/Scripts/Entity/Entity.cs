using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 实体基类
/// </summary>
public abstract class EntityBase : MonoBehaviour
{
    #region 外部接口

    /// <summary>
    /// 球体射线检测
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="distance"></param>
    /// <param name="hit"></param>
    /// <param name="layer"></param>
    /// <param name="queryTriggerInteraction"></param>
    /// <returns></returns>
    public virtual bool SphereCast(Vector3 direction, float distance, out RaycastHit hit,
        int layer = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        var castDistance = Mathf.Abs(distance- Radius);
        
        return Physics.SphereCast(Position, Radius, direction, out hit, castDistance, layer, queryTriggerInteraction);
    }
    
    /// <summary>
    /// 该点是否处于实体位置下方
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public virtual bool IsPointUnderStep(Vector3 point) => StepPosition.y > point.y;

    #endregion
    
    #region 属性
    public virtual Vector3 UnsizedPosition => transform.position;

    /// <summary>
    /// 实体事件
    /// </summary>
    public virtual EntityEvents EntityEvents => m_entityEvents;
    
    /// <summary>
    /// 实体的角色控制器
    /// </summary>
    public virtual CharacterController CharacterController { get; protected set; }
    
    /// <summary>
    /// 实体初始高度
    /// </summary>
    public virtual float OriginHeight {get; protected set;}
    
    /// <summary>
    /// 上次处于地面时间
    /// </summary>
    public virtual float LastGroundTime { get; protected set; }
    
    /// <summary>
    /// 当前地面角度
    /// </summary>
    public virtual float GroundAngle { get; protected set; }
    
    /// <summary>
    /// 当前地面法线
    /// </summary>
    public virtual Vector3 GroundNormal { get; protected set; }
    
    /// <summary>
    /// 当前地面的局部倾斜方向
    /// </summary>
    public virtual Vector3 LocalSlopeDirection { get; protected set; }
    
    /// <summary>
    /// 当前地面的碰撞信息
    /// </summary>
    public virtual RaycastHit GroundHit { get; protected set; }
    
    /// <summary>
    /// 实体碰撞器高度
    /// </summary>
    public virtual float Height => CharacterController.height;
    
    /// <summary>
    /// 实体碰撞器半径
    /// </summary>
    public virtual float Radius => CharacterController.radius;
    
    /// <summary>
    /// 实体碰撞器中心点
    /// </summary>
    public virtual Vector3 Center => CharacterController.center;

    /// <summary>
    /// 实体当前位置
    /// </summary>
    public virtual Vector3 Position => transform.position + Center;
    
    /// <summary>
    /// 实体底部位置
    /// </summary>
    public Vector3 StepPosition => Position - transform.up * (Height * 0.5f - CharacterController.stepOffset) ;

    /// <summary>
    /// 实体是否处于地面
    /// </summary>
    public virtual bool IsGrounded { get; set; } = true;
    
    /// <summary>
    /// 实体是否处于斜坡上
    /// </summary>
    public virtual bool IsOnSlopingGround { get; protected set; } = false;

    #endregion

    #region 字段
    
    /// <summary>
    /// 实体事件
    /// </summary>
    [Header("实体事件")]
    public EntityEvents m_entityEvents;

    /// <summary>
    /// 地面偏移检测
    /// </summary>
    public readonly float m_groundOffest = 0.1f;

    #endregion
}

/// <summary>
/// 泛型实体类，具体实体类型由此派生
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Entity<T> : EntityBase where T : Entity<T>
{
    #region 外部接口

    /// <summary>
    /// 平滑移动实体
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="turningDrag"></param>
    /// <param name="acceleration"></param>
    /// <param name="topSpeed"></param>
    public virtual void Accelerate(Vector3 direction, float turningDrag, float acceleration, float topSpeed)
    {
        if (direction.sqrMagnitude <= 0)
        {
            return;
        }

        var speed = Vector3.Dot(direction, LateralVelocity);
        var velocity = speed * direction;
        var turningVelocity = LateralVelocity -  velocity;
        var turningDelta = turningDrag * m_turningDragMultiplier  * Time.deltaTime;
        var targetSpeed = topSpeed * m_topSpeedMultiplier;

        if (LateralVelocity.sqrMagnitude < targetSpeed || speed < 0)
        {
            speed += acceleration * m_accelerationMultiplier * Time.deltaTime;
            speed = Mathf.Clamp(speed, -targetSpeed, targetSpeed);
        }

        velocity = direction * speed;
        turningVelocity = Vector3.MoveTowards(turningVelocity, Vector3.zero, turningDelta);
        LateralVelocity = velocity + turningVelocity;

    }

    /// <summary>
    /// 实体旋转到指定方向
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="degreesPerSecond"></param>
    public virtual void FaceDirection(Vector3 direction, float degreesPerSecond)
    {
        if (direction == Vector3.zero)
        {
            return;
        }
        
        var rotation = transform.rotation;
        var rotationDetail = degreesPerSecond * Time.deltaTime;
        var target = Quaternion.LookRotation(direction,  Vector3.up);
        transform.rotation = Quaternion.RotateTowards(rotation, target, rotationDetail);
    }

    /// <summary>
    /// 平滑减速
    /// </summary>
    /// <param name="deceleration"></param>
    public virtual void Decelerate(float deceleration)
    {
        var delta =  deceleration * m_decelerationMultiplier * Time.deltaTime;
        LateralVelocity = Vector3.MoveTowards(LateralVelocity, Vector3.zero, delta);
    }

    /// <summary>
    /// 使实体吸附地面
    /// </summary>
    /// <param name="force"></param>
    public virtual void SnapToGround(float force)
    {
        if (IsGrounded && VerticalVelocity.y <= 0)
        {
            VerticalVelocity =  Vector3.down * force;
        }
    }
    
    #endregion
    
    #region 生命周期

    protected virtual void Awake()
    {
        InitializeStateManager();
        InitializeCharacterController();
    }

    protected virtual void Update()
    {
        if(m_stateManager == null || Time.timeScale <= 0) return;

        if (CharacterController.enabled)
        {
            HandleState();
            HandleController();
            HandleGround();
        }
    }

    #endregion

    #region 内部函数
    
    /// <summary>
    /// 初始化实体状态控制器
    /// </summary>
    protected virtual void InitializeStateManager() => m_stateManager = GetComponent<EntityStateManager<T>>();

    /// <summary>
    /// 初始化角色控制器 
    /// </summary>
    protected virtual void InitializeCharacterController()
    {
        CharacterController = GetComponent<CharacterController>();

        if (!CharacterController)
        {
            CharacterController = gameObject.AddComponent<CharacterController>();
        }

        CharacterController.skinWidth = 0.0005f;
        CharacterController.minMoveDistance = 0f;
        
        OriginHeight = CharacterController.height;
    }

    /// <summary>
    /// 检测是否符合着陆条件
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    protected virtual bool EvaluateLanding(RaycastHit hit)
    {
        return IsPointUnderStep(hit.point) && Vector3.Angle(hit.normal, Vector3.up) < CharacterController.slopeLimit;
    }

    /// <summary>
    /// 实体进入地面
    /// </summary>
    /// <param name="hit"></param>
    protected virtual void EnterGround(RaycastHit hit)
    {
        if (!IsGrounded)
        {
            GroundHit = hit;
            IsGrounded = true;
            EntityEvents.EventOnGroundEnter?.Invoke();
        }
    }

    /// <summary>
    /// 实体离开地面
    /// </summary>
    /// <param name="hit"></param>
    protected virtual void ExitGround(RaycastHit hit)
    {
        if (IsGrounded)
        {
            IsGrounded = false;
            transform.parent = null;
            LastGroundTime = Time.time;
            VerticalVelocity = Vector3.Max(VerticalVelocity, Vector3.zero);
            EntityEvents.EventOnGroundExit?.Invoke();
        }
    }

    /// <summary>
    /// 更新地面相关数据
    /// </summary>
    /// <param name="hit"></param>
    protected virtual void UpdateGround(RaycastHit hit)
    {
        if (IsGrounded)
        {
            GroundHit = hit;
            GroundNormal = hit.normal;
            GroundAngle = Vector3.Angle(Vector3.up, hit.normal);
            LocalSlopeDirection = new Vector3(hit.normal.x, 0f, hit.normal.z).normalized;
            transform.parent = hit.collider.CompareTag(GameTags.Platform) ? hit.transform : null;
        }
    }
    
    /// <summary>
    /// 驱动状态执行
    /// </summary>
    protected virtual void HandleState() => m_stateManager.Step();

    /// <summary>
    /// 处理角色移动
    /// </summary>
    protected virtual void HandleController()
    {
        if (CharacterController.enabled)
        {
            CharacterController.Move(Velocity * Time.deltaTime);
            return;
        }
        
        transform.position += m_velocity * Time.deltaTime;
    }

    /// <summary>
    /// 实体与地面检测逻辑
    /// </summary>
    protected virtual void HandleGround()
    {
        var distacne = (Height * 0.5f) + m_groundOffest;

        if (SphereCast(Vector3.down, distacne, out var hit) && VerticalVelocity.y <= 0)
        {
            if (!IsGrounded)
            {
                if (EvaluateLanding(hit))
                {
                    EnterGround(hit);
                }
            }
            else if (IsPointUnderStep(hit.point))
            {
                UpdateGround(hit);
            }
        }
        else
        {
            ExitGround(hit);
        }
    }

    #endregion
    
    #region 属性

    /// <summary>
    /// 实体状态管理器
    /// </summary>
    public EntityStateManager<T> StateManager => m_stateManager;

    /// <summary>
    /// 速度
    /// </summary>
    public Vector3 Velocity
    {
        get => m_velocity;
        set => m_velocity = value;
    }

    /// <summary>
    /// 水平速度
    /// </summary>
    public Vector3 LateralVelocity
    {
        get { return new Vector3(m_velocity.x, 0f, m_velocity.z); }
        set { m_velocity = new Vector3(value.x, m_velocity.y, value.z); }
    }

    /// <summary>
    /// 垂直速度
    /// </summary>
    public Vector3 VerticalVelocity
    {
        get { return new Vector3(0f, m_velocity.y, 0f); }
        set { m_velocity = new Vector3(m_velocity.x, value.y, m_velocity.z); }
    }

    /// <summary>
    /// 加速度倍率
    /// </summary>
    public float AccelerationMultiplier
    {
        get => m_accelerationMultiplier; 
        set => m_accelerationMultiplier = value;
    }

    /// <summary>
    /// 重力倍率
    /// </summary>
    public float GravityMultiplier
    {
        get => m_gravityMultiplier;
        set => m_gravityMultiplier = value;
    }

    /// <summary>
    /// 最高速度倍率
    /// </summary>
    public float TopSpeedMultiplier
    {
        get => m_topSpeedMultiplier;
        set => m_topSpeedMultiplier = value;
    }

    /// <summary>
    /// 转向阻力倍率
    /// </summary>
    public float TurningDragMultiplier
    {
        get => m_turningDragMultiplier;
        set => m_turningDragMultiplier = value;
    }

    /// <summary>
    /// 减速度倍率
    /// </summary>
    public float DecelerationMultiplier
    {
        get => m_decelerationMultiplier;
        set => m_decelerationMultiplier = value;
    }

    #endregion
    
    #region 字段

    /// <summary>
    /// 实体状态管理器
    /// </summary>
    protected EntityStateManager<T> m_stateManager;

    /// <summary>
    /// 速度
    /// </summary>
    protected Vector3 m_velocity;
    
    /// <summary>
    /// 加速度倍率
    /// </summary>
    protected float m_accelerationMultiplier = 1f;
    
    /// <summary>
    /// 重力倍率
    /// </summary>
    protected float m_gravityMultiplier = 1f;
    
    /// <summary>
    /// 最高速度倍率
    /// </summary>
    protected float m_topSpeedMultiplier = 1f;
    
    /// <summary>
    /// 转向阻力倍率
    /// </summary>
    protected float m_turningDragMultiplier = 1f;
    
    /// <summary>
    /// 减速度倍率
    /// </summary>
    protected float m_decelerationMultiplier = 1f;

    #endregion
}