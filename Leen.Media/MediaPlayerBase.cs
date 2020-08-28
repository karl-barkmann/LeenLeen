using System;
using System.IO;
using System.Threading.Tasks;
using Leen.Media.Properties;

namespace Leen.Media
{
    /// <summary>
    /// 抽象封装媒体播放器基类。
    /// <para>
    /// 用户使用具体的继承类来完成媒体播放。
    /// </para>
    /// </summary>
    public abstract class MediaPlayerBase : IMediaPlayer
    {
        private double m_Balance;
        private double m_Volume;
        private bool m_IsMuted;
        private MediaControlCaps _mediaControlsCaps;
        private bool m_MediaOpened;
        private bool m_IsMediaOpening;
        private object m_MediaSource;
       
        /// <summary>
        /// 一个值表示当前媒体是否有音频输出。
        /// </summary>
        protected bool m_HasAudio;

        /// <summary>
        /// 一个值表示当前媒体是否有视频输出。
        /// </summary>
        protected bool m_HasVideo;

        /// <summary>
        /// 一个值表示当前媒体的视频像素宽度。
        /// </summary>
        protected int m_NaturalVideoWidth;

        /// <summary>
        /// 一个值表示当前媒体的视频像素高度。
        /// </summary>
        protected int m_NaturalVideoHeight;

        /// <summary>
        /// 一个值表示当前媒体的视频帧率。
        /// </summary>
        protected float m_FrameRate;

        /// <summary>
        /// 一个值表示当前媒体的视频码率。
        /// </summary>
        protected float m_BitRate;

        /// <summary>
        /// 当前媒体播放状态。
        /// </summary>
        protected MediaPlayState m_CurrentState;

        /// <summary>
        /// Gets or sets the balance on the audio.
        /// </summary>
        public double Balance
        {
            get
            {
                return m_Balance;
            }

            set
            {
                EnsureObjectAccess();

                if (m_Balance != value)
                {
                    m_Balance = value;
                    OnBalanceChanged(value);
                }
            }
        }

        /// <summary>
        /// 获取当前媒体播放器的播控能力。
        /// </summary>
        public MediaControlCaps ControlCaps
        {
            get
            {
                return _mediaControlsCaps;
            }
        }

        /// <summary>
        /// If media has audio content.
        /// </summary>
        public bool HasAudio
        {
            get
            {
                return m_HasAudio;
            }
        }

        /// <summary>
        /// If the media has video content.
        /// </summary>
        public bool HasVideo
        {
            get
            {
                return m_HasVideo;
            }
        }

        /// <summary>
        /// Returns the whether the given media is muted and sets whether it is.
        /// </summary>
        public bool IsMuted
        {
            get
            {
                return m_IsMuted;
            }

            set
            {
                EnsureObjectAccess();

                if (m_IsMuted != value)
                {
                    m_IsMuted = value;
                    OnVolumeChanged(0);
                }
            }
        }

        /// <summary>
        /// 获取一个值指示当前媒体播放器是否已关闭媒体。
        /// </summary>
        public bool IsMediaClosed
        {
            get { return !m_MediaOpened && !m_IsMediaOpening; }
        }

        /// <summary>
        /// 获取一个值指示媒体是否已打开。
        /// </summary>
        public bool IsMediaOpened
        {
            get { return m_MediaOpened; }
        }

        /// <summary>
        /// 获取一个值指示媒体是否正在打开。
        /// </summary>
        public bool IsMediaOpening
        {
            get => m_IsMediaOpening;
        }

        /// <summary>
        /// Gets the natural pixel height of the current media.  
        /// The value will be 0 if there is no video in the media.
        /// </summary>
        public int NaturalVideoHeight
        {
            get
            {
                return m_NaturalVideoHeight;
            }
        }

        /// <summary>
        /// Gets the natural pixel width of the current media.
        /// The value will be 0 if there is no video in the media.
        /// </summary>
        public int NaturalVideoWidth
        {
            get
            {
                return m_NaturalVideoWidth;
            }
        }

        /// <summary>
        /// 获取视频帧率。
        /// </summary>
        public float FrameRate
        {
            get
            {
                return m_FrameRate;
            }
        }

        /// <summary>
        /// 获取视频码率(kbps)。
        /// </summary>
        public float BitRate
        {
            get
            {
                return m_BitRate;
            }
        }

