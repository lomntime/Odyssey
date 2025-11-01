using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 实体状态管理器
/// </summary>
public abstract class EntityStateManager : MonoBehaviour
{
    
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