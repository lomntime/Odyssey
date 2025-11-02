using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 实体状态
/// </summary>
/// <typeparam name="T">实体派生类型</typeparam>
public abstract class EntityState<T> where T : Entity<T>
{
    #region 外部接口

    /// <summary>
    /// 进入状态
    /// </summary>
    /// <param name="entity"></param>
    public void Enter(T entity)
    {
        m_tiemSinceEnter = 0f;
        
        EventOnEnter?.Invoke();
        OnEnter(entity);
    }

    /// <summary>
    /// 离开状态
    /// </summary>
    /// <param name="entity"></param>
    public void Exit(T entity)
    {
        EventOnExit?.Invoke();
        OnExit(entity);
    }

    /// <summary>
    /// 保持状态，每帧调用
    /// </summary>
    /// <param name="entity"></param>
    public void Step(T entity)
    {
        m_tiemSinceEnter += Time.deltaTime;
        OnStep(entity);
    }

    /// <summary>
    /// 碰撞检测逻辑
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="other"></param>
    public void Contact(T entity, Collider other)
    {
        OnContact(entity, other);
    }

    #endregion

    #region 内部函数

    /// <summary>
    /// 当实体进入状态
    /// </summary>
    /// <param name="entity"></param>
    protected abstract void OnEnter(T entity);
    
    /// <summary>
    /// 当实体离开状态
    /// </summary>
    /// <param name="entity"></param>
    protected abstract void OnExit(T entity);
    
    /// <summary>
    /// 当实体保持状态，每帧调用
    /// </summary>
    /// <param name="entity"></param>
    protected abstract void OnStep(T entity);
    
    /// <summary>
    /// 当实体碰撞时
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="other"></param>>
    protected abstract void OnContact(T entity, Collider other);

    #endregion
    
    #region 事件
    
    /// <summary>
    /// 状态进入事件
    /// </summary>
    public UnityEvent EventOnEnter;

    /// <summary>
    /// 状态退出事件
    /// </summary>
    public UnityEvent EventOnExit;

    /// <summary>
    /// 状态保持事件
    /// </summary>
    public UnityEvent EventOnStep;

    #endregion

    #region 属性

    /// <summary>
    /// 实体计入当前状态后经过时间，单位:秒
    /// </summary>
    public float TimeScineEnter
    {
        get
        {
            return m_tiemSinceEnter;
        }
    }

    #endregion

    #region 字段

    /// <summary>
    /// 实体计入当前状态后经过时间，单位:秒
    /// </summary>
    protected float m_tiemSinceEnter;

    #endregion
}