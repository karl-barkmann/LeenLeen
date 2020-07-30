using System;

namespace Leen.Media
{
    /// <summary>
    /// 为 <see cref="IMediaPlayer.FrameAllocated"/> 事件提供事件参数。
    /// </summary>
    public class FrameAllocatedEventArgs : EventArgs
    {
        /// <summary>
        /// 构造 <see cref="FrameAllocatedEventArgs"/> 类的实例。
        /// </summary>
        /// <param name="buffers">视频帧数据缓冲。</param>
        /// <param name="pitches">视频帧数据缓冲对应的宽度。</param>
        /// <param name="pixelHeight">视频像素高度。</param>
        /// <param name="pixelWidth">视频像素宽度。</param>
        public FrameAllocatedEventArgs(IntPtr[] buffers, int[] pitches, int pixelWidth, int pixelHeight) : base()
        {
            Buffers = buffers;
            Pitches = pitches;
            PixelHeight = pixelHeight;
            PixelWidth = pixelWidth;
        }

        /// <summary>
        /// 获取视频帧数据缓冲。
        /// </summary>
        public IntPtr[] Buffers { get; private set; }

        /// <summary>
        /// 获取视频像素高度。
        /// </summary>
        public int PixelHeight { get; private set; }

        /// <summary>
        /// 获取视频像素宽度。
        /// </summary>
        public int PixelWidth { get; private set; }

        /// <summary>
        /// 获取视频帧数据缓冲对应的宽度。
        /// </summary>
        public int[] Pitches { get; private set; }
    }
}
