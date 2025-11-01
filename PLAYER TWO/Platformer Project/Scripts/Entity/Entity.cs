using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
}

public abstract class Entity<T> : EntityBase where T : Entity<T>
{
}