using System;

namespace Leen.Media
{
    /// <summary>
    /// 为 <see cref="IMediaPlayer.StateChanged"/> 事件提供数据。
    /// </summary>
    public class PlayStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 构造 <see cref="PlayStateChangedEventArgs"/> 类的实例。
        /// </summary>
        /// <param name="oldState">前一播放状态。</param>
        /// <param name="newState">当前播放状态。</param>
        public PlayStateChangedEventArgs(MediaPlayState oldState, MediaPlayState newState) : base()
        {
            OldState = oldState;
            NewState = newState;
        }

        /// <summary>
        ///  表示 <see cref="IMediaPlayer"/> 发生事件前的播放状态。
        /// </summary>
        public MediaPlayState OldState { get; private set; }

        /// <summary>
        /// 表示 <see cref="IMediaPlayer"/> 发生事件后的播放状态。
        /// </summary>
        public MediaPlayState NewState { get; private set; }
    }
}
