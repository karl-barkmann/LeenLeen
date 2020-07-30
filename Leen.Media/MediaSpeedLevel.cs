namespace Leen.Media
{
    /// <summary>
    /// 表示媒体播放器的播放速率级别。
    /// </summary>
    public enum MediaSpeedLevel : int
    {
        /// <summary>
        /// 慢速。
        /// </summary>
        LevelDown = -1,

        /// <summary>
        /// 正常速率。
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 快速。
        /// </summary>
        LevelUp = 1,
    }
}
