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

    #region 生命周期

    protected virtual void Awake()
    {
        m_stateManager = GetComponent<EntityStateManager<T>>();
    }

    protected virtual void Update()
    {
        if(m_stateManager == null || Time.timeScale <= 0) return;
        
        HandleState();
    }

    #endregion

    #region 内部函数

    protected virtual void HandleState() => m_stateManager.Step();

    #endregion
    
    #region 属性

    /// <summary>
    /// 实体状态管理器
    /// </summary>
    public EntityStateManager StateManager => m_stateManager;

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

    #endregion
}