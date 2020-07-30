using System;

namespace Leen.Media
{
    /// <summary>
    /// 描述媒体播放源的播放能力。
    /// </summary>
    [Flags]
    public enum MediaSeekingCaps
    {
        /// <summary>
        /// 初始值。
        /// </summary>
        None = 0,

        /// <summary>
        /// 是否支持定位到绝对位置。
        /// </summary>
        CanSeekAbsolute = 1,

        /// <summary>
        /// 是否支持向前定位。
        /// </summary>
        CanSeekForwards = 2,

        /// <summary>
        /// 是否支持向后定位。
        /// </summary>
        CanSeekBackwards = 4,

        /// <summary>
        /// 是否支持获取当前位置。
        /// </summary>
        CanGetCurrentPos = 8,

        /// <summary>
        /// 是否支持获取停止位置。
        /// </summary>
        CanGetStopPos = 16,

        /// <summary>
        /// 是否支持获取原始时长。
        /// </summary>
        CanGetDuration = 32,

        /// <summary>
        /// 是否支持向后播放（倒播）。
        /// </summary>
        CanPlayBackwards = 64,
    }
}
