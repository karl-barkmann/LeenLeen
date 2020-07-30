using System;

namespace Leen.Media.Renderer
{
    /// <summary>
    /// 表示客户端产品中的媒体播放渲染器。
    /// </summary>
    public interface IVideoRenderer : IDisposable
    {
        /// <summary>
        /// 当后台缓冲发生变化时引发。
        /// </summary>
        event EventHandler IsBackBufferAvailableChanged;

        /// <summary>
        /// 当后台缓冲刷新时引发。
        /// </summary>
        event EventHandler BackBufferRefreshed;

        /// <summary>
        /// 获取渲染交换链的后台缓冲。
        /// </summary>
        IntPtr BackBuffer { get; }

        /// <summary>
        /// 确认该渲染器是否支持指定视频格式。
        /// </summary>
        /// <param name="format">需要检测的视频帧格式。</param>
        /// <returns></returns>
        /// <exception cref="MediaDeviceUnavailableException">设备不可用。</exception>
        bool CheckFormat(FrameFormat format);

        /// <summary>
        /// 使用指定视频参数初始化渲染缓冲表面。
        /// </summary>
        /// <param name="surfaceWidth">缓冲表面宽度。</param>
        /// <param name="surfaceHeight">缓冲表面高度。</param>
        /// <param name="pixelWidth">视频像素宽度。</param>
        /// <param name="pixelHeight">视频像素高度。</param>
        /// <param name="format">视频帧采样格式。</param>
        /// <returns></returns>
        /// <exception cref="MediaDeviceUnavailableException">设备不可用。</exception>
        bool SetupSurface(int surfaceWidth, int surfaceHeight, int pixelWidth, int pixelHeight, FrameFormat format);

        /// <summary>
        /// 渲染一帧图像。
        /// </summary>
        /// <param name="buffers">帧数据缓冲，以不同的帧格式对应不同的帧数据数组。</param>
        /// <exception cref="InvalidOperationException">设备或缓冲表面未初始化。</exception>
        void Render(IntPtr[] buffers);

        /// <summary>
        /// 清除渲染表面。
        /// </summary>
        void ClearSurface();
    }
}
