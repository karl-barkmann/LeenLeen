using System;

namespace Leen.Media
{
    /// <summary>
    /// 抽象封装支持画面缩放的媒体播放器基类。
    /// </summary>
    public abstract class ZoomableMediaPlayerBase : MediaPlayerBase, IZoomableMediaPlayer
    {
        protected float m_MaxZoomFactor;
        protected float m_MinZoomFactor;
        protected float m_ZoomFactor;

        /// <summary>
        /// 获取允许的最大缩放比例。
        /// </summary>
        public float MaxZoomFactor
        {
            get
            {
                return m_MaxZoomFactor;
            }
        }

        /// <summary>
        /// 获取允许的最小缩放比例。
        /// </summary>
        public float MinZoomFactor
        {
            get
            {
                return m_MinZoomFactor;
            }
        }

        /// <summary>
        /// 获取当前缩放比例。
        /// </summary>
        public float ZoomFactor
        {
            get
            {
                return m_ZoomFactor;
            }
        }

        /// <summary>
        /// 根据指定参数缩放视频画面区域。
        /// </summary>
        /// <param name="x">缩放的视频区域左上角X轴坐标。</param>
        /// <param name="y">缩放的视频区域左上角Y轴坐标。</param>
        /// <param name="width">缩放的视频区域宽度。</param>
        /// <param name="height">缩放的视频区域高度。</param>
        /// <param name="zoomFactor">缩放比例。</param>
        public void Zoom(int x, int y, int width, int height, float zoomFactor)
        {
            EnsureObjectAccess();
            EnsureMediaOpened();

            if (x < 0 || y < 0)
            {
                throw new ArgumentException("缩放的视频区域坐标不应小于0");
            }

            if (width < 0 || height < 0)
            {
                throw new ArgumentException("缩放的视频区域的宽度或高度不应小于0");
            }

            ZoomImpl(x, y, width, height);
        }

        /// <summary>
        /// 重写该方法以正确实现播放画面的缩放。
        /// </summary>
        /// <param name="x">缩放的视频区域左上角X轴坐标。</param>
        /// <param name="y">缩放的视频区域左上角Y轴坐标。</param>
        /// <param name="width">缩放的视频区域宽度。</param>
        /// <param name="height">缩放的视频区域高度。</param>
        protected virtual void ZoomImpl(int x, int y, int width, int height)
        {

        }
    }
}
