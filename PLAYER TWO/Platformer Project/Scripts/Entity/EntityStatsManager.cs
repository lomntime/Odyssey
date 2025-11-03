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
}