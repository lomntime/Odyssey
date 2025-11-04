using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 实体状态管理器
/// </summary>
public abstract class EntityStateManager : MonoBehaviour
{
    /// <summary>
    /// 实体状态管理器事件
    /// </summary>
    [Header("实体状态管理器事件")]
    public EntityStateManagerEvents m_events;
}

/// <summary>
/// 实体状态管理器
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class EntityStateManager<T> : EntityStateManager where T : Entity<T>
{
    #region 生命周期

    private void Start()
    {
        InitializeEntity();
        IninializeStateManager();
    }

    #endregion

    #region 外部接口
    public virtual void Step()
    {
        if (m_currState != null && Time.timeScale > 0)
        {
            m_currState.Step(m_entity);
        }
    }

    /// <summary>
    /// 更改状态
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public virtual void Change<TState>() where TState : EntityState<T>
    {
        var type = typeof(TState);

        if (m_stateDict.ContainsKey(type))
        {
            Change(m_stateDict[type]);
        }
    }

    /// <summary>
    /// 更改状态
    /// </summary>
    /// <param name="state"></param>
    public virtual void Change(EntityState<T> state)
    {
        if (state != null && Time.timeScale > 0)
        {
            if (m_currState != null)
            {
                m_currState.EventOnExit?.Invoke();
                m_events.EventOnExit?.Invoke(m_currState.GetType());
                m_prevState = m_currState;
            }
            
            m_currState = state;
            m_currState.EventOnEnter?.Invoke();
            m_events.EventOnEnter?.Invoke(m_currState.GetType());
            m_events.EventOnChange?.Invoke();
        }
    }

    #endregion
    
    #region 内部函数

    /// <summary>
    /// 初始化关联的实体实例
    /// </summary>
    protected virtual void InitializeEntity()
    {
        m_entity = GetComponent<T>();
    }
    
    /// <summary>
    /// 初始化实体状态管理器
    /// </summary>
    protected virtual void IninializeStateManager()
    {
        m_states = new();
        m_stateDict = new();
        
        m_states.AddRange(GetAllStates());

        foreach (var state in m_states)
        {
            var type =  state.GetType();

            if (!m_stateDict.ContainsKey(type))
            {
                m_stateDict.Add(type, state);
            }
        }

        if (m_states.Count > 0)
        {
            m_currState = m_states[0];
        }
    }

    /// <summary>
    /// 获取全部状态
    /// </summary>
    /// <returns></returns>
    protected abstract List<EntityState<T>> GetAllStates();
    
    #endregion

    #region 属性

    /// <summary>
    /// 关联的实体实例
    /// </summary>
    public T Entity
    {
        get { return m_entity; }
    }
    
    /// <summary>
    /// 当前实体状态
    /// </summary>
    public EntityState<T> CurrentState
    {
        get { return m_currState; }
    }

    /// <summary>
    /// 上一个状态
    /// </summary>
    public EntityState<T> PreviousState
    {
        get { return m_prevState; }
    }

    /// <summary>
    /// 当前状态在列表中的索引
    /// </summary>
    public int CurrIndex
    {
        get { return m_states.IndexOf(m_currState); }
    }

    /// <summary>
    /// 上一个状态在列表中的索引
    /// </summary>
    public int PrevIndex
    {
        get { return m_states.IndexOf(m_prevState); }
    }

    #endregion
    
    #region 字段
    
    /// <summary>
    /// 关联的实体实例
    /// </summary>
    protected T m_entity;

    /// <summary>
    /// 当前状态
    /// </summary>
    protected EntityState<T> m_currState;
    
    /// <summary>
    /// 上一个状态
    /// </summary>
    protected EntityState<T> m_prevState;

    /// <summary>
    /// 状态集合
    /// </summary>
    protected List<EntityState<T>> m_states;
    
    /// <summary>
    /// 状态字典
    /// Key:实体状态类型
    /// Value:实体状态实例
    /// </summary>
    protected Dictionary<Type, EntityState<T>> m_stateDict;

    #endregion
}