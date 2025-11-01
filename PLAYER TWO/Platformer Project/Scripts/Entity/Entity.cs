using UnityEngine;

/// <summary>
/// 
/// </summary>
public abstract class EntityBase : MonoBehaviour
{
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Entity<T> : EntityBase where T : Entity<T>
{
}