namespace Leen.Common
{
    /// <summary>
    /// 定义通知可观察的具体动作。
    /// </summary>
    public enum ObservableAction
    {
        /// <summary>
        /// 未知动作，默认值。
        /// </summary>
        Unknown,
        /// <summary>
        /// 创建动作。
        /// </summary>
        Create,
        /// <summary>
        /// 更新动作。
        /// </summary>
        Update,
        /// <summary>
        /// 删除动作。
        /// </summary>
        Delete,
    }
}
