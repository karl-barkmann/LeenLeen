using System;
using System.Windows.Documents;
using Leen.Media.Renderer;

namespace Leen.Media.Presenter
{
    /// <summary>
    /// 实现一个默认的视频呈现接口。
    /// </summary>
    public class DefaultVideoPresenter : IVideoPresenter, IDisposable
    {
        private volatile bool m_IsPresenting;
        private int m_Pitch;
        private readonly FrameFormat m_FrameFormat;
        private bool m_SurfaceReady;
        private bool _mediaClosed;

        /// <summary>
        /// 构造 <see cref="DefaultVideoPresenter"/> 类的实例。
        /// </summary>
        /// <param name="frameFormat">视频帧编码格式。</param>
        /// <param name="mediaPlayer">媒体播放器。</param>
        /// <param name="renderer">视频渲染接口。</param>
        public DefaultVideoPresenter(FrameFormat frameFormat, IMediaPlayer mediaPlayer, IVideoRenderer renderer)
        {
            Player = mediaPlayer ?? throw new ArgumentNullException(nameof(mediaPlayer));
            Renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            m_FrameFormat = frameFormat;
            Player.StateChanged += MediaPlayer_StateChanged;
            Player.MediaFailed += MediaPlayer_MediaFailed;
            Player.FrameAllocated += MediaPlayer_FrameAllocated;
        }

        /// <summary>
        /// 获取或设置一个值用于切换视频呈现。
        /// </summary>
        public bool IsPresenting
        {
            get
            {
                return m_IsPresenting;
            }
            set
            {
                m_IsPresenting = value;
            }
        }

        /// <summary>
        /// 获取此视频呈现接口关联的媒体播放器。
        /// </summary>
        public IMediaPlayer Player { get; }

        /// <summary>
        /// 获取此视频呈现接口关联的视频渲染器。
        /// </summary>
        public IVideoRenderer Renderer { get; }

        #region IDisposable Support

        private bool m_Disposed = false;

        /// <summary>
        /// 释放此媒体呈现接口关联的资源.
        /// </summary>
        /// <param name="disposing">指示是否正在通过 <see cref="Dispose()"/>进行释放。</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    Player.StateChanged -= MediaPlayer_StateChanged;
                    Player.MediaFailed -= MediaPlayer_MediaFailed;
                    Player.FrameAllocated -= MediaPlayer_FrameAllocated;
                }

                m_Disposed = true;
            }
        }

        /// <summary>
        /// 释放此媒体呈现接口关联的资源.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        private void MediaPlayer_MediaFailed(object sender, MediaFailedEventArgs e)
        {
            m_SurfaceReady = false;
        }

        private void MediaPlayer_StateChanged(object sender, PlayStateChangedEventArgs e)
        {
            if (e.NewState == MediaPlayState.None)
            {
                m_SurfaceReady = false;
                _mediaClosed = true;
            }
            else
            {
                _mediaClosed = false;
            }
        }

        private void MediaPlayer_FrameAllocated(object sender, FrameAllocatedEventArgs e)
        {
            if (!IsPresenting || _mediaClosed)
            {
                return;
            }

            if (!m_SurfaceReady || m_Pitch != e.Pitches[0])
            {
                m_Pitch = e.Pitches[0];
                Renderer.SetupSurface(m_Pitch, e.PixelHeight, e.PixelWidth, e.PixelHeight, m_FrameFormat);
                m_SurfaceReady = true;
            }

            Renderer.Render(e.Buffers);
        }
    }
}
