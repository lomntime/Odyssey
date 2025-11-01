using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
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
        Ininialize();
    }

    #endregion
    
    #region 内部函数
    
    /// <summary>
    /// 初始化实体状态管理器
    /// </summary>
    protected virtual void Ininialize()
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
    
    #region 字段

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