using System;

namespace Leen.Media
{
    /// <summary>
    /// 表示 <see cref="ISeekableMediaPlayer"/> 正在进行的播放进度发生变化时的处理方法。
    /// </summary>
    /// <param name="sender"><see cref="IMediaPlayer"/> 的实例。</param>
    /// <param name="e">表示事件参数。</param>
    public delegate void PlayProgressChangedEventHandler(object sender, PlayProgressChangedEventArgs e);

    /// <summary>
    /// 定义客户端产品中的支持多种播放控制的媒体播放器。
    /// </summary>
    public interface ISeekableMediaPlayer : IZoomableMediaPlayer
    {
        /// <summary>
        /// 当 <see cref="ISeekableMediaPlayer"/> 的播放进度改变时发生。
        /// </summary>
        event PlayProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// 获取当前播放媒体的原始时长。
        /// </summary>
        TimeSpan NaturalDuration { get; }

        /// <summary>
        /// 获取或设置播放媒体的当前播放位置。
        /// </summary>
        TimeSpan CurrentPosition { get; set; }

        /// <summary>
        /// Retrieves the position where playback stopped.
        /// </summary>
        TimeSpan StopPosition { get; set; }

        /// <summary>
        /// 获取或设置媒体的播放速度倍级。
        /// <para>
        /// 以0表示正常速率，并以绝对值1的间隔为刻度。小于零时表示减慢，大于零时表示加快。
        /// </para>
        /// </summary>
        MediaSpeedLevel CurrentSpeed { get; set; }

        /// <summary>
        /// 获取播放器允许的最大播放速度倍级。
        /// </summary>
        int MinimumSpeed { get; }

        /// <summary>
        /// 获取播放器允许的最小播放速度倍级。
        /// </summary>
        int MaximumSpeed { get; }

        /// <summary>
        /// 从当前播放位置开始倒播。
        /// </summary>
        void PlayBackwards();

        /// <summary>
        /// 恢复正常播放(重置播放速度及播放方向)。
        /// </summary>
        void Resume();

        /// <summary>
        /// Seeks to a specific playback postion.
        /// </summary>
        /// <param name="time">Position to seek.
        /// This parameter is ignored when <paramref name="seekingFlags"/> is <see cref="MediaSeekingFlags.NoPositioning"/>. 
        /// When <paramref name="seekingFlags"/> is <see cref="MediaSeekingFlags.SeekToKeyFrame"/> , <paramref name="time"/> is bigger than
        /// <see cref="TimeSpan.Zero"/> means seek to next frame.
        /// </param>
        /// <param name="seekingFlags">The position seek type to use.</param>
        /// <returns>Returns the position after the seek.</returns>
        TimeSpan SeekCurrentPosition(TimeSpan time, MediaSeekingFlags seekingFlags);

        /// <summary>
        /// Sets a new stop postion in playback.
        /// </summary>
        /// <param name="time">Position that is the new stop time for playback. </param>
        /// <param name="seekingFlags">The position seek type to use to set the new stop position.</param>
        /// <returns>Returns the new stop position.</returns>
        TimeSpan SeekStopPostion(TimeSpan time, MediaSeekingFlags seekingFlags);
    }
}
