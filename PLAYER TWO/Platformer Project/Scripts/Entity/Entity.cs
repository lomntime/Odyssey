using UnityEngine;

/// <summary>
/// 实体基类
/// </summary>
public abstract class EntityBase : MonoBehaviour
{
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

    #endregion
    
    #region 生命周期

    protected virtual void Awake()
    {
        m_stateManager = GetComponent<EntityStateManager<T>>();
    }

    protected virtual void Update()
    {
        if(m_stateManager == null || Time.timeScale <= 0) return;
        
        HandleState();
        HandleController();
    }

    #endregion

    #region 内部函数

    /// <summary>
    /// 驱动状态执行
    /// </summary>
    protected virtual void HandleState() => m_stateManager.Step();

    /// <summary>
    /// 处理角色移动
    /// </summary>
    protected virtual void HandleController()
    {
        transform.position += m_velocity * Time.deltaTime;
    }

    #endregion
    
    #region 属性

    /// <summary>
    /// 实体状态管理器
    /// </summary>
    public EntityStateManager<T> StateManager => m_stateManager;

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