using System;
using UnityEngine.Events;

/// <summary>
/// 玩家事件
/// </summary>
[Serializable]
public class PlayerEvents
{
    /// <summary>
    /// 玩家跳跃事件
    /// </summary>
    public UnityEvent EventOnJump;
    
    /// <summary>
    /// 当玩家受到伤害时调用。
    /// </summary>
    public UnityEvent EventOnHurt;

    /// <summary>
    /// 当玩家死亡时调用。
    /// </summary>
    public UnityEvent EventOnDie;

    /// <summary>
    /// 当玩家使用旋转攻击时调用。
    /// </summary>
    public UnityEvent EventOnSpin;

    /// <summary>
    /// 当玩家拾取物体时调用。
    /// </summary>
    public UnityEvent EventOnPickUp;

    /// <summary>
    /// 当玩家投掷物体时调用。
    /// </summary>
    public UnityEvent EventOnThrow;

    /// <summary>
    /// 当玩家开始使用踩踏攻击时调用。
    /// </summary>
    public UnityEvent EventOnStompStarted;

    /// <summary>
    /// 当玩家在踩踏攻击中开始下落时调用。
    /// </summary>
    public UnityEvent EventOnStompFalling;

    /// <summary>
    /// 当玩家从踩踏攻击中落地时调用。
    /// </summary>
    public UnityEvent EventOnStompLanding;

    /// <summary>
    /// 当玩家结束踩踏攻击时调用。
    /// </summary>
    public UnityEvent EventOnStompEnding;

    /// <summary>
    /// 当玩家抓住边缘时调用。
    /// </summary>
    public UnityEvent EventOnLedgeGrabbed;

    /// <summary>
    /// 当玩家攀爬上平台边缘时调用。
    /// </summary>
    public UnityEvent EventOnLedgeClimbing;

    /// <summary>
    /// 当玩家执行空中俯冲时调用。
    /// </summary>
    public UnityEvent EventOnAirDive;

    /// <summary>
    /// 当玩家执行后空翻时调用。
    /// </summary>
    public UnityEvent EventOnBackflip;

    /// <summary>
    /// 当玩家开始滑翔时调用。
    /// </summary>
    public UnityEvent EventOnGlidingStart;

    /// <summary>
    /// 当玩家停止滑翔时调用。
    /// </summary>
    public UnityEvent EventOnGlidingStop;

    /// <summary>
    /// 当玩家开始冲刺时调用。
    /// </summary>
    public UnityEvent EventOnDashStarted;

    /// <summary>
    /// 当玩家结束冲刺时调用。
    /// </summary>
    public UnityEvent EventOnDashEnded;
}