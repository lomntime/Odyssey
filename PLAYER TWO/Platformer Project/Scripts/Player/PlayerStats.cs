using UnityEngine;

/// <summary>
/// 玩家实体属性数据
/// </summary>
    public class PlayerStats : EntityStats<PlayerStats>
    {
        //==============================【运动属性】==============================//
        [Header("运动属性")]
        public bool m_applySlopeFactor = true;   // 是否考虑坡度因子
        public float m_acceleration = 13f;       // 加速度
        public float m_deceleration = 28f;       // 减速度
        public float m_friction = 28f;           // 地面摩擦力
        public float m_slopeFriction = 18f;      // 坡面摩擦力
        public float m_topSpeed = 6f;            // 最高速度
        public float m_turningDrag = 28f;        // 转向时的阻力
        public float m_airAcceleration = 32f;    // 空中加速度
        public float m_brakeThreshold = -0.8f;   // 刹车判定阈值
        public float m_slopeUpwardForce = 25f;   // 上坡时的额外推力
        public float m_slopeDownwardForce = 28f; // 下坡时的额外推力
    }
