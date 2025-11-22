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

    protected virtual void Update()
    {
        if (m_jump.WasPressedThisFrame())
        {
            m_lastJumpTime = Time.time;
        }
    }
    
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

    /// <summary>
    /// 获取玩家观察方向
    /// </summary>
    /// <returns></returns>
    public virtual Vector3 LookDirectionGet()
    {
        var val = m_look.ReadValue<Vector2>();

        if (IsLookingWithMouse())
        {
            return new  Vector3(val.x, 0f, val.y);
        }
        
        return AxisWithCrossDeadZone(val);
    }
    
    /// <summary>
    /// 是否使用使用鼠标设备控制观察视角
    /// </summary>
    /// <returns></returns>
    public virtual bool IsLookingWithMouse()
    {
        if (m_look == null)
        {
            return false;
        }
        
        // return m_look.activeControl.device.name.Equals(MouseDeviceName);
        
        // todo : 临时做法
        return true;
    }
    
    /// <summary>
    /// 冲刺键按下
    /// </summary>
    /// <returns></returns>
    public virtual bool DashDownGet() => m_dash.IsPressed();

    /// <summary>
    /// 是否支持跳跃 跳跃键按下 
    /// </summary>
    /// <returns></returns>
    public virtual bool JumpDownGet()
    {
        if(m_lastJumpTime != null &&  Time.time - m_lastJumpTime < JumpBuffer)
        {
            m_lastJumpTime = null;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 跳跃键抬起
    /// </summary>
    /// <returns></returns>
    public virtual bool JumpUpGet() => m_jump.WasReleasedThisFrame();

    public virtual bool CrouchAndCrawGet() => m_crouch.IsPressed();
    
    /// <summary>
    /// 下砸键按下
    /// </summary>
    /// <returns></returns>
    public virtual bool StompDownGet() => m_stomp.WasPressedThisFrame();

    /// <summary>
    /// 临时锁定移动方向
    /// </summary>
    /// <param name="duration">锁定时长</param>
    public virtual void LockMovementDirection(float duration = 0.25f)
    {
        m_movementDirectionUnlockTime = Time.time + duration;
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
        m_look = m_inputAction.FindAction(LookActionName);
        m_jump = m_inputAction.FindAction(JumpActionName);
        m_crouch = m_inputAction.FindAction(CrouchActionName);
        m_dash = m_inputAction.FindAction(DashActionName);
        m_stomp = m_inputAction.FindAction(StompActionName);
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
    /// 玩家观察方向
    /// </summary>
    protected InputAction m_look;
    
    /// <summary>
    /// 跳跃
    /// </summary>
    protected InputAction m_jump;

    /// <summary>
    /// 下蹲
    /// </summary>
    protected InputAction m_crouch;
    
    /// <summary>
    /// 冲刺
    /// </summary>
    protected InputAction m_dash;
    
    /// <summary>
    /// 下砸
    /// </summary>
    protected InputAction m_stomp;
    
    /// <summary>
    /// 玩家视角相机相
    /// </summary>
    protected Camera m_camera;

    /// <summary>
    /// 上次跳跃时间
    /// </summary>
    protected float? m_lastJumpTime;

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

    /// <summary>
    /// 移动Action名称
    /// </summary>
    protected const string MovementActionName = "Movement";
    
    /// <summary>
    /// 玩家观察方向Action名称
    /// </summary>
    protected const string LookActionName = "Look";
    
    /// <summary>
    /// 跳跃Action名称
    /// </summary>
    protected const string JumpActionName = "Jump";
    
    /// <summary>
    /// 下蹲Action名称
    /// </summary>
    protected const string CrouchActionName = "Crouch";
    
    /// <summary>
    /// 冲刺Action名称
    /// </summary>
    protected const string DashActionName = "Dash";
    
    /// <summary>
    /// 下砸Action状态
    /// </summary>
    protected const string StompActionName = "Stomp";
    
    /// <summary>
    /// 鼠标设备名称
    /// </summary>
    protected const string MouseDeviceName = "Mouse";
    

    /// <summary>
    /// 跳跃缓冲时间
    /// </summary>
    protected const float JumpBuffer = 0.15f;

    #endregion
}
