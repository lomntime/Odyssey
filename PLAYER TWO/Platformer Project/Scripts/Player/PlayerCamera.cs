using Cinemachine;
using Cinemachine.Utility;
using UnityEngine;

/// <summary>
/// 玩家相机
/// </summary>
[RequireComponent(typeof(CinemachineVirtualCamera))]
[AddComponentMenu("PLAYER TWO/Platformer Project/Player/Player Camera")]
public class PlayerCamera : MonoBehaviour
{
    #region 生命周期

    protected virtual void Start()
    {
        InitializeComponents();
        InitializeFoller();
        InitializeCamera();
    }

    protected virtual void LateUpdate()
    {
        HandleOrbit();
        HandleVelocityOrbit();
        HandleOffest();
        
        MoveTarget();
    }

    #endregion

    #region 外部接口

    /// <summary>
    /// 重置相机到初始位置与角度
    /// </summary>
    public virtual void Reset()
    {
        m_cameraDistance = m_maxDistance;
        m_cameraTargerPitch = m_initialAngle;
        m_cameraTargerYaw = m_player.transform.eulerAngles.y;
        
        m_cameraTargetPosition = m_player.UnsizedPosition + Vector3.up * m_heightOffset;
        MoveTarget();
        m_brain.ManualUpdate();
    }

    #endregion
    
    #region 内部函数

    /// <summary>
    /// 初始化组件
    /// </summary>
    protected virtual void InitializeComponents()
    {
        if (!m_player)
        {
            m_player = FindObjectOfType<Player>();
        }
        
        m_camera = GetComponent<CinemachineVirtualCamera>();
        m_cameraBody = m_camera.AddCinemachineComponent<Cinemachine3rdPersonFollow>();
        m_brain = Camera.main.GetComponent<CinemachineBrain>();
    }
    
    /// <summary>
    /// 初始化跟随点
    /// </summary>
    protected virtual void InitializeFoller()
    {
        m_target = new GameObject(TargetName).transform;
        m_target.position = m_player.transform.position;
    }

    /// <summary>
    /// 初始化相机
    /// </summary>
    protected virtual void InitializeCamera()
    {
        m_camera.Follow =  m_target.transform;
        m_camera.LookAt = m_player.transform;
        
        Reset();
    }

    /// <summary>
    /// 移动相机到跟随点
    /// </summary>
    protected virtual void MoveTarget()
    {
        m_target.position = m_cameraTargetPosition;
        m_target.rotation = Quaternion.Euler(m_cameraTargerPitch,  m_cameraTargerYaw, 0.0f);
        m_cameraBody.CameraDistance =  m_cameraDistance;
    }

    /// <summary>
    /// 手动环绕相机
    /// </summary>
    protected virtual void HandleOrbit()
    {
        if (!m_canOrbit)
        {
            return;
        }

        var direction = m_player.InputManager.LookDirectionGet();

        if (direction.sqrMagnitude > 0)
        {
            bool isUsingMouse = m_player.InputManager.IsLookingWithMouse();
            
            float deltaTimeMultiplier = isUsingMouse ? Time.timeScale : Time.deltaTime;

            m_cameraTargerYaw += direction.x * deltaTimeMultiplier;
            m_cameraTargerPitch += direction.z * deltaTimeMultiplier;
            
            m_cameraTargerPitch = ClampAngle(m_cameraTargerPitch, m_verticalMinRotation, m_verticalMaxRotation);
        }
    }

