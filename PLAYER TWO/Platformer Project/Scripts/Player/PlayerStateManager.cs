using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家实体状态管理器
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerStateManager : EntityStateManager<Player>
{

    #region 内部函数

    /// <inheritdoc />
    protected override List<EntityState<Player>> GetAllStates()
    {
        return EntityStateHelper<Player>.CreateInsListFromStringArray(m_stateNames);
    }

    #endregion
    
    #region 字段
    
    /// <summary>
    /// 状态名称集合
    /// </summary>
    [ClassTypeName(typeof(PlayerState))]
    public string[] m_stateNames;
    
    #endregion

}