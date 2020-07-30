using System;

namespace Leen.Media
{
    /// <summary>
    /// 表示媒体播放器的播控接口能力。
    /// </summary>
    [Flags]
    public enum MediaControlCaps
    {
        /// <summary>
        /// 初始值，无任何播放能力。
        /// </summary>
        None = 0,

        /// <summary>
        /// 是否支持暂停。
        /// </summary>
        CanPause = 1,

        /// <summary>
        /// 是否支持进度定位。
        /// </summary>
        CanSeek = 2,

        /// <summary>
        /// 是否支持速度控制。
        /// </summary>
        CanSpeedControl = 4,

        /// <summary>
        /// 是否支持向前播放。
        /// <para>
        /// 通常是支持的，定义此能力是为了统一接口。
        /// </para>
        /// </summary>
        CanPlayForwards = 8,

        /// <summary>
        /// 是否支持向后播放（倒播）。
        /// </summary>
        CanPlayBackwards = 16,

        /// <summary>
        /// 是否支持前进到下一帧（帧进）。
        /// </summary>
        CanFrameForwards = 32,

        /// <summary>
        /// 是否支持后退到上一帧（帧退）。
        /// </summary>
        CanFrameBackwards = 64,

        /// <summary>
        /// 支持全部播放能力。
        /// </summary>
        Full = CanPause | CanSeek | CanSpeedControl | CanPlayForwards | CanPlayBackwards | CanFrameForwards | CanFrameBackwards,
    }
}
