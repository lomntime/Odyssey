using System.Collections.Generic;

/// <summary>
/// 实体状态
/// </summary>
/// <typeparam name="T">实体派生类型</typeparam>
public abstract class EntityState<T> where T : Entity<T>
{
    /// <summary>
    /// 使用实体状态类型名称创建对应实体状态实例
    /// </summary>
    /// <param name="typeName">实体状态名称</param>
    /// <returns></returns>
    public static EntityState<T> CreateInsFromString(string typeName)
    {
        var ins = System.Activator.CreateInstance(System.Type.GetType(typeName));
        
        return (EntityState<T>)ins;
    }

    /// <summary>
    /// 使用实体状态类型名称数组创建对应实体状态实例列表
    /// </summary>
    /// <param name="typeNames">实体状态名称数组</param>
    /// <returns></returns>
    public static List<EntityState<T>> CreateInsListFromStringArray(string[] typeNames)
    {
        var tempList = new List<EntityState<T>>();

        foreach (var typeName in typeNames)
        {
            tempList.Add(CreateInsFromString(typeName));
        }
        
        return tempList;
    }
}