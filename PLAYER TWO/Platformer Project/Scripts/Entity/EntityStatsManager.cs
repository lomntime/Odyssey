using UnityEngine;

/// <summary>
/// 实体属性数据管理类
/// </summary>
public abstract class EntityStatsManager : MonoBehaviour
{
}

/// <summary>
/// 实体属性数据管理类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class EntityStatsManager<T> : EntityStatsManager where T : EntityStats<T>
{
    #region 外部接口

    /// <summary>
    /// 启动
    /// </summary>
    public virtual void Start()
    {
        if (m_stats.Length > 0)
        {
             m_currStats = m_stats[0];
        }
    }

    /// <summary>
    /// 更改实体当前使用的属性数据
    /// </summary>
    /// <param name="to"></param>
    public virtual void Change(int to)
    {
        if (to >= 0 && to < m_stats.Length)
        {
            if (m_currStats != m_stats[to])
            {
                m_currStats = m_stats[to];
            }
        }
    }

    #endregion

    #region 内部函数
    

    #endregion

    #region 属性
    
    /// <summary>
    /// 当前实体使用的属性数据
    /// </summary>
    public T CurrStats => m_currStats;

    #endregion

    #region 字段

    /// <summary>
    /// 实体属性数据集合
    /// </summary>
    [Header("实体属性数据集合")]
    public T[] m_stats;

    /// <summary>
    /// 当前实体使用的属性数据
    /// </summary>
    protected T m_currStats;

    #endregion
}