        /// <summary>
        /// 获取或设置一个值指示是否持续解码最后一帧画面。
        /// </summary>
        public bool RequireLastFrame { get; set; }

        /// <summary>
        /// 获取媒体播放器的当前播放状态。
        /// </summary>
        public MediaPlayState PlayState
        {
            get
            {
                return m_CurrentState;
            }
        }

        /// <summary>
        /// 获取正在播放的媒体播放源。
        /// </summary>
        public object Source
        {
            get
            {
                return m_MediaSource;
            }
        }

        /// <summary>
        /// Gets or sets the audio volume.  Specifies the volume, as a 
        /// number from 0 to 1.  Full volume is 1, and 0 is silence.
        /// </summary>
        public double Volume
        {
            get
            {
                return m_Volume;
            }

            set
            {
                EnsureObjectAccess();

                if (m_Volume != value)
                {
                    m_Volume = value;
                    if (value <= 0)
                    {
                        IsMuted = true;
                    }
                    else
                    {
                        OnVolumeChanged(value);
                    }
                }
            }
        }

        /// <summary>
        /// 当 <see cref="IMediaPlayer"/> 的视频帧解码数据刷新时。
        /// </summary>
        public event FrameAllocatedEventHandler FrameAllocated;

        /// <summary>
        /// 当 <see cref="IMediaPlayer"/> 的播放接口发生错误时发生。
        /// </summary>
        public event EventHandler<MediaFailedEventArgs> MediaFailed;

        /// <summary>
        /// 当 <see cref="IMediaPlayer"/> 的播放状态改变时发生。
        /// </summary>
        public event PlayStateChangedEventHandler StateChanged;

        /// <summary>
        /// 截取当前帧并生成图片文件。
        /// </summary>
        /// <param name="fileName">截取成功后保存为图片的文件名。</param>
        /// <returns></returns>
        public bool TakeSnapshot(out string fileName)
        {
            return TakeSnapshotImpl(out fileName);
        }

        /// <summary>
        /// 截取当前帧并保存为图片文件到指定位置下。
        /// </summary>
        /// <param name="fileName">
        /// 保存的图片文件的文件名称, 若文件名为空则将保存到临时目录。
        /// </param>
        /// <param name="createFileIfNotExists">当文件名不存在时是否创建文件。</param>
        /// <returns>截取成功后保存为图片的文件名。</returns>
        public bool TakeSnapshot(string fileName, bool createFileIfNotExists)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Path.GetTempFileName();
                fileName = Path.ChangeExtension(fileName, "png");
            }

