using System;
using System.ComponentModel;

namespace Leen.Media
{
    /// <summary>
    /// 为 <see cref="ISeekableMediaPlayer.ProgressChanged"/> 事件提供数据。
    /// </summary>
    public class PlayProgressChangedEventArgs : ProgressChangedEventArgs
    {
        /// <summary>
        /// 构造 <see cref="PlayProgressChangedEventArgs"/> 类的实例。
        /// </summary>
        /// <param name="progressPercentage">表示播放进度的百分比。</param>
        /// <param name="currentPosition">表示发生事件后的播放进度。</param>
        /// <param name="naturalDuration">自播放开始时间的时间间隔。</param>
        public PlayProgressChangedEventArgs(TimeSpan currentPosition, float progressPercentage, TimeSpan naturalDuration) : base((int)progressPercentage, null)
        {
            ProgressPercentage = progressPercentage;
            CurrentPosition = currentPosition;
            NaturalDuration = naturalDuration;
        }

        /// <summary>
        /// 获取当前播放的进度百分比。
        /// </summary>
        public new float ProgressPercentage { get; private set; }

        /// <summary>
        /// 获取 <see cref="IMediaPlayer"/> 进行发生变化时相对于开始时间的时间间隔。
        /// </summary>
        public TimeSpan CurrentPosition { get; private set; }

        /// <summary>
        /// 获取 <see cref="IMediaPlayer"/> 自播放开始时间的时间间隔。
        /// </summary>
        public TimeSpan NaturalDuration { get; private set; }
    }
}
