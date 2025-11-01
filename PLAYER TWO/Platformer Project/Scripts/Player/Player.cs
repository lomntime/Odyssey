public class Player : Entity<Player>
{
    #region Override of Entity

    protected override void Awake()
    {
        base.Awake();
        InitializeInputManager();
    }

    #endregion

    #region 内部函数

    /// <summary>
    /// 初始化输入管理器
    /// </summary>
    protected virtual void InitializeInputManager() => m_inputManager = GetComponent<PlayerInputManager>();

    #endregion

    #region 属性

    /// <summary>
    /// 输入管理器
    /// </summary>
    public PlayerInputManager InputManager => m_inputManager;

    #endregion

    #region 字段

    /// <summary>
    /// 输入管理器
    /// </summary>
    protected PlayerInputManager m_inputManager;

    #endregion
}