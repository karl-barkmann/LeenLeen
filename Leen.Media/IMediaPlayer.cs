using System;
using System.Threading.Tasks;

namespace Leen.Media
{
    /// <summary>
    /// 表示 <see cref="IMediaPlayer"/> 正在进行的播放状态发生变化的处理方法。
    /// </summary>
    /// <param name="sender"><see cref="IMediaPlayer"/> 的实例。</param>
    /// <param name="e">表示事件参数。</param>
    public delegate void PlayStateChangedEventHandler(object sender, PlayStateChangedEventArgs e);

    /// <summary>
    /// 表示 <see cref="IMediaPlayer"/> 视频帧解码数据刷新时的处理方法。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FrameAllocatedEventHandler(object sender, FrameAllocatedEventArgs e);

    /// <summary>
    /// 定义客户端产品中的通用媒体播放器。
    /// </summary>
    public interface IMediaPlayer : IDisposable
    {
        /// <summary>
        /// 当 <see cref="IMediaPlayer"/> 的播放状态改变时发生。
        /// </summary>
        event PlayStateChangedEventHandler StateChanged;

        /// <summary>
        /// 当 <see cref="IMediaPlayer"/> 的视频帧解码数据刷新时。
        /// </summary>
        event FrameAllocatedEventHandler FrameAllocated;

        /// <summary>
        /// 当 <see cref="IMediaPlayer"/> 的播放接口发生错误时发生。
        /// </summary>
        event EventHandler<MediaFailedEventArgs> MediaFailed;

        /// <summary>
        /// 获取媒体播放器的当前播放状态。
        /// </summary>
        MediaPlayState PlayState { get; }

        /// <summary>
        /// 获取当前媒体播放器的播控能力。
        /// </summary>
        MediaControlCaps ControlCaps { get; }

        /// <summary>
        /// If the media has video content.
        /// </summary>
        bool HasVideo { get; }

        /// <summary>
        /// If media has audio content.
        /// </summary>
        bool HasAudio { get; }

        /// <summary>
        /// Returns the whether the given media is muted and sets whether it is.
        /// </summary>
        bool IsMuted { get; set; }

        /// <summary>
        /// 获取一个值指示当前媒体播放器是否已关闭媒体。
        /// </summary>
        bool IsMediaClosed { get; }

        /// <summary>
        /// 获取一个值指示媒体是否已打开。
        /// </summary>
        bool IsMediaOpened { get; }

        /// <summary>
        /// 获取一个值指示媒体是否正在打开。
        /// </summary>
        bool IsMediaOpening { get; }

        /// <summary>
        /// Gets the natural pixel width of the current media.
        /// The value will be 0 if there is no video in the media.
        /// </summary>
        int NaturalVideoWidth { get; }

        /// <summary>
        /// Gets the natural pixel height of the current media.  
        /// The value will be 0 if there is no video in the media.
        /// </summary>
        int NaturalVideoHeight { get; }

        /// <summary>
        /// 获取视频帧率。
        /// </summary>
        float FrameRate { get; }

        /// <summary>
        /// 获取视频码率(kbps)。
        /// </summary>
        float BitRate { get; }

        /// <summary>
        /// Gets or sets the audio volume.  Specifies the volume, as a 
        /// number from 0 to 1.  Full volume is 1, and 0 is silence.
        /// </summary>
        double Volume { get; set; }

        /// <summary>
        /// Gets or sets the balance on the audio.
        /// </summary>
        double Balance { get; set; }

        /// <summary>
        /// 获取或设置一个值指示是否持续解码最后一帧画面。
        /// </summary>
        bool RequireLastFrame { get; set; }

        /// <summary>
        /// 获取正在播放的媒体播放源。
        /// </summary>
        object Source { get; }

        /// <summary>
        /// 打开播放媒体，初始化播放资源。
        /// </summary>
        /// <remarks>
        /// 在调用此方法前播放器不可用。
        /// </remarks>
        /// <param name="source">需要播放的媒体源。</param>
        void Open(object source);

        /// <summary>
        /// 异步打开播放媒体，初始化播放资源。
        /// </summary>
        /// <remarks>
        /// 在调用此方法前播放器不可用。
        /// </remarks>
        /// <param name="source">需要播放的媒体源。</param>
        Task OpenAsync(object source);

        /// <summary>
        /// 开始播放或从前一暂停状态继续播放。
        /// </summary>
        void Play();

        /// <summary>
        /// Halt play at current position. 
        /// </summary>
        void Pause();

        /// <summary>
        /// Halt play. 
        /// </summary>
        /// <remarks>
        /// Stopping playback will cause the position to be reset to the beginning.
        /// </remarks>
        void Stop();

        /// <summary>
        /// Closes the underlying media. This de-allocates all of the native resources in
        /// the media. The mediaplayer can be opened again by calling the Open method.
        /// </summary>
        void Close();

        /// <summary>
        /// 截取当前帧并生成图片文件。
        /// </summary>
        /// <param name="fileName">截取成功后保存为图片的文件名。</param>
        /// <returns></returns>
        bool TakeSnapshot(out string fileName);

        /// <summary>
        /// 截取当前帧并保存为图片文件到指定位置下。
        /// </summary>
        /// <param name="fileName">
        /// 保存的图片文件的文件名称, 若文件名为空则将保存到临时目录。
        /// </param>
        /// <param name="createFileIfNotExists">当文件名不存在时是否创建文件。</param>
        /// <returns>截取成功后保存为图片的文件名。</returns>
        bool TakeSnapshot(string fileName, bool createFileIfNotExists);
    }
}
