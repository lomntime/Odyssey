using UnityEngine;

public class WalkPlayerState : PlayerState
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
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    protected override void OnContact(Player entity, Collider collider)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region 内部函数

    /// <summary>
    /// 获取相机视角下的玩家输入方向
    /// </summary>
    /// <returns></returns>
    protected virtual Vector3 MovementCameraDirectionGet()
    {
        return Vector3.zero;
    }

    #endregion
}