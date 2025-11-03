using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 玩家输入管理器
/// </summary>
public class PlayerInputManager : MonoBehaviour
{
    #region 生命周期

    protected virtual void Awake() => CacheAction();
    
    protected virtual void OnEnable() => m_inputAction.Enable();
    
    protected virtual void OnDisable() => m_inputAction.Disable();

    #endregion

    #region 外部接口

    /// <summary>
    /// 获取移动方向
    /// </summary>
    /// <returns>归一化后的移动方向</returns>
    public virtual Vector3 MovementDirectionGet()
    {
        if (Time.time < m_movementDirectionUnlockTime) return Vector3.zero;
        
        var value = m_movement.ReadValue<Vector2>();
        return AxisWithCrossDeadZone(value);
    }

    /// <summary>
    /// 获取玩家视角相机下的移动方向
    /// </summary>
    /// <returns>玩家相机视角下的归一化后的移动方向</returns>
    public virtual Vector3 MovementCameraDirectionGet()
    {
        var direction = MovementDirectionGet();

        if (direction.sqrMagnitude > 0)
        {
            var rotation = Quaternion.AngleAxis(m_camera.transform.eulerAngles.y, Vector3.up);
            
            direction = rotation * direction;

            direction = direction.normalized;
        }

        return direction;
    }
    
    #endregion

    #region 内部函数

    /// <summary>
    /// 缓存InputAction
    /// </summary>
    protected virtual void CacheAction()
    {
        m_camera = Camera.main;
        
        m_movement = m_inputAction.FindAction(MovementActionName);
    }

    /// <summary>
    /// 修正输入的十字形死区
    /// </summary>
    /// <param name="axis"></param>
    /// <returns></returns>
   protected Vector3 AxisWithCrossDeadZone(Vector2 axis)
    {
        var deadZone = InputSystem.settings.defaultDeadzoneMin;
        var xAxis = Mathf.Abs(axis.x > deadZone ? RemapToDeadZone(axis.x, deadZone) : 0f);
        var yAxix = Mathf.Abs(axis.y > deadZone ? RemapToDeadZone(axis.y, deadZone) : 0f);
        
        return new Vector3(xAxis, 0f, yAxix);
    }

    /// <summary>
    /// 重新映射输入到0-1
    /// </summary>
    /// <param name="value"></param>
    /// <param name="deadZone"></param>
    /// <returns></returns>
    protected float RemapToDeadZone(float value, float deadZone)
    {
        return (value - (value > 0 ? -deadZone : deadZone)) / (1 - deadZone);
    }

    #endregion
    
    #region 字段

    /// <summary>
    /// 移动
    /// </summary>
    protected InputAction m_movement;
    
    /// <summary>
    /// 玩家视角相机相
    /// </summary>
    protected Camera m_camera;

    /// <summary>
    /// InputAction资产
    /// </summary>
    [Header("InputAction资产")]
    public InputActionAsset m_inputAction;
    
    /// <summary>
    /// 锁定移动方向的时间戳
    /// </summary>
    [Header("锁定移动方向的时间戳")] 
    public float m_movementDirectionUnlockTime;
    
    #endregion

    #region 常量

    protected const string MovementActionName = "Movement";

    #endregion
}