    /// <summary>
    /// 限定角度范围
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    protected virtual float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) 
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

    /// <summary>
    /// 依据玩家速度自动环绕相机
    /// </summary>
    protected virtual void HandleVelocityOrbit()
    {
        if (m_canOrbitWithVelocity && m_player.IsGrounded)
        {
            var localVelocity = m_target.InverseTransformDirection(m_player.Velocity);
            
            m_cameraTargerYaw += localVelocity.x * m_orbitVelocityMultiplier *  Time.deltaTime;
        }
    }

    /// <summary>
    /// 处理相机的高度偏移
    /// </summary>
    protected virtual void HandleOffest()
    {
        var target = m_player.UnsizedPosition + Vector3.up * m_heightOffset;
        var previousPosition = m_cameraTargetPosition;
        var targetHeight = previousPosition.y;

        // 地面跟随
        if (m_player.IsGrounded || VerticalFollowingStates())
        {
            if (target.y > previousPosition.y + m_verticalUpDeadZone)
            {
                // 玩家上升时，相机缓慢跟随
                var offset = target.y - previousPosition.y - m_verticalUpDeadZone;
                targetHeight += Mathf.Min(offset, m_maxVerticalSpeed * Time.deltaTime);
            }
            else if (target.y < previousPosition.y - m_verticalDownDeadZone)
            {
                // 玩家下降时，相机缓慢跟随
                var offset = target.y - previousPosition.y + m_verticalDownDeadZone;
                targetHeight += Mathf.Max(offset, -m_maxVerticalSpeed * Time.deltaTime);
            }
        }
        // 空中跟随
        else if (target.y > previousPosition.y + m_verticalAirUpDeadZone)
        {
            var offset = target.y - previousPosition.y - m_verticalAirUpDeadZone;
            targetHeight += Mathf.Min(offset, m_maxAirVerticalSpeed * Time.deltaTime);
        }
        else if (target.y < previousPosition.y - m_verticalAirDownDeadZone)
        {
            var offset = target.y - previousPosition.y + m_verticalAirDownDeadZone;
            targetHeight += Mathf.Max(offset, -m_maxAirVerticalSpeed * Time.deltaTime);
        }

        m_cameraTargetPosition = new Vector3(target.x, targetHeight, target.z);
    }

    /// <summary>
    /// 是否处于需要垂直跟随的状态
    /// </summary>
    /// <returns></returns>
    protected virtual bool VerticalFollowingStates()
    {
        return true;
    }

    #endregion

    #region 字段

    [Header("相机设置")]
    public Player m_player;
    
    public float m_maxDistance = 15f;

    public float m_initialAngle = 20f;

    public float m_heightOffset = 1f;
    
    [Header("相机环绕设置")]
    public bool m_canOrbit = true;
    
    public bool m_canOrbitWithVelocity = true;

    public float m_orbitVelocityMultiplier = 5f;

    [Range(0f, 90f)]
    public float m_verticalMaxRotation = 80f;
    
    [Range(-90f, 0f)]
    public float m_verticalMinRotation = -80f;
    
    [Header("跟随设置")] 
    public float m_verticalUpDeadZone = 0.15f;      // 在地面时，相机向上跟随的死区
    public float m_verticalDownDeadZone = 0.15f;    // 在地面时，相机向下跟随的死区
    public float m_verticalAirUpDeadZone = 4f;      // 在空中时，相机向上跟随的死区
    public float m_verticalAirDownDeadZone = 0;     // 在空中时，相机向下跟随的死区
    public float m_maxVerticalSpeed = 10f;          // 相机在地面时的最大垂直跟随速度
    public float m_maxAirVerticalSpeed = 100f;      // 相机在空中时的最大垂直跟随速度
    
    protected CinemachineVirtualCamera m_camera;

    protected Cinemachine3rdPersonFollow m_cameraBody;

    protected CinemachineBrain m_brain;

    protected Transform m_target;
    
    /// <summary>
    /// 相机与玩家实体距离
    /// </summary>
    protected float m_cameraDistance;

    /// <summary>
    /// 相机目标的水平角
    /// </summary>
    protected float m_cameraTargerYaw;
    
    /// <summary>
    /// 相机目标的俯仰角
    /// </summary>
    protected float m_cameraTargerPitch;
    
    /// <summary>
    /// 相机目标位置
    /// </summary>
    protected Vector3 m_cameraTargetPosition;

    #endregion

    #region 常量

    /// <summary>
    /// 目标名称
    /// </summary>
    protected const string TargetName = "Player Follower Camera Target";

    #endregion
}
