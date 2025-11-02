using System;
using UnityEngine.Events;

/// <summary>
/// 实例状态机事件
/// </summary>
[Serializable]
public class EntityStateManagerEvents
{
    /// <summary>
    /// 实体状态修改时事件
    /// </summary>
    public UnityEvent EventOnChange;

    /// <summary>
    /// 实体进入状态时事件
    /// Type:状态类型
    /// </summary>
    public UnityEvent<Type> EventOnEnter;

    /// <summary>
    /// 实体离开状态时事件
    /// Type:状态类型
    /// </summary>
    public UnityEvent<Type> EventOnExit;
}