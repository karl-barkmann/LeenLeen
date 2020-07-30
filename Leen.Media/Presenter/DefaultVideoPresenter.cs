using System;
using Leen.Media.Renderer;

namespace Leen.Media.Presenter
{
    /// <summary>
    /// 实现一个默认的视频呈现接口。
    /// </summary>
    public class DefaultVideoPresenter : IVideoPresenter, IDisposable
    {
        private volatile bool m_IsPresenting;
        private readonly IMediaPlayer r_MediaPlayer;
        private readonly IVideoRenderer r_VideoRenderer;
        private int m_Pitch;
        private readonly FrameFormat m_FrameFormat;
        private bool m_SurfaceReady;

        /// <summary>
        /// 构造 <see cref="DefaultVideoPresenter"/> 类的实例。
        /// </summary>
        /// <param name="frameFormat">视频帧编码格式。</param>
        /// <param name="mediaPlayer">媒体播放器。</param>
        /// <param name="renderer">视频渲染接口。</param>
        public DefaultVideoPresenter(FrameFormat frameFormat, IMediaPlayer mediaPlayer, IVideoRenderer renderer)
        {
            r_MediaPlayer = mediaPlayer ?? throw new ArgumentNullException(nameof(mediaPlayer));
            r_VideoRenderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            m_FrameFormat = frameFormat;
            r_MediaPlayer.StateChanged += MediaPlayer_StateChanged;
            r_MediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
            r_MediaPlayer.FrameAllocated += MediaPlayer_FrameAllocated;
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
        public IMediaPlayer Player
        {
            get
            {
                return r_MediaPlayer;
            }
        }

        /// <summary>
        /// 获取此视频呈现接口关联的视频渲染器。
        /// </summary>
        public IVideoRenderer Renderer
        {
            get
            {
                return r_VideoRenderer;
            }
        }

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
                    r_MediaPlayer.StateChanged -= MediaPlayer_StateChanged;
                    r_MediaPlayer.MediaFailed -= MediaPlayer_MediaFailed;
                    r_MediaPlayer.FrameAllocated -= MediaPlayer_FrameAllocated;
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
            }
        }

        private void MediaPlayer_FrameAllocated(object sender, FrameAllocatedEventArgs e)
        {
            if (!IsPresenting)
            {
                return;
            }

            if (!m_SurfaceReady || m_Pitch != e.Pitches[0])
            {
                m_Pitch = e.Pitches[0];
                r_VideoRenderer.SetupSurface(m_Pitch, e.PixelHeight, e.PixelWidth, e.PixelHeight, m_FrameFormat);
                m_SurfaceReady = true;
            }

            r_VideoRenderer.Render(e.Buffers);
        }
    }
}
