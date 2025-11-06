using System;
using UnityEngine.Events;

/// <summary>
/// 实体事件
/// </summary>
[Serializable]
public class EntityEvents
{
    /// <summary>
    /// 实体落地事件
    /// </summary>
    public UnityEvent EventOnGroundEnter;
    
    /// <summary>
    /// 实体离开地面事件
    /// </summary>
    public UnityEvent EventOnGroundExit;

    /// <summary>
    /// 实体进入轨道事件
    /// </summary>
    public UnityEvent EventOnRailsEnter;
    
    /// <summary>
    /// 实体离开轨道事件
    /// </summary>
    public UnityEvent EventOnRailsExit;
}