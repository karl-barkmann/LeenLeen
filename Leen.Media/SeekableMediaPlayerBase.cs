using System;
using Leen.Media.Properties;

namespace Leen.Media
{
    /// <summary>
    /// 抽象封装支持回放的媒体播放器基类。
    /// </summary>
    public abstract class SeekableMediaPlayerBase : ZoomableMediaPlayerBase, ISeekableMediaPlayer
    {
        private TimeSpan m_CurrentPosition;
        private TimeSpan m_StopPosition;
        private MediaSpeedLevel m_CurrentSpeed;
        //备用字段
        private TimeSpan m_PreviousSeekingTime;
        private TimeSpan m_PreviousSeekingStopTime;
        private TimeSpan m_PreviousIncrementalTime;
        private TimeSpan m_previousIncrementalStopTime;

        /// <summary>
        /// 一个值指示当前媒体支持的最大快进速率。
        /// </summary>
        protected int m_MaximumSpeed = 5;

        /// <summary>
        /// 一个值指示当前媒体支持的最大慢进速率。
        /// </summary>
        protected int m_MinimumSpeed = -5;

        /// <summary>
        /// 一个值指示当前媒体的总时长。
        /// </summary>
        protected TimeSpan m_NaturalDuration;


        /// <summary>
        /// 获取一个值指示是否启用了片段播放。
        /// </summary>
        protected bool IsSectionPlayEnabled
        {
            get
            {
                return (StopPosition > TimeSpan.Zero && StopPosition > CurrentPosition) || CurrentPosition > TimeSpan.Zero;
            }
        }

        /// <summary>
        /// 获取或设置播放媒体的当前播放位置。
        /// </summary>
        public TimeSpan CurrentPosition
        {
            get
            {
                return m_CurrentPosition;
            }
            set
            {
                EnsureObjectAccess();
                if (value < TimeSpan.Zero)
                {
                    throw new ArgumentException(Resources.STR_ERR_PositionShouldBiggerThanZero, nameof(CurrentPosition));
                }

                if (m_CurrentPosition != value)
                {
                    if (IsMediaOpened && value >= TimeSpan.Zero && value <= m_NaturalDuration)
                    {
                        m_CurrentPosition = SeekCurrentPosition(value, MediaSeekingFlags.AbsolutePositioning);
                    }
                    else
                    {
                        m_CurrentPosition = value;
                        //当播放未打开时，设置停止位置将在播放打开时设置。
                        //因此此时需要记录此值。
                        m_PreviousSeekingTime = value;
                    }
                }
            }
        }

        /// <summary>
        /// 获取播放器允许的最大播放速度。
        /// </summary>
        public int MaximumSpeed
        {
            get
            {
                return m_MaximumSpeed;
            }
        }

        /// <summary>
        /// 获取播放器允许的最小播放速度。
        /// </summary>
        public int MinimumSpeed
        {
            get
            {
                return m_MinimumSpeed;
            }
        }

        /// <summary>
        /// 获取当前播放媒体的原始时长。
        /// </summary>
        public TimeSpan NaturalDuration
        {
            get
            {
                return m_NaturalDuration;
            }
        }

