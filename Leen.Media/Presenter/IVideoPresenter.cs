using Leen.Media.Renderer;

namespace Leen.Media.Presenter
{
    /// <summary>
    /// 表示视频呈现接口。视频呈现接口从 <see cref="IMediaPlayer"/> 接收视频帧数据并将其呈现出来，通常是通过 <see cref="IVideoRenderer"/> 进行视频渲染并输出到显示设备。
    /// </summary>
    public interface IVideoPresenter
    {
        /// <summary>
        /// 获取此视频呈现接口关联的媒体播放器。
        /// </summary>
        IMediaPlayer Player { get; }

        /// <summary>
        /// 获取此视频呈现接口关联的视频渲染器。
        /// </summary>
        IVideoRenderer Renderer { get; }

        /// <summary>
        /// 获取或设置一个值用于切换视频呈现。
        /// </summary>
        bool IsPresenting { get; set; }
    }
}
