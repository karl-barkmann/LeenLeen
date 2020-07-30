namespace Leen.Media
{
    /// <summary>
    /// 定义客户端媒体播放中支持的视频帧格式。
    /// </summary>
    public enum FrameFormat
    {
        /// <summary>
        /// 12 bits per pixel planar format with Y plane followed by V and U planes，known as YUV420 planar.
        /// </summary>
        YV12 = 0,

        /// <summary>
        /// 12 bits per pixel planar format with Y plane and interleaved UV plane.
        /// </summary>
        NV12 = 1,

        /// <summary>
        /// 16 bits per pixel packed YUYV array.
        /// </summary>
        YUY2 = 2,

        /// <summary>
        /// 16 bits per pixel packed UYVY array.
        /// </summary>
        UYVY = 3,

        /// <summary>
        /// 16 bits per pixel with 5 bits for each RGB channel.
        /// </summary>
        RGB15 = 10,

        /// <summary>
        /// 16 bits per pixel with 5 bits Red, 6 bits Green, and 5 bits Blue.
        /// </summary>
        RGB16 = 11,

        /// <summary>
        /// 24 bits per pixel 8 bits for each RGB channel.
        /// </summary>
        RGB24 = 12,

        /// <summary>
        /// 32 bits per pixel with 8 bits unused and 8 bits for each RGB channel.
        /// </summary>
        RGB32 = 13,

        /// <summary>
        /// 32 bits per pixel with 8 bits Alpha and 8 bits for each RGB channel.
        /// </summary>
        ARGB32 = 14,

        /// <summary>
        /// 32 bits per pixel with 8 bits Alpha and 8 bits for each BGR channel.
        /// </summary>
        BGRA32 = 15,
    }
}
