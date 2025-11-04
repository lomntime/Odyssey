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
        
        m_cameraTargetPosition = m_player.transform.position + Vector3.up * m_heightOffset;
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

    #endregion

    #region 字段

    [Header("相机设置")]
    public Player m_player;
    
    public float m_maxDistance = 15f;

    public float m_initialAngle = 20f;

    public float m_heightOffset = 1f;
    
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
