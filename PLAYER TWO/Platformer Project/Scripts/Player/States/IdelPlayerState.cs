using UnityEngine;

public class IdelPlayerState : PlayerState
{
    #region override of EntityState

    /// <inheritdoc/>
    protected override void OnEnter(Player entity)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    protected override void OnExit(Player entity)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    protected override void OnStep(Player entity)
    {
        var direction = entity.InputManager.MovementDirectionGet();
        
        Debug.Log("IdelPlayerState::OnStep Direction: " + direction);
    }

    /// <inheritdoc/>
    protected override void OnContact(Player entity, Collider collider)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}       