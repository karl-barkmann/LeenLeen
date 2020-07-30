namespace Leen.Media
{
    /// <summary>
    /// 表示媒体播放器的一组播放状态。
    /// </summary>
    public enum MediaPlayState
    {
        /// <summary>
        /// 初始值或默认值，表示媒体已关闭。
        /// </summary>
        None,

        /// <summary>
        /// 媒体已打开。
        /// </summary>
        Opened = 1,

        /// <summary>
        /// 媒体播放中。
        /// </summary>
        Playing = 2,

        /// <summary>
        /// 媒体播放停止。
        /// </summary>
        Stopped = 3,

        /// <summary>
        /// 媒体播放暂停。
        /// </summary>
        Paused = 4,

        /// <summary>
        /// 媒体播放完成。
        /// </summary>
        Complete = 5,

        /// <summary>
        /// 媒体倒播中。
        /// </summary>
        Rewinding = 6,
    }
}
