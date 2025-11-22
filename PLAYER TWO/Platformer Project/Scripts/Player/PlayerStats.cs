using UnityEngine;

/// <summary>
/// 玩家实体属性数据
/// </summary>
public class PlayerStats : EntityStats<PlayerStats>
{
    //==============================【基础属性】==============================//
    [Header("基础属性")] 
    public float m_pushForce = 4f; // 推动物体的力量
    public float m_snapForce = 15f; // 将角色贴合到地面的吸附力
    public float m_slideForce = 10f; // 下坡滑动的额外推力
    public float m_rotationSpeed = 970f; // 玩家角色旋转速度（度/秒）
    public float m_gravity = 38f; // 普通重力加速度
    public float m_fallGravity = 65f; // 下落时额外重力加速度
    public float m_gravityTopSpeed = 50f; // 重力作用下的最大下落速度

    //==============================【运动属性】==============================//
    [Header("运动属性")] 
    public bool m_applySlopeFactor = true; // 是否考虑坡度因子
    public float m_acceleration = 13f; // 加速度
    public float m_deceleration = 28f; // 减速度
    public float m_friction = 28f; // 地面摩擦力
    public float m_slopeFriction = 18f; // 坡面摩擦力
    public float m_topSpeed = 6f; // 最高速度
    public float m_turningDrag = 28f; // 转向时的阻力
    public float m_airAcceleration = 32f; // 空中加速度
    public float m_brakeThreshold = -0.8f; // 刹车判定阈值
    public float m_slopeUpwardForce = 25f; // 上坡时的额外推力
    public float m_slopeDownwardForce = 28f; // 下坡时的额外推力
    
    //==============================【跳跃】==============================//
    [Header("跳跃属性")]
    public int m_multiJumps = 1;                 // 允许的额外跳跃次数（多段跳）
    public float m_coyoteJumpThreshold = 0.15f;  // 土狼跳判定时间（离地后还能跳的时间窗口）
    public float m_maxJumpHeight = 17f;          // 最大跳跃高度
    public float m_minJumpHeight = 10f;          // 最小跳跃高度（轻按跳）
    
    //==============================【受伤反应】==============================//
    [Header("受伤反应")]
    public float m_hurtUpwardForce = 10f;        // 受伤时向上的力
    public float m_hurtBackwardsForce = 5f;      // 受伤时向后的力
    
    //==============================【下蹲】==============================//
    [Header("下蹲属性")]
    public float m_crouchHeight = 1f;            // 下蹲时角色高度
    public float m_crouchFriction = 10f;         // 下蹲时摩擦力
    
    //==============================【匍匐爬行】==============================//
    [Header("匍匐爬行")]
    public float m_crawlingAcceleration = 8f;    // 爬行加速度
    public float m_crawlingFriction = 32f;       // 爬行摩擦力
    public float m_crawlingTopSpeed = 2.5f;      // 爬行最高速度
    public float m_crawlingTurningSpeed = 3f;    // 爬行转向速度
    
    //==============================【踩踏攻击】==============================//
    [Header("踩踏攻击")]
    public bool m_canStompAttack = true;         // 是否能进行踩踏攻击
    public float m_stompDownwardForce = 20f;     // 踩踏时向下的力
    public float m_stompAirTime = 0.8f;          // 空中踩踏时间
    public float m_stompGroundTime = 0.5f;       // 落地后的硬直时间
    public float m_stompGroundLeapHeight = 10f;  // 踩踏落地后的反弹高度
    
    //==============================【后空翻】==============================//
    [Header("后空翻")]
    public bool m_canBackflip = true;            // 是否能后空翻
    public bool m_backflipLockMovement = true;   // 后空翻时是否锁定移动
    public float m_backflipAirAcceleration = 12f;// 空中加速度
    public float m_backflipTurningDrag = 2.5f;   // 转向阻力
    public float m_backflipTopSpeed = 7.5f;      // 最高速度
    public float m_backflipJumpHeight = 23f;     // 跳跃高度
    public float m_backflipGravity = 35f;        // 重力
    public float m_backflipBackwardForce = 4f;   // 向后推力
    public float m_backflipBackwardTurnForce = 8f;// 向后转向力

    //==============================【冲刺/疾跑】==============================//
    [Header("冲刺/疾跑")]
    public bool m_canAirDash = true;             // 是否能空中冲刺
    public bool m_canGroundDash = true;          // 是否能地面冲刺
    public float m_dashForce = 25f;              // 冲刺推力
    public float m_dashDuration = 0.3f;          // 冲刺持续时间
    public float m_groundDashCoolDown = 0.5f;    // 地面冲刺冷却时间
    public float m_allowedAirDashes = 1;         // 允许的空中冲刺次数
    
}