            return TakeSnapshotImpl(fileName, createFileIfNotExists);
        }

        /// <summary>
        /// Closes the underlying media. This de-allocates all of the native resources in
        /// the media. The mediaplayer can be opened again by calling the Open method.
        /// </summary>
        public void Close()
        {
            EnsureObjectAccess();
            EnsureMediaOpened();
            try
            {
                CloseImpl();

                m_Balance = 0;
                m_BitRate = 0;
                m_FrameRate = 0;
                m_HasAudio = false;
                m_HasVideo = false;
                m_IsMuted = false;
                m_NaturalVideoHeight = 0;
                m_NaturalVideoWidth = 0;
                m_Volume = 0;
                m_MediaSource = null;
            }
            finally
            {
                InvokeMediaStateChanged(MediaPlayState.None);
                m_IsMediaOpening = false;
                m_MediaOpened = false;
            }
        }

        /// <summary>
        /// 打开播放媒体，初始化播放资源，获取视频分辨率大小、帧率、码率等信息。
        /// </summary>
        /// <remarks>
        /// 在调用此方法前播放器不可用。
        /// </remarks>
        /// <param name="source">需要播放的媒体源。</param>
        public void Open(object source)
        {
            EnsureObjectAccess();

            if (m_MediaOpened || m_IsMediaOpening)
            {
                throw new InvalidOperationException(Resources.STR_ERR_MediaAlreadyOpened);
            }

            m_MediaSource = source ?? throw new ArgumentNullException(nameof(source));

            bool mediaOpened;
            try
            {
                m_IsMediaOpening = true;
                mediaOpened = OpenImpl(source);
                m_IsMediaOpening = false;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                m_IsMediaOpening = false;
                OnMediaFailed(ex);
                return;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            if (mediaOpened)
            {
                m_MediaOpened = mediaOpened;
                InvokeMediaStateChanged(MediaPlayState.Opened);
            }
        }

        /// <summary>
        /// 异步打开播放媒体，初始化播放资源，获取视频分辨率大小、帧率、码率等信息。
        /// </summary>
        /// <remarks>
        /// 在调用此方法前播放器不可用。
        /// </remarks>
        /// <param name="source">需要播放的媒体源。</param>
        /// <returns>返回此方法的异步任务对象。</returns>
        public async Task OpenAsync(object source)
        {
            EnsureObjectAccess();

            if (m_MediaOpened || m_IsMediaOpening)
            {
                throw new InvalidOperationException(Resources.STR_ERR_MediaAlreadyOpened);
            }

            m_MediaSource = source ?? throw new ArgumentNullException(nameof(source));
            bool mediaOpened;
            try
            {
                m_IsMediaOpening = true;
                mediaOpened = await OpenImplAsync(source).ConfigureAwait(false);
                m_IsMediaOpening = false;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                m_IsMediaOpening = false;
                OnMediaFailed(ex);
                return;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            if (mediaOpened)
            {
                m_MediaOpened = mediaOpened;
                InvokeMediaStateChanged(MediaPlayState.Opened);
            }
        }

        /// <summary>
        /// 获取打开媒体支持的播控能力。
        /// </summary>
        /// <returns></returns>
        protected virtual MediaControlCaps GetMediaControlsCaps()
        {
            return MediaControlCaps.Full;
        }

        /// <summary>
        /// Halt play at current position. 
        /// </summary>
        public void Pause()
        {
            EnsureObjectAccess();
            EnsureMediaOpened();
            PauseImpl();
            InvokeMediaStateChanged(MediaPlayState.Paused);
        }

        /// <summary>
        /// 恢复正常播放(重置播放速度及播放方向)。
        /// </summary>
        public void Resume()
        {
            EnsureObjectAccess();
            EnsureMediaOpened();
            ResumeImpl();
            InvokeMediaStateChanged(MediaPlayState.Playing);
        }

        /// <summary>
        /// 开始播放或从前一暂停状态继续播放。
        /// </summary>
        public void Play()
        {
            EnsureObjectAccess();
            EnsureMediaOpened();
            PlayImpl();
            InvokeMediaStateChanged(MediaPlayState.Playing);
        }

        /// <summary>
        /// Halt play. 
        /// </summary>
        /// <remarks>
        /// Stopping playback will cause the position to be reset to the beginning.
        /// </remarks>
        public void Stop()
        {
            EnsureObjectAccess();
            EnsureMediaOpened();
            StopImpl();
            InvokeMediaStateChanged(MediaPlayState.Stopped);
        }

        #region IDisposable Support

        private bool m_Disposed = false;

        /// <summary>
        /// 回收此播放器内关联的资源。
        /// </summary>
        /// <param name="disposing">指示是否正在通过调用 <see cref="IDisposable.Dispose"/> 进行资源回收。</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    CloseImpl();
                    try
                    {
                        InvokeMediaStateChanged(MediaPlayState.None);
                    }
                    catch (Exception)
                    {
                        //swallow exceptions while disposing
                    }
                }

                m_Disposed = true;
            }
        }

        /// <summary>
        /// 释放由此实例引用的资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Virtual Functions

        /// <summary>
        /// 当音量平衡发生改变时。
        /// </summary>
        /// <param name="newValue">当前的音量平衡值。</param>
        protected virtual void OnBalanceChanged(double newValue)
        {

        }

        /// <summary>
        /// 当音量发生改变时。
        /// </summary>
        /// <param name="newValue">当前的音量值，当该值小于等于0时是为静音。</param>
        /// <remarks>
        /// 当该值小于等于0时是为静音。
        /// </remarks>
        protected virtual void OnVolumeChanged(double newValue)
        {

        }

        /// <summary>
        /// 打开播放媒体的具体实现。
        /// <para>
        /// 子类应确保在打开完成时可获取播放信息，例如时长、分辨率等静态信息。
        /// </para>
        /// </summary>
        /// <returns>返回值表示媒体是否已成功打开。</returns>
        protected abstract bool OpenImpl(object source);

        /// <summary>
        /// 异步打开播放媒体的具体实现。
        /// <para>
        /// 子类应确保在打开完成时可获取播放信息，例如时长、分辨率等静态信息。
        /// </para>
        /// </summary>
        /// <returns>返回值表示媒体是否已成功打开的异步任务对象。</returns>
        protected abstract Task<bool> OpenImplAsync(object source);

        /// <summary>
        /// 截取当前帧并保存为图片文件。
        /// </summary>
        /// <param name="fileName">截取成功后保存为图片的文件名。</param>
        /// <returns></returns>
        protected virtual bool TakeSnapshotImpl(out string fileName)
        {
            fileName = Path.GetTempFileName();
            fileName = Path.ChangeExtension(fileName, "png");

            return TakeSnapshotImpl(fileName, false);
        }

        /// <summary>
        /// 截取当前帧并保存为图片文件到指定位置下。
        /// </summary>
        /// <param name="fileName">保存的图片文件的文件名称。</param>
        /// <param name="createFileIfNotExists">当文件名不存在时是否创建文件。</param>
        /// <returns></returns>
        protected abstract bool TakeSnapshotImpl(string fileName, bool createFileIfNotExists);

        /// <summary>
        /// 暂停播放或暂停倒播的具体实现。
        /// </summary>
        protected abstract void PauseImpl();

        /// <summary>
        /// 恢复到正常状态下播放。
        /// </summary>
        protected abstract void ResumeImpl();

        /// <summary>
        /// 停止播放。
        /// </summary>
        protected abstract void StopImpl();

        /// <summary>
        /// 开始播放或从暂停前的状态继续播放。
        /// </summary>
        protected abstract void PlayImpl();

        /// <summary>
        /// 关闭播放，释放播放资源。
        /// </summary>
        protected abstract void CloseImpl();

        /// <summary>
        /// 当尝试播放媒体发生错误时调用。
        /// </summary>
        /// <param name="error">尝试播放媒体发生错误对象。</param>
        protected virtual void OnMediaFailed(Exception error)
        {
            if (m_IsMediaOpening)
            {
                m_MediaOpened = false;
            }
            m_IsMediaOpening = false;
            MediaFailed?.Invoke(this, new MediaFailedEventArgs(error));
        }

        /// <summary>
        /// 当媒体播放状态发生时调用。
        /// <para>
        /// 继承类通常应通过调用 <see cref="InvokeMediaStateChanged(MediaPlayState)"/> 来触发播放状态改变，
        /// 因为该方法会判断最新状态是否与前一状态相同，若不同才通知事件订阅者并更新<see cref="m_CurrentState"/>。
        /// 如果继承类选择不调用 <see cref="InvokeMediaStateChanged(MediaPlayState)"/>方法则应由其实现该逻辑。
        /// </para>
        /// </summary>
        /// <param name="oldState">前一状态。</param>
        /// <param name="newState">最新状态。</param>
        protected virtual void OnMediaStateChanged(MediaPlayState oldState, MediaPlayState newState)
        {
            StateChanged?.Invoke(this, new PlayStateChangedEventArgs(oldState, newState));
        }

        /// <summary>
        /// 当媒体视频帧数据刷新时调用。
        /// </summary>
        /// <param name="e">视频帧刷新事件参数。</param>
        protected virtual void OnFrameAllocated(FrameAllocatedEventArgs e)
        {
            FrameAllocated?.Invoke(this, e);
        }

        #endregion

        /// <summary>
        /// 尝试通知订阅者最新状态并更新播放器状态。
        /// </summary>
        /// <param name="newState">媒体最新状态。</param>
        protected void InvokeMediaStateChanged(MediaPlayState newState)
        {
            if(m_CurrentState == newState)
            {
                return;
            }

            if (newState == MediaPlayState.Opened)
            {
                m_IsMediaOpening = false;
                m_MediaOpened = true;
                _mediaControlsCaps = GetMediaControlsCaps();
            }

            var oldState = m_CurrentState;
            m_CurrentState = newState;

            OnMediaStateChanged(oldState, m_CurrentState);
        }

        /// <summary>
        /// 确保媒体已打开，若未打开则引发 <see cref="InvalidOperationException"/>异常。
        /// </summary>
        protected void EnsureMediaOpened()
        {
            if (!m_MediaOpened && !m_IsMediaOpening)
            {
                throw new InvalidOperationException(Resources.STR_ERR_MediaNotOpen);
            }
        }

        /// <summary>
        /// 确保对象未释放，若未打开则引发 <see cref="ObjectDisposedException"/>异常。
        /// </summary>
        protected void EnsureObjectAccess()
        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException(GetType().Name, Resources.STR_ERR_MediaDisposed);
            }
        }
    }
}
