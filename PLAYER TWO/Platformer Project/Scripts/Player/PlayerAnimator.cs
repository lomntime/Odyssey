using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家动画控制器
/// </summary>
[RequireComponent(typeof(Player))]
[AddComponentMenu("PLAYER TWO/Platformer Project/Player/Player Animator")]
public class PlayerAnimator : MonoBehaviour
{

    #region 生命周期

    protected void Start()
    {
        InitializePlayer();
        InitializeForcedTransitions();
        InitializeParametersHash();
        InitializeAnimatorTriggers();
    }

    protected virtual void LateUpdate() => HandleAnimatorParameters(); 

    #endregion

    #region 内部函数

    /// <summary>
    /// 初始化玩家实例
    /// </summary>
    protected virtual void InitializePlayer()
    {
        m_player = GetComponent<Player>();
        
        m_player.StateManager.m_events.EventOnChange.AddListener(HandleForceTransitions);
    }

    /// <summary>
    /// 初始化玩家实体状态-动画状态表
    /// </summary>
    protected virtual void InitializeForcedTransitions()
    {
        m_forcedTransitions = new Dictionary<int, ForcedTransition>();

        foreach (var transition in m_forcedTransitionList)
        {
            if (!m_forcedTransitions.ContainsKey(transition.fromStateId))
            {
                m_forcedTransitions.Add(transition.fromStateId, transition);
            }
        }
    }

    /// <summary>
    /// 把参数名转换为 Hash，提高性能
    /// </summary>
    protected virtual void InitializeParametersHash()
    {
        m_stateHash = Animator.StringToHash(stateName);
        m_lastStateHash = Animator.StringToHash(lastStateName);
        m_lateralSpeedHash = Animator.StringToHash(lateralSpeedName);
        m_verticalSpeedHash = Animator.StringToHash(verticalSpeedName);
        m_lateralAnimationSpeedHash = Animator.StringToHash(lateralAnimationSpeedName);
        m_healthHash = Animator.StringToHash(healthName);
        m_jumpCounterHash = Animator.StringToHash(jumpCounterName);
        m_isGroundedHash = Animator.StringToHash(isGroundedName);
        m_isHoldingHash = Animator.StringToHash(isHoldingName);
        m_onStateChangedHash = Animator.StringToHash(onStateChangedName);
    }

    protected virtual void InitializeAnimatorTriggers()
    {
        m_player.StateManager.m_events.EventOnChange.AddListener(() => m_animator.SetTrigger(m_onStateChangedHash));
    }

    /// <summary>
    /// 强制设置过渡动画
    /// </summary>
    protected virtual void HandleForceTransitions()
    {
        var lastStateIndex = m_player.StateManager.PrevIndex;

        if (m_forcedTransitions.ContainsKey(lastStateIndex))
        {
            var layer = m_forcedTransitions[lastStateIndex].animationLayer;
            m_animator.Play(m_forcedTransitions[lastStateIndex].toAnimationState,  layer);
        }
    }
    
    /// <summary>
    /// 驱动Animation更新
    /// </summary>
    protected virtual void HandleAnimatorParameters()
    {
        var lateralSpeed = m_player.LateralVelocity.magnitude; // 横向速度
        var verticalSpeed = m_player.VerticalVelocity.y;       // 纵向速度
        // 横向动画播放速度 = 横向速度 / 最大速度，保证最小速度不低于 minLateralAnimationSpeed
        var lateralAnimationSpeed = Mathf.Max(minLateralAnimationSpeed, lateralSpeed / m_player.StatsManager.CurrStats.m_topSpeed);

        // 设置 Animator 参数
        m_animator.SetInteger(m_stateHash, m_player.StateManager.CurrIndex);
        m_animator.SetInteger(m_lastStateHash, m_player.StateManager.PrevIndex );
        m_animator.SetFloat(m_lateralSpeedHash, lateralSpeed);
        m_animator.SetFloat(m_verticalSpeedHash, verticalSpeed);
        m_animator.SetFloat(m_lateralAnimationSpeedHash, lateralAnimationSpeed);
        m_animator.SetFloat(m_jumpCounterHash, m_player.JumpCounter);
        m_animator.SetBool(m_isGroundedHash, m_player.IsGrounded);
    }

    #endregion

    #region 字段
    
    /// <summary>
    /// 动画播放器
    /// </summary>
    [Header("动画播放器")]
    public Animator m_animator;
    
    [Header("Parameters Names")] // Animator 参数的变量名（可在 Inspector 修改）
    public string stateName = "State";                      // 当前状态
    public string lastStateName = "Last State";             // 上一个状态
    public string lateralSpeedName = "Lateral Speed";       // 横向速度
    public string verticalSpeedName = "Vertical Speed";     // 纵向速度
    public string lateralAnimationSpeedName = "Lateral Animation Speed"; // 横向动画播放速度
    public string healthName = "Health";                    // 血量
    public string jumpCounterName = "Jump Counter";         // 跳跃计数
    public string isGroundedName = "Is Grounded";           // 是否落地
    public string isHoldingName = "Is Holding";             // 是否正在抓取物品
    public string onStateChangedName = "On State Changed";  // 状态切换触发器

    [Header("设置")]
    public float minLateralAnimationSpeed = 0.5f; // 横向动画播放的最小速度，防止太慢
    public List<ForcedTransition> m_forcedTransitionList; // 强制过渡的列表

    /// <summary>
    /// 玩家实例
    /// </summary>
    protected Player m_player;
    
    /// <summary>
    /// 玩家实体状态-动画状态映射
    /// Key:玩家状态索引
    /// Value:动画状态描述实例
    /// </summary>
    protected Dictionary<int, ForcedTransition> m_forcedTransitions;
    
    // Animator 参数的 Hash 值，避免字符串查找开销
    protected int m_stateHash;
    protected int m_lastStateHash;
    protected int m_lateralSpeedHash;
    protected int m_verticalSpeedHash;
    protected int m_lateralAnimationSpeedHash;
    protected int m_healthHash;
    protected int m_jumpCounterHash;
    protected int m_isGroundedHash;
    protected int m_isHoldingHash;
    protected int m_onStateChangedHash;

    #endregion
    
    #region 内部类

    /// <summary>
    /// 定义一个强制过渡的类，用于指定从某个玩家状态退出时，
    /// 强制跳转到 Animator 控制器中的某个动画。
    /// </summary>
    [System.Serializable]
    public class ForcedTransition
    {
        [Tooltip("玩家状态机中 'fromStateId' 的状态结束时，强制跳转到某个动画")]
        public int fromStateId;

        [Tooltip("目标动画所在的 Animator 层索引。默认0表示 Base Layer")]
        public int animationLayer;

        [Tooltip("要强制播放的动画状态名")]
        public string toAnimationState;
    }
    
    #endregion
}