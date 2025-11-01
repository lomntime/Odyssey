using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerStateManager : EntityStateManager<Player>
{

    [ClassTypeName(typeof(PlayerState))]
    public string[] m_stateNames;
}