        /// <summary>
        /// 获取或设置媒体的播放速度倍级。
        /// <para>
        /// 以0表示正常速率，并以绝对值1的间隔为刻度。小于零时表示减慢，大于零时表示加快。
        /// </para>
        /// </summary>
        public MediaSpeedLevel CurrentSpeed
        {
            get
            {
                return m_CurrentSpeed;
            }
            set
            {
                EnsureObjectAccess();

                if (m_CurrentSpeed != value)
                {
                    m_CurrentSpeed = (MediaSpeedLevel)Math.Min(MaximumSpeed, (int)value);
                    m_CurrentSpeed = (MediaSpeedLevel)Math.Max(MinimumSpeed, (int)m_CurrentSpeed);

                    if (IsMediaOpened)
                    {
                        SetSpeedLevel(m_CurrentSpeed);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the position where playback stopped.
        /// </summary>
        public TimeSpan StopPosition
        {
            get
            {
                return m_StopPosition;
            }
            set
            {
                EnsureObjectAccess();
                if (m_StopPosition != value)
                {
                    TimeSpan target = value;
                    if (IsMediaOpened /*&& value >= CurrentPosition*/)
                    {
                        if (value > m_NaturalDuration)
                        {
                            target = m_NaturalDuration;
                        }
                        
                        m_StopPosition = SeekStopPostion(target, MediaSeekingFlags.AbsolutePositioning);
                    }
                    else
                    {
                        m_StopPosition = target;
                        //当播放未打开时，设置停止位置将在播放打开时设置。
                        //因此此时需要记录此值。
                        m_PreviousSeekingStopTime = value;
                    }
                }
            }
        }

        /// <summary>
        /// 当 <see cref="ISeekableMediaPlayer"/> 的播放进度改变时发生。
        /// </summary>
        public event PlayProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// 从当前播放位置开始倒播。
        /// </summary>
        public void PlayBackwards()
        {
            EnsureObjectAccess();
            EnsureMediaOpened();

            PlayBackwardsImpl();

            var oldState = m_CurrentState;

            InvokeMediaStateChanged(MediaPlayState.Rewinding);
        }

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
        public TimeSpan SeekCurrentPosition(TimeSpan time, MediaSeekingFlags seekingFlags)
        {
            EnsureObjectAccess();
            EnsureMediaOpened();

            TimeSpan target = m_CurrentPosition;
            switch (seekingFlags)
            {
                case MediaSeekingFlags.AbsolutePositioning:
                    target = time;
                    SetCurrentPosition(target);
                    break;
                case MediaSeekingFlags.RelativePositioning:
                    target += time;
                    SetCurrentPosition(target);
                    break;
                case MediaSeekingFlags.IncrementalPositioning:
                    m_PreviousIncrementalTime += time;
                    target += m_PreviousIncrementalTime;
                    SetCurrentPosition(target);
                    break;
                case MediaSeekingFlags.SeekToKeyFrame:
                    if (time > TimeSpan.Zero)
                    {
                        GoToNextFrame();
                    }
                    else
                    {
                        GoToPreviousFrame();
                    }
                    target = m_CurrentPosition;
                    InvokeMediaStateChanged(MediaPlayState.Paused);
                    break;
                case MediaSeekingFlags.NoPositioning:
                default:
                    break;
            }

            m_PreviousSeekingTime = target;

            return target;
        }

        /// <summary>
        /// Sets a new stop postion in playback.
        /// </summary>
        /// <param name="time">Position that is the new stop time for playback. </param>
        /// <param name="seekingFlags">The position seek type to use to set the new stop position.</param>
        /// <returns>Returns the new stop position.</returns>
        public TimeSpan SeekStopPostion(TimeSpan time, MediaSeekingFlags seekingFlags)
        {
            EnsureObjectAccess();

            switch (seekingFlags)
            {
                case MediaSeekingFlags.AbsolutePositioning:
                    m_StopPosition = time;
                    SetStopPosition(m_StopPosition);
                    break;
                case MediaSeekingFlags.RelativePositioning:
                    m_StopPosition += time;
                    SetStopPosition(m_StopPosition);
                    break;
                case MediaSeekingFlags.IncrementalPositioning:
                    m_previousIncrementalStopTime += time;
                    m_StopPosition += m_previousIncrementalStopTime;
                    SetStopPosition(m_StopPosition);
                    break;
                case MediaSeekingFlags.SeekToKeyFrame:
                case MediaSeekingFlags.NoPositioning:
                default:
                    break;
            }

            m_PreviousSeekingStopTime = m_StopPosition;
            return m_StopPosition;
        }

        /// <summary>
        /// 关闭播放，释放播放资源。
        /// </summary>
        protected override void CloseImpl()
        {
            m_CurrentPosition = TimeSpan.Zero;
            m_StopPosition = TimeSpan.Zero;
            m_CurrentSpeed = MediaSpeedLevel.Normal;
            m_PreviousSeekingTime = TimeSpan.Zero;
            m_PreviousSeekingStopTime = TimeSpan.Zero;
            m_previousIncrementalStopTime = TimeSpan.Zero;
            m_PreviousIncrementalTime = TimeSpan.Zero;
        }

        /// <summary>
        /// 当播放进度改变时调用，通知订阅者并更新当前播放进度信息。
        /// </summary>
        /// <param name="e">播放进度改变事件参数。</param>
        protected virtual void OnPlayProgressChanged(PlayProgressChangedEventArgs e)
        {
            m_CurrentPosition = e.CurrentPosition;
            m_NaturalDuration = e.NaturalDuration;

            ProgressChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 当 <see cref="CurrentSpeed"/> 发生改变时调用，继承类应重写该方法以实现正确的速率设置。
        /// </summary>
        /// <param name="level">最新的播放速率。以0.0F表示正常速率，并以绝对值1的间隔为刻度。小于零时表示减慢，大于零表示加快。</param>
        protected abstract void SetSpeedLevel(MediaSpeedLevel level);

        /// <summary>
        /// 向后播放（倒播）的内部实现，继承类应重写该方法以实现正确的倒播功能。
        /// </summary>
        protected abstract void PlayBackwardsImpl();

        /// <summary>
        /// 改变当前进度的内部实现，继承类应重写该方法以实现正确的进度控制。
        /// </summary>
        /// <param name="absolutePosition">距视频开始位置的时间间隔。</param>
        protected abstract void SetCurrentPosition(TimeSpan absolutePosition);

        /// <summary>
        /// 设置停止位置的内部实现，继承类应重写该方法以实现正确的片段播放。
        /// </summary>
        /// <param name="absolutePosition">距视频开始位置的时间间隔。</param>
        protected abstract void SetStopPosition(TimeSpan absolutePosition);

        /// <summary>
        /// 将媒体播放切换到下一帧的内部实现，继承类应重写该方法以实现正确的帧进功能。
        /// </summary>
        protected abstract void GoToNextFrame();

        /// <summary>
        /// 将媒体播放切换到上一帧的内部实现，继承类应重写该方法以实现正确的帧退功能。
        /// </summary>
        protected abstract void GoToPreviousFrame();

        /// <summary>
        /// 获取视频原始时长的内部实现。
        /// </summary>
        /// <returns></returns>
        protected virtual TimeSpan GetNaturalDuration()
        {
            return TimeSpan.Zero;
        }
    }
}
