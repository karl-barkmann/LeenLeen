namespace Leen.Media
{
    /// <summary>
    /// 定义客户端产品中的支持画面缩放的媒体播放器。
    /// </summary>
    public interface IZoomableMediaPlayer : IMediaPlayer
    {
        /// <summary>
        /// 获取允许的最大缩放比例。
        /// </summary>
        float MaxZoomFactor { get; }

        /// <summary>
        /// 获取允许的最小缩放比例。
        /// </summary>
        float MinZoomFactor { get; }

        /// <summary>
        /// 获取当前缩放比例。
        /// </summary>
        float ZoomFactor { get; }

        /// <summary>
        /// 使用指定的缩放比例对视频指定区域进行缩放并在当前媒体播放器中进行播放。
        /// </summary>
        /// <param name="x">指定视频区域的原点x轴坐标（相对于视频区域左上角原点）。</param>
        /// <param name="y">指定视频区域的原点y轴坐标（相对于视频区域左上角原点）。</param>
        /// <param name="width">指定视频区域的宽度。</param>
        /// <param name="height">指定视频区域的高度。</param>
        /// <param name="zoomFactor">最新缩放比例。</param>
        void Zoom(int x, int y, int width, int height, float zoomFactor);
    }